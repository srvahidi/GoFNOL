using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GoFNOL.tests.Integration
{
	public class ConfigControllerTests : TestServerFixture
	{
		[Fact]
		public async Task UserDataController_WhenDataGetIsInvoked_ShouldReturnUserData()
		{
			// Setup
			var isEndpoint = GetService<IEnvironmentConfiguration>().ISEndpoint;

			// Execute
			var response = await Client.GetAsync("/api/config");

			// Verify
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
			var responseContent = await response.Content.ReadAsStringAsync();
			var jContent = JObject.Parse(responseContent);
			jContent.ShouldBeEquivalentTo(new JObject
			{
				["isEndpoint"] = isEndpoint
			});
		}
	}
}