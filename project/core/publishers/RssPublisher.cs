using System.Collections;
using System.IO;
using System.Xml.Serialization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("rss")]
	// This publisher generates a rss file reporting the latest results for a Project.
	// We use .NET's XMLSerialization to generate the XML
	// ToDo - more on this, or delete it!
	public class RssPublisher : ITask
	{
		private const string RSSFilename = "RSSData.xml";

        private static string RSSDataFileLocation(string artifactDirectory)
        {
            return System.IO.Path.Combine(artifactDirectory, RSSFilename);            
        }

        public static string LoadRSSDataDocument(string artifactDirectory)
        {
            string result = string.Empty;

            if (File.Exists(RSSDataFileLocation(artifactDirectory)))
            {
                result = File.ReadAllText(RSSDataFileLocation(artifactDirectory));
            }

            return result;
        }

		public void Run(IIntegrationResult result)
		{
            using (StreamWriter stream = File.CreateText(RSSDataFileLocation(result.ArtifactDirectory)))
			{
				stream.Write(GenerateDocument(result));
			}
		}

		public Document GenerateDocument(IIntegrationResult result)
		{
			Document document = new Document();
			document.Channel = GenerateChannel(result);

			return document;
		}

		public Channel GenerateChannel(IIntegrationResult result)
		{
			Channel channel = new Channel();
			channel.Link = result.ProjectUrl;
			channel.Title = "CruiseControl.NET - " + result.ProjectName;
			channel.Description = "The latest build results for " + result.ProjectName;
			channel.Items = GenerateItems(result);

			return channel;
		}

		public ArrayList GenerateItems(IIntegrationResult result)
		{
			ArrayList items = new ArrayList();

			Item item = new Item();
			items.Add(item);

            item.Title = string.Format("Build {0} : {1}", result.Label, result.Status.ToString());
            item.Description = GetBuildModifications(result);


			return items;
		}

        private string GetBuildModifications(IIntegrationResult result)
        {
            System.Text.StringBuilder mods = new System.Text.StringBuilder();

            if (result.HasModifications())
            {
                mods.AppendLine("Modifications in Build :");

                for (int i = 0; i < result.Modifications.Length; i++)
                {
                    ArrayList LoggedModifications  = new ArrayList();
                    if (!LoggedModifications.Contains(result.Modifications[i].Comment ))
                    {
                        LoggedModifications.Add(result.Modifications[i].Comment);
                        
                        mods.AppendLine(string.Format("- {0} {1}", 
                                        result.Modifications[i].UserName, 
                                        result.Modifications[i].Comment));                        
                    }
                }
            }
            else
            {
                mods.Append("No Modifications found in Build");
            }
           
            return mods.ToString();        
        }
	}

	[XmlRoot(ElementName = "rss")]
	public class Document
	{
		[XmlAttribute("version")]
		public string Version = "0.91";

		[XmlElement("channel")]
		public Channel Channel;

		public override string ToString()
		{
			return XmlUtil.StringSerialize(this);
		}
	}

	public class Channel
	{
		[XmlElement("title")]
		public string Title;

		[XmlElement("link")]
		public string Link;

		[XmlElement("description")]
		public string Description;

		[XmlElement("language")]
		public string Language = "en";

        [XmlElement("ttl")]
        public string TTL = "10";


		[XmlElement(Type = typeof(Item), ElementName="item")]
		public ArrayList Items = new ArrayList();
	}

	public class Item
	{
		[XmlElement("title")]
		public string Title;

		[XmlElement("link")]
		public string Link;

		[XmlElement(ElementName = "description")]
		public string Description;
	}
}
