using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using FluentAssertions;
using GoFNOL.Outside.Repositories;
using GoFNOL.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;
using FluentAssertionsServices = FluentAssertions.Common.Services;

namespace GoFNOL.tests.Integration
{
	public class FNOLControllerTests : TestServerFixture
	{
		private const string ExpectedEndpoint = "http://dum.my";

		private const string ExpectedEAIUsername = "eai user";

		private const string ExpectedEAIPassword = "eai pass";

		private readonly HttpContent _requestContentClaimNumberProvided;

		private readonly HttpContent _requestContentClaimNumberGenerated;

		private Mock<IEnvironmentConfiguration> _mockConfig;

		private Mock<IHTTPService> _mockHTTPService;

		private Mock<IClaimNumberCounterRepository> _mockClaimNumberCounterRepository;

		private readonly XmlSchemaSet _schemaSet;

		public FNOLControllerTests()
		{
			_requestContentClaimNumberProvided = new StringContent(new JObject
			{
				["profileId"] = "1234567890",
				["mobileFlowIndicator"] = "D",
				["claimNumber"] = "ABC-123",
				["owner"] = new JObject
				{
					["firstName"] = "1st name",
					["lastName"] = "nst name",
					["phoneNumber"] = "(012)345-6789",
					["email"] = "a@b.c",
					["address"] = new JObject
					{
						["city"] = "Cityville",
						["zipCode"] = "34567",
						["state"] = "ST"
					}
				},
				["vin"] = "0123456789ABCDEFG",
				["lossType"] = "COLL",
				["deductible"] = "500",
				["isStayingInProgress"] = true,
				["autoGenerateClaim"] = false
			}.ToString());
			_requestContentClaimNumberProvided.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			_requestContentClaimNumberProvided.Headers.Add("org-id", "PGI");

			_requestContentClaimNumberGenerated = new StringContent(new JObject
			{
				["profileId"] = "PGI4567890",
				["mobileFlowIndicator"] = "D",
				["claimNumber"] = "blah",
				["owner"] = new JObject
				{
					["firstName"] = "1st name",
					["lastName"] = "nst name",
					["phoneNumber"] = "(012)345-6789",
					["email"] = "a@b.c",
					["address"] = new JObject
					{
						["city"] = "Cityville",
						["zipCode"] = "34567",
						["state"] = "ST"
					}
				},
				["vin"] = "0123456789ABCDEFG",
				["lossType"] = "COLL",
				["deductible"] = "500",
				["isStayingInProgress"] = false,
				["autoGenerateClaim"] = true
			}.ToString());
			_requestContentClaimNumberGenerated.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			_requestContentClaimNumberGenerated.Headers.Add("org-id", "PGI");

			_mockConfig.SetupGet(c => c.EAIEndpoint).Returns(ExpectedEndpoint);
			_mockConfig.SetupGet(c => c.EAIUsername).Returns(ExpectedEAIUsername);
			_mockConfig.SetupGet(c => c.EAIPassword).Returns(ExpectedEAIPassword);

			_schemaSet = new XmlSchemaSet();
			_schemaSet.Add("", XmlReader.Create(new StringReader(FixtureFiles.GetFixture("Fixtures/fnol.xsd"))));
		}

		protected override void RegisterServices(IServiceCollection customServices)
		{
			base.RegisterServices(customServices);
			_mockConfig = new Mock<IEnvironmentConfiguration>();
			customServices.AddSingleton(_mockConfig.Object);
			_mockHTTPService = new Mock<IHTTPService>();
			customServices.AddSingleton(_mockHTTPService.Object);
			_mockClaimNumberCounterRepository = new Mock<IClaimNumberCounterRepository>();
			customServices.AddScoped(s => _mockClaimNumberCounterRepository.Object);
		}

