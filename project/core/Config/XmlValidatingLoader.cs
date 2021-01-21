using System.Xml;
using System.Xml.Schema;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// 	
    /// </summary>
    public class XmlValidatingLoader
    {
        private readonly XmlReader innerReader;
        private XmlReaderSettings xmlReaderSettings;
        private bool valid;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlValidatingLoader" /> class.	
        /// </summary>
        /// <param name="innerReader">The inner reader.</param>
        /// <remarks></remarks>
        public XmlValidatingLoader(XmlReader innerReader)
        {
            this.innerReader = innerReader;

            var resolver = new XmlUrlResolver();

            // This is a bit of a hack - Turn on DTD entity resolution if it is not already on.
            // Also set a resolver, this is required under Mono
            var dummy = innerReader as XmlTextReader;
            if ( dummy != null )
            {
                dummy.EntityHandling = EntityHandling.ExpandEntities;
                dummy.XmlResolver = resolver;
            }
            xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.ValidationType = ValidationType.None;
            xmlReaderSettings.ProhibitDtd = false;
            xmlReaderSettings.XmlResolver = resolver;
            xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
            xmlReaderSettings.ValidationEventHandler += ValidationHandler;
        }

        /// <summary>
        /// Occurs when [validation event handler].	
        /// </summary>
        /// <remarks></remarks>
        public event ValidationEventHandler ValidationEventHandler
        {
            add { xmlReaderSettings.ValidationEventHandler += value; }
            remove { xmlReaderSettings.ValidationEventHandler -= value; }
        }

        /// <summary>
        /// Adds the schema.	
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <remarks></remarks>
        public void AddSchema(XmlSchema schema)
        {
            xmlReaderSettings.Schemas.Add(schema);
            xmlReaderSettings.ValidationType = ValidationType.Schema;
        }

        /// <summary>
        /// Loads this instance.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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