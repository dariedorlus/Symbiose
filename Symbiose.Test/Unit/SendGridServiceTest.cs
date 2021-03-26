using Microsoft.Extensions.Logging;
using Symbiose.Mail.Email_Delivery;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Symbiose.Mail.Models;

namespace Symbiose.Test.Unit
{
    
    [TestFixture]
    public class SendGridServiceTest:TestBase
    {
        private NullLogger<SendgridService> logger;
        
        private HttpClient httpClient;
        private HttpMessageHandler handler;
        private SendgridService sut;
        private Email email;

        [SetUp]
        public void Setup()
        {
            logger = A.Fake<NullLogger<SendgridService>>();
            handler = A.Fake<HttpMessageHandler>();
            httpClient = new HttpClient(handler);

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            sut = new SendgridService(logger, config, httpClient);
            email = fixture.Create<Email>();

        }


        [Test]
        public async Task SendEmailTest_Success()
        {
            //Arrange
            var httpResponse = A.Fake<HttpResponseMessage>();
            httpResponse.StatusCode = HttpStatusCode.Accepted;

            A.CallTo(handler).Where( h => h.Method.Name == "SendAsync" )
                .WithReturnType<Task<HttpResponseMessage>>()
                .Returns(httpResponse);

            //Act
            var result = await sut.SendEmail(email);

            //Assert
            result.Should().BeTrue();
            A.CallTo(handler).Where(h => h.Method.Name == "SendAsync")
                .WithReturnType<Task<HttpResponseMessage>>()
                .MustHaveHappened();
        }


        [Test]
        public async Task SendEmailTest_Http_Response_Failure()
        {
            //Arrange
            var httpResponse = A.Fake<HttpResponseMessage>();
            httpResponse.StatusCode = HttpStatusCode.Forbidden;

            A.CallTo(handler).Where(h => h.Method.Name == "SendAsync")
                .WithReturnType<Task<HttpResponseMessage>>()
                .Returns(httpResponse);

            //Act
            var result = await sut.SendEmail(email);

            //Assert
            result.Should().BeFalse();
            A.CallTo(handler).Where(h => h.Method.Name == "SendAsync")
                .WithReturnType<Task<HttpResponseMessage>>()
                .MustHaveHappened();
        }


        [Test]
        public async Task SendEmailTest_Null_Email_Failure()
        {
            //Arrange

            //Act
            var result = await sut.SendEmail(null);

            //Assert
            result.Should().BeFalse();
            A.CallTo(handler).Where(h => h.Method.Name == "SendAsync")
                .WithReturnType<Task<HttpResponseMessage>>()
                .MustNotHaveHappened();
        }
    }
}
