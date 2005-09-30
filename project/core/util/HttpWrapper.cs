using System;
using System.Net;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	/// <summary>
	/// Used to wrap calls to HttpWebRequest.
	/// </summary>
	public class HttpWrapper
	{
		public virtual DateTime GetLastModifiedTimeFor(Uri url, DateTime previousModifiedTime)
		{
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
			request.ProtocolVersion = HttpVersion.Version11;
			request.IfModifiedSince = previousModifiedTime;

			try
			{
				using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
				{
					if (response.Headers["Last-Modified"] == null) return previousModifiedTime;
					return response.LastModified;
				}
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					// Get HttpWebResponse so that you can check the HTTP status code.
					// If we get a not modified back then it was still successful.
					HttpWebResponse httpResponse = (HttpWebResponse) ex.Response;
					if (httpResponse.StatusCode == HttpStatusCode.NotModified)
						return previousModifiedTime;
				}
				throw;
			}
		}
	}
}