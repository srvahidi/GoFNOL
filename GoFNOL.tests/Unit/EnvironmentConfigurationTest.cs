using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace GoFNOL.tests.Unit
{
	public class EnvironmentConfigurationTest
	{
		[Fact]
		public void EnvironmentConfiguration_WhenCreated_ShouldGetValuesFromEnvironment()
		{
			// Setup
			var environmentData = FixtureFiles.GetFixture("Fixtures/env.json");
			var mockedConfigRoot = new Mock<IConfigurationRoot>();
			mockedConfigRoot.Setup(cr => cr["VCAP_SERVICES"]).Returns(environmentData);
			var environmentConfiguration = new EnvironmentConfiguration(mockedConfigRoot.Object);

			// Execute
			var eaiEndpoint = environmentConfiguration.EAIEndpoint;
			var isEndpoint = environmentConfiguration.ISEndpoint;
			var ngpUsersEndpoint = environmentConfiguration.NGPUsersEndpoint;
			var eaiUsername = environmentConfiguration.EAIUsername;
			var eaiPassword = environmentConfiguration.EAIPassword;
			var dbConnectionString = environmentConfiguration.DbConnectionString;

			// Verify
			eaiEndpoint.Should().Be("http://eai.dummy");
			isEndpoint.Should().Be("http://is.dummy");
			ngpUsersEndpoint.Should().Be("http://npg.dummy/users");
			eaiUsername.Should().Be("dummyUsername");
			eaiPassword.Should().Be("dummyPassword");
			dbConnectionString.Should().Be("db-connect-string");
		}
	}
}