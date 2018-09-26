using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using GoFNOL.Controllers;
using GoFNOL.Models;
using GoFNOL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace GoFNOL.tests.Unit
{
    public class FNOLControllerTests
    {
        [Fact]
        public void FNOLController_WhenLogoutIsInvoked_ShouldReturnSignOutResult()
        {
            // Setup
            var fixture = new FNOLController(new Mock<IFNOLService>().Object, new Mock<INGPService>().Object, NullLogger<FNOLController>.Instance);

            // Execute
            var actual = fixture.Logout();

            // Verify
            actual.Should().BeEquivalentTo(new SignOutResult(new[] {"Cookies", "oidc"}));
        }

        [Fact]
        public async Task FNOLController_WhenMainViewIsInvoked_ShouldRenderProfileId()
        {
            // Setup
            var mockNGPService = new Mock<INGPService>();
            string actualName = null;
            const string profileId = "profileid";
            mockNGPService.Setup(s => s.GetUserProfileIdAsync(It.IsAny<string>()))
                .Callback<string>(r => actualName = r)
                .ReturnsAsync(profileId);
            var client = Helpers.CreateTestServer(collection => collection.AddSingleton(mockNGPService.Object)).CreateClient();

            // Execute
            var response = await client.GetAsync("/fnol");

            // Verify
            mockNGPService.VerifyAll();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualName.Should().Be("pe2generic1");
            var html = await response.Content.ReadAsStringAsync();
            var header = Helpers.GetHtmlElement(html, "h2");
            header.Should().Be($"Create Claim for Profile: {profileId}");
        }

        [Theory]
        [InlineData("localhost", "Local")]
        [InlineData("localhost-acceptance", "Acceptance")]
        [InlineData("localhost-int", "Int")]
        [InlineData("localhost-demo", "Demo")]
        [InlineData("localhost-prod", "Local")]
        public async Task FNOLController_WhenMainViewIsInvoked_ShouldRenderEnvironment(string hostName, string displayString)
        {
            // Setup
            var mockNGPService = new Mock<INGPService>();
            mockNGPService.Setup(s => s.GetUserProfileIdAsync(It.IsAny<string>())).ReturnsAsync("profileid");
            var server = Helpers.CreateTestServer(collection => collection.AddSingleton(mockNGPService.Object));
            server.BaseAddress = new Uri($"http://{hostName}");
            var client = server.CreateClient();

            // Execute
            var response = await client.GetAsync("/fnol");

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var html = await response.Content.ReadAsStringAsync();
            var header = Helpers.GetHtmlElement(html, "h3");
            header.Should().Be($"GoFNOL - {displayString}");
        }

        [Fact]
        public async Task FNOLController_WhenDeductibleWaived_ShouldSetDeductibleTo_W_()
        {
            // Setup
            var mockFNOLService = new Mock<IFNOLService>();
            FNOLRequest actualRequest = null;
            mockFNOLService.Setup(s => s.CreateAssignment(It.IsAny<FNOLRequest>()))
                .Callback<FNOLRequest>(r => actualRequest = r)
                .ReturnsAsync(new Claim());
            var client = Helpers.CreateTestServer(collection =>
            {
                collection.AddSingleton(mockFNOLService.Object);
            }).CreateClient();
            var formData = new Dictionary<string, string>
            {
                {"deductible-waived", "on"},
                {"deductible", "500"}
            };
            var formContent = new FormUrlEncodedContent(formData);

            // Execute
            var response = await client.PostAsync("/fnol", formContent);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.Found);
            actualRequest.Deductible.Should().Be("W");
        }

        [Fact]
        public async Task FNOLController_WhenDeductibleNotWaived_ShouldUseUserProvidedDeductible()
        {
            // Setup
            var mockFNOLService = new Mock<IFNOLService>();
            FNOLRequest actualRequest = null;
            mockFNOLService.Setup(s => s.CreateAssignment(It.IsAny<FNOLRequest>()))
                .Callback<FNOLRequest>(r => actualRequest = r)
                .ReturnsAsync(new Claim());
            var client = Helpers.CreateTestServer(collection =>
            {
                collection.AddSingleton(mockFNOLService.Object);
            }).CreateClient();
            var formData = new Dictionary<string, string>
            {
                {"deductible", "500"}
            };
            var formContent = new FormUrlEncodedContent(formData);

            // Execute
            var response = await client.PostAsync("/fnol", formContent);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.Found);
            actualRequest.Deductible.Should().Be("500");
        }
    }
}