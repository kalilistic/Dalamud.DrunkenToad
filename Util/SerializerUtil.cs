using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DalamudPluginCommon
{
    public static class SerializerUtil
    {
        public static JsonSerializerSettings CamelCaseJsonSerializer()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
        }

        public static JsonSerializerSettings CamelCaseIncludeJsonSerializer()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }
    }
}