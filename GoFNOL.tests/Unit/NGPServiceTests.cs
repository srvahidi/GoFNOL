using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using GoFNOL.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace GoFNOL.tests.Unit
{
	public class NGPServiceTests
	{
		private readonly INGPService fixture;

		private readonly Mock<IEnvironmentConfiguration> mockEnvironmentConfiguration;

		private readonly Mock<IHTTPService> mockHTTPService;

		public NGPServiceTests()
		{
			mockEnvironmentConfiguration = new Mock<IEnvironmentConfiguration>();
			mockHTTPService = new Mock<IHTTPService>();
			fixture = new NGPService(mockEnvironmentConfiguration.Object, mockHTTPService.Object, NullLogger<NGPService>.Instance);
		}

		[Fact]
		public async Task GetUserProfileIdAsync_WhenUserUidProvided_ShouldExtractProfileIdOfAppraiserRole()
		{
			// Setup
			const string username = "samantha";
			const string userProfileId = "sam123";
			const string ngpUsersEndpoint = "http://ng.p/users";
			mockEnvironmentConfiguration.SetupGet(c => c.NGPUsersEndpoint).Returns(ngpUsersEndpoint);

			var ngpResponse = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    <soap:Body>
        <GetUserByExactUidResponse xmlns=""http://ngp.audatex.com/"">
            <GetUserByExactUidResult>&lt;xmldata&gt;&lt;users&gt;&lt;user&gt;&lt;roles&gt;&lt;role&gt;&lt;role_type&gt;Estimate Reviewer&lt;/role_type&gt;&lt;ext_identity&gt;NULL&lt;/ext_identity&gt;&lt;/role&gt;&lt;role&gt;&lt;role_type&gt;Appraiser&lt;/role_type&gt;&lt;ext_identity&gt;{userProfileId}&lt;/ext_identity&gt;&lt;/role&gt;&lt;/roles&gt;&lt;/user&gt;&lt;/users&gt;&lt;/xmldata&gt;</GetUserByExactUidResult>
        </GetUserByExactUidResponse>
    </soap:Body>
</soap:Envelope>";

			(Uri actualUri, HttpContent actualContent)? actualRequest = null;
			mockHTTPService.Setup(s => s.PostAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>()))
				.Callback<Uri, HttpContent>((uri, content) => actualRequest = (uri, content))
				.ReturnsAsync(new HttpResponseMessage {Content = new StringContent(ngpResponse)});

			// Execute
			var actual = await fixture.GetUserProfileIdAsync(username);

			// Verify
			mockEnvironmentConfiguration.VerifyAll();
			mockHTTPService.VerifyAll();
			actualRequest.Value.actualUri.OriginalString.Should().Be(ngpUsersEndpoint);
			var expectedRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
  <soap12:Body>
    <GetUserByExactUid xmlns=""http://ngp.audatex.com/"">
      <uid>{username}</uid>
      <requestedBy>string</requestedBy>
      <bypassUserPref>true</bypassUserPref>
    </GetUserByExactUid>
  </soap12:Body>
</soap12:Envelope>";
			actualRequest.Value.actualContent.Headers.ContentType.Should().Be(new MediaTypeHeaderValue("application/soap+xml"));
			(await actualRequest.Value.actualContent.ReadAsStringAsync()).Should().Be(expectedRequest);
			actual.Should().Be(userProfileId);
		}
	}
}