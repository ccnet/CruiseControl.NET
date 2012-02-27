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


        public ChiliAutomation()
        {
            LoadMapping();
        }

        public bool Login(string username, string password)
        {
            ie = new IE();

            ie.GoTo("http://www.cruisecontrolnet.org/login");
            if (!AtChiliCCNet()) return false;

            ie.TextField(WatiN.Core.Find.ByName("username")).SetAttributeValue("value", username);

            ie.TextField(WatiN.Core.Find.ByName("password")).SetAttributeValue("value", password);
            ie.Button(WatiN.Core.Find.ByName("login")).Click();

            if (ie.Elements.Exists("password")) return false; // still on logon page, so login failed

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
                    }
                }

            }

            WriteToOutput("", OutputType.Info);

            WriteToOutput(string.Format("Total  duration : {0} ms", stopwatch.ElapsedMilliseconds), OutputType.Info);
            stopwatch.Stop();

            //string editurl = string.Concat(wikiurl, "/", wantedPage, "/edit");
            //ie.GoTo(editurl);

            ////ie.TextField(WatiN.Core.Find.ById("content_text")).TypeText(content);
            //ie.TextField(WatiN.Core.Find.ById("content_text")).SetAttributeValue("value", content);

            //ie.Button(WatiN.Core.Find.ByName("commit")).Click();

        }

        public int ReportNotMappedWikiFiles(string docFolder)
        {
            int missingFiles = 0;

            var existingFiles = System.IO.Directory.GetFiles(docFolder, "*.wiki");
            WriteToOutput(string.Format("Files in output folder {0}, files in wiki mapping {1}", existingFiles.Length, FileName2ChiliPageNameMapping.Count), OutputType.Info);

            foreach (var ef in existingFiles)
            {
                var barefilename = new System.IO.FileInfo(ef).Name;
                if (!FileName2ChiliPageNameMapping.ContainsKey(barefilename))
                {
                    missingFiles++;
                    WriteToOutput(string.Format("{0}", barefilename), OutputType.Warning);
                }
            }

            if (missingFiles > 0)
            {
                WriteToOutput(missingFiles.ToString() + " files are not mapped !", OutputType.Error);
            }


            return missingFiles;
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
            FileName2ChiliPageNameMapping.Add("actionFilter.wiki", "ActionFilter");
            FileName2ChiliPageNameMapping.Add("alienbrain.wiki", "AlienBrain");
            FileName2ChiliPageNameMapping.Add("andCondition.wiki", "And_Condition");
            FileName2ChiliPageNameMapping.Add("antsPerformance.wiki", "ANTS_Performance_Profiler_Task");
            FileName2ChiliPageNameMapping.Add("artifactcleanup.wiki", "Artifact_Cleanup_Publisher");
            FileName2ChiliPageNameMapping.Add("assemblyMatch.wiki", "Assembly_Match");
            FileName2ChiliPageNameMapping.Add("assemblyVersionLabeller.wiki", "Assembly_Version_Labeller");
            FileName2ChiliPageNameMapping.Add("bitkeeper.wiki", "BitKeeper");
            FileName2ChiliPageNameMapping.Add("buildCondition.wiki", "Build_Condition");
            FileName2ChiliPageNameMapping.Add("buildpublisher.wiki", "Build_Publisher");
            FileName2ChiliPageNameMapping.Add("changeSynergy.wiki", "SynergyIssueTracker");
            FileName2ChiliPageNameMapping.Add("checkHttpStatus.wiki", "HTTP_Status_Task");
            FileName2ChiliPageNameMapping.Add("clearCase.wiki", "ClearCase");
            FileName2ChiliPageNameMapping.Add("codeItRight.wiki", "CodeItRight_Analysis_Task");
            FileName2ChiliPageNameMapping.Add("commentFilter.wiki", "CommentFilter");
            FileName2ChiliPageNameMapping.Add("commentTask.wiki", "Comment_Task");
            FileName2ChiliPageNameMapping.Add("compareCondition.wiki", "Compare_Values_Condition");
            FileName2ChiliPageNameMapping.Add("conditional.wiki", "Conditional_Task");
            FileName2ChiliPageNameMapping.Add("conditionalPublisher.wiki", "Conditional_Publisher");
            FileName2ChiliPageNameMapping.Add("controlAction.wiki", "CruiseServer_Control_Action");
            FileName2ChiliPageNameMapping.Add("coverageFilter.wiki", "Coverage_Filter");
            FileName2ChiliPageNameMapping.Add("coverageThreshold.wiki", "Coverage_Threshold");
            FileName2ChiliPageNameMapping.Add("cronTrigger.wiki", "Cron_Trigger");
            FileName2ChiliPageNameMapping.Add("cruiseServerControl.wiki", "CruiseServer_Control_Task");
            FileName2ChiliPageNameMapping.Add("cvs.wiki", "CVS");
            FileName2ChiliPageNameMapping.Add("dateLabeller.wiki", "Date_Labeller");
            FileName2ChiliPageNameMapping.Add("defaultIssueTracker.wiki", "DefaultIssueTracker");
            FileName2ChiliPageNameMapping.Add("defaultlabeller.wiki", "Default_Labeller");
            FileName2ChiliPageNameMapping.Add("defaultManifestGenerator.wiki", "Default_Manifest_Generator");
            FileName2ChiliPageNameMapping.Add("defaultProjectSecurity.wiki", "Default_Project_Security");
            FileName2ChiliPageNameMapping.Add("devenv.wiki", "Visual_Studio_Task");
            FileName2ChiliPageNameMapping.Add("directValue.wiki", "Direct_Dynamic_Value");
            FileName2ChiliPageNameMapping.Add("dupfinder.wiki", "Duplicate_Finder_Task");
            FileName2ChiliPageNameMapping.Add("dumpValue.wiki", "DumpValue");
            FileName2ChiliPageNameMapping.Add("dumpValueItem.wiki", "DumpValue_Item");
            FileName2ChiliPageNameMapping.Add("email.wiki", "Email_Publisher");
            FileName2ChiliPageNameMapping.Add("encryptedChannel.wiki", "Encrypted_Messages_Channel");
            FileName2ChiliPageNameMapping.Add("exec.wiki", "Executable_Task");
            FileName2ChiliPageNameMapping.Add("external.wiki", "External");
            FileName2ChiliPageNameMapping.Add("externalLink.wiki", "External_Links");
            FileName2ChiliPageNameMapping.Add("externalFileSecurity.wiki", "External_File_Server_Security");
            FileName2ChiliPageNameMapping.Add("fake.wiki", "FAKE_Task");
            FileName2ChiliPageNameMapping.Add("FBVariable.wiki", "FBVariable");
            FileName2ChiliPageNameMapping.Add("fileBasedCache.wiki", "File_Based_Security_Cache");
            FileName2ChiliPageNameMapping.Add("fileExistsCondition.wiki", "File_Exists_Condition");
            FileName2ChiliPageNameMapping.Add("fileLabeller.wiki", "File_Labeller");
            FileName2ChiliPageNameMapping.Add("filesystem.wiki", "FileSystem");
            FileName2ChiliPageNameMapping.Add("fileToMerge.wiki", "Merge_File");
            FileName2ChiliPageNameMapping.Add("filtered.wiki", "Filtered");
            FileName2ChiliPageNameMapping.Add("filterTrigger.wiki", "Filter_Trigger");
            FileName2ChiliPageNameMapping.Add("FinalBuilder.wiki", "FinalBuilder_Task");
            FileName2ChiliPageNameMapping.Add("firstMatch.wiki", "FirstMatch");
            FileName2ChiliPageNameMapping.Add("folderExistsCondition.wiki", "Folder_Exists_Condition");
            FileName2ChiliPageNameMapping.Add("forcebuild.wiki", "ForceBuildPublisher");
            FileName2ChiliPageNameMapping.Add("ftp.wiki", "Ftp_task_-_Publisher");
            FileName2ChiliPageNameMapping.Add("ftpSourceControl.wiki", "FtpSourceControl");
            FileName2ChiliPageNameMapping.Add("gendarme.wiki", "Gendarme_Task");
            FileName2ChiliPageNameMapping.Add("git.wiki", "Git");
            FileName2ChiliPageNameMapping.Add("group.wiki", "Email_Group");
            FileName2ChiliPageNameMapping.Add("header.wiki", "HTTP_Request_Header");
            FileName2ChiliPageNameMapping.Add("hg.wiki", "Mercurial_(Hg)");
            FileName2ChiliPageNameMapping.Add("hgweb.wiki", "MercurialIssueTracker");
            FileName2ChiliPageNameMapping.Add("httpRequest.wiki", "HTTP_Settings");
            FileName2ChiliPageNameMapping.Add("impersonation.wiki", "Impersonation");
            FileName2ChiliPageNameMapping.Add("importManifest.wiki", "Manifest_Importer");
            FileName2ChiliPageNameMapping.Add("inheritedProjectSecurity.wiki", "Inherited_Project_Security");
            FileName2ChiliPageNameMapping.Add("inMemoryCache.wiki", "In_Memory_Security_Cache");
            FileName2ChiliPageNameMapping.Add("internalSecurity.wiki", "Internal_Server_Security");
            FileName2ChiliPageNameMapping.Add("intervalTrigger.wiki", "Interval_Trigger");
            FileName2ChiliPageNameMapping.Add("iterationlabeller.wiki", "Iteration_Labeller");
            FileName2ChiliPageNameMapping.Add("lastBuildTimeCondition.wiki", "Last_Build_Time_Condition");
            FileName2ChiliPageNameMapping.Add("lastChangeLabeller.wiki", "Last_Change_Labeller");
            FileName2ChiliPageNameMapping.Add("lastStatusCondition.wiki", "Last_Build_Status_Condition");
            FileName2ChiliPageNameMapping.Add("ldapConverter.wiki", "LDAP_Email_Converter");
            FileName2ChiliPageNameMapping.Add("ldapUser.wiki", "LDAP_User_Authentication");
            FileName2ChiliPageNameMapping.Add("merge.wiki", "File_Merge_Task");
            FileName2ChiliPageNameMapping.Add("mks.wiki", "Mks");
            FileName2ChiliPageNameMapping.Add("modificationHistory.wiki", "ModificationHistory_Publisher");
            FileName2ChiliPageNameMapping.Add("modificationReader.wiki", "Modification_Reader_Task");
            FileName2ChiliPageNameMapping.Add("modificationWriter.wiki", "Modification_Writer_Task");
            FileName2ChiliPageNameMapping.Add("msbuild.wiki", "MsBuild_Task");
            FileName2ChiliPageNameMapping.Add("multi.wiki", "Multi_Source_Control");
            FileName2ChiliPageNameMapping.Add("multiIssueTracker.wiki", "MultiIssueTracker");
            FileName2ChiliPageNameMapping.Add("multiTrigger.wiki", "Multi_Trigger");
            FileName2ChiliPageNameMapping.Add("multiFilter.wiki", "MultiFilter");
            FileName2ChiliPageNameMapping.Add("namespaceMapping.wiki", "Namespace_Mapping");
            FileName2ChiliPageNameMapping.Add("nant.wiki", "NAnt_Task");
            FileName2ChiliPageNameMapping.Add("ncoverProfile.wiki", "NCover_Profiler_Task");
            FileName2ChiliPageNameMapping.Add("ncoverReport.wiki", "NCover_Reporting_Task");
            FileName2ChiliPageNameMapping.Add("ndepend.wiki", "NDepend_Task");
            FileName2ChiliPageNameMapping.Add("nullProjectSecurity.wiki", "Null_Project_Security");
            FileName2ChiliPageNameMapping.Add("nullSecurity.wiki", "Null_Server_Security");
            FileName2ChiliPageNameMapping.Add("nullSourceControl.wiki", "Null_Source_Control");
            FileName2ChiliPageNameMapping.Add("nullTask.wiki", "Null_Task");
            FileName2ChiliPageNameMapping.Add("nunit.wiki", "NUnit_Task");
            FileName2ChiliPageNameMapping.Add("orCondition.wiki", "Or_Condition");
            FileName2ChiliPageNameMapping.Add("p4.wiki", "Perforce_(P4)");
            FileName2ChiliPageNameMapping.Add("package.wiki", "Package_Publisher");
            FileName2ChiliPageNameMapping.Add("packageFile.wiki", "PackageFile");
            FileName2ChiliPageNameMapping.Add("packageFolder.wiki", "PackageFolder");
            FileName2ChiliPageNameMapping.Add("parallel.wiki", "Parallel_Task");
            FileName2ChiliPageNameMapping.Add("parameterTrigger.wiki", "Parameter_Trigger");
            FileName2ChiliPageNameMapping.Add("passwordUser.wiki", "User_Password_Authentication");
            FileName2ChiliPageNameMapping.Add("pathFilter.wiki", "PathFilter");
            // FileName2ChiliPageNameMapping.Add("permissions.wiki", "");
            FileName2ChiliPageNameMapping.Add("plasticscm.wiki", "Plastic");
            FileName2ChiliPageNameMapping.Add("powershell.wiki", "PowerShell_Task");
            FileName2ChiliPageNameMapping.Add("project.wiki", "Project_Configuration_Block");
            // FileName2ChiliPageNameMapping.Add("projectSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("projectTrigger.wiki", "Project_Trigger");
            FileName2ChiliPageNameMapping.Add("pvcs.wiki", "PVCS");
            FileName2ChiliPageNameMapping.Add("queue.wiki", "Queue_Configuration");
            FileName2ChiliPageNameMapping.Add("rake.wiki", "Rake_Task");
            FileName2ChiliPageNameMapping.Add("regexConverter.wiki", "Regular_Expression_Email_Converter");
            FileName2ChiliPageNameMapping.Add("regexIssueTracker.wiki", "RegexIssueTracker");
            FileName2ChiliPageNameMapping.Add("remoteProjectLabeller.wiki", "Remote_Project_Labeller");
            FileName2ChiliPageNameMapping.Add("replacementValue.wiki", "Replacement_Dynamic_Value");
            FileName2ChiliPageNameMapping.Add("robocopy.wiki", "Robocopy");
            FileName2ChiliPageNameMapping.Add("rolePermission.wiki", "Role_Permission");
            FileName2ChiliPageNameMapping.Add("rollUpTrigger.wiki", "RollUp_Trigger");
            FileName2ChiliPageNameMapping.Add("rss.wiki", "RSS_Publisher");
            FileName2ChiliPageNameMapping.Add("scheduleTrigger.wiki", "Schedule_Trigger");
            FileName2ChiliPageNameMapping.Add("sequential.wiki", "Sequential_Task");
            FileName2ChiliPageNameMapping.Add("simpleUser.wiki", "User_Name_Authentication");
            FileName2ChiliPageNameMapping.Add("starteam.wiki", "StarTeam");
            FileName2ChiliPageNameMapping.Add("state.wiki", "File_State_Manager");
            FileName2ChiliPageNameMapping.Add("stateFileLabeller.wiki", "State_File_Labeller");
            FileName2ChiliPageNameMapping.Add("statistic.wiki", "Statistic");
            FileName2ChiliPageNameMapping.Add("statistics.wiki", "Statistics_Publisher");
            FileName2ChiliPageNameMapping.Add("statusCondition.wiki", "Status_Condition");
            FileName2ChiliPageNameMapping.Add("subject.wiki", "Email_Subject");
            FileName2ChiliPageNameMapping.Add("surround.wiki", "Seapine_Surround");
            FileName2ChiliPageNameMapping.Add("svn.wiki", "Subversion_(svn)");
            FileName2ChiliPageNameMapping.Add("synchronised.wiki", "Synchronisation_Context_Task");
            FileName2ChiliPageNameMapping.Add("synergy.wiki", "Telelogic_Synergy");
            FileName2ChiliPageNameMapping.Add("synergyConnection.wiki", "Synergy_Client_Session");
            FileName2ChiliPageNameMapping.Add("synergyProject.wiki", "Synergy_Project");
            FileName2ChiliPageNameMapping.Add("updateConfig.wiki", "Update_Configuration_Task");
            FileName2ChiliPageNameMapping.Add("urlHeaderValueCondition.wiki", "URL_Header_Value_Condition");
            FileName2ChiliPageNameMapping.Add("urlPingCondition.wiki", "URL_Ping_Condition");
            FileName2ChiliPageNameMapping.Add("urlTrigger.wiki", "Url_Trigger");
            FileName2ChiliPageNameMapping.Add("user.wiki", "Email_User");
            FileName2ChiliPageNameMapping.Add("userFilter.wiki", "UserFilter");
            FileName2ChiliPageNameMapping.Add("userName.wiki", "User_Name_Authentication");
            FileName2ChiliPageNameMapping.Add("userPermission.wiki", "User_Permission");
            FileName2ChiliPageNameMapping.Add("variable.wiki", "Environment_Variable");
            FileName2ChiliPageNameMapping.Add("vault.wiki", "SourceGear_Vault");
            FileName2ChiliPageNameMapping.Add("viewcvs.wiki", "View_CVS_URL_Builder");
            FileName2ChiliPageNameMapping.Add("vss.wiki", "SourceSafe_(vss)");
            FileName2ChiliPageNameMapping.Add("vsts.wiki", "Team_Foundation_Server_(Tfs)");
            FileName2ChiliPageNameMapping.Add("websvn.wiki", "WebSVN_URL_Builder");
            FileName2ChiliPageNameMapping.Add("xmlFileAudit.wiki", "XML_File_Audit_Logger");
            FileName2ChiliPageNameMapping.Add("xmlFileAuditReader.wiki", "XML_File_Audit_Reader");
            //    FileName2ChiliPageNameMapping.Add("xmlFolderData.wiki", "");
            FileName2ChiliPageNameMapping.Add("xmllogger.wiki", "Xml_Log_Publisher");


            FileName2ChiliPageNameMapping.Add("booleanParameter.wiki", "BooleanParameter");
            FileName2ChiliPageNameMapping.Add("dateParameter.wiki", "dateParameter");
            FileName2ChiliPageNameMapping.Add("namedValue.wiki", "namedValue");
            FileName2ChiliPageNameMapping.Add("numericParameter.wiki", "numericParameter");
            FileName2ChiliPageNameMapping.Add("selectParameter.wiki", "selectParameter");
            FileName2ChiliPageNameMapping.Add("textParameter.wiki", "TextParameter");


            FileName2ChiliPageNameMapping.Add("administrationPlugin.wiki", "Dashboard_Administration_Plugin");
            FileName2ChiliPageNameMapping.Add("buildLogBuildPlugin.wiki", "Build_Log_Build_Plugin");
            FileName2ChiliPageNameMapping.Add("buildReportBuildPlugin.wiki", "Build_Report_Build_Plugin");
            FileName2ChiliPageNameMapping.Add("categorizedFarmReportFarmPlugin.wiki", "");
            FileName2ChiliPageNameMapping.Add("cctrayDownloadPlugin.wiki", "CCTray_Download_Plugin");
            FileName2ChiliPageNameMapping.Add("configurablePlugin.wiki", "");
            FileName2ChiliPageNameMapping.Add("cookieStore.wiki", "");
            FileName2ChiliPageNameMapping.Add("farmReportFarmPlugin.wiki", "Farm_Report_Farm_Plugin");
            FileName2ChiliPageNameMapping.Add("finalBuildStatusPlugin.wiki", "Final_Build_Status_Display_Plugin");
            FileName2ChiliPageNameMapping.Add("htmlReportPlugin.wiki", "");
            FileName2ChiliPageNameMapping.Add("latestBuildReportProjectPlugin.wiki", "latestBuildReportProjectPlugin.wiki");
            FileName2ChiliPageNameMapping.Add("multipleXslReportAction.wiki", "Multiple_XSL_Report_Build_Plugin");
            FileName2ChiliPageNameMapping.Add("namedAction.wiki", "");
            FileName2ChiliPageNameMapping.Add("ohlohProjectPlugin.wiki", "Ohloh_Stats_Display_Plugin");
            FileName2ChiliPageNameMapping.Add("packageListPlugin.wiki", "Package_List_Plugin");
            FileName2ChiliPageNameMapping.Add("plugins.wiki", "Plugins");
            FileName2ChiliPageNameMapping.Add("projectConfigurationServerPlugin.wiki", "Project_Configuration_Server_Plugin");
            FileName2ChiliPageNameMapping.Add("projectReportProjectPlugin.wiki", "Project_Report_Project_Plugin");
            FileName2ChiliPageNameMapping.Add("projectStatisticsPlugin.wiki", "projectStatisticsPlugin");
            FileName2ChiliPageNameMapping.Add("projectTimelinePlugin.wiki", "Project_Timeline_Plugin");
            FileName2ChiliPageNameMapping.Add("queueStatusServerPlugin.wiki", "View_ServerQueue_Server_Plugin");
            FileName2ChiliPageNameMapping.Add("remoteServices.wiki", "RemoteServices");
            FileName2ChiliPageNameMapping.Add("server.wiki", "Server");
            FileName2ChiliPageNameMapping.Add("serverAuditHistoryProjectPlugin.wiki", "Audit_History_Project_Plugin");
            FileName2ChiliPageNameMapping.Add("serverAuditHistoryServerPlugin.wiki", "Server_Audit_History_Server_Plugin");
            FileName2ChiliPageNameMapping.Add("serverInformationServerPlugin.wiki", "Server_Information_Server_Plugin");
            FileName2ChiliPageNameMapping.Add("serverLogProjectPlugin.wiki", "Server_Log_Project_Plugin");
            FileName2ChiliPageNameMapping.Add("serverLogServerPlugin.wiki", "Server_Log_Server_Plugin");
            FileName2ChiliPageNameMapping.Add("serverReportServerPlugin.wiki", "Server_Report_Server_Plugin");
            FileName2ChiliPageNameMapping.Add("serverSecurityConfigurationProjectPlugin.wiki", "Security_Configuration_Project_Plugin");
            FileName2ChiliPageNameMapping.Add("serverSecurityConfigurationServerPlugin.wiki", "");
            FileName2ChiliPageNameMapping.Add("serverUserListProjectPlugin.wiki", "User_List_Project_Plugin");
            FileName2ChiliPageNameMapping.Add("serverUserListServerPlugin.wiki", "User_List_Server_Plugin");
            FileName2ChiliPageNameMapping.Add("simpleSecurity.wiki", "");
            FileName2ChiliPageNameMapping.Add("stylesheet.wiki", "");
            FileName2ChiliPageNameMapping.Add("viewAllBuildsProjectPlugin.wiki", "View_All_Builds_Project_Plugin");
            FileName2ChiliPageNameMapping.Add("viewConfigurationProjectPlugin.wiki", "View_Configuration_Project_Plugin");
            FileName2ChiliPageNameMapping.Add("viewProjectStatusPlugin.wiki", "Project_Status_Plugin");
            FileName2ChiliPageNameMapping.Add("xslMultiReportBuildPlugin.wiki", "");
            FileName2ChiliPageNameMapping.Add("xslReportBuildAction.wiki", "");
            FileName2ChiliPageNameMapping.Add("xslReportBuildPlugin.wiki", "XSL_Report_Build_Plugin");
            FileName2ChiliPageNameMapping.Add("xsltParameter.wiki", "");

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
