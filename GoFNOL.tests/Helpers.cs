using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoFNOL.tests
{
    public static class Helpers
    {
        public static async Task<XDocument> GetClaimDocument(string wapk)
        {
            var falconClient = new HttpClient();
            var getClaimRequest = new StringContent($"<Request><Header><Action>OONShopGetAssignment</Action></Header><ServiceInput><CreatedForProfileId>4774PE200001</CreatedForProfileId><WorkAssignmentID>{wapk}</WorkAssignmentID><GUID/></ServiceInput></Request>");

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
            return CreateTestServer(collection => { });
        }

        public static TestServer CreateTestServer(Action<IServiceCollection> configureCustomServices)
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (currentDirectory.Name != "GoFNOL.tests")
            {
                currentDirectory = currentDirectory.Parent;
            }

            var contentRoot = Path.Combine(currentDirectory.Parent.FullName, "GoFNOL");
            return new TestServer(new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddEnvironmentVariables();
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddUserSecrets("GoFNOL");
                    }
                })
                .ConfigureServices(configureCustomServices)
                .UseStartup<Startup>());
        }

        public static XDocument ParseAssignment(string data)
        {
            var xRequest = XDocument.Parse(data);
            var soapNs = (XNamespace)"http://schemas.xmlsoap.org/soap/envelope/";
            var adpNs = (XNamespace)"http://csg.adp.com";
            var innerPayload = xRequest.Element(soapNs + "Envelope")
                .Element(soapNs + "Body")
                .Element(adpNs + "Transmit")
                .Element(adpNs + "parameters")
                .Value;
            return XDocument.Parse(innerPayload);
        }
    }
}