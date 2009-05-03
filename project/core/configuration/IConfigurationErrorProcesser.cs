using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// Defines how to handle errors and warnings from the configuration file.
    /// </summary>
    public interface IConfigurationErrorProcesser
    {
        #region Methods
        #region ProcessError()
        /// <summary>
        /// Process an error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        void ProcessError(string message);

        /// <summary>
        /// Process an exception.
        /// </summary>
        /// <param name="error">The exception.</param>
        void ProcessError(Exception error);
        #endregion

        #region ProcessWarning()
        /// <summary>
        /// Process a warning message.
        /// </summary>
        /// <param name="message">The warning message.</param>
        void ProcessWarning(string message);
        #endregion

        #region ProcessUnhandledNode()
        /// <summary>
        /// Process an unhandled node.
        /// </summary>
        /// <param name="node">The unhandled node.</param>
        bool ProcessUnhandledNode(XmlNode node);
        #endregion
        #endregion
    }
}
