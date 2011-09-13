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

                        if (AtChiliCCNet())
                        {
                            ie.TextField(WatiN.Core.Find.ById("content_text")).SetAttributeValue("value", content);
                            ie.Button(WatiN.Core.Find.ByName("commit")).Click();
                        }
                        else
                        {
                            WriteToOutput(string.Format("  page for {0} not found.", wikiFile), OutputType.Error);
                        }

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
            FileName2ChiliPageNameMapping.Add("accurev.wiki", "Accurev");
            FileName2ChiliPageNameMapping.Add("actionFilter.wiki", "");
            FileName2ChiliPageNameMapping.Add("alienbrain.wiki", "AlienBrain");
            FileName2ChiliPageNameMapping.Add("andCondition.wiki", "And_Condition");
            FileName2ChiliPageNameMapping.Add("antsPerformance.wiki", "ANTS_Performance_Profiler_Task");
            FileName2ChiliPageNameMapping.Add("artifactcleanup.wiki", "Artifact_Cleanup_Publisher");
            FileName2ChiliPageNameMapping.Add("assemblyMatch.wiki", "");
            FileName2ChiliPageNameMapping.Add("assemblyVersionLabeller.wiki", "Assembly_Version_Labeller");
            FileName2ChiliPageNameMapping.Add("bitkeeper.wiki", "BitKeeper");
            FileName2ChiliPageNameMapping.Add("buildCondition.wiki", "Build_Condition");
            FileName2ChiliPageNameMapping.Add("buildpublisher.wiki", "Build_Publisher");
            FileName2ChiliPageNameMapping.Add("changeSynergy.wiki", "");
            FileName2ChiliPageNameMapping.Add("checkHttpStatus.wiki", "");
            FileName2ChiliPageNameMapping.Add("clearCase.wiki", "ClearCase");
            FileName2ChiliPageNameMapping.Add("codeItRight.wiki", "CodeItRight_Analysis_Task");
            FileName2ChiliPageNameMapping.Add("commentFilter.wiki", "");
            FileName2ChiliPageNameMapping.Add("commentTask.wiki", "Comment_Task");
            FileName2ChiliPageNameMapping.Add("compareCondition.wiki", "Compare_Values_Condition");
            FileName2ChiliPageNameMapping.Add("conditional.wiki", "Conditional_Task");
            FileName2ChiliPageNameMapping.Add("conditionalPublisher.wiki", "Conditional_Publisher");
            FileName2ChiliPageNameMapping.Add("controlAction.wiki", "CruiseServer_Control_Action");
            FileName2ChiliPageNameMapping.Add("coverageFilter.wiki", "");
            FileName2ChiliPageNameMapping.Add("coverageThreshold.wiki", "");
            FileName2ChiliPageNameMapping.Add("cronTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("cruiseServerControl.wiki", "CruiseServer_Control_Task");
            FileName2ChiliPageNameMapping.Add("cvs.wiki", "CVS");
            FileName2ChiliPageNameMapping.Add("dateLabeller.wiki", "Date_Labeller");
            FileName2ChiliPageNameMapping.Add("defaultIssueTracker.wiki", "");
            FileName2ChiliPageNameMapping.Add("defaultlabeller.wiki", "Default_Labeller");
            FileName2ChiliPageNameMapping.Add("defaultManifestGenerator.wiki", "");
            FileName2ChiliPageNameMapping.Add("defaultProjectSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("devenv.wiki", "Visual_Studio_Task");
            FileName2ChiliPageNameMapping.Add("directValue.wiki", "Direct_Dynamic_Value");
            FileName2ChiliPageNameMapping.Add("dupfinder.wiki", "Duplicate_Finder_Task");
            FileName2ChiliPageNameMapping.Add("email.wiki", "Email_Publisher");
            FileName2ChiliPageNameMapping.Add("encryptedChannel.wiki", "");
            FileName2ChiliPageNameMapping.Add("exec.wiki", "Executable_Task");
            FileName2ChiliPageNameMapping.Add("external.wiki", "External");
            FileName2ChiliPageNameMapping.Add("externalFileSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("fake.wiki", "FAKE_Task");
            FileName2ChiliPageNameMapping.Add("FBVariable.wiki", "FinalBuilder_Task");
            FileName2ChiliPageNameMapping.Add("fileBasedCache.wiki", "");
            FileName2ChiliPageNameMapping.Add("fileExistsCondition.wiki", "File_Exists_Condition");
            FileName2ChiliPageNameMapping.Add("fileLabeller.wiki", "File_Labeller");
            FileName2ChiliPageNameMapping.Add("filesystem.wiki", "FileSystem");
            FileName2ChiliPageNameMapping.Add("fileToMerge.wiki", "File_Merge_Task");
            FileName2ChiliPageNameMapping.Add("filtered.wiki", "Filtered");
            FileName2ChiliPageNameMapping.Add("filterTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("FinalBuilder.wiki", "");
            FileName2ChiliPageNameMapping.Add("firstMatch.wiki", "");
            FileName2ChiliPageNameMapping.Add("folderExistsCondition.wiki", "Folder_Exists_Condition");
            FileName2ChiliPageNameMapping.Add("forcebuild.wiki", "ForceBuildPublisher");
            FileName2ChiliPageNameMapping.Add("ftp.wiki", "Ftp_task_-_Publisher");
            FileName2ChiliPageNameMapping.Add("ftpSourceControl.wiki", "FtpSourceControl");
            FileName2ChiliPageNameMapping.Add("gendarme.wiki", "Gendarme_Task");
            FileName2ChiliPageNameMapping.Add("git.wiki", "Git");
            FileName2ChiliPageNameMapping.Add("group.wiki", "");
            FileName2ChiliPageNameMapping.Add("header.wiki", "");
            FileName2ChiliPageNameMapping.Add("hg.wiki", "Mercurial_(Hg)");
            FileName2ChiliPageNameMapping.Add("hgweb.wiki", "");
            FileName2ChiliPageNameMapping.Add("httpRequest.wiki", "HTTP_Status_Task");
            FileName2ChiliPageNameMapping.Add("impersonation.wiki", "");
            FileName2ChiliPageNameMapping.Add("importManifest.wiki", "");
            FileName2ChiliPageNameMapping.Add("inheritedProjectSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("inMemoryCache.wiki", "");
            FileName2ChiliPageNameMapping.Add("internalSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("intervalTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("iterationlabeller.wiki", "Iteration_Labeller");
            FileName2ChiliPageNameMapping.Add("lastBuildTimeCondition.wiki", "Last_Build_Time_Condition");
            FileName2ChiliPageNameMapping.Add("lastChangeLabeller.wiki", "Last_Change_Labeller");
            FileName2ChiliPageNameMapping.Add("lastStatusCondition.wiki", "Last_Build_Status_Condition");
            FileName2ChiliPageNameMapping.Add("ldapConverter.wiki", "");
            FileName2ChiliPageNameMapping.Add("ldapUser.wiki", "");
            FileName2ChiliPageNameMapping.Add("merge.wiki", "");
            FileName2ChiliPageNameMapping.Add("mks.wiki", "Mks");
            FileName2ChiliPageNameMapping.Add("modificationHistory.wiki", "ModificationHistory_Publisher");
            FileName2ChiliPageNameMapping.Add("modificationReader.wiki", "Modification_Reader_Task");
            FileName2ChiliPageNameMapping.Add("modificationWriter.wiki", "Modification_Writer_Task");
            FileName2ChiliPageNameMapping.Add("msbuild.wiki", "MsBuild_Task");
            FileName2ChiliPageNameMapping.Add("multi.wiki", "Multi_Source_Control");
            FileName2ChiliPageNameMapping.Add("multiIssueTracker.wiki", "");
            FileName2ChiliPageNameMapping.Add("multiTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("namespaceMapping.wiki", "");
            FileName2ChiliPageNameMapping.Add("nant.wiki", "NAnt_Task");
            FileName2ChiliPageNameMapping.Add("ncoverProfile.wiki", "NCover_Profiler_Task");
            FileName2ChiliPageNameMapping.Add("ncoverReport.wiki", "NCover_Reporting_Task");
            FileName2ChiliPageNameMapping.Add("ndepend.wiki", "NDepend_Task");
            FileName2ChiliPageNameMapping.Add("nullProjectSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("nullSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("nullSourceControl.wiki", "");
            FileName2ChiliPageNameMapping.Add("nullTask.wiki", "Null_Task");
            FileName2ChiliPageNameMapping.Add("nunit.wiki", "NUnit_Task");
            FileName2ChiliPageNameMapping.Add("orCondition.wiki", "Or_Condition");
            FileName2ChiliPageNameMapping.Add("p4.wiki", "Perforce_(P4)");
            FileName2ChiliPageNameMapping.Add("package.wiki", "Package_Publisher");
            FileName2ChiliPageNameMapping.Add("packageFile.wiki", "");
            FileName2ChiliPageNameMapping.Add("packageFolder.wiki", "");
            FileName2ChiliPageNameMapping.Add("parallel.wiki", "Parallel_Task");
            FileName2ChiliPageNameMapping.Add("parameterTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("passwordUser.wiki", "");
            FileName2ChiliPageNameMapping.Add("pathFilter.wiki", "");
            FileName2ChiliPageNameMapping.Add("permissions.wiki", "");
            FileName2ChiliPageNameMapping.Add("plasticscm.wiki", "Plastic");
            FileName2ChiliPageNameMapping.Add("powershell.wiki", "PowerShell_Task");
            FileName2ChiliPageNameMapping.Add("project.wiki", "Project_Configuration_Block");
            FileName2ChiliPageNameMapping.Add("projectSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("projectTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("pvcs.wiki", "PVCS");
            FileName2ChiliPageNameMapping.Add("queue.wiki", "");
            FileName2ChiliPageNameMapping.Add("rake.wiki", "Rake_Task");
            FileName2ChiliPageNameMapping.Add("regexConverter.wiki", "");
            FileName2ChiliPageNameMapping.Add("regexIssueTracker.wiki", "");
            FileName2ChiliPageNameMapping.Add("remoteProjectLabeller.wiki", "Remote_Project_Labeller");
            FileName2ChiliPageNameMapping.Add("replacementValue.wiki", "Replacement_Dynamic_Value");
            FileName2ChiliPageNameMapping.Add("robocopy.wiki", "Robocopy");
            FileName2ChiliPageNameMapping.Add("rolePermission.wiki", "");
            FileName2ChiliPageNameMapping.Add("rollUpTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("rss.wiki", "RSS_Publisher");
            FileName2ChiliPageNameMapping.Add("scheduleTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("sequential.wiki", "Sequential_Task");
            FileName2ChiliPageNameMapping.Add("simpleUser.wiki", "");
            FileName2ChiliPageNameMapping.Add("starteam.wiki", "StarTeam");
            FileName2ChiliPageNameMapping.Add("state.wiki", "");
            FileName2ChiliPageNameMapping.Add("stateFileLabeller.wiki", "State_File_Labeller");
            FileName2ChiliPageNameMapping.Add("statistic.wiki", "");
            FileName2ChiliPageNameMapping.Add("statistics.wiki", "Statistics_Publisher");
            FileName2ChiliPageNameMapping.Add("statusCondition.wiki", "Status_Condition");
            FileName2ChiliPageNameMapping.Add("subject.wiki", "");
            FileName2ChiliPageNameMapping.Add("surround.wiki", "Seapine_Surround");
            FileName2ChiliPageNameMapping.Add("svn.wiki", "Subversion_(svn)");
            FileName2ChiliPageNameMapping.Add("synchronised.wiki", "Synchronisation_Context_Task");
            FileName2ChiliPageNameMapping.Add("synergy.wiki", "Telelogic_Synergy");
            FileName2ChiliPageNameMapping.Add("synergyConnection.wiki", "");
            FileName2ChiliPageNameMapping.Add("synergyProject.wiki", "");
            FileName2ChiliPageNameMapping.Add("updateConfig.wiki", "");
            FileName2ChiliPageNameMapping.Add("urlHeaderValueCondition.wiki", "URL_Header_Value_Condition");
            FileName2ChiliPageNameMapping.Add("urlPingCondition.wiki", "URL_Ping_Condition");
            FileName2ChiliPageNameMapping.Add("urlTrigger.wiki", "");
            FileName2ChiliPageNameMapping.Add("user.wiki", "");
            FileName2ChiliPageNameMapping.Add("userFilter.wiki", "");
            FileName2ChiliPageNameMapping.Add("userName.wiki", "");
            FileName2ChiliPageNameMapping.Add("userPermission.wiki", "");
            FileName2ChiliPageNameMapping.Add("variable.wiki", "");
            FileName2ChiliPageNameMapping.Add("vault.wiki", "SourceGear_Vault");
            FileName2ChiliPageNameMapping.Add("viewcvs.wiki", "");
            FileName2ChiliPageNameMapping.Add("vss.wiki", "SourceSafe_(vss)");
            FileName2ChiliPageNameMapping.Add("vsts.wiki", "Team_Foundation_Server_(Tfs)");
            FileName2ChiliPageNameMapping.Add("websvn.wiki", "");
            FileName2ChiliPageNameMapping.Add("xmlFileAudit.wiki", "");
            FileName2ChiliPageNameMapping.Add("xmlFileAuditReader.wiki", "");
            FileName2ChiliPageNameMapping.Add("xmlFolderData.wiki", "");
            FileName2ChiliPageNameMapping.Add("xmllogger.wiki", "Xml_Log_Publisher");

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
