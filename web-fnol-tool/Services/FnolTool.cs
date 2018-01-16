using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using web_fnol_tool.Models;

namespace FnolTools
{
    public class Claim
    {
        public string ClaimNumber { get; set; }
        public string WorkAssignmentId { get; set; }
        public string CreatedForProfileId { get; set; }
    }

    public class FnolTool
    {
        private readonly HttpClient client;

        public FnolTool()
        {
            client = new HttpClient();
        }

        public async Task<Claim> CreateAssignment(FNOLRequest request)
        {
            var fnolData = SetAssignmentValues(request);
            var result = await ExecuteRequest(fnolData);
            return new Claim
            {
                ClaimNumber = request.ClaimNumber,
                WorkAssignmentId = Regex.Match(result, @"ADP_TRANSACTION_ID&gt;(\w+)&lt;/ADP_TRANSACTION_ID").Groups[1].Value,
                CreatedForProfileId = "477T2PPCOMPAPP2"
            };
        }

        private XDocument SetAssignmentValues(FNOLRequest request)
        {
            var xRequest = XDocument.Parse(ReadResoruce("web-fnol-tool.Assets.assignment.xml"));
            xRequest.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/CLAIM_NBR").Value = request.ClaimNumber;
            xRequest.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_FIRST_NAME").Value = request.Owner.FirstName;
            xRequest.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_LAST_NAME").Value = request.Owner.LastName;
            xRequest.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CONTACT_PHONE_NBR_3").Value = request.Owner.PhoneNumber;
            xRequest.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_POSTAL_CODE").Value = request.Owner.Address.PostalCode;
            xRequest.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_STATE").Value = request.Owner.Address.State;
            xRequest.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/OWNER_CONTACT_EMAIL").Value = request.Owner.Email;
            xRequest.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/VEHICLE_VIN").Value = request.VIN;
            xRequest.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/LOSS_TYPE").Value = request.LossType;
            xRequest.XPathSelectElement("//ADP_FNOL_ASGN_INPUT/CLAIM/DEDUCTIBLE_AMT").Value = request.Deductible;
            return xRequest;
        }

        private async Task<string> ExecuteRequest(XDocument payload)
        {
            var request = XDocument.Parse(ReadResoruce("web-fnol-tool.Assets.request.xml"));
            var soapNs = (XNamespace) "http://schemas.xmlsoap.org/soap/envelope/";
            var adpNs = (XNamespace) "http://csg.adp.com";
            request.Element(soapNs + "Envelope")
                .Element(soapNs + "Body")
                .Element(adpNs + "Transmit")
                .Element(adpNs + "parameters")
                .Value = payload.ToString();

            var content = new StringContent(request.ToString(), Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", "http://csg.adp.com/Transmit");
            using (var response = await client.PostAsync(new Uri("http://sys-arch-svr12.pdlab.adp.com/wsproxy/ws_proxy.asmx"), content))
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        private static string ReadResoruce(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = assembly.GetManifestResourceStream(name);
            var streamreader = new StreamReader(resource, Encoding.UTF8);
            var readstring = streamreader.ReadToEnd();
            return readstring;
        }
    }
}