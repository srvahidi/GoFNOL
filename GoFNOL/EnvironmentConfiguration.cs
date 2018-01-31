using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace GoFNOL
{
    public interface IEnvironmentConfiguration
    {
        string EAIEndpoint { get; }
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
    }
}