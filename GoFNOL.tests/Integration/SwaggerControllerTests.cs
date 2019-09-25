using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace GoFNOL.tests.Integration
{
	public class SwaggerControllerTests : TestServerFixture
	{
		[Fact]
		public async Task Get_WhenInvoked_ShouldReturnOk()
		{
			// Setup

			// Execute
			var response = await Client.GetAsync("/swagger/index.html");

			// Verify
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}
	}
}
