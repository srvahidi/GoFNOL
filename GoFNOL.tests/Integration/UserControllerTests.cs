using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using GoFNOL.Controllers;
using GoFNOL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GoFNOL.tests.Integration
{
	public class UserControllerTests
	{
		[Fact]
		public async Task UserDataController_WhenDataGetIsInvoked_ShouldReturnUserData()
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
			var response = await client.GetAsync("/api/user/data");

			// Verify
			mockNGPService.VerifyAll();
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
			actualName.Should().Be("pe2generic1");
			var responseContent = await response.Content.ReadAsStringAsync();
			var jContent = JObject.Parse(responseContent);
			jContent.ShouldBeEquivalentTo(new JObject
			{
				["profileId"] = "profileid"
			});
		}

		[Fact]
		public void FNOLController_WhenLogoutIsInvoked_ShouldReturnSignOutResult()
		{
			// Setup
			var fixture = new UserController();

			// Execute
			var actual = fixture.PostLogout();

			// Verify
			actual.Should().BeEquivalentTo(new SignOutResult(new[] { "Cookies", "oidc" }));
		}
	}
}