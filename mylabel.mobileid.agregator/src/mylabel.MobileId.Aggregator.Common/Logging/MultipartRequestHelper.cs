using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace mylabel.MobileId.Aggregator.Common.Logging
{
	public static class MultipartRequestHelper
	{
		// Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
		// The spec says 70 characters is a reasonable limit.
		public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
		{
			var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary);
			if (string.IsNullOrWhiteSpace(boundary.Value))
			{
				throw new InvalidDataException("Missing content-type boundary.");
			}

			if (boundary.Length > lengthLimit)
			{
				throw new InvalidDataException(
					$"Multipart boundary length limit {lengthLimit} exceeded.");
			}

			return boundary.Value;
		}

		/// <summary>
		/// check multipart/ content-type
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		public static bool IsMultipartContentType(string contentType)
		{
			return !string.IsNullOrEmpty(contentType)
				   && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}
}
