using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;

namespace Console
{
    public class ChiliAutomation
    {
        private IE ie;
        private Dictionary<string, string> FileName2ChiliPageNameMapping = new Dictionary<string, string>();

        public bool Login(string username, string password)
        {
            ie = new IE();

            ie.GoTo("http://www.cruisecontrolnet.org/login");
            if (!AtChiliCCNet()) return false;

            ie.TextField(WatiN.Core.Find.ByName("username")).SetAttributeValue("value", username);

            ie.TextField(WatiN.Core.Find.ByName("password")).SetAttributeValue("value", password);
            ie.Button(WatiN.Core.Find.ByName("login")).Click();

            if (ie.Elements.Exists("password")) return false; // still on logon page, so login failed

            LoadMapping();

            return true;
        }

        public void logout()
        {
            ie.Close();
            ie.Dispose();

        }


        public void UpdateDocs(string docFolder)
        {

            string wikiurl = "http://www.cruisecontrolnet.org/projects/ccnet/wiki";


            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();


            foreach (var wikiFile in FileName2ChiliPageNameMapping.Keys)
            {
                WriteToOutput(string.Format("updating page of {0}", wikiFile), OutputType.Info);
                var wikipage = FileName2ChiliPageNameMapping[wikiFile];


                if (string.IsNullOrEmpty(FileName2ChiliPageNameMapping[wikiFile]))
                {
                    WriteToOutput(string.Format("  {0} not mapped yet", wikiFile), OutputType.Warning);
                }
                else
                {
                    var fullWikiFile = System.IO.Path.Combine(docFolder, wikiFile);
                    if (!System.IO.File.Exists(fullWikiFile))
                    {
                        WriteToOutput(string.Format("  {0} not found.", wikiFile), OutputType.Error);
                    }
                    else
                    {
                        string content = System.IO.File.ReadAllText(fullWikiFile);
                        string editurl = string.Concat(wikiurl, "/", FileName2ChiliPageNameMapping[wikiFile], "/edit");

                        ie.GoTo(editurl);

                        ie.TextField(WatiN.Core.Find.ById("content_text")).SetAttributeValue("value", content);
                        ie.Button(WatiN.Core.Find.ByName("commit")).Click();


                        WriteToOutput(string.Format("  duration : {0} ms", stopwatch.ElapsedMilliseconds), OutputType.Info);

                    }
                }

            }

            WriteToOutput(string.Format("Total  duration : {0} ms", stopwatch.ElapsedMilliseconds), OutputType.Info);
            stopwatch.Stop();

            //string editurl = string.Concat(wikiurl, "/", wantedPage, "/edit");
            //ie.GoTo(editurl);

            ////ie.TextField(WatiN.Core.Find.ById("content_text")).TypeText(content);
            //ie.TextField(WatiN.Core.Find.ById("content_text")).SetAttributeValue("value", content);

            //ie.Button(WatiN.Core.Find.ByName("commit")).Click();

        }



        private bool AtChiliCCNet()
        {
            return ie.Title.StartsWith("CruiseControl.NET");
        }


