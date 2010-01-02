
using System.Xml;
using System.Xml.Schema;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    public class XmlValidatingLoader
    {
        private readonly XmlReader innerReader;
        private XmlReaderSettings xmlReaderSettings;
        private bool valid;

        public XmlValidatingLoader(XmlReader innerReader)
        {
            this.innerReader = innerReader;
            // This is a bit of a hack - Turn on DTD entity resolution if it is not already on.
            if ( innerReader is XmlTextReader )
            {
                ((XmlTextReader)(innerReader)).EntityHandling = EntityHandling.ExpandEntities;
            }
            xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.ValidationType = ValidationType.None;
            xmlReaderSettings.ProhibitDtd = false;
            xmlReaderSettings.XmlResolver = new XmlUrlResolver();
            xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
            xmlReaderSettings.ValidationEventHandler += ValidationHandler;
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
                        doc.XmlResolver = new XmlUrlResolver();
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