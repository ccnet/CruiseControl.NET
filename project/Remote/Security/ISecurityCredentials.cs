using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Defines the security credentials to use for logging in.
    /// </summary>
    public interface ISecurityCredentials
    {
        /// <summary>
        /// An identifier for these credentials.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets or sets a security credential.
        /// </summary>
        /// <param name="credential">The credential name.</param>
        /// <returns></returns>
        string this[string credential] { get; set; }

        /// <summary>
        /// Serialises these credentials to a string.
        /// </summary>
        /// <returns>The serialised credentials.</returns>
        string Serialise();

        /// <summary>
        /// Deserialises the credentials.
        /// </summary>
        /// <param name="credentials">The credentials to deserialise.</param>
        void Deserialise(string credentials);

        /// <summary>
        /// Gets the currently set credentials.
        /// </summary>
        string[] Credentials { get; }
    }
}
