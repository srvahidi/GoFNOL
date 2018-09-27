using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.XPath;
using FluentAssertions;
using GoFNOL.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GoFNOL.tests.Integration
{
	public class FNOLControllerTests
	{
		[Fact]
		public async Task FNOLController_WhenFormIsPOSTed_ShouldFormProperFollowingRequest()
		{
			// Setup
			const string expectedEndpoint = "http://dum.my";
			var mockConfig = new Mock<IEnvironmentConfiguration>();
			mockConfig.SetupGet(c => c.EAIEndpoint).Returns(expectedEndpoint);

			HttpContent actualContent = null;
			Uri actualEAIEndpoint = null;
			var mockHTTPService = new Mock<IHTTPService>();
			mockHTTPService.Setup(service => service.PostAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>()))
				.Callback<Uri, HttpContent>((uri, content) =>
				{
					actualEAIEndpoint = uri;
					actualContent = content;
				})
				.ReturnsAsync(new HttpResponseMessage
				{
					Content = new StringContent("<ADP_TRANSACTION_ID>123</ADP_TRANSACTION_ID>")
				});
			var client = Helpers.CreateTestServer(collection =>
			{
				collection.AddSingleton(mockConfig.Object);
				collection.AddSingleton(mockHTTPService.Object);
			}).CreateClient();
			var formData = new Dictionary<string, string>
			{
				{"mobile-flow-ind", "D"},
				{"claim-number", "01AB"},
				{"first-name", "Na"},
				{"last-name", "Me"},
				{"phone-number", "(123)456-7890"},
				{"zip-code", "00000"},
				{"zip-addon", "1111"},
				{"state", "ST"},
				{"email", "somebody@some.where"},
				{"vin", "0"},
				{"loss-type", "T"},
				{"deductible", "0"}
			};
			var formContent = new FormUrlEncodedContent(formData);

			// Execute
			await client.PostAsync("/fnol", formContent);

			// Verify
			mockHTTPService.VerifyAll();
			actualEAIEndpoint.AbsoluteUri.TrimEnd('/').Should().Be(expectedEndpoint);
			var xAssignment = Helpers.ParseAssignment(await actualContent.ReadAsStringAsync());
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/MOBILE_FLOW_IND").Value.Should().Be("D");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/CLAIM_NBR").Value.Should().Be("01AB");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_FIRST_NAME").Value.Should().Be("Na");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_LAST_NAME").Value.Should().Be("Me");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CONTACT_PHONE_NBR_3").Value.Should().Be("(123)456-7890");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_POSTAL_CODE").Value.Should().Be("00000-1111");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_STATE").Value.Should().Be("ST");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CONTACT_EMAIL").Value.Should().Be("somebody@some.where");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/VEHICLE_VIN").Value.Should().Be("0");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/LOSS_TYPE").Value.Should().Be("T");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/DEDUCTIBLE_AMT").Value.Should().Be("0");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_LAST_NAME").Value.Should().Be("Me");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_FIRST_NAME").Value.Should().Be("Na");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_PHONE_TYPE_1").Value.Should().Be("CP");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_PHONE_NBR_1").Value.Should().Be("(123)456-7890");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_EMAIL").Value.Should().Be("somebody@some.where");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/FNOL_DATETIME_TZ").Value.Should().Be("MST");
		}
	}
}