        /// <summary>
        /// this is the mapping between the chili wiki name and the file name
        /// Making it possible to give the wiki pages a readable name
        /// </summary>
        private void LoadMapping()
        {
            FileName2ChiliPageNameMapping.Add("accurev.wiki", "");
            FileName2ChiliPageNameMapping.Add("actionFilter.wiki", "");
            FileName2ChiliPageNameMapping.Add("alienbrain.wiki", "");
            FileName2ChiliPageNameMapping.Add("andCondition.wiki", "");
            FileName2ChiliPageNameMapping.Add("antsPerformance.wiki", "");
            FileName2ChiliPageNameMapping.Add("artifactcleanup.wiki", "");
            FileName2ChiliPageNameMapping.Add("assemblyMatch.wiki", "");
            FileName2ChiliPageNameMapping.Add("assemblyVersionLabeller.wiki", "Assembly_Version_Labeller");
            FileName2ChiliPageNameMapping.Add("bitkeeper.wiki", "");
            FileName2ChiliPageNameMapping.Add("buildCondition.wiki", "");
            FileName2ChiliPageNameMapping.Add("buildpublisher.wiki", "");
            FileName2ChiliPageNameMapping.Add("changeSynergy.wiki", "");
            FileName2ChiliPageNameMapping.Add("checkHttpStatus.wiki", "");
            FileName2ChiliPageNameMapping.Add("clearCase.wiki", "");
            FileName2ChiliPageNameMapping.Add("codeItRight.wiki", "");
            FileName2ChiliPageNameMapping.Add("commentFilter.wiki", "");
            FileName2ChiliPageNameMapping.Add("commentTask.wiki", "");
            FileName2ChiliPageNameMapping.Add("compareCondition.wiki", "");
            FileName2ChiliPageNameMapping.Add("conditional.wiki", "");
            FileName2ChiliPageNameMapping.Add("conditionalPublisher.wiki", "");
            FileName2ChiliPageNameMapping.Add("controlAction.wiki", "");
            FileName2ChiliPageNameMapping.Add("coverageFilter.wiki", "");
            FileName2ChiliPageNameMapping.Add("coverageThreshold.wiki", "");
            FileName2ChiliPageNameMapping.Add("cronTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("cruiseServerControl.wiki", "");
            FileName2ChiliPageNameMapping.Add("cvs.wiki", "");
            FileName2ChiliPageNameMapping.Add("dateLabeller.wiki", "Date_Labeller");
            FileName2ChiliPageNameMapping.Add("defaultIssueTracker.wiki", "");
            FileName2ChiliPageNameMapping.Add("defaultlabeller.wiki", "Default_Labeller");
            FileName2ChiliPageNameMapping.Add("defaultManifestGenerator.wiki", "");
            FileName2ChiliPageNameMapping.Add("defaultProjectSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("devenv.wiki", "");
            FileName2ChiliPageNameMapping.Add("directValue.wiki", "");
            FileName2ChiliPageNameMapping.Add("dupfinder.wiki", "");
            FileName2ChiliPageNameMapping.Add("email.wiki", "");
            FileName2ChiliPageNameMapping.Add("encryptedChannel.wiki", "");
            FileName2ChiliPageNameMapping.Add("exec.wiki", "");
            FileName2ChiliPageNameMapping.Add("external.wiki", "");
            FileName2ChiliPageNameMapping.Add("externalFileSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("fake.wiki", "");
            FileName2ChiliPageNameMapping.Add("FBVariable.wiki", "");
            FileName2ChiliPageNameMapping.Add("fileBasedCache.wiki", "");
            FileName2ChiliPageNameMapping.Add("fileExistsCondition.wiki", "");
            FileName2ChiliPageNameMapping.Add("fileLabeller.wiki", "File_Labeller");
            FileName2ChiliPageNameMapping.Add("filesystem.wiki", "");
            FileName2ChiliPageNameMapping.Add("fileToMerge.wiki", "");
            FileName2ChiliPageNameMapping.Add("filtered.wiki", "");
            FileName2ChiliPageNameMapping.Add("filterTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("FinalBuilder.wiki", "");
            FileName2ChiliPageNameMapping.Add("firstMatch.wiki", "");
            FileName2ChiliPageNameMapping.Add("folderExistsCondition.wiki", "");
            FileName2ChiliPageNameMapping.Add("forcebuild.wiki", "");
            FileName2ChiliPageNameMapping.Add("ftp.wiki", "");
            FileName2ChiliPageNameMapping.Add("ftpSourceControl.wiki", "");
            FileName2ChiliPageNameMapping.Add("gendarme.wiki", "");
            FileName2ChiliPageNameMapping.Add("git.wiki", "");
            FileName2ChiliPageNameMapping.Add("group.wiki", "");
            FileName2ChiliPageNameMapping.Add("header.wiki", "");
            FileName2ChiliPageNameMapping.Add("hg.wiki", "");
            FileName2ChiliPageNameMapping.Add("hgweb.wiki", "");
            FileName2ChiliPageNameMapping.Add("httpRequest.wiki", "");
            FileName2ChiliPageNameMapping.Add("impersonation.wiki", "");
            FileName2ChiliPageNameMapping.Add("importManifest.wiki", "");
            FileName2ChiliPageNameMapping.Add("inheritedProjectSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("inMemoryCache.wiki", "");
            FileName2ChiliPageNameMapping.Add("internalSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("intervalTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("iterationlabeller.wiki", "Iteration_Labeller");
            FileName2ChiliPageNameMapping.Add("lastBuildTimeCondition.wiki", "");
            FileName2ChiliPageNameMapping.Add("lastChangeLabeller.wiki", "Last_Change_Labeller");
            FileName2ChiliPageNameMapping.Add("lastStatusCondition.wiki", "");
            FileName2ChiliPageNameMapping.Add("ldapConverter.wiki", "");
            FileName2ChiliPageNameMapping.Add("ldapUser.wiki", "");
            FileName2ChiliPageNameMapping.Add("merge.wiki", "");
            FileName2ChiliPageNameMapping.Add("mks.wiki", "");
            FileName2ChiliPageNameMapping.Add("modificationHistory.wiki", "");
            FileName2ChiliPageNameMapping.Add("modificationReader.wiki", "");
            FileName2ChiliPageNameMapping.Add("modificationWriter.wiki", "");
            FileName2ChiliPageNameMapping.Add("msbuild.wiki", "");
            FileName2ChiliPageNameMapping.Add("multi.wiki", "");
            FileName2ChiliPageNameMapping.Add("multiIssueTracker.wiki", "");
            FileName2ChiliPageNameMapping.Add("multiTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("namespaceMapping.wiki", "");
            FileName2ChiliPageNameMapping.Add("nant.wiki", "");
            FileName2ChiliPageNameMapping.Add("ncoverProfile.wiki", "");
            FileName2ChiliPageNameMapping.Add("ncoverReport.wiki", "");
            FileName2ChiliPageNameMapping.Add("ndepend.wiki", "");
            FileName2ChiliPageNameMapping.Add("nullProjectSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("nullSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("nullSourceControl.wiki", "");
            FileName2ChiliPageNameMapping.Add("nullTask.wiki", "");
            FileName2ChiliPageNameMapping.Add("nunit.wiki", "");
            FileName2ChiliPageNameMapping.Add("orCondition.wiki", "");
            FileName2ChiliPageNameMapping.Add("p4.wiki", "");
            FileName2ChiliPageNameMapping.Add("package.wiki", "");
            FileName2ChiliPageNameMapping.Add("packageFile.wiki", "");
            FileName2ChiliPageNameMapping.Add("packageFolder.wiki", "");
            FileName2ChiliPageNameMapping.Add("parallel.wiki", "");
            FileName2ChiliPageNameMapping.Add("parameterTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("passwordUser.wiki", "");
            FileName2ChiliPageNameMapping.Add("pathFilter.wiki", "");
            FileName2ChiliPageNameMapping.Add("permissions.wiki", "");
            FileName2ChiliPageNameMapping.Add("plasticscm.wiki", "");
            FileName2ChiliPageNameMapping.Add("powershell.wiki", "");
            FileName2ChiliPageNameMapping.Add("project.wiki", "");
            FileName2ChiliPageNameMapping.Add("projectSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("projectTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("pvcs.wiki", "");
            FileName2ChiliPageNameMapping.Add("queue.wiki", "");
            FileName2ChiliPageNameMapping.Add("rake.wiki", "");
            FileName2ChiliPageNameMapping.Add("regexConverter.wiki", "");
            FileName2ChiliPageNameMapping.Add("regexIssueTracker.wiki", "");
            FileName2ChiliPageNameMapping.Add("remoteProjectLabeller.wiki", "Remote_Project_Labeller");
            FileName2ChiliPageNameMapping.Add("replacementValue.wiki", "");
            FileName2ChiliPageNameMapping.Add("robocopy.wiki", "");
            FileName2ChiliPageNameMapping.Add("rolePermission.wiki", "");
            FileName2ChiliPageNameMapping.Add("rollUpTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("rss.wiki", "");
            FileName2ChiliPageNameMapping.Add("scheduleTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("sequential.wiki", "");
            FileName2ChiliPageNameMapping.Add("simpleUser.wiki", "");
            FileName2ChiliPageNameMapping.Add("starteam.wiki", "");
            FileName2ChiliPageNameMapping.Add("state.wiki", "");
            FileName2ChiliPageNameMapping.Add("stateFileLabeller.wiki", "State_File_Labeller");
            FileName2ChiliPageNameMapping.Add("statistic.wiki", "");
            FileName2ChiliPageNameMapping.Add("statistics.wiki", "");
            FileName2ChiliPageNameMapping.Add("statusCondition.wiki", "");
            FileName2ChiliPageNameMapping.Add("subject.wiki", "");
            FileName2ChiliPageNameMapping.Add("surround.wiki", "");
            FileName2ChiliPageNameMapping.Add("svn.wiki", "");
            FileName2ChiliPageNameMapping.Add("synchronised.wiki", "");
            FileName2ChiliPageNameMapping.Add("synergy.wiki", "");
            FileName2ChiliPageNameMapping.Add("synergyConnection.wiki", "");
            FileName2ChiliPageNameMapping.Add("synergyProject.wiki", "");
            FileName2ChiliPageNameMapping.Add("updateConfig.wiki", "");
            FileName2ChiliPageNameMapping.Add("urlHeaderValueCondition.wiki", "");
            FileName2ChiliPageNameMapping.Add("urlPingCondition.wiki", "");
            FileName2ChiliPageNameMapping.Add("urlTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("user.wiki", "");
            FileName2ChiliPageNameMapping.Add("userFilter.wiki", "");
            FileName2ChiliPageNameMapping.Add("userName.wiki", "");
            FileName2ChiliPageNameMapping.Add("userPermission.wiki", "");
            FileName2ChiliPageNameMapping.Add("variable.wiki", "");
            FileName2ChiliPageNameMapping.Add("vault.wiki", "");
            FileName2ChiliPageNameMapping.Add("viewcvs.wiki", "");
            FileName2ChiliPageNameMapping.Add("vss.wiki", "");
            FileName2ChiliPageNameMapping.Add("vsts.wiki", "");
            FileName2ChiliPageNameMapping.Add("websvn.wiki", "");
            FileName2ChiliPageNameMapping.Add("xmlFileAudit.wiki", "");
            FileName2ChiliPageNameMapping.Add("xmlFileAuditReader.wiki", "");
            FileName2ChiliPageNameMapping.Add("xmlFolderData.wiki", "");
            FileName2ChiliPageNameMapping.Add("xmllogger.wiki", "");

        }


        /// <summary>
        /// just copied from main program
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        private static void WriteToOutput(string message, OutputType type)
        {
            var current = System.Console.ForegroundColor;
            switch (type)
            {
                case OutputType.Debug:
                    System.Console.ForegroundColor = ConsoleColor.Gray;
                    break;

                case OutputType.Info:
                    System.Console.ForegroundColor = ConsoleColor.Blue;
                    break;

                case OutputType.Warning:
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case OutputType.Error:
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            System.Console.WriteLine(message);
            System.Console.ForegroundColor = current;
        }


    }
}
