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

		private readonly Lazy<JObject> _jVcap;

		public EnvironmentConfiguration(IConfiguration configuration)
		{
			_jVcap = new Lazy<JObject>(() => JObject.Parse(configuration[VCAPKey]));
		}

		public string EAIEndpoint => _jVcap.Value["user-provided"].First(s => s["name"].Value<string>() == "EAI")["credentials"]["endpoint"].Value<string>();

		public string EAIUsername => _jVcap.Value["user-provided"].First(s => s["name"].Value<string>() == "EAI")["credentials"]["username"].Value<string>();

		public string EAIPassword => _jVcap.Value["user-provided"].First(s => s["name"].Value<string>() == "EAI")["credentials"]["password"].Value<string>();

		public string ISEndpoint => _jVcap.Value["user-provided"].First(s => s["name"].Value<string>() == "IS")["credentials"]["endpoint"].Value<string>();

		public string NGPUsersEndpoint => _jVcap.Value["user-provided"].First(s => s["name"].Value<string>() == "NGP")["credentials"]["endpoint"].Value<string>();

		public string DbConnectionString => _jVcap.Value.Properties().First(p=>p.Name.Contains("mongodb")).Value[0]["credentials"]["uri"].Value<string>();

		public Uri A2EDataDiscoveryUri
		{
			get
			{
				var value = _jVcap.Value["user-provided"].FirstOrDefault(s => s["name"].Value<string>() == "a2e-data")?["credentials"]["DiscoveryUri"].Value<string>();
				return value != null ? new Uri(value) : null;
			}
		}
	}
}