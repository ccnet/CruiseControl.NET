//-----------------------------------------------------------------------
// <copyright file="MksHistoryParser.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Xsl;
    using System.Xml.XPath;

    public class MksHistoryParser : IHistoryParser
	{
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

		public virtual void ParseMemberInfoAndAddToModification(Modification modification, StringReader reader)
		{
            XPathDocument doc = new XPathDocument(reader);
            XPathNavigator nav = doc.CreateNavigator();
            XPathNavigator node = nav.SelectSingleNode("Response/WorkItems/WorkItem");
            modification.UserName = node.SelectSingleNode("Field[@name='author']/Value").Value;
            modification.ModifiedTime = XmlConvert.ToDateTime(node.SelectSingleNode("Field[@name='date']/Value").Value, XmlDateTimeSerializationMode.Local);
            modification.Comment = node.SelectSingleNode("Field[@name='description']/Value").Value;		
		}
	}
}
