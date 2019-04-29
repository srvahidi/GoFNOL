using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace GoFNOL
{
	public interface IEnvironmentConfiguration
	{
		string EAIEndpoint { get; }

		string EAIUsername { get; }

		string EAIPassword { get; }

		string ISEndpoint { get; }

		string NGPUsersEndpoint { get; }

		Uri A2EDataDiscoveryUri { get; }

		string DbConnectionString { get; }
	}

	public class EnvironmentConfiguration : IEnvironmentConfiguration
	{
		private const string VCAPKey = "VCAP_SERVICES";

		private readonly Lazy<JToken> jvcap;

		public EnvironmentConfiguration(IConfiguration configuration)
		{
			jvcap = new Lazy<JToken>(() => JObject.Parse(configuration[VCAPKey]));
		}

		public string EAIEndpoint => jvcap.Value["user-provided"].First(s => s["name"].Value<string>() == "EAI")["credentials"]["endpoint"].Value<string>();

		public string EAIUsername => jvcap.Value["user-provided"].First(s => s["name"].Value<string>() == "EAI")["credentials"]["username"].Value<string>();

		public string EAIPassword => jvcap.Value["user-provided"].First(s => s["name"].Value<string>() == "EAI")["credentials"]["password"].Value<string>();

		public string ISEndpoint => jvcap.Value["user-provided"].First(s => s["name"].Value<string>() == "IS")["credentials"]["endpoint"].Value<string>();

		public string NGPUsersEndpoint => jvcap.Value["user-provided"].First(s => s["name"].Value<string>() == "NGP")["credentials"]["endpoint"].Value<string>();

		public string DbConnectionString => jvcap.Value["mongodb"].First()["credentials"]["uri"].Value<string>();

		public Uri A2EDataDiscoveryUri
		{
			get
			{
				var value = jvcap.Value["user-provided"].FirstOrDefault(s => s["name"].Value<string>() == "a2e-data")?["credentials"]["DiscoveryUri"].Value<string>();
				return value != null ? new Uri(value) : null;
			}
		}
	}
}