using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.XPath;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace GoFNOL.tests.E2E
{
    public class AppTests
    {
        [Fact]
        public async Task GoFNOL_WhenPOSTingForm_ShouldCallEAIAndDisplayWAID()
        {
            // Setup
            var client = Helpers.CreateTestServer().CreateClient();
            var claimNumber = ((int) DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString("D");
            const string ownerFirstName = "Ernest";
            const string ownerLastName = "Hemingway";
            const string ownerPhoneNumber = "(212)736-5000";
            const string ownerZipCode = "33040";
            const string ownerZipAddon = "7473";
            const string ownerState = "FL";
            const string ownerEmail = "e.hemi@19th.com";
            const string vin = "WP0ZZZ99ZTS392124";
            const string lossType = "COLL";
            const string deductible = "100500";
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("mobile-flow-ind", "N"),
                new KeyValuePair<string, string>("claim-number", claimNumber),
                new KeyValuePair<string, string>("first-name", ownerFirstName),
                new KeyValuePair<string, string>("last-name", ownerLastName),
                new KeyValuePair<string, string>("phone-number", ownerPhoneNumber),
                new KeyValuePair<string, string>("zip-code", ownerZipCode),
                new KeyValuePair<string, string>("zip-addon", ownerZipAddon),
                new KeyValuePair<string, string>("state", ownerState),
                new KeyValuePair<string, string>("email", ownerEmail),
                new KeyValuePair<string, string>("vin", vin),
                new KeyValuePair<string, string>("loss-type", lossType),
                new KeyValuePair<string, string>("deductible", deductible)
            });

            // Execute
            var response = await client.PostAsync("/fnol", formContent);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location.OriginalString.Should().Be("/");
            var getRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            if (response.Headers.TryGetValues("Set-Cookie", out var cookieHeaderValue))
            {
                foreach (var cookie in SetCookieHeaderValue.ParseList(cookieHeaderValue.ToList()))
                {
                    getRequest.Headers.Add("Cookie", new CookieHeaderValue(cookie.Name, cookie.Value).ToString());
                }
            }
            var redirectResponse = await client.SendAsync(getRequest);
            var html = await redirectResponse.Content.ReadAsStringAsync();
            var wapk = Helpers.ParseWorkAssignmentPK(html);
            wapk.Should().NotBeEmpty();
            var claimNo = Helpers.ParseClaimNumber(html);
            claimNo.Should().NotBeEmpty();
            var xClaim = await Helpers.GetClaimDocument(wapk);
            xClaim.XPathSelectElement("//Response/Claim/Estimates/Estimate[1]/ClaimNo").Value.Should().Be(claimNumber);
            xClaim.XPathSelectElement("//Response/Claim/Estimates/Estimate[1]/Owner/Party/First").Value.Should().Be(ownerFirstName);
            xClaim.XPathSelectElement("//Response/Claim/Estimates/Estimate[1]/Owner/Party/Last").Value.Should().Be(ownerLastName);
            xClaim.XPathSelectElement("//Response/Claim/Estimates/Estimate[1]/Owner/Party/Fax").Value.Should().Be(ownerPhoneNumber);
            xClaim.XPathSelectElement("//Response/Claim/Estimates/Estimate[1]/Owner/Party/Postal").Value.Should().Be($"{ownerZipCode}-{ownerZipAddon}");
            xClaim.XPathSelectElement("//Response/Claim/Estimates/Estimate[1]/Owner/Party/State").Value.Should().Be(ownerState);
            xClaim.XPathSelectElement("//Response/Claim/Estimates/Estimate[1]/Vehicle/AssignmentVIN").Value.Should().Be(vin);
            xClaim.XPathSelectElement("//Response/Claim/Estimates/Estimate[1]/LossType").Value.Should().Be(lossType);
            xClaim.XPathSelectElement("//Response/Claim/Estimates/Estimate[1]/Payers/Payer[1]/Totals/Deductible").Value.Should().Be($"{deductible}00");
        }
    }
}