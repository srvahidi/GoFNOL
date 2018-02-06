using System.Collections.Generic;
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
        public async Task FNOLController_WhenDeductibleWaived_ShouldSetDeductibleTo_W_()
        {
            // Setup
            var mockFNOLService = new Mock<IFNOLService>();
            FNOLRequest actualRequest = null;
            mockFNOLService.Setup(s => s.CreateAssignment(It.IsAny<FNOLRequest>()))
                .Callback<FNOLRequest>(r => actualRequest = r)
                .Returns(Task.FromResult(new Claim()));
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
            await client.PostAsync("/fnol", formContent);

            // Verify
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
                .Returns(Task.FromResult(new Claim()));
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
            await client.PostAsync("/fnol", formContent);

            // Verify
            actualRequest.Deductible.Should().Be("500");
        }
    }
}