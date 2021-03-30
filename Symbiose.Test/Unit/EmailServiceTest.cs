using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Symbiose.Mail.Email_Delivery;
using Symbiose.Mail.Models;
using Symbiose.Mail.Repositories;
using Symbiose.Mail.Services;

namespace Symbiose.Test.Unit
{


    public class EmailServiceTest : TestBase
    {
        private IEmailRepository repo;
        private IMailgunService mailgunService;
        private ISendgridService sendgridService;
        private ILogger<EmailService> logger;
        private Email email;
        private EmailService sut;

        [SetUp]
        public void Setup()
        {
            repo = A.Fake<IEmailRepository>();
            mailgunService = A.Fake<IMailgunService>();
            sendgridService = A.Fake<ISendgridService>();
            logger = A.Fake<ILogger<EmailService>>();
            email = fixture.Create<Email>();
            email.BodyHtml = string.Concat("<body>", email.BodyHtml, "<body/>");
            sut = new EmailService(repo, logger, mailgunService, sendgridService);

        }

        [Test]
        public async Task ProcessEmail_MailGun_Success()
        {
            //Arrange
            email.IsSent = false;
            A.CallTo(() => mailgunService.SendEmail(email)).Returns(true);
            A.CallTo(() => repo.InsertOneEntity(email)).Returns(email);

            //Act
            var result = await sut.ProcessEmail(email);

            //Assert
            result.IsSent.Should().BeTrue();
            result.BodyHtml.Should().Contain(email.BodyText);
            result.CreatedDate.Should().Be(result.UpdatedDate);

            A.CallTo(() => mailgunService.SendEmail(email)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repo.InsertOneEntity(email)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ProcessEmail_Sendgrid_Success()
        {
            //Arrange
            email.IsSent = false;
            A.CallTo(() => mailgunService.SendEmail(email)).Returns(false);
            A.CallTo(() => sendgridService.SendEmail(email)).Returns(true);
            A.CallTo(() => repo.InsertOneEntity(email)).Returns(email);

            //Act
            var result = await sut.ProcessEmail(email);

            //Assert
            result.IsSent.Should().BeTrue();
            result.BodyHtml.Should().Contain(email.BodyText);
            result.CreatedDate.Should().Be(result.UpdatedDate);

            A.CallTo(() => mailgunService.SendEmail(email)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sendgridService.SendEmail(email)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repo.InsertOneEntity(email)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ProcessEmail_EmailNotSent()
        {
            //Arrange
            email.IsSent = true;
            A.CallTo(() => mailgunService.SendEmail(email)).Returns(false);
            A.CallTo(() => sendgridService.SendEmail(email)).Returns(false);
            A.CallTo(() => repo.InsertOneEntity(email)).Returns(email);

            //Act
            var result = await sut.ProcessEmail(email);

            //Assert
            result.IsSent.Should().BeFalse();


            A.CallTo(() => mailgunService.SendEmail(email)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sendgridService.SendEmail(email)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repo.InsertOneEntity(email)).MustHaveHappenedOnceExactly();
        }

        [TestCaseSource(nameof(EmailValidationCases))]
        public async Task ProcessEmail_Validation(Email newEmail)
        {
            //Arrange

            //Act
            var result = await sut.ProcessEmail(newEmail);

            //Assert
            result.Should().BeNull();


        }

        private static IEnumerable<Email> EmailValidationCases()
        {
            var fixture = new Fixture();
            var email = fixture.Create<Email>();
            email.BodyHtml = String.Empty;
            yield return email;

            email = fixture.Create<Email>();
            email.To = String.Empty;
            yield return email;

            email = fixture.Create<Email>();
            email.ToName = String.Empty;
            yield return email;

            email = fixture.Create<Email>();
            email.From = String.Empty;
            yield return email;

            email = fixture.Create<Email>();
            email.FromName = String.Empty;
            yield return email;

        }
    }
}
