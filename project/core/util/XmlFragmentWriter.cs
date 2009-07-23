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
        public XmlFragmentWriter(TextWriter writer) : base(writer)
        {
        }

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

        public override void WriteProcessingInstruction(string name, string text)
        {
            // no-op
        }

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