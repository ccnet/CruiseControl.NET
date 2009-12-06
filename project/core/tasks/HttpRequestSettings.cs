//-----------------------------------------------------------------------
// <copyright file="HttpRequestSettings.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Net;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// <para>
    /// The settings for an HTTP request.
    /// </para>
    /// </summary>
    /// <title>HTTP Settings</title>
    /// <version>1.5</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;httpRequest&gt;
    /// &lt;uri&gt;http://somewhere.com&lt;/uri&gt;
    /// &lt;/httpRequest&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;httpRequest&gt;
    /// &lt;method&gt;GET&lt;/method&gt;
    /// &lt;uri&gt;http://somewhere.com/&lt;/uri&gt;
    /// &lt;useDefaultCredentials&gt;false&lt;/useDefaultCredentials&gt;
    /// &lt;/httpRequest&gt;
    /// </code>
    /// </example>
    [ReflectorType("httpRequest")]
    public class HttpRequestSettings
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestSettings"/> class.
        /// </summary>
        public HttpRequestSettings()
        {
            this.Method = "GET";
        }
        #endregion

        #region Public properties
        #region Method
        /// <summary>
        /// The method to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>GET</default>
        /// <remarks>
        /// This can be any valid HTTP method, e.g. GET, POST, etc.
        /// </remarks>
        [ReflectorProperty("method", Required = false)]
        public string Method { get; set; }
        #endregion

        #region HasMethod
        /// <summary>
        /// Gets a value indicating whether this instance has method.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has method; otherwise, <c>false</c>.
        /// </value>
        public bool HasMethod
        {
            get
            {
                return !StringUtil.IsWhitespace(this.Method);
            }
        }
        #endregion

        #region Headers
        /// <summary>
        /// The HTTP headers to send.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorArray("headers", Required = false)]
        public HttpRequestHeader[] Headers { get; set; }
        #endregion

        #region Body
        /// <summary>
        /// The body of the request to send.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("body", Required = false)]
        public string Body { get; set; }
        #endregion

        #region HasBody
        /// <summary>
        /// Gets a value indicating whether this instance has body.
        /// </summary>
        /// <value><c>true</c> if this instance has body; otherwise, <c>false</c>.</value>
        public bool HasBody
        {
            get
            {
                return !StringUtil.IsWhitespace(this.Body);
            }
        }
        #endregion

        #region HasSendFile
        /// <summary>
        /// Gets a value indicating whether this instance has send file.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has send file; otherwise, <c>false</c>.
        /// </value>
        public bool HasSendFile
        {
            get
            {
                return !StringUtil.IsWhitespace(this.SendFile);
            }
        }
        #endregion

        #region SendFile
        /// <summary>
        /// A file to send in the request.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("sendFile", Required = false)]
        public string SendFile { get; set; }
        #endregion

        #region Uri
        /// <summary>
        /// The URL to make the request to.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("uri", typeof(UriSerializerFactory), Required = true)]
        public Uri Uri { get; set; }
        #endregion

        #region Timeout
        /// <summary>
        /// The timeout period before cancelling the request.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("timeout", typeof(TimeoutSerializerFactory), Required = false)]
        public Timeout Timeout { get; set; }
        #endregion

        #region OverrideTimeout
        /// <summary>
        /// Gets or sets the override timeout.
        /// </summary>
        /// <value>The override timeout.</value>
        public Timeout OverrideTimeout { get; set; }
        #endregion

        #region HasTimeout
        /// <summary>
        /// Gets a value indicating whether this instance has timeout.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has timeout; otherwise, <c>false</c>.
        /// </value>
        public bool HasTimeout
        {
            get { return this.Timeout != null; }
        }
        #endregion

        #region ReadWriteTimeout
        /// <summary>
        /// The read/write timeout period.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("readWriteTimeout", typeof(TimeoutSerializerFactory), Required = false)]
        public Timeout ReadWriteTimeout { get; set; }
        #endregion

        #region HasReadWriteTimeout
        /// <summary>
        /// Gets a value indicating whether this instance has read write timeout.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has read write timeout; otherwise, <c>false</c>.
        /// </value>
        public bool HasReadWriteTimeout
        {
            get { return this.ReadWriteTimeout != null; }
        }
        #endregion

        #region Credentials
        /// <summary>
        /// The credentials to use in the request.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("credentials", typeof(NetworkCredentialSerializerFactory), Required = false)]
        public NetworkCredential Credentials { get; set; }
        #endregion

        #region HasCredentials
        /// <summary>
        /// Gets a value indicating whether this instance has credentials.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has credentials; otherwise, <c>false</c>.
        /// </value>
        public bool HasCredentials
        {
            get { return this.Credentials != null; }
        }
        #endregion

        #region HasHeaders
        /// <summary>
        /// Gets a value indicating whether this instance has headers.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has headers; otherwise, <c>false</c>.
        /// </value>
        public bool HasHeaders
        {
            get { return (this.Headers != null) && (this.Headers.Length > 0); }
        }
        #endregion

        #region UseDefaultCredentials
        /// <summary>
        /// Whether to use the default credentials or not.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("useDefaultCredentials", Required = false)]
        public bool UseDefaultCredentials { get; set; }
        #endregion

        #region HasOverrideTimeout
        /// <summary>
        /// Gets a value indicating whether this instance has override timeout.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has override timeout; otherwise, <c>false</c>.
        /// </value>
        public bool HasOverrideTimeout
        {
            get { return this.OverrideTimeout != null; }
        }
        #endregion
        #endregion
    }
}
