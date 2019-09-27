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

		string DbConnectionString { get; }

		bool DisableAuth { get; }
	}

	public class EnvironmentConfiguration : IEnvironmentConfiguration
	{
		private const string VCAPKey = "VCAP_SERVICES";

		private readonly Lazy<JObject> _jVcap;

		public EnvironmentConfiguration(IConfiguration configuration)
		{
			_jVcap = new Lazy<JObject>(() => JObject.Parse(configuration[VCAPKey]));
			DisableAuth = string.Equals(configuration["DISABLE_AUTH"], bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
		}

		public string EAIEndpoint => _jVcap.Value["user-provided"].First(s => s["name"].Value<string>() == "EAI")["credentials"]["url"].Value<string>();

		public string EAIUsername => _jVcap.Value["user-provided"].First(s => s["name"].Value<string>() == "EAI")["credentials"]["username"].Value<string>();

		public string EAIPassword => _jVcap.Value["user-provided"].First(s => s["name"].Value<string>() == "EAI")["credentials"]["password"].Value<string>();

		public string ISEndpoint => _jVcap.Value["user-provided"].First(s => s["name"].Value<string>() == "IS")["credentials"]["url"].Value<string>();

		public string NGPUsersEndpoint => _jVcap.Value["user-provided"].First(s => s["name"].Value<string>() == "NGP")["credentials"]["url"].Value<string>();

		public string DbConnectionString => _jVcap.Value.Properties().First(p => p.Name.Contains("mongodb")).Value[0]["credentials"]["uri"].Value<string>();

		public bool DisableAuth { get; }
	}
}