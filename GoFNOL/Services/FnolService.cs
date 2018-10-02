using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using GoFNOL.Models;

namespace GoFNOL.Services
{
	public class FNOLService : IFNOLService
	{
		private const string _AssignmentResourceName = "GoFNOL.Assets.assignment.xml";

		private const string _EAIRequestResourceName = "GoFNOL.Assets.request.xml";

		private const string _FixedContactPhoneType = "CP";

		private readonly XNamespace _SOAPNamespace = "http://schemas.xmlsoap.org/soap/envelope/";

		private readonly XNamespace _ADPNamespace = "http://csg.adp.com";

		private readonly IHTTPService _Client;

		private readonly IEnvironmentConfiguration _EnvironmentConfiguration;

		public FNOLService(IHTTPService client, IEnvironmentConfiguration environmentConfiguration)
		{
			_Client = client;
			_EnvironmentConfiguration = environmentConfiguration;
		}

		/// <summary>
		/// Returns the current date using Mountain timezone in the format "yyyy-MM-dd"
		/// </summary>
		public string FormattedLossDate => DateTime.UtcNow.ToString("yyyy-MM-dd");

		public async Task<Claim> CreateAssignment(FNOLRequest fnolRequest)
		{
			var fnolData = SetAssignmentValues(fnolRequest);
			var eaiResponseString = await ExecuteEAIRequest(fnolData);

			return new Claim
			{
				ClaimNumber = fnolRequest.ClaimNumber,
				WorkAssignmentId = Regex.Match(eaiResponseString, @"ADP_TRANSACTION_ID&gt;(\w+)&lt;/ADP_TRANSACTION_ID").Groups[1].Value,
				CreatedForProfileId = fnolRequest.CreatedForProfileId
			};
		}

		private async Task<string> ExecuteEAIRequest(XDocument payload)
		{
			var xEAIRequest = XDocument.Parse(ReadResource(_EAIRequestResourceName));

			xEAIRequest.Element(_SOAPNamespace + "Envelope")
				.Element(_SOAPNamespace + "Body")
				.Element(_ADPNamespace + "Transmit")
				.Element(_ADPNamespace + "parameters")
				.Value = payload.ToString();

			using (var response = await _Client.PostAsync(new Uri(_EnvironmentConfiguration.EAIEndpoint), new EAIRequest(xEAIRequest.ToString())))
			{
				return await response.Content.ReadAsStringAsync();
			}
		}

		private XDocument SetAssignmentValues(FNOLRequest fnolRequest)
		{
			var xAssignment = XDocument.Parse(ReadResource(_AssignmentResourceName));

			// NOTE: Processing meta information
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/MOBILE_FLOW_IND").Value = fnolRequest.MobileFlowIndicator;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/COMPANY_ID").Value = fnolRequest.CreatedForProfileId.Substring(0,3);
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/OFFICE_ID").Value = fnolRequest.CreatedForProfileId.Substring(0,7);
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/USER_ID").Value = fnolRequest.CreatedForProfileId;

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
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_POSTAL_CODE").Value = fnolRequest.Owner.Address.PostalCode;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_STATE").Value = fnolRequest.Owner.Address.State;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CONTACT_EMAIL").Value = fnolRequest.Owner.Email;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_LAST_NAME").Value = fnolRequest.Owner.LastName;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_FIRST_NAME").Value = fnolRequest.Owner.FirstName;
			xAssignment.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/ALTERNATE_CONTACT_PHONE_TYPE_1").Value = _FixedContactPhoneType;
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