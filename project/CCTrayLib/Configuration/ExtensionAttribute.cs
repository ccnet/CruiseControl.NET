using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
    /// <summary>
    /// Defines metadata on an extension.
    /// </summary>
    public class ExtensionAttribute
        : Attribute
    {
        private string displayName;

        /// <summary>
        /// A user-friendly display name.
        /// </summary>
        public string DisplayName
        {
            get { return this.displayName; }
            set { this.displayName = value; }
        }
    }
}
