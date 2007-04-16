using System.Xml;
using System.Xml.Schema;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    public class XmlValidatingLoader
    {
        private readonly XmlTextReader innerReader;
        private XmlReaderSettings xmlReaderSettings;
        private bool valid;

        public XmlValidatingLoader(XmlTextReader innerReader)
        {
            this.innerReader = innerReader;
            innerReader.EntityHandling = EntityHandling.ExpandEntities;
            xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.ValidationType = ValidationType.None;
            xmlReaderSettings.ProhibitDtd = false;
            xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);
        }

        public event ValidationEventHandler ValidationEventHandler
        {
            add { xmlReaderSettings.ValidationEventHandler += value; }
            remove { xmlReaderSettings.ValidationEventHandler -= value; }
        }

        public void AddSchema(XmlSchema schema)
        {
            xmlReaderSettings.Schemas.Add(schema);
            xmlReaderSettings.ValidationType = ValidationType.Schema;
        }

        public XmlDocument Load()
        {
            // lock in case this object is used in a multi-threaded situation
            lock (this)
            {
                // set the flag true
                valid = true;

                using (XmlReader reader = XmlReader.Create(innerReader, xmlReaderSettings))
                {
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
        }

        private void ValidationHandler(object sender, ValidationEventArgs args)
        {
            valid = false;
        }
    }
}