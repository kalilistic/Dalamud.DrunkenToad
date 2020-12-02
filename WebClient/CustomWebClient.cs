// ReSharper disable PossibleNullReferenceException

using System;
using System.Net;

namespace PriceCheck
{
	internal class CustomWebClient : WebClient
	{
		protected override WebRequest GetWebRequest(Uri uri)
		{
			var webRequest = base.GetWebRequest(uri);
			webRequest.Timeout = (int) TimeSpan.FromSeconds(10).TotalMilliseconds;
			return webRequest;
		}
	}
}