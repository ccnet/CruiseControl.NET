using System.IO;
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
			XmlReader reader = CreateXmlReader(xml);
			try
			{
				WriteNode(reader, true);
			}
			catch (XmlException ex)
			{
				Log.Warning(ex);
				WriteCData(xml);
			}
			reader.Close();
		}

		/// <summary>
		/// Use XmlValidatingReader in order to bypass root-level rules for document validation.  In other words,
		/// accept xml that is not single rooted (contains text or multiple root elements).
		/// </summary>
		private XmlValidatingReader CreateXmlReader(string xml)
		{
			return new XmlValidatingReader(xml, XmlNodeType.Element, null);
		}

		public override void WriteNode(XmlReader reader, bool defattr)
		{
			StringWriter buffer = new StringWriter();
			new XmlFragmentWriter(buffer).WriteNodeBase(reader, defattr);
			WriteRaw(buffer.ToString());
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
	}
}