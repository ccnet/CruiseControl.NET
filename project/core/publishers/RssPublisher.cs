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
	public class RssPublisher : PublisherBase
	{
		private string filename;

		[ReflectorProperty("filename", Required=true)]
		public string Filename
		{
			get { return filename; }
			set { filename = value;}
		}

		public override void PublishIntegrationResults(IIntegrationResult result)
		{
			using (StreamWriter stream = File.CreateText(Filename))
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
			if (result.Succeeded)
			{
				item.Title = "Successful Build";
			}

			return items;
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
