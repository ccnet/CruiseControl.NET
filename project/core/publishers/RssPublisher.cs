using System.Collections;
using System.IO;
using System.Xml.Serialization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("rss")]
	// This publisher generates a rss file reporting the latest results for a Project.
	// We use .NET's XMLSerialization to genarate the XML
	public class RssPublisher : PublisherBase
	{
		private string filename;
		private string logDir;

		[ReflectorProperty("filename", Required=true)]
		public string Filename
		{
			get { return filename; }
			set { filename = value;}
		}

		[ReflectorProperty("logdir", Required=true)]
		public string LogDir
		{
			get { return logDir; }
			set { logDir = value;}
		}

		public override void PublishIntegrationResults(IProject project, IIntegrationResult result)
		{
			using (StreamWriter stream = File.CreateText(Filename))
			{
				stream.Write(GenerateDocument(project, result));
			}
		}

		public Document GenerateDocument(IProject project, IIntegrationResult result)
		{
			Document document = new Document();
			document.Channel = GenerateChannel(project, result);

			return document;
		}

		public Channel GenerateChannel(IProject project, IIntegrationResult result)
		{
			Channel channel = new Channel();
			channel.Link = project.WebURL;
			channel.Title = "CruiseControl.NET - " + project.Name;
			channel.Description = "The latest build results for " + project.Name;
			channel.Items = GenerateItems(project, result);

			return channel;
		}

		public ArrayList GenerateItems(IProject project, IIntegrationResult result)
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
