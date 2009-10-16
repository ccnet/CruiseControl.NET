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
    /// Settings for an HTTP request.
    /// </summary>
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
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
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
        /// Gets or sets the headers.
        /// </summary>
        /// <value>The headers.</value>
        [ReflectorArray("headers", Required = false)]
        public HttpRequestHeader[] Headers { get; set; }
        #endregion

        #region Body
        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body of the request.</value>
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
        /// Gets or sets the send file.
        /// </summary>
        /// <value>The send file.</value>
        [ReflectorProperty("sendFile", Required = false)]
        public string SendFile { get; set; }
        #endregion

        #region Uri
        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI to send to.</value>
        [ReflectorProperty("uri", typeof(UriSerializerFactory), Required = true)]
        public Uri Uri { get; set; }
        #endregion

        #region Timeout
        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
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
        /// Gets or sets the read write timeout.
        /// </summary>
        /// <value>The read write timeout.</value>
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
        /// Gets or sets the credentials.
        /// </summary>
        /// <value>The credentials.</value>
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
        /// Gets or sets a value indicating whether [use default credentials].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use default credentials]; otherwise, <c>false</c>.
        /// </value>
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
