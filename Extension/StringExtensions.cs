using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DalamudPluginCommon
{
	public static class StringExtensions
	{
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
	}
}