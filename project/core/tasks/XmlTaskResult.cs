//-----------------------------------------------------------------------
// <copyright file="XmlTaskResult.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// An XML task result.
    /// </summary>
    /// <remarks>
    /// Not thread safe!
    /// </remarks>
    public class XmlTaskResult : ITaskResult
    {
        #region Public properties
        #region Success
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="XmlTaskResult"/> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        public bool Success { get; set; }
        #endregion

        #region Data
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data from the result.</value>
        public string Data
        {
            get
            {
                if (this.CachedData != null)
                {
                    return this.CachedData;
                }

                if (this.Writer.WriteState != WriteState.Closed)
                {
                    this.Writer.Close();
                }

                this.CachedData = Encoding.UTF8.GetString(this.BackingStream.ToArray());
                this.BackingStream.Dispose();
                this.Writer = null;
                this.BackingStream = null;
                return this.CachedData;
            }
        }
        #endregion
        #endregion

        #region Protected Properties
        #region BackingStream
        /// <summary>
        /// Gets or sets the backing stream.
        /// </summary>
        /// <value>The backing stream.</value>
        protected MemoryStream BackingStream { get; set; }
        #endregion

        #region Writer
        /// <summary>
        /// Gets or sets the writer.
        /// </summary>
        /// <value>The <see cref="XmlWriter"/>.</value>
        protected XmlWriter Writer { get; set; }
        #endregion

        #region CachedData
        /// <summary>
        /// Gets or sets the cached data.
        /// </summary>
        /// <value>The cached data.</value>
        protected string CachedData { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region GetWriter()
        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <returns>A new <see cref="XmlWriter"/>.</returns>
        public XmlWriter GetWriter()
        {
            if (this.Data != null)
            {
                throw new InvalidOperationException("Result already written.");
            }

            if (this.Writer == null)
            {
                this.BackingStream = new MemoryStream();
                var settings = new XmlWriterSettings() { Indent = true, CloseOutput = false, ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true };
                this.Writer = XmlWriter.Create(this.BackingStream, settings);
            }

            return this.Writer;
        }
        #endregion

        #region CheckIfSuccess()
        /// <summary>
        /// Checks whether the result was successful.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the result was successful, <c>false</c> otherwise.
        /// </returns>
        public bool CheckIfSuccess()
        {
            return this.Success;
        }
        #endregion
        #endregion
    }
}
