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

        /// <summary>
        /// Description used for the visualisation of the buildstage, if left empty the process name will be shown
        /// </summary>
        [ReflectorProperty("description", Required = false)]
        public string Description = string.Empty;


        public void Run(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(Description != string.Empty ? Description : "Making RSS feed");                

            using (StreamWriter stream = File.CreateText(RSSDataFileLocation(result.ArtifactDirectory)))
            {
                stream.Write(GenerateDocument(result));
            }
        }

        public string GenerateDocument(IIntegrationResult result)
        {

            System.IO.StringWriter RSSInfo = new StringWriter();

            RSSInfo.WriteLine("<?xml version=\"1.0\"?>");
            RSSInfo.WriteLine("<rss version=\"2.0\" ");
            RSSInfo.WriteLine("     xmlns:content=\"http://purl.org/rss/1.0/modules/content/\" ");
            RSSInfo.WriteLine(">");
            RSSInfo.WriteLine("  <channel> ");
            RSSInfo.WriteLine("    <title>CruiseControl.NET - {0}</title>", result.ProjectName);
            RSSInfo.WriteLine("    <link>{0}</link>", result.ProjectUrl);
            RSSInfo.WriteLine("    <description>The latest build results for {0}</description>", result.ProjectName);
            RSSInfo.WriteLine("    <language>en</language>");
            RSSInfo.WriteLine("    <ttl>5</ttl>");
            RSSInfo.WriteLine("    <item>");
            RSSInfo.WriteLine("        <title>Build {0} : {1}  {2}  {3}</title>", result.Label, result.Status.ToString(), GetAmountOfModifiedfiles(result), GetFirstCommentedModification(result));
            RSSInfo.WriteLine("        <description>{0}</description>", GetAmountOfModifiedfiles(result));

            if (result.HasModifications())
            {
                RSSInfo.WriteLine("        <content:encoded>{0}</content:encoded>", GetBuildModifications(result));
            }

            RSSInfo.WriteLine("    </item>");
            RSSInfo.WriteLine("  </channel> ");
            RSSInfo.WriteLine("</rss>");

            return RSSInfo.ToString();
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

            mods.WriteLine("<![CDATA[");

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

            mods.WriteLine("]]>");

            return mods.ToString();
        }
    }

}
