using System.Collections;
using System.IO;
using Exortech.NetReflector;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// Publishes the results in an RSS feed.
    /// </summary>
    [ReflectorType("rss")]
    public class RssPublisher : ITask
    {
        private const string RSSFilename = "RSSData.xml";
        private const string contentNamespace = "http://purl.org/rss/1.0/modules/content/";
        private int numberOfItems = 20;

        /// <summary>
        /// The number of items to be displayed.
        /// </summary>
        [ReflectorProperty("items", Required = false)]
        public int NumberOfItems
        {
            get { return numberOfItems; }
            set
            {
                if (value > 255)
                {
                    numberOfItems = 255;
                }
                else
                {
                    numberOfItems = value;
                }
            }
        }

        private static string RSSDataFileLocation(string artifactDirectory)
        {
            return Path.Combine(artifactDirectory, RSSFilename);
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

        /// <summary>
        /// Description used for the visualisation of the buildstage, if left empty the process name will be shown
        /// </summary>
        [ReflectorProperty("description", Required = false)]
        public string Description = string.Empty;

        public void Run(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(Description != string.Empty ? Description : "Making RSS feed");

            string feedFile = RSSDataFileLocation(result.ArtifactDirectory);
            XmlDocument rssFeed = new XmlDocument();
            XmlElement channelElement = null;

            // Attempt to load the file and the channel element
            if (File.Exists(feedFile))
            {
                try
                {
                    rssFeed.Load(feedFile);
                }
                catch (XmlException)
                {
                    rssFeed = new XmlDocument();
                }
                channelElement = rssFeed.SelectSingleNode("/rss/channel") as XmlElement;
                if (channelElement == null) rssFeed = new XmlDocument();
            }
            
            // If the channel element isn't loaded, then this is a new document (or an invalid document that is being overwritten)
            if (channelElement == null)
            {
                channelElement = InitialiseFeed(rssFeed, result.ProjectName, result.ProjectUrl);
            }
            GenerateDocument(result, channelElement);
            rssFeed.Save(feedFile);
        }

        private XmlElement InitialiseFeed(XmlDocument rssFeed, string projectName, string projectUrl)
        {
            XmlElement rootElement = CreateElement(rssFeed, "rss");
            rootElement.SetAttribute("version", "2.0");
            rssFeed.AppendChild(rootElement);

            XmlElement channelElement = CreateElement(rssFeed, "channel");
            rootElement.AppendChild(channelElement);
            channelElement.AppendChild(CreateTextElement(rssFeed, "title", "CruiseControl.Net - {0}", projectName));
            if (!string.IsNullOrEmpty(projectUrl)) channelElement.AppendChild(CreateTextElement(rssFeed, "link", projectUrl));
            channelElement.AppendChild(CreateTextElement(rssFeed, "description", "Latest build results for '{0}'", projectName));
            channelElement.AppendChild(CreateTextElement(rssFeed, "language", "en"));
            channelElement.AppendChild(CreateTextElement(rssFeed, "ttl", "5"));
            channelElement.AppendChild(CreateTextElement(rssFeed, "generator", "CruiseControl.Net"));

            return channelElement;
        }

        private XmlElement CreateElement(XmlDocument document, string name)
        {
            XmlElement element = document.CreateElement(name);
            return element;
        }

        private XmlElement CreateContentElement(XmlDocument document, string name)
        {
            XmlElement element = document.CreateElement(name, contentNamespace);
            return element;
        }

        private XmlElement CreateTextElement(XmlDocument document, string name, string text, params object[] values)
        {
            XmlElement element = CreateElement(document, name);
            if (values == null)
            {
                element.InnerText = text;
            }
            else
            {
                element.InnerText = string.Format(text, values);
            }
            return element;
        }

        private void GenerateDocument(IIntegrationResult result, XmlElement channelElement)
        {
            // Esnure there is space for the new item
            XmlNodeList existingElements = channelElement.SelectNodes("item");
            int count = existingElements.Count + 1;
            int position = 0;
            while (count > numberOfItems)
            {
                existingElements[position].ParentNode.RemoveChild(existingElements[position]);
                position++;
                count--;
            }

            XmlElement itemElement = CreateElement(channelElement.OwnerDocument, "item");
            itemElement.AppendChild(CreateTextElement(channelElement.OwnerDocument,
                "title",
                "Build {0} : {1}  {2}  {3}", 
                result.Label, 
                result.Status, 
                GetAmountOfModifiedfiles(result), 
                GetFirstCommentedModification(result)));
            itemElement.AppendChild(CreateTextElement(channelElement.OwnerDocument, "description", GetAmountOfModifiedfiles(result)));
            itemElement.AppendChild(CreateTextElement(itemElement.OwnerDocument, "guid", System.Guid.NewGuid().ToString()));
            itemElement.AppendChild(CreateTextElement(itemElement.OwnerDocument, "pubDate", System.DateTime.Now.ToString("r")));
            channelElement.AppendChild(itemElement);

            if (result.HasModifications())
            {
                XmlElement dataElement = CreateContentElement(
                    itemElement.OwnerDocument,
                    "encoded");
                XmlCDataSection cdata = itemElement.OwnerDocument.CreateCDataSection(
                    GetBuildModifications(result));
                dataElement.AppendChild(cdata);
                itemElement.AppendChild(dataElement);
            }
        }

        private string GetAmountOfModifiedfiles(IIntegrationResult result)
        {
            switch (result.Modifications.Length)
            {
                case 0:
                    return "No changed files found in build";
                case 1:
                    return "1 changed file found in build";
                default:
                    return string.Format("{0} changed files found in build", result.Modifications.Length);
            }
        }

        private string GetFirstCommentedModification(IIntegrationResult result)
        {
            if (result.HasModifications() )
            {
                for (int i = 0; i <= result.Modifications.Length - 1; i++)
                {
                    if (!(result.Modifications[i].Comment == null) &&  result.Modifications[i].Comment.Length > 0 )
                        return "First Comment : " + result.Modifications[i].Comment;
                }
                
                return "";
            }
            else
            {
                return "";
            }
        }

        private string GetBuildModifications(IIntegrationResult result)
        {

            System.IO.StringWriter mods = new StringWriter();
            string ModificationCheck = "";
            string PreviousModificationCheck = "";

            ArrayList LoggedModifications = new ArrayList();

            mods.WriteLine("<h4>Modifications in build :</h4>");

            mods.WriteLine("<table cellpadding=\"5\">");

            for (int i = 0; i < result.Modifications.Length; i++)
            {
                ModificationCheck = result.Modifications[i].UserName + "__CCNET__" + result.Modifications[i].Comment;

                if (!LoggedModifications.Contains(ModificationCheck))
                {
                    LoggedModifications.Add(ModificationCheck);

                    mods.WriteLine(string.Format("<tr><td>{0}</td><td>{1}</td></tr>",
                                    result.Modifications[i].UserName,
                                    result.Modifications[i].Comment));

                    if (result.Modifications[i].IssueUrl != null &&  result.Modifications[i].IssueUrl.Length > 0)
                    {
                        mods.WriteLine(string.Format("<tr><td>IssueLink</td><td><a href=\"{0}\">{0}</a></td></tr>",                                        
                            result.Modifications[i].IssueUrl));
                    }
                }
            }
            mods.WriteLine("</table>");


            mods.WriteLine("<h4>Detailed information of the modifications in the build :</h4>");

            mods.WriteLine("<table cellpadding=\"5\">");

            PreviousModificationCheck = "";
            LoggedModifications = new ArrayList();

            for (int i = 0; i < result.Modifications.Length; i++)
            {
                ModificationCheck = result.Modifications[i].UserName + "__CCNET__" + result.Modifications[i].Comment;

                if (PreviousModificationCheck != ModificationCheck)
                {
                    mods.WriteLine(string.Format("<tr><td><b>{0}</b></td><td>{1}</td></tr>",
                                    result.Modifications[i].UserName,
                                    result.Modifications[i].Comment));

                    mods.WriteLine(string.Format("<tr><td><font size=2>{2}</font></td><td><font size=2>{0}/{1}</font></td></tr>",
                                    result.Modifications[i].FolderName,
                                    result.Modifications[i].FileName,
                                    result.Modifications[i].Type));

                    PreviousModificationCheck = ModificationCheck;
                }
                else
                {

                    mods.WriteLine(string.Format("<tr><td><font size=2>{2}</font></td><td><font size=2>{0}/{1}</font></td></tr>",
                                    result.Modifications[i].FolderName,
                                    result.Modifications[i].FileName,
                                    result.Modifications[i].Type));
                }
            }
            mods.WriteLine("</table>");

            return mods.ToString();
        }
    }

}
