namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using Exortech.NetReflector;
    using System.Xml;
    using ThoughtWorks.CruiseControl.Core.Tasks;

    /// <summary>
    /// <para>
    /// This publisher generates an RSS file reporting the latest results for a Project.
    /// </para>
    /// <para>
    /// The RSS feed is available via the Dasboard in the Project Report. There needs to be 1 build done with this publisher for the icon
    /// to show up.
    /// </para>
    /// </summary>
    /// <title>RSS Publisher</title>
    /// <version>1.3</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;rss /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;rss items="30" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("rss")]
    public class RssPublisher 
        : TaskBase
    {
        private const string RssFilename = "RSSData.xml";
        private const string contentNamespace = "http://purl.org/rss/1.0/modules/content/";
        private int numberOfItems = 20;

        /// <summary>
        /// The number of items to be displayed.
        /// </summary>
        /// <default>20</default>
        /// <version>1.4.4</version>
        [ReflectorProperty("items", Required = false)]
        public int NumberOfItems
        {
            get { return numberOfItems; }
            set { numberOfItems = value > 255 ? 255 : value; }
        }

        private static string RSSDataFileLocation(string artifactDirectory)
        {
            return Path.Combine(artifactDirectory, RssFilename);
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

        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Making RSS feed");

            string feedFile = RSSDataFileLocation(result.ArtifactDirectory);
            XmlElement channelElement = LoadOrInitialiseChannelElement(result, feedFile);
            GenerateDocument(result, channelElement);
            channelElement.OwnerDocument.Save(feedFile);

            return true;
        }

        private XmlElement LoadOrInitialiseChannelElement(IIntegrationResult result, string feedFile)
        {
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
            
            // If the channel element isn't loaded, then this is a new document,
            // or an invalid document that is being overwritten
            if (channelElement == null)
            {
                channelElement = InitialiseFeed(rssFeed, result.ProjectName, result.ProjectUrl);
            }
            return channelElement;
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
            element.InnerText = string.Format(text, values);
            return element;
        }

        private void GenerateDocument(IIntegrationResult result, XmlNode channelElement)
        {
            // Ensure there is space for the new item
            XmlNodeList existingElements = channelElement.SelectNodes("item");
            int count = existingElements.Count + 1;
            int position = 0;
            while (count > numberOfItems)
            {
                existingElements[position].ParentNode.RemoveChild(existingElements[position]);
                position++;
                count--;
            }

            XmlDocument ownerDocument = channelElement.OwnerDocument;
            XmlElement integrationElement = BuildIntegrationElement(ownerDocument, result);
            channelElement.AppendChild(integrationElement);
        }

        private XmlElement BuildIntegrationElement(XmlDocument ownerDocument, IIntegrationResult result)
        {
            XmlElement integrationElement = CreateElement(ownerDocument, "item");
            integrationElement.AppendChild(CreateTextElement(integrationElement.OwnerDocument,
                "title",
                "Build {0} : {1}  {2}  {3}",
                result.Label,
                result.Status,
                GetAmountOfModifiedFiles(result),
                GetFirstCommentedModification(result)));
            integrationElement.AppendChild(CreateTextElement(
                integrationElement.OwnerDocument, "description", GetAmountOfModifiedFiles(result)));
            integrationElement.AppendChild(CreateTextElement(
                integrationElement.OwnerDocument, "guid", System.Guid.NewGuid().ToString()));
            integrationElement.AppendChild(CreateTextElement(
                integrationElement.OwnerDocument, "pubDate", System.DateTime.Now.ToString("r")));

            if (result.HasModifications())
            {
                XmlElement modsElement = CreateContentElement(
                    integrationElement.OwnerDocument,
                    "encoded");
                XmlCDataSection cdata = integrationElement.OwnerDocument.CreateCDataSection(
                    GetBuildModifications(result));
                modsElement.AppendChild(cdata);
                integrationElement.AppendChild(modsElement);
            }
            return integrationElement;
        }

        private string GetAmountOfModifiedFiles(IIntegrationResult result)
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
            if (result.HasModifications())
            {
                foreach (Modification modification in result.Modifications)
                {
                    if (!string.IsNullOrEmpty(modification.Comment) )
                        return "First Comment : " + modification.Comment;
                }
            }

            return string.Empty;
        }

        private string GetBuildModifications(IIntegrationResult result)
        {
            Modification[] modifications = result.Modifications;

            return WriteModificationsSummary(modifications) + WriteModificationsDetails(modifications);
        }

        private string WriteModificationsSummary(IEnumerable<Modification> modifications)
        {
            const string modificationHeaderFormat = "<tr><td>{0}</td><td>{1}</td></tr>";
            const string issueLinkFormat = "<tr><td>IssueLink</td><td><a href=\"{0}\">{0}</a></td></tr>";
            StringWriter mods = new StringWriter();

            mods.WriteLine("<h4>Modifications in build :</h4>");
            mods.WriteLine("<table cellpadding=\"5\">");
            ArrayList alreadyAdded = new ArrayList();
            foreach (Modification modification in modifications)
            {
                string modificationChecksum = modification.UserName + "__CCNET__" + modification.Comment;

                if (!alreadyAdded.Contains(modificationChecksum))
                {
                    alreadyAdded.Add(modificationChecksum);

                    mods.WriteLine(string.Format(modificationHeaderFormat,
                                                 modification.UserName,
                                                 modification.Comment));

                    if (!string.IsNullOrEmpty(modification.IssueUrl))
                    {
                        mods.WriteLine(string.Format(issueLinkFormat,
                                                     modification.IssueUrl));
                    }
                }
            }
            mods.WriteLine("</table>");

            return mods.ToString();
        }

        private string WriteModificationsDetails(IEnumerable<Modification> modifications)
        {
            const string modificationLine = "<tr><td><font size=2>{0}</font></td><td><font size=2>{1}/{2}</font></td></tr>";
            const string changesetHeader = "<tr><td><b>{0}</b></td><td>{1}</td></tr>";
            StringWriter mods = new StringWriter();

            mods.WriteLine("<h4>Detailed information of the modifications in the build :</h4>");
            mods.WriteLine("<table cellpadding=\"5\">");
            string previousModificationChecksum = string.Empty;
            foreach (Modification modification in modifications)
            {
                string modificationChecksum = modification.UserName + "__CCNET__" + modification.Comment;

                if (previousModificationChecksum != modificationChecksum)
                {
                    mods.WriteLine(string.Format(changesetHeader,
                                                 modification.UserName,
                                                 modification.Comment));
                }

                mods.WriteLine(
                    string.Format(modificationLine,
                                  modification.Type,
                                  modification.FolderName,
                                  modification.FileName
                        ));

                previousModificationChecksum = modificationChecksum;
            }
            mods.WriteLine("</table>");

            return mods.ToString();
        }
    }

}
