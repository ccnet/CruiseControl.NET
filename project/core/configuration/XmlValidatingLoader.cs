using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace ThoughtWorks.CruiseControl.Core.Configuration
{
	/// <summary>
	/// Summary description for XmlValidatingLoader.
	/// </summary>
	public class XmlValidatingLoader : IDisposable
	{
		XmlValidatingReader _reader;
		bool _valid;

		public XmlValidatingLoader(XmlReader reader)
		{
			_reader = new XmlValidatingReader(reader);
			_reader.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);
		}

		public event ValidationEventHandler ValidationEventHandler 
		{
			add 
			{
				_reader.ValidationEventHandler += value;
			}
			remove 
			{
				_reader.ValidationEventHandler -= value;
			}
		}	

		public XmlSchemaCollection Schemas 
		{
			get 
			{
				return _reader.Schemas;
			}
		}

		public XmlDocument Load() 
		{
			// lock in case this object is used in a multi-threaded situation
			lock (this)
			{
				// set the flag true
				_valid = true;

				try
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(_reader);

					// if the load failed, our event handler will have set _worked to false
					return _valid ? doc : null;
				} 
				finally 
				{
					_valid = true;
				}
			}
		}

		private void ValidationHandler(object sender, ValidationEventArgs args) 
		{
			_valid = false;
		}

		public void Dispose() 
		{
			_reader.Close();
		}
	}
}