		[Fact]
		public async Task FNOLController_WhenPostIsInvoked_ShouldSubmitToEAIAndReturnData()
		{
			// Setup
			_mockConfig.SetupGet(c => c.A2EDataDiscoveryUri).Returns(new Uri("http://a2e.data"));

			var jA2EDataDiscoveryDoc = new JObject
			{
				["assignmentsInProgress"] = "http://a2e.data/api/assignmentsinprogress"
			};
			_mockHTTPService.Setup(service => service.GetAsync(new Uri("http://a2e.data")))
				.ReturnsAsync(new HttpResponseMessage
				{
					Content = new StringContent(jA2EDataDiscoveryDoc.ToString())
				});

			_mockHTTPService.Setup(service => service.PutAsync(new Uri("http://a2e.data/api/assignmentsinprogress/123"), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

			(Uri uri, HttpContent content)? actualEAI = null;
			_mockHTTPService.Setup(service => service.PostAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>()))
				.Callback<Uri, HttpContent>((uri, content) => actualEAI = (uri, content))
				.ReturnsAsync(new HttpResponseMessage
				{
					Content = new StringContent("&lt;ADP_TRANSACTION_ID&gt;123&lt;/ADP_TRANSACTION_ID&gt;")
				});

			// Execute
			var response = await Client.PostAsync("/api/fnol", _requestContentClaimNumberProvided);

			// Verify
			_mockHTTPService.VerifyAll();
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
			var jResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
			jResponse.ShouldBeEquivalentTo(new JObject
			{
				["workAssignmentId"] = "123",
				["claimNumber"] = "ABC-123"
			});

			actualEAI.Value.uri.AbsoluteUri.TrimEnd('/').Should().Be(ExpectedEndpoint);
			var eaiContent = await actualEAI.Value.content.ReadAsStringAsync();
			var eaiCredentials = Helpers.ParseCredentials(eaiContent);
			eaiCredentials.username.Should().Be(ExpectedEAIUsername);
			eaiCredentials.password.Should().Be(ExpectedEAIPassword);
			var xAssignment = Helpers.ParseAssignment(eaiContent);
			xAssignment.Validate(_schemaSet, (sender, args) => FluentAssertionsServices.ThrowException(args.Message));
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/MOBILE_FLOW_IND").Value.Should().Be("D");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/USER_ID").Value.Should().Be("1234567890");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/COMPANY_ID").Value.Should().Be("123");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/OFFICE_ID").Value.Should().Be("1234567");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/CLAIM_NBR").Value.Should().Be("ABC-123");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_FIRST_NAME").Value.Should().Be("1st name");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_LAST_NAME").Value.Should().Be("nst name");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CONTACT_PHONE_NBR_3").Value.Should().Be("(012)345-6789");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_POSTAL_CODE").Value.Should().Be("34567");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CITY").Value.Should().Be("Cityville");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_STATE").Value.Should().Be("ST");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CONTACT_EMAIL").Value.Should().Be("a@b.c");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/VEHICLE_VIN").Value.Should().Be("0123456789ABCDEFG");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/LOSS_TYPE").Value.Should().Be("COLL");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/DEDUCTIBLE_AMT").Value.Should().Be("500");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_LAST_NAME").Value.Should().Be("nst name");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_FIRST_NAME").Value.Should().Be("1st name");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_PHONE_NBR_1").Value.Should().Be("(012)345-6789");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_EMAIL").Value.Should().Be("a@b.c");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/LOSS_DATE").Value.Should().Be(DateTime.UtcNow.ToString("yyyy-MM-dd"));
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/CLAIMANT_STATE").Value.Should().Be("ST");
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/VEHICLE_LOCATION_STATE").Value.Should().Be("ST");
		}

		[Fact]
		public async Task FNOLController_WhenPostIsInvokedAndNoWorkAssignmentIdReturned_ShouldReturnErrorThatEAIFailed()
		{
			// Setup
			_mockHTTPService.Setup(service => service.PostAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage
				{
					Content = new StringContent("&lt;ADP_TRANSACTION_ID&gt;&lt;/ADP_TRANSACTION_ID&gt;")
				});

			// Execute
			var response = await Client.PostAsync("/api/fnol", _requestContentClaimNumberProvided);

			// Verify
			_mockHTTPService.VerifyAll();
			response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
		}

		[Fact]
		public async Task FNOLController_WhenPostIsInvokedAndHttpRequestExceptionThrown_ShouldReturnErrorThatNetworkFailed()
		{
			// Setup
			_mockHTTPService.Setup(service => service.PostAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>()))
				.Throws<HttpRequestException>();

			// Execute
			var response = await Client.PostAsync("/api/fnol", _requestContentClaimNumberProvided);

			// Verify
			_mockHTTPService.VerifyAll();
			response.StatusCode.Should().Be(HttpStatusCode.GatewayTimeout);
		}

		[Fact]
		public async Task FNOLController_WhenPostIsInvokedAndExceptionThrown_ShouldReturnErrorThatAPIFailed()
		{
			// Setup
			var jRequest = new JObject();
			var emptyRequestContent = new StringContent(jRequest.ToString());
			emptyRequestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			emptyRequestContent.Headers.Add("org-id", "PGI");

			// Execute
			var response = await Client.PostAsync("/api/fnol", emptyRequestContent);

			// Verify
			response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
		}

		[Fact]
		public async Task FNOLController_WhenPostIsInvokedWithoutOrgIdHeader_ShouldReturnBadRequest()
		{
			// Setup
			var jRequest = new JObject();
			var emptyRequestContent = new StringContent(jRequest.ToString());
			emptyRequestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			// Execute
			var response = await Client.PostAsync("/api/fnol", emptyRequestContent);

			// Verify
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task FNOLController_WhenPostIsInvokedWithAutoGenerateTrue_ShouldGenerateClaimNumberAndSubmitToEAIAndReturnData()
		{
			// Setup
			_mockConfig.SetupGet(c => c.A2EDataDiscoveryUri).Returns(new Uri("http://a2e.data"));
			_mockClaimNumberCounterRepository.Setup(m => m.IncrementCounter("PGI")).ReturnsAsync(456);

			(Uri uri, HttpContent content)? actualEAI = null;
			_mockHTTPService.Setup(service => service.PostAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>()))
				.Callback<Uri, HttpContent>((uri, content) => actualEAI = (uri, content))
				.ReturnsAsync(new HttpResponseMessage
				{
					Content = new StringContent("&lt;ADP_TRANSACTION_ID&gt;123&lt;/ADP_TRANSACTION_ID&gt;")
				});

			// Execute
			var response = await Client.PostAsync("/api/fnol", _requestContentClaimNumberGenerated);

			// Verify
			_mockHTTPService.VerifyAll();
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
			var jResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
			jResponse.ShouldBeEquivalentTo(new JObject
			{
				["workAssignmentId"] = "123",
				["claimNumber"] = "14-PGIA-00456"
			});

			var eaiContent = await actualEAI.Value.content.ReadAsStringAsync();
			var xAssignment = Helpers.ParseAssignment(eaiContent);
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/CLAIM_NBR").Value.Should().Be("14-PGIA-00456");
		}
	}
}