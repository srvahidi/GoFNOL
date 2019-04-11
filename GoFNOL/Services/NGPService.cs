using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Extensions.Logging;

namespace GoFNOL.Services
{
	public class NGPService : INGPService
	{
		private readonly IEnvironmentConfiguration envConfig;

		private readonly IHTTPService httpService;

		private readonly ILogger<NGPService> logger;

		public NGPService(IEnvironmentConfiguration envConfig, IHTTPService httpService, ILogger<NGPService> logger)
		{
			this.envConfig = envConfig;
			this.httpService = httpService;
			this.logger = logger;
		}

		public async Task<string> GetUserProfileIdAsync(string userUid)
		{
			var request = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
  <soap12:Body>
    <GetUserByExactUid xmlns=""http://ngp.audatex.com/"">
      <uid>{userUid}</uid>
      <requestedBy>string</requestedBy>
      <bypassUserPref>true</bypassUserPref>
    </GetUserByExactUid>
  </soap12:Body>
</soap12:Envelope>";
			var requestContent = new StringContent(request);
			requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/soap+xml");
			var response = await httpService.PostAsync(new Uri(envConfig.NGPUsersEndpoint), requestContent);
			var content = await response.Content.ReadAsStringAsync();
			var xContent = XDocument.Parse(content);
			XNamespace soapNs = "http://www.w3.org/2003/05/soap-envelope";
			var xBody = xContent.Element(soapNs + "Envelope").Element(soapNs + "Body");
			XNamespace ngpNs = "http://ngp.audatex.com/";
			var innerPayload = xBody.Element(ngpNs + "GetUserByExactUidResponse").Element(ngpNs + "GetUserByExactUidResult").Value;
			var xPayload = XDocument.Parse(innerPayload);
			var xRoles = xPayload.XPathSelectElements("//xmldata/users/user[1]/roles/role");
			var xRole = xRoles.Where(x => x.Element("role_type").Value == "Appraiser" && x.Element("role_logical_delete_flg").Value != "Y").ToArray();
			if (xRole.Length == 0)
			{
				logger.LogError("Appraiser role was not found in NGP response");
			}
			else if (xRole.Length > 1)
			{
				logger.LogError("More than one appraiser role was found in NGP response");
			}

			return xRole.Single().Element("ext_identity").Value;
		}
	}
}