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
				}
			};
		}
	}
}