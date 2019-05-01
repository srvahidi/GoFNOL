using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
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
			var a2eDataDiscoveryUri = environmentConfiguration.A2EDataDiscoveryUri;
			var dbConnectionString = environmentConfiguration.DbConnectionString;

			// Verify
			eaiEndpoint.Should().Be("http://eai.dummy");
			isEndpoint.Should().Be("http://is.dummy");
			ngpUsersEndpoint.Should().Be("http://npg.dummy/users");
			eaiUsername.Should().Be("dummyUsername");
			eaiPassword.Should().Be("dummyPassword");
			a2eDataDiscoveryUri.Should().Be(new Uri("http://a2e.data/discovery"));
			dbConnectionString.Should().Be("db-connect-string");
		}

		[Fact]
		public void EnvironmentConfiguration_WhenA2EDataIsNotProvided_ShouldSetNull()
		{
			// Setup
			var jEnvironmentData = JObject.Parse(FixtureFiles.GetFixture("Fixtures/env.json"));
			jEnvironmentData["user-provided"].First(e => e["name"].Value<string>() == "a2e-data").Remove();
			var mockedConfigRoot = new Mock<IConfigurationRoot>();
			mockedConfigRoot.Setup(cr => cr["VCAP_SERVICES"]).Returns(jEnvironmentData.ToString());
			var environmentConfiguration = new EnvironmentConfiguration(mockedConfigRoot.Object);

			// Execute
			var a2eDataDiscoveryUri = environmentConfiguration.A2EDataDiscoveryUri;

			// Verify
			a2eDataDiscoveryUri.Should().BeNull();
		}
	}
}