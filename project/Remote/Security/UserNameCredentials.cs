using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Define a default implement of the security credentials that allows the setting of a user name.
    /// </summary>
    [Serializable]
    public class UserNameCredentials
        : ISecurityCredentials
    {
        private const string userNameCredential = "username";
        [NonSerialized]
        private Dictionary<string, string> credentialsStore = new Dictionary<string,string>();
        private string asXml;

        /// <summary>
        /// Start a set of blank credentials.
        /// </summary>
        public UserNameCredentials()
        {
            credentialsStore.Add(userNameCredential, null);
        }

        /// <summary>
        /// Start a new set of credentials with a user name.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        public UserNameCredentials(string userName)
        {
            credentialsStore.Add(userNameCredential, userName);
        }

        /// <summary>
        /// The username in the credentials.
        /// </summary>
        public string UserName
        {
            get { return this[userNameCredential]; }
            set { this[userNameCredential] = value; }
        }

        /// <summary>
        /// An identifier for these credentials.
        /// </summary>
        public string Identifier
        {
            get { return UserName; }
        }

        /// <summary>
        /// Gets or sets a security credential.
        /// </summary>
        /// <param name="credential">The credential name.</param>
        /// <returns></returns>
        public string this[string credential]
        {
            get
            {
                // Always cast to lower case as we want to be case insensitive
                string key = credential.ToLowerInvariant();
                if (credentialsStore.ContainsKey(key))
                {
                    return credentialsStore[key];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                // Always cast to lower case as we want to be case insensitive
                string key = credential.ToLowerInvariant();
                if ((value == null) && (key != userNameCredential))
                {
                    if (credentialsStore.ContainsKey(key)) credentialsStore.Remove(key);
                }
                else
                {
                    if (credentialsStore.ContainsKey(key))
                    {
                        credentialsStore[key] = value;
                    }
                    else
                    {
                        credentialsStore.Add(key, value);
                    }
                }
            }
        }

        /// <summary>
        /// Serialises these credentials to a string.
        /// </summary>
        /// <returns>The serialised credentials.</returns>
        public string Serialise()
        {
            // Convert to an XML structure - use the XML DOM so everything is safe
            XmlDocument document = new XmlDocument();
            XmlElement rootNode = document.CreateElement("credentials");
            document.AppendChild(rootNode);
            foreach (string key in credentialsStore.Keys)
            {
                XmlElement keyNode = document.CreateElement("value");
                string value = credentialsStore[key];
                if (value == null) value = string.Empty;
                keyNode.SetAttribute("key", key);
                keyNode.SetAttribute("value", value);
                rootNode.AppendChild(keyNode);
            }
            return document.OuterXml;
        }

        /// <summary>
        /// Deserialises the credentials.
        /// </summary>
        /// <param name="credentials">The credentials to deserialise.</param>
        public void Deserialise(string credentials)
        {
            // Load the XML document
            XmlDocument document = new XmlDocument();
            document.LoadXml(credentials);

            // Wipe the current credentials
            if (credentialsStore == null)
            {
                credentialsStore = new Dictionary<string, string>();
            }
            else
            {
                credentialsStore.Clear();
            }
            credentialsStore.Add(userNameCredential, null);

            // Load each value
            foreach (XmlElement keyNode in document.SelectNodes("/credentials/value"))
            {
                string key = keyNode.GetAttribute("key");
                string value = keyNode.GetAttribute("value");
                // TODO: validate data
                this[key] = value;
            }
        }

        /// <summary>
        /// Gets the currently set credentials.
        /// </summary>
        public string[] Credentials
        {
            get
            {
                string[] credentials = new string[credentialsStore.Count];
                credentialsStore.Keys.CopyTo(credentials, 0);
                return credentials;
            }
        }

        [OnSerializing()]
        internal void OnSerializingMethod(StreamingContext context)
        {
            asXml = Serialise();
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Deserialise(asXml);
        }
    }
}
