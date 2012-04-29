namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// <para>
    /// The DumpValue task is used to write values from the configuration file to a given XML file.
    /// This is most useful if you want to dump the dynamic values created from parameters so that they
    /// can be used by another task later on.
    /// The created file is encoded using UTF-8 and the values are put in CDATA sections
    /// </para>
    /// </summary>
    /// <title>Dump Value Task</title>
    /// <version>1.7</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;dumpValue&gt;
    /// &lt;xmlFileName&gt;somefile.xml&lt;/xmlFileName&gt;
    /// &lt;dumpValueItems&gt;
    /// &lt;dumpValueItem name="MyValue" value="ValueContent" /&gt;
    /// &lt;/dumpValueItems&gt;
    /// &lt;/dumpValue&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;dumpValue&gt;
    /// &lt;xmlFileName&gt;somefile.xml&lt;/xmlFileName&gt;
    /// &lt;dumpValueItems&gt;
    /// &lt;dumpValueItem name="MyValue" value="ValueContent" /&gt;
    /// &lt;dumpValueItem name="MyValueNotInCDATA" value="some other content" valueInCDATA="false" /&gt;
    /// &lt;/dumpValueItems&gt;
    /// &lt;/dumpValue&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <includePage>Integration Properties</includePage>
    /// <para>
    /// Originally developped by Olivier Sannier.
    /// </para>
    /// </remarks>
    [ReflectorType("dumpValue")]
    public class DumpValueTask : TaskBase
    {

        private string xmlFileName = string.Empty;

        /// <summary>
        /// The name of the XML file to write
        /// </summary>
        /// <version>1.7</version>
        /// <default>None</default>
        [ReflectorProperty("xmlFileName", Required = true)]
        public string XmlFileName { get { return xmlFileName; } set { xmlFileName = value; } }

        /// <summary>
        /// The values to dump in the given XML file.
        /// </summary>
        /// <version>1.7</version>
        /// <default>n/a</default>
        [ReflectorProperty("dumpValueItems", Required = false)]
        public DumpValueItem[] Items { get; set; }

        [Serializable]
        public class ValueDumperItem
        {
            private string value;
            private bool valueInCDATA;

            public ValueDumperItem() { }
            public ValueDumperItem(DumpValueItem Item)
            {
                Name = Item.Name;
                value = Item.Value;
                valueInCDATA = Item.ValueInCDATA;
            }

            public string Name { get; set; }

            [XmlElement("Value")]
            public XmlCharacterData Message
            {
                get
                {
                    if (valueInCDATA)
                    {
                        XmlDocument doc = new XmlDocument();
                        return doc.CreateCDataSection(value);
                    }
                    else
                    {
                        XmlDocument doc = new XmlDocument();
                        return doc.CreateTextNode(value);
                    }
                }
                set
                {
                    this.value = value.Value;
                }
            }
            //public string Value { get; set; }
        }

        [Serializable]
        [XmlRoot("ValueDumper")]
        public class ValueDumper : List<ValueDumperItem>
        {
            private DumpValueTask parent;

            public ValueDumper() { }
            public ValueDumper(DumpValueTask parent) 
            { 
                this.parent = parent;

                foreach (DumpValueItem Item in parent.Items)
                {
                    Add(new ValueDumperItem(Item));
                }
            }
        }

        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description :
                string.Format("Executing DumpValue: Dumping {0} value(s) into {1}", Items.Length, XmlFileName));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.CloseOutput = true;

            XmlWriter writer = XmlTextWriter.Create(XmlFileName, settings);
            try
            {
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                XmlSerializer serializer = new XmlSerializer(typeof(ValueDumper));
                serializer.Serialize(writer, new ValueDumper(this), namespaces);
            }
            finally
            {
                writer.Close();
            }

            return true;
        }
    }
}

