using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace tw.ccnet.core.configuration
{
	/// <summary>
	/// Summary description for XmlValidatingLoader.
	/// </summary>
	public class XmlValidatingLoader : IDisposable
	{
		private XmlValidatingReader reader;
		private bool worked;

		public XmlValidatingLoader(XmlReader reader)
		{
			this.reader = new XmlValidatingReader(reader);
			this.reader.ValidationEventHandler += new ValidationEventHandler(handler);
		}

		public event ValidationEventHandler ValidationEventHandler 
		{
			add 
			{
				reader.ValidationEventHandler += value;
			}

			remove 
			{
				reader.ValidationEventHandler -= value;
			}
		}	

		public XmlSchemaCollection Schemas 
		{
			get 
			{
				return reader.Schemas;
			}
		}

		public XmlDocument Load() 
		{
			worked = true;
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(reader);

				return worked ? doc : null;
			} 
			finally 
			{
				worked = true;
			}
		}

		private void handler(object sender, ValidationEventArgs args) 
		{
			worked = false;
		}

		public void Dispose() 
		{
			reader.Close();
		}
	}
}
