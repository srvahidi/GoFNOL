using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using GoFNOL.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GoFNOL.Services
{
	public class FNOLService : IFNOLService
	{
		private const string AssignmentResourceName = "GoFNOL.Assets.assignment.xml";

		private const string EAIRequestResourceName = "GoFNOL.Assets.request.xml";

		private readonly XNamespace _soapNamespace = "http://schemas.xmlsoap.org/soap/envelope/";

		private readonly XNamespace _adpNamespace = "http://csg.adp.com";

		private readonly IHTTPService _client;

		private readonly IEnvironmentConfiguration _environmentConfiguration;

		private readonly ILogger<FNOLService> _logger;

		public FNOLService(IHTTPService client, IEnvironmentConfiguration environmentConfiguration, ILogger<FNOLService> logger)
		{
			_client = client;
			_environmentConfiguration = environmentConfiguration;
			_logger = logger;
		}

		/// <summary>
		/// Returns the current date using Mountain timezone in the format "yyyy-MM-dd"
		/// </summary>
		public string FormattedLossDate => DateTime.UtcNow.ToString("yyyy-MM-dd");

		public async Task<FNOLResponse> CreateAssignment(FNOLRequest fnolRequest)
		{
			_logger.LogInformation($"Creating an assignment for request {JsonConvert.SerializeObject(fnolRequest, Formatting.None)}");
			var fnolData = SetAssignmentValues(fnolRequest);
			var eaiResponseString = await ExecuteEAIRequest(fnolData);

			var workAssignmentId = Regex.Match(eaiResponseString, @"ADP_TRANSACTION_ID&gt;(\w+)&lt;/ADP_TRANSACTION_ID").Groups[1].Value;
			_logger.LogInformation($"New assignment waId = '{workAssignmentId}'");
			if (string.IsNullOrEmpty(workAssignmentId)) throw new EAIException();
			return new FNOLResponse(workAssignmentId);
		}

		private async Task<string> ExecuteEAIRequest(XDocument payload)
		{
			var xEAIRequest = XDocument.Parse(ReadResource(EAIRequestResourceName));

			xEAIRequest.Element(_soapNamespace + "Envelope")
				.Element(_soapNamespace + "Body")
				.Element(_adpNamespace + "Transmit")
				.Element(_adpNamespace + "parameters")
				.Value = payload.ToString();

			var xHeader = xEAIRequest.Element(_soapNamespace + "Envelope")
				.Element(_soapNamespace + "Header")
				.Element(_adpNamespace + "SOAPHeader");

			xHeader.Element(_adpNamespace + "Client").Value = _environmentConfiguration.EAIUsername;
			xHeader.Element(_adpNamespace + "Passphrase").Value = _environmentConfiguration.EAIPassword;

			using (var response = await _client.PostAsync(new Uri(_environmentConfiguration.EAIEndpoint), new EAIRequest(xEAIRequest.ToString())))
			{
				_logger.LogInformation($"EAI response status = {response.StatusCode}");
				return await response.Content.ReadAsStringAsync();
			}
		}

		private XDocument SetAssignmentValues(FNOLRequest fnolRequest)
		{
			var xAssignment = XDocument.Parse(ReadResource(AssignmentResourceName));

			// NOTE: Processing meta information
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/MOBILE_FLOW_IND").Value = fnolRequest.MobileFlowIndicator;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/COMPANY_ID").Value = fnolRequest.ProfileId.Substring(0, 3);
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/OFFICE_ID").Value = fnolRequest.ProfileId.Substring(0, 7);
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/USER_ID").Value = fnolRequest.ProfileId;

			// NOTE: Claim information
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/CLAIM_NBR").Value = fnolRequest.ClaimNumber;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/VEHICLE_VIN").Value = fnolRequest.VIN;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/LOSS_TYPE").Value = fnolRequest.LossType;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/DEDUCTIBLE_AMT").Value = fnolRequest.Deductible;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/LOSS_DATE").Value = FormattedLossDate;

			// NOTE: Owner information
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_FIRST_NAME").Value = fnolRequest.Owner.FirstName;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_LAST_NAME").Value = fnolRequest.Owner.LastName;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CONTACT_PHONE_NBR_3").Value = fnolRequest.Owner.PhoneNumber;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_POSTAL_CODE").Value = fnolRequest.Owner.Address.ZIPCode;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CITY").Value = fnolRequest.Owner.Address.City;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_STATE").Value = fnolRequest.Owner.Address.State;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CONTACT_EMAIL").Value = fnolRequest.Owner.Email;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_LAST_NAME").Value = fnolRequest.Owner.LastName;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_FIRST_NAME").Value = fnolRequest.Owner.FirstName;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_PHONE_NBR_1").Value = fnolRequest.Owner.PhoneNumber;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_EMAIL").Value = fnolRequest.Owner.Email;

			return xAssignment;
		}

		private static string ReadResource(string name)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceStream = assembly.GetManifestResourceStream(name);
			var streamReader = new StreamReader(resourceStream, Encoding.UTF8);
			var allStreamContent = streamReader.ReadToEnd();

			return allStreamContent;
		}
	}
}