using Newtonsoft.Json;

namespace SandS.Tests.Base.Settings
{
    internal class TestSettingsModel
    {
        [JsonProperty("assembly-name-prefix")]
        public string AssemblyNamePrefix { get; set; }

        [JsonProperty("namespace-prefix")]
        public string NamespacePrefix { get; set; }
    }
}