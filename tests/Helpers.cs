using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using web_fnol_tool;

namespace tests
{
    public static class Helpers
    {
        public static async Task<XDocument> GetClaimDocument(string wapk)
        {
            var falconClient = new HttpClient();
            var getClaimRequest = new StringContent($"<Request><Header><Action>OONShopGetAssignment</Action></Header><ServiceInput><CreatedForProfileId>477T2PPCOMPAPP2</CreatedForProfileId><WorkAssignmentID>{wapk}</WorkAssignmentID><GUID/></ServiceInput></Request>");

            getClaimRequest.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var getClaimResponse = await falconClient.PostAsync("http://dev-adxe-4.pdlab.adp.com/FalconApp/Mobile.asp", getClaimRequest);
            var claim = await getClaimResponse.Content.ReadAsStringAsync();
            return XDocument.Parse(claim);
        }

        public static string ParseWorkAssignmentPK(string html)
        {
            const string encodedQuote = "&#x27;";
            var startPos = html.IndexOf(encodedQuote, html.IndexOf("Work Assignment ID:")) + encodedQuote.Length;
            var endPos = html.IndexOf(encodedQuote, startPos);
            return html.Substring(startPos, endPos - startPos);
        }

        public static TestServer CreateTestServer()
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (currentDirectory.Name != "web-fnol-tool")
            {
                currentDirectory = currentDirectory.Parent;
            }

            var contentRoot = Path.Combine(currentDirectory.FullName, "web-fnol-tool");
            return new TestServer(new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .UseStartup<Startup>());
        }
    }
}