using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>
    /// The DumpValueItem is used to specify which values are written to the dump file
    /// The values are put in CDATA sections by default
    /// </para>
    /// </summary>
    /// <title>DumpValue Item</title>
    /// <version>1.7</version>
    [ReflectorType("dumpValueItem")]
    public class DumpValueItem : NameValuePair
    {
        bool valueInCDATA = true;

        /// <summary>
        /// Starts a new <see cref="DumpValueItem"/> with no name and no value.
        /// </summary>
        public DumpValueItem() : base() { }

        /// <summary>
        /// Starts a new <see cref="DumpValueItem"/> with a name and value.
        /// </summary>
        public DumpValueItem(string name, string value) : base(name, value) { }

        /// <summary>
        /// Starts a new <see cref="DumpValueItem"/> with a name, a value, and a flag for ValueInCDATA.
        /// </summary>
        public DumpValueItem(string name, string value, bool valueInCDATA)
            : this(name, value)
        {
            this.valueInCDATA = valueInCDATA;
        }

        /// <summary>
        /// Whether to put the value in CDATA or not
        /// </summary>
        /// <version>1.7</version>
        /// <default>true</default>
        [ReflectorProperty("valueInCDATA", Required = false)]
        public bool ValueInCDATA { get { return valueInCDATA; } set { valueInCDATA = value; } }
    }

}
