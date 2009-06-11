using System;
using System.IO;
using System.Collections;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    public class GitHistoryParser : IHistoryParser
    {
        /// <summary>
        /// Parse and filter the supplied modifications.  The position of each modification in the list is used as the ChangeNumber.
        /// </summary>
        /// <param name="history"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public Modification[] Parse(TextReader history, DateTime from, DateTime to)
        {
        	StringReader sr = new StringReader(string.Concat(
        				@"<ArrayOfModification xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">",
        				history.ReadToEnd(),
						"</ArrayOfModification>"));

            XmlSerializer serializer = new XmlSerializer(typeof(Modification[]));
            Modification[] mods;
            try
            {
                mods = (Modification[])serializer.Deserialize(sr);
            }
            catch (Exception ex)
            {
                throw new CruiseControlException("Git History Parsing Failed", ex);
            }

            ArrayList results = new ArrayList();
            foreach (Modification mod in mods)
            {
                if ((mod.ModifiedTime >= from) & (mod.ModifiedTime <= to))
                {
                    results.Add(mod);
                }
            }
            return (Modification[])results.ToArray(typeof(Modification));
        }
    }
}
