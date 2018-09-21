using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using GoFNOL.Models;
using GoFNOL.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GoFNOL.tests.Unit
{
    public class FNOLControllerTests
    {
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
            var start = html.IndexOf("<h2>") + 4;
            var end = html.IndexOf("</h2>", start);
            var header = html.Substring(start, end - start);
            header.Should().Be($"Create Claim for Profile: {profileId}");
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