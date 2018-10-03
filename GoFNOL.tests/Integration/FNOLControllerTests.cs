using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.XPath;
using FluentAssertions;
using GoFNOL.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GoFNOL.tests.Integration
{
	public class FNOLControllerTests
	{
		[Fact]
		public async Task FNOLController_WhenPostIsInvoked_ShouldSubmitToEAIAndReturnData()
		{
			// Setup
			const string expectedEndpoint = "http://dum.my";
			var mockConfig = new Mock<IEnvironmentConfiguration>();
			mockConfig.SetupGet(c => c.EAIEndpoint).Returns(expectedEndpoint);

			var mockHTTPService = new Mock<IHTTPService>();
			(Uri uri, HttpContent content)? actualEAI = null;
			mockHTTPService.Setup(service => service.PostAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>()))
				.Callback<Uri, HttpContent>((uri, content) => actualEAI = (uri, content))
				.ReturnsAsync(new HttpResponseMessage
				{
					Content = new StringContent("<ADP_TRANSACTION_ID>123</ADP_TRANSACTION_ID>")
				});
			var client = Helpers.CreateTestServer(collection =>
			{
				collection.AddSingleton(mockConfig.Object);
				collection.AddSingleton(mockHTTPService.Object);
			}).CreateClient();
			var jRequest = new JObject
			{
				["profileId"] = "1234567890",
				["mobileFlowIndicator"] = "D",
				["claimNumber"] = "ABC-123",
				["owner"] = new JObject
				{
					["firstName"] = "1st name",
					["lastName"] = "nst name",
					["phoneNumber"] = "(012) 345 67-89",
					["email"] = "a@b.c",
					["address"] = new JObject
					{
						["postalCode"] = "34567-0110",
						["state"] = "ST"
					}
				},
				["vin"] = "0123456789ABCDEFG",
				["lossType"] = "COLL",
				["deductible"] = "500"
			};
			var requestContent = new StringContent(jRequest.ToString());
			requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			// Execute
			var response = await client.PostAsync("/api/fnol", requestContent);

			// Verify
			mockHTTPService.VerifyAll();
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
			var jResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
			jResponse.Should().BeEquivalentTo(new JObject
			{
				["workAssignmentId"] = "123"
			});

			actualEAI.Value.uri.AbsoluteUri.TrimEnd('/').Should().Be(expectedEndpoint);
			var xAssignment = Helpers.ParseAssignment(await actualEAI.Value.content.ReadAsStringAsync());
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/MOBILE_FLOW_IND").Value.Should().Be("D");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/USER_ID").Value.Should().Be("1234567890");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/COMPANY_ID").Value.Should().Be("123");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/OFFICE_ID").Value.Should().Be("1234567");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/CLAIM_NBR").Value.Should().Be("ABC-123");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_FIRST_NAME").Value.Should().Be("1st name");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_LAST_NAME").Value.Should().Be("nst name");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CONTACT_PHONE_NBR_3").Value.Should().Be("(012) 345 67-89");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_POSTAL_CODE").Value.Should().Be("34567-0110");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_STATE").Value.Should().Be("ST");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CONTACT_EMAIL").Value.Should().Be("a@b.c");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/VEHICLE_VIN").Value.Should().Be("0123456789ABCDEFG");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/LOSS_TYPE").Value.Should().Be("COLL");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/DEDUCTIBLE_AMT").Value.Should().Be("500");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_LAST_NAME").Value.Should().Be("nst name");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_FIRST_NAME").Value.Should().Be("1st name");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_PHONE_TYPE_1").Value.Should().Be("CP");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_PHONE_NBR_1").Value.Should().Be("(012) 345 67-89");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_EMAIL").Value.Should().Be("a@b.c");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/LOSS_DATE").Value.Should().Be(DateTime.UtcNow.ToString("yyyy-MM-dd"));
		}
	}
}