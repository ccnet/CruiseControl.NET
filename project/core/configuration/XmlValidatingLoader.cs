using System;
using System.Xml;
using System.Xml.Schema;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	/// <summary>
	/// Summary description for XmlValidatingLoader.
	/// </summary>
	public class XmlValidatingLoader : IDisposable
	{
		private XmlValidatingReader reader;
		private bool valid;

		public XmlValidatingLoader(XmlReader reader)
		{
			this.reader = new XmlValidatingReader(reader);
			this.reader.ValidationType = ValidationType.None;
			this.reader.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);
		}

		public event ValidationEventHandler ValidationEventHandler
		{
			add { reader.ValidationEventHandler += value; }
			remove { reader.ValidationEventHandler -= value; }
		}

		public void AddSchema(XmlSchema schema)
		{
			reader.Schemas.Add(schema);
			reader.ValidationType = ValidationType.Schema;
		}

		public XmlDocument Load()
		{
			// lock in case this object is used in a multi-threaded situation
			lock (this)
			{
				// set the flag true
				valid = true;

				try
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(reader);

					// if the load failed, our event handler will have set flag to false
					return valid ? doc : null;
				}
				finally
				{
					valid = true;
				}
			}
		}

		private void ValidationHandler(object sender, ValidationEventArgs args)
		{
			valid = false;
		}

		public void Dispose()
		{
			reader.Close();
		}
	}
}