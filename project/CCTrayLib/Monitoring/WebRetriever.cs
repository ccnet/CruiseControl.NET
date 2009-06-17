using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class WebRetriever : IWebRetriever
	{
		public string Get(Uri uri)
		{
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

			WebRequest request = WebRequest.Create(uri);
			if (uri.UserInfo != string.Empty)
			{
				request.Credentials = BasicAuthentication(uri);
			}
			using (WebResponse response = request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					StreamReader streamReader = new StreamReader(responseStream);
					return streamReader.ReadToEnd();
				}
			}
		}

		private ICredentials BasicAuthentication(Uri uri)
		{
			string username = uri.UserInfo.Split(':')[0];
			string password = uri.UserInfo.Split(':')[1];

			NetworkCredential myCred = new NetworkCredential(username, password);
			CredentialCache myCache = new CredentialCache();
			myCache.Add(uri, "Basic", myCred);

			return myCache;
		}

		public string Post(Uri uri, NameValueCollection input)
		{
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
			WebRequest request = WebRequest.Create(uri);
			string strInput = GetInputString(input);
			
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = strInput.Length;
			
			Stream writeStream = request.GetRequestStream();
			System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
			byte[] bytes = encoding.GetBytes(strInput);
			writeStream.Write(bytes, 0, bytes.Length);
			writeStream.Close();

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(responseStream);
                    return streamReader.ReadToEnd();
                }
            }
		}
		
		private static string GetInputString(NameValueCollection input)
		{
			string strInput = string.Empty;
			
			for (int i = 0; i < input.Keys.Count; i++)
			{
				if (strInput.Length == 0)
				{
					strInput = string.Format("{0}={1}", input.Keys[i], input[input.Keys[i]]);
				}
				else
				{
					strInput += string.Format("&{0}={1}", input.Keys[i], input[input.Keys[i]]);
				}
			}

			return strInput;
		}
	}
}
