using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// A login request  message.
    /// </summary>
    [XmlRoot("loginMessage")]
    [Serializable]
    public class LoginRequest
        : ServerRequest
    {
        #region Public consts
        #region UserNameCredential
        /// <summary>
        /// The credential name for a user name.
        /// </summary>
        public const string UserNameCredential = "userName";
        #endregion

        #region PasswordCredential
        /// <summary>
        /// The credential name for a password.
        /// </summary>
        public const string PasswordCredential = "password";
        #endregion

        #region TypeCredential
        /// <summary>
        /// The credential name for a type.
        /// </summary>
        public const string TypeCredential = "type";
        #endregion

        #region DomainCredential
        /// <summary>
        /// The credential name for a domain.
        /// </summary>
        public const string DomainCredential = "domain";
        #endregion
        #endregion

        #region Private fields
        private List<NameValuePair> credentials = new List<NameValuePair>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new empty <see cref="LoginRequest"/>.
        /// </summary>
        public LoginRequest()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="LoginRequest"/> with a user name.
        /// </summary>
        /// <param name="userName"></param>
        public LoginRequest(string userName)
        {
            credentials.Add(new NameValuePair(LoginRequest.UserNameCredential, userName));
        }
        #endregion

        #region Public properties
        #region Credentials
        /// <summary>
        /// The credentials to use in logging in.
        /// </summary>
        [XmlElement("credential")]
        public List<NameValuePair> Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region AddCredential
        /// <summary>
        /// Adds a new credential.
        /// </summary>
        /// <param name="name">The name of the credential.</param>
        /// <param name="value">The value of the credential.</param>
        /// <returns>The new credential.</returns>
        public NameValuePair AddCredential(string name, string value)
        {
            NameValuePair credential = new NameValuePair(name, value);
            credentials.Add(credential);
            return credential;
        }
        #endregion
        #endregion
    }
}
