using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using GoFNOL.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GoFNOL.tests.Integration
{
	public class UserControllerTests : TestServerFixture
	{
		private Mock<INGPService> _mockNGPService;

		protected override void RegisterServices(IServiceCollection customServices)
		{
			base.RegisterServices(customServices);
			_mockNGPService = new Mock<INGPService>();
			customServices.AddSingleton(_mockNGPService.Object);
		}

		[Fact]
		public async Task UserDataController_WhenDataGetIsInvoked_ShouldReturnUserData()
		{
			// Setup
			string actualUserName = null;
			const string profileId = "sam123";
			_mockNGPService.Setup(s => s.GetUserProfileIdAsync(It.IsAny<string>()))
				.Callback<string>(r => actualUserName = r)
				.ReturnsAsync(profileId);

			// Execute
			var response = await Client.GetAsync("/api/user/samantha");

			// Verify
			_mockNGPService.VerifyAll();
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
			actualUserName.Should().Be("samantha");
			var responseContent = await response.Content.ReadAsStringAsync();
			var jContent = JObject.Parse(responseContent);
			jContent.ShouldBeEquivalentTo(new JObject
			{
				["profileId"] = "sam123"
			});
		}
	}
}