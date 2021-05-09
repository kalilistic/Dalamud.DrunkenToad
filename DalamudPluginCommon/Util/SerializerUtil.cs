using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DalamudPluginCommon
{
    /// <summary>
    /// Json Serializer.
    /// </summary>
    public static class SerializerUtil
    {
        /// <summary>
        /// Create camel case serializer.
        /// </summary>
        /// <returns>Json Serializer with camel case.</returns>
        public static JsonSerializerSettings CamelCaseJsonSerializer()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
        }

        /// <summary>
        /// Create camel case serializer and include null/defaults.
        /// </summary>
        /// <returns>Json Serializer with camel case with null/defaults.</returns>
        public static JsonSerializerSettings CamelCaseIncludeJsonSerializer()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,
            };
        }
    }
}
