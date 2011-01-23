
using System.IO;
using System.Text;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// XmlFragmentWriter buffers xml written using the WriteNode method so that 
    /// It swallows any requests to write processing instructions.
    /// </summary>
    public class XmlFragmentWriter : XmlTextWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFragmentWriter" /> class.	
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <remarks></remarks>
        public XmlFragmentWriter(TextWriter writer) : base(writer)
        {
        }

        /// <summary>
        /// Writes the node.	
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <remarks></remarks>
        public void WriteNode(string xml)
        {
            xml = StripIllegalCharacters(xml);
            XmlReader reader = CreateXmlReader(xml);
            try
            {
                WriteNode(reader, true);
            }
            catch (XmlException ex)
            {
                Log.Debug("Supplied output is not valid xml.  Writing as CDATA");
                Log.Debug("Output: " + xml);
                Log.Debug("Exception: " + ex);
                WriteCData(xml);
            }
            reader.Close();
        }

        /// <summary>
        /// Use XmlValidatingReader in order to bypass root-level rules for document validation.  In other words,
        /// accept xml that is not single rooted (contains text or multiple root elements).
        /// </summary>
        private static XmlReader CreateXmlReader(string xml)
        {
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.CloseInput = true;

            //use ConformanceLevel.Auto as discussed in
            //https://bugzilla.novell.com/show_bug.cgi?id=520080
            xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;

            xmlReaderSettings.IgnoreProcessingInstructions = true;
            return XmlReader.Create(new StringReader(xml), xmlReaderSettings);
        }

        /// <summary>
        /// Writes the node.	
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="defattr">The defattr.</param>
        /// <remarks></remarks>
        public override void WriteNode(XmlReader reader, bool defattr)
        {
            StringWriter buffer = new StringWriter();
            XmlFragmentWriter writer = CreateXmlWriter(buffer);
            writer.WriteNodeBase(reader, defattr);
            writer.Close();
            WriteRaw(buffer.ToString());
        }

        private XmlFragmentWriter CreateXmlWriter(StringWriter buffer)
        {
            XmlFragmentWriter writer = new XmlFragmentWriter(buffer);
            writer.Formatting = Formatting;
            return writer;
        }

        private void WriteNodeBase(XmlReader reader, bool defattr)
        {
            base.WriteNode(reader, defattr);
        }

        /// <summary>
        /// Writes the processing instruction.	
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="text">The text.</param>
        /// <remarks></remarks>
        public override void WriteProcessingInstruction(string name, string text)
        {
            // no-op
        }

        /// <summary>
        /// Writes the C data.	
        /// </summary>
        /// <param name="text">The text.</param>
        /// <remarks></remarks>
        public override void WriteCData(string text)
        {
            base.WriteCData(XmlUtil.EncodeCDATA(text));
        }

        /// <summary>
        /// Character values in the range 0x-0x1F (excluding white space characters 0x9, 0xA, and 0xD) are illegal in xml documents.
        /// This method removes all occurrences of these characters from the document.
        /// </summary>
        /// <param name="xml">The xml string to preprocess.</param>
        /// <returns></returns>
        private static string StripIllegalCharacters(string xml)
        {
            StringBuilder builder = new StringBuilder(xml.Length);
            foreach (char c in xml)
            {
                if (c > 31 || c == 9 || c == 10 || c == 13) builder.Append(c);
            }
            return builder.ToString();
        }
    }
}