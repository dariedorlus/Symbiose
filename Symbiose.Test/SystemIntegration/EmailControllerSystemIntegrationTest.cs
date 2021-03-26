using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Symbiose.Mail;
using Symbiose.Mail.Models;

namespace Symbiose.Test.SystemIntegration
{
    [TestFixture]
    public class EmailControllerSystemIntegrationTest
    {
        private readonly HttpClient client;

        public EmailControllerSystemIntegrationTest()
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .UseConfiguration(new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build())
                    .UseStartup<Startup>());
            client = server.CreateClient();
        }

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public async Task PostEmail_Success()
        {
            var load = new
            {
                to = "fake@.com",
                to_name = "Mr. F",
                from = "no-reply@from.com",
                from_name = "No Re",
                subject = "A message from The Fake Family",
                body = "<h1>Your Bill</h1><p>$10</p>"
            };

            var response = await client.PostAsJsonAsync("/api/emails", load);
            var actual = await response.Content.ReadFromJsonAsync<Email>();
           
            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            actual.From.Should().Be(load.from);

        }

        [Test]
        public async Task PostEmail_Missing_RequiredField_Failure()
        {
            //Arrange
            var load = new
            {
                to_name = "Mr. F",
                from = "no-reply@from.com",
                from_name = "No Re",
                subject = "A message from The Fake Family",
                body = "<h1>Your Bill</h1><p>$10</p>"
            };

            //Act
            var response = await client.PostAsJsonAsync("/api/emails", load);
            var actualError = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actualError.Should().Contain("field is required");


        }


        [Test]
        public async Task PostEmail_IncorrectEmail_Failure()
        {
            //Arrange
            var load = new
            {
                to = "fake.com",
                to_name = "Mr. F",
                from = "no-reply@from.com",
                from_name = "No Re",
                subject = "A message from The Fake Family",
                body = "<h1>Your Bill</h1><p>$10</p>"
            };

            //Act
            var response = await client.PostAsJsonAsync("/api/emails", load);
            var actualError = await response.Content.ReadAsStringAsync();
            
            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actualError.Should().Contain("email is not valid");

        }
    }
}
