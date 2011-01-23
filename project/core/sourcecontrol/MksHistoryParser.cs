//-----------------------------------------------------------------------
// <copyright file="MksHistoryParser.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// 	
    /// </summary>
    public class MksHistoryParser : IHistoryParser
	{
        /// <summary>
        /// Parses the specified history.	
        /// </summary>
        /// <param name="history">The history.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public virtual Modification[] Parse(TextReader history, DateTime from, DateTime to)
		{
            XPathDocument doc = new XPathDocument(history);
		    XmlReader reader;

            // Transform xml output
		    try
		    {
		        Assembly execAssem = Assembly.GetExecutingAssembly();
		        Stream s =
		            execAssem.GetManifestResourceStream(
		                "ThoughtWorks.CruiseControl.Core.sourcecontrol.MksHistory-viewsandbox-mods.xsl");

                if (s != null)
                {
                    reader = XmlReader.Create(s);
                }
                else
                {
                    throw new CruiseControlException("Exception encountered while retrieving MksHistory-viewsandbox-mods.xsl");
                }
		    }
		    catch (Exception e)
		    {
		        throw new CruiseControlException("Exception encountered while retrieving MksHistory-viewsandbox-mods.xsl", e);
		    }

            XslCompiledTransform tran = new XslCompiledTransform();
            tran.Load(reader);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            StringBuilder transformed = new StringBuilder(1024);
            XmlWriter writer = XmlWriter.Create(transformed, settings);

            if (writer != null)
            {
                tran.Transform(doc.CreateNavigator(), new XsltArgumentList(), writer);
                writer.Close();

                Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Transformed MKS Mods:\n{0}\n", transformed.ToString()));

                List<Modification> result = new List<Modification>();
                StringReader sr = new StringReader(transformed.ToString());
                XmlReader rdr = XmlReader.Create(sr);
                doc = new XPathDocument(rdr);
                XPathNavigator nav = doc.CreateNavigator();
                XPathNodeIterator nodes = nav.Select("//modification");
                while (nodes.MoveNext())
                {
                    Modification mod = new Modification();
                    FileInfo info = new FileInfo(nodes.Current.SelectSingleNode("fullname").Value);
                    mod.FileName = info.Name;
                    mod.FolderName = info.DirectoryName;
                    mod.Type = nodes.Current.SelectSingleNode("modificationtype").Value;
                    mod.Version = nodes.Current.SelectSingleNode("memberrev").Value;
                    result.Add(mod);
                }

                return result.ToArray();
            }
		    throw new CruiseControlException("Failed to create XmlWriter for transforming MKS modifications report.");
		}

        /// <summary>
        /// Parses the member info and add to modification.	
        /// </summary>
        /// <param name="modification">The modification.</param>
        /// <param name="reader">The reader.</param>
        /// <remarks></remarks>
        public virtual void ParseMemberInfoAndAddToModification(Modification modification, StringReader reader)
        {
            XPathDocument doc = new XPathDocument(reader);
            XPathNavigator nav = doc.CreateNavigator();
            XPathNavigator node = nav.SelectSingleNode("Response/WorkItems/WorkItem");
            modification.UserName = node.SelectSingleNode("Field[@name='author']/Value").Value;
            modification.ModifiedTime = DateTime.Parse(node.SelectSingleNode("Field[@name='date']/Value").Value, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
            modification.Comment = node.SelectSingleNode("Field[@name='description']/Value").Value;

            // Parse task ID from change package ID.
            string cpid = node.SelectSingleNode("Field[@name='cpid']/Value").Value;
            modification.ChangeNumber = ParseTaskIDFromCPID(cpid);
        }

        private string ParseTaskIDFromCPID(string cpid)
        {
            if (cpid.Contains(":"))
            {
                cpid = cpid.Substring(0, cpid.IndexOf(":"));
            }

            return cpid;
        }
	}
}
