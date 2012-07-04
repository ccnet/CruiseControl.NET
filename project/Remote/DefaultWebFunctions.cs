namespace ThoughtWorks.CruiseControl.Remote
{
    using System;
    using System.Net;
    using System.Text;

    public class DefaultWebFunctions : IWebFunctions
    {
        /// <summary>
        /// Sets credentials on client if address contains user info.
        /// </summary>
        /// <param name="webClient">The <see cref="WebClient"/> to set credentials on.</param>
        /// <param name="address">The address to check for user info.</param>
        /// <param name="forceAuthorization">Whether to force an Authorization header or allow WebClient credentials to handle it.</param>
        public void SetCredentials(WebClient webClient, Uri address, bool forceAuthorization)
        {
            if (address.UserInfo.Length <= 0) return;

            var userInfoValues = address.UserInfo.Split(':');
            var credentials = new NetworkCredential
                                  {
                                      UserName = userInfoValues[0]
                                  };

            if (userInfoValues.Length > 1)
                credentials.Password = userInfoValues[1];

            if (forceAuthorization)
                webClient.Headers.Add("Authorization", GenerateAuthorizationFromCredentials(credentials));
            else
                webClient.Credentials = credentials;
        }

        /// <summary>
        /// Generates the body of a Basic HTTP Authorization header.
        /// </summary>
        /// <param name="credentials">The <see cref="NetworkCredential"/> to pull username and password from."/></param>
        /// <returns>The body of a basic HTTP Authorization header.</returns>
        private static string GenerateAuthorizationFromCredentials(NetworkCredential credentials)
        {
            string credentialsText = String.Format("{0}:{1}", credentials.UserName, credentials.Password);
            byte[] bytes = Encoding.ASCII.GetBytes(credentialsText);
            string base64 = Convert.ToBase64String(bytes);
            return String.Concat("Basic ", base64);
        }
    }
}