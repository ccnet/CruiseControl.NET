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
            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(responseStream);
                    return streamReader.ReadToEnd();
                }
            }
        }
		
		public void Post(Uri uri, NameValueCollection input)
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
		}
		
		private static string GetInputString(NameValueCollection input)
		{
			string strInput = "";
			
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
