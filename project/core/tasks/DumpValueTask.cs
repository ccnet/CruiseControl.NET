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
    /// <title>DumpValue Task</title>
    /// <version>1.7</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;DumpValue&gt;
    /// &lt;xmlFileName&gt;somefile.xml&lt;/xmlFileName&gt;
    /// &lt;values&gt;
    /// &lt;namedValue name="MyValue" value="ValueContent" /&gt;
    /// &lt;/values&gt;
    /// &lt;/DumpValue&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <includePage>Integration Properties</includePage>
    /// <para>
    /// Originally developped by Olivier Sannier.
    /// </para>
    /// </remarks>
    [ReflectorType("DumpValue")]
    public class DumpValueTask : TaskBase
    {
        /// <summary>
        /// The name of the XML file to write
        /// </summary>
        /// <version>1.7</version>
        /// <default>None</default>
        [ReflectorProperty("xmlFileName", Required = true)]
        public string XmlFileName = string.Empty;

        /// <summary>
        /// The values to dump in the given XML file.
        /// </summary>
        /// <version>1.7</version>
        /// <default>n/a</default>
        [ReflectorProperty("values", Required = false)]
        public NameValuePair[] Values { get; set; }

        [Serializable]
        public class ValueDumperItem
        {
            private string value;

            public ValueDumperItem() { }
            public ValueDumperItem(NameValuePair ValuePair)
            {
                Name = ValuePair.Name;
                value = ValuePair.Value;
            }

            public string Name { get; set; }

            [XmlElement("Value")]
            public XmlCDataSection Message
            {
                get
                {
                    XmlDocument doc = new XmlDocument();
                    return doc.CreateCDataSection(value);
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

                foreach (NameValuePair Value in parent.Values)
                {
                    Add(new ValueDumperItem(Value));
                }
            }
        }

        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description :
                string.Format("Executing DumpValue: Dumping {0} value(s) into {1}", Values.Length, XmlFileName));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.CloseOutput = true;

            XmlWriter writer = XmlTextWriter.Create(XmlFileName, settings);

            XmlSerializer serializer = new XmlSerializer(typeof(ValueDumper));
            serializer.Serialize(writer, new ValueDumper(this));
            writer.Close();

            return true;
        }
    }
}

