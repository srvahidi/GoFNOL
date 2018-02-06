using System.IO;
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
            var environmentData = File.ReadAllText("Fixtures/env.json");
            var mockedConfigRoot = new Mock<IConfigurationRoot>();
            mockedConfigRoot.Setup(cr => cr["VCAP_SERVICES"]).Returns(environmentData);
            var environmentConfiguration = new EnvironmentConfiguration(mockedConfigRoot.Object);

            // Execute
            var eaiEndpoint = environmentConfiguration.EAIEndpoint;

            // Verify
            eaiEndpoint.Should().Be("http://dummy.com");
        }
    }
}