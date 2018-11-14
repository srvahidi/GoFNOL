﻿using System;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GoFNOL.tests
{
	public static class Helpers
	{
		public static void ShouldBeEquivalentTo(this JToken actual, JToken expected)
		{
			var result = JToken.DeepEquals(actual, expected);
			Assert.True(result, $"expected {actual.ToString()} to equal {expected.ToString()}");
		}

		public static TestServer CreateTestServer(Action<IServiceCollection> configureCustomServices)
		{
			return new TestServer(new WebHostBuilder()
				.ConfigureAppConfiguration((context, builder) =>
				{
					builder.AddEnvironmentVariables();
					if (context.HostingEnvironment.IsDevelopment())
					{
						builder.AddUserSecrets("GoFNOL");
					}
				})
				.ConfigureServices(configureCustomServices)
				.UseStartup<TestStartup>());
		}

		public static XDocument ParseAssignment(string data)
		{
			var xRequest = XDocument.Parse(data);
			var soapNs = (XNamespace) "http://schemas.xmlsoap.org/soap/envelope/";
			var adpNs = (XNamespace) "http://csg.adp.com";
			var innerPayload = xRequest.Element(soapNs + "Envelope")
				.Element(soapNs + "Body")
				.Element(adpNs + "Transmit")
				.Element(adpNs + "parameters")
				.Value;
			return XDocument.Parse(innerPayload);
		}

		public static (string username, string password) ParseCredentials(string eaiContent)
		{
			var xRequest = XDocument.Parse(eaiContent);
			var soapNs = (XNamespace)"http://schemas.xmlsoap.org/soap/envelope/";
			var adpNs = (XNamespace)"http://csg.adp.com";
			var xHeader = xRequest.Element(soapNs + "Envelope")
				.Element(soapNs + "Header")
				.Element(adpNs + "SOAPHeader");
			return (xHeader.Element(adpNs + "Client").Value, xHeader.Element(adpNs + "Passphrase").Value);
		}
	}
}