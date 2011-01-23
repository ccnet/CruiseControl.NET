//-----------------------------------------------------------------------
// <copyright file="HttpRequestHeader.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using Exortech.NetReflector;

    /// <summary>
    /// A header for an HTTP request.
    /// </summary>
    /// <title>HTTP Request Header</title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;header name="name" value="value" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("header")]
    public class HttpRequestHeader
    {
        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <default>n/a</default>
        /// <version>1.5</version>
        [ReflectorProperty("name", Required = true)]
        public string Name { get; set; }
        #endregion

        #region Value
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value of the header.</value>
        /// <default>n/a</default>
        /// <version>1.5</version>
        [ReflectorProperty("value", Required = true)]
        public string Value { get; set; }
        #endregion
        #endregion
    }
}
