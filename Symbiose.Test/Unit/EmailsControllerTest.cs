// ReSharper disable once RedundantUsingDirective

using System;
using System.Threading.Tasks;
using AutoFixture;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Symbiose.Mail.Controllers;
using Symbiose.Mail.Models;
using Symbiose.Mail.Services;

namespace Symbiose.Test.Unit
{
    [TestFixture]

    public class EmailsControllerTest
    {
        private EmailsController sut;
        private IEmailService emailService;
        private Email email;

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();
            emailService = A.Fake<IEmailService>();
            email = fixture.Create<Email>();
            sut = new EmailsController(emailService);

        }

        [Test]
        public async Task PostNullEmailTest()
        {
            //Arrange
            

            //Act
            var res = await sut.PostEmail(null);

            //Assert
            res.Result.Should().NotBeNull();
            res.Result.Should().BeOfType<BadRequestObjectResult>();
            A.CallTo(() => emailService.ProcessEmail(A<Email>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task PostEmailTest_NullServiceResponse()
        {
            //Arrange
            email.BodyHtml = String.Empty;
            A.CallTo(() => emailService.ProcessEmail(A<Email>._)).Returns(default(Email));

            //Act
            var res = await sut.PostEmail(email);

            //Assert
            res.Result.Should().NotBeNull();
            res.Result.Should().BeOfType<BadRequestObjectResult>();
            A.CallTo(() => emailService.ProcessEmail(A<Email>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task PostEmailTest_Success()
        {
            //Arrange
            A.CallTo(() => emailService.ProcessEmail(A<Email>._)).Returns(email);

            //Act
            var res = await sut.PostEmail(email);

            //Assert
            res.Result.Should().NotBeNull();
            res.Result.Should().BeOfType<CreatedAtActionResult>();

            var createdResObj = (CreatedAtActionResult) res.Result;
            ((Email) createdResObj.Value).Id.Should().Be(email.Id);
            A.CallTo(() => emailService.ProcessEmail(A<Email>._)).MustHaveHappenedOnceExactly();
        }
    }
}