using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using static System.String;

namespace DalamudPluginCommon
{
	public static class StringExtensions
	{
		private static readonly List<KeyValuePair<string, string>> SanitizeList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("\u0002\u001F\u0001\u0003", "-"),
			new KeyValuePair<string, string>("\u0002\u001a\u0002\u0002\u0003", Empty),
			new KeyValuePair<string, string>("\u0002\u001a\u0002\u0001\u0003", Empty)
		};


		public static string Compress(this string value)
		{
			string compressed;
			using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
			{
				using (var compressedMemoryStream = new MemoryStream())
				{
					var gzipStream = new GZipStream(compressedMemoryStream, CompressionMode.Compress);
					memoryStream.CopyTo(gzipStream);
					gzipStream.Dispose();
					compressed = Convert.ToBase64String(compressedMemoryStream.ToArray());
				}
			}

			return compressed;
		}

		public static string Decompress(this string value)
		{
			string decompressed;
			using (var memoryStream = new MemoryStream(Convert.FromBase64String(value)))
			{
				using (var decompressedMemoryStream = new MemoryStream())
				{
					var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
					gzipStream.CopyTo(decompressedMemoryStream);
					gzipStream.Dispose();

					decompressed = Encoding.UTF8.GetString(decompressedMemoryStream.ToArray());
				}
			}

			return decompressed;
		}

		public static string Sanitize(this string value)
		{
			var sanitizedValue = new StringBuilder(value);
			foreach (var item in SanitizeList) sanitizedValue.Replace(item.Key, item.Value);

			return sanitizedValue.ToString();
		}
	}
}