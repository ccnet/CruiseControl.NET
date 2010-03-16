using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    ///   Source Control Plugin for CruiseControl.NET that talks to VSTS Team Foundation Server.
    /// </summary>
    /// <title>VSTS Team Foundation Server Source Control Block</title>
    /// <version>1.5</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>vsts</value>
    /// </key>
    /// <example>
    /// <code>
    /// &lt;sourcecontrol type="vsts"&gt;
    /// &lt;server&gt;http://vstsb2:8080&lt;/server&gt;
    /// &lt;project&gt;$\VSTSPlugins&lt;/project&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// There is an alternate plug-in which uses the Team Foundation assemblies directly (<link>Visual Studio Team 
    /// Foundation Server Plugin</link>).
    /// </remarks>
    [ReflectorType("vsts")]
    public class Vsts : ProcessSourceControl
    {
        #region Constants

        private const string VS2008_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\9.0";
        private const string VS2005_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\8.0";
        private const string VS2008_64_REGISTRY_PATH = @"Software\Wow6432Node\Microsoft\VisualStudio\9.0";
        private const string VS2005_64_REGISTRY_PATH = @"Software\Wow6432Node\Microsoft\VisualStudio\8.0";
        private const string VS_REGISTRY_KEY = @"InstallDir";        
        private const string DEFAULT_WORKSPACE_NAME = "CCNET";
        private const string TF_EXE = "TF.exe";
        private const string DEFAULT_WORKSPACE_COMMENT = "Temporary CruiseControl.NET Workspace";
        private const string UtcXmlDateFormat = "yyyy-MM-ddTHH:mm:ssZ";

        #endregion Constants

        #region PrivateVariables	

        private readonly IRegistry registry;
        private VstsHistoryParser parser;
        private string executable;
       
        #endregion

        public Vsts(ProcessExecutor executor, IHistoryParser parser, IRegistry registry)
            : base(parser, executor)
		{
			this.registry = registry;
            this.executor = executor;
            this.parser = parser as VstsHistoryParser;
		}
        
        public Vsts() :
            this(new ProcessExecutor(), new VstsHistoryParser(), new Registry()) { }

        #region NetReflectored Properties

        /// <summary>
        /// The name or URL of the team foundation server.  For example http://vstsb2:8080 or vstsb2 if it has already 
        /// been registered on the machine.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("server")]
        public string Server;

        /// <summary>
        /// The path to the executable
        /// </summary>
        /// <version>1.5</version>
        /// <default>From registry</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable
        {
            get
            {
                if (executable == null)
                    executable = ReadTFFromRegistry();

                return executable;
            }
            set { executable = value; }
        }

        /// <summary>
        /// The path to the project in source control, for example $\VSTSPlugins
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("project")]
        public string ProjectPath;

        /// <summary>
        /// Whether this repository should be labeled.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("applyLabel", Required = false)]
        public bool ApplyLabel = false;

        /// <summary>
        /// Whether to automatically get the source.
        /// </summary>
        /// <version>1.5</version>
        /// <default>falsea</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource = false;

        /// <summary>
        /// Username that should be used.  Domain cannot be placed here, rather in domain property.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("username", Required = false)]
        public string Username = String.Empty;

        /// <summary>
        /// The password in clear text of the domain user to be used.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("password", typeof(PrivateStringSerialiserFactory), Required = false)]
        public PrivateString Password = String.Empty;

        /// <summary>
        ///  The domain of the user to be used.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("domain", Required = false)]
        public string Domain = String.Empty;

        /// <summary>
        /// The working directory to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory = String.Empty;

        /// <summary>
        /// Whether to do a clean copy.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("cleanCopy", Required = false)]
        public bool CleanCopy = false;

        /// <summary>
        /// Whether to force or not.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("force", Required = false)]
        public bool Force = false;

        private string workspaceName;
        /// <summary>
        /// Name of the workspace to create.  This will revert to the DEFAULT_WORKSPACE_NAME if not passed.
        /// </summary>
        /// <version>1.5</version>
        /// <default>CCNET</default>
        [ReflectorProperty("workspace", Required = false)]
        public string Workspace
        {
            get
            {
                if (workspaceName == null)
                {
                    workspaceName = DEFAULT_WORKSPACE_NAME;
                }
                return workspaceName;
            }
            set
            {
                workspaceName = value;
            }
        }

        /// <summary>
        /// Flag indicating if workspace should be deleted every time or if it should be left (the default).  Leaving
        /// the workspace will mean that subsequent gets will only need to transfer the modified files, improving
        /// performance considerably.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("deleteWorkspace", Required = false)]       
        public bool DeleteWorkspace = false;

        #endregion NetReflectored Properties

        #region ISourceControl Implementation

        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            ProcessResult result = null;
            try
            {
                if ( projectExists( from ) )
                {
                    Log.Debug("Checking Team Foundation Server for Modifications");
                    Log.Debug("From: " + from.StartTime + " - To: " + to.StartTime);

                    List<Modification> modifications = new List<Modification>();

                    result = executor.Execute(NewHistoryProcessInfo(from, to));

                    lookForErrorReturns(result);
                }
                else
                {
                    Log.Error(String.Format("Project {0} is not valid on {1} TFS server", ProjectPath, Server));
                    throw new Exception("Project Name is not valid on this TFS server");
                }
               
            }
            catch (Exception)
            {                
                throw;
            }            
            
            return ParseModifications(result, from.StartTime, to.StartTime);           
        }
       
        public override void LabelSourceControl(IIntegrationResult result)
        {
            try
            {
                if (ApplyLabel && result.Succeeded)
                {
                    lookForErrorReturns(executor.Execute(NewLabelProcessInfo(result)));
                }
            }
            catch (Exception)
            {
                
                throw;
            }            
        }        

        public override void GetSource(IIntegrationResult result)
        {
            if (AutoGetSource && projectExists(result))
            {
                try
                {                
                    this.WorkingDirectory = result.BaseFromWorkingDirectory(this.WorkingDirectory);

                    if (CleanCopy)
                    {
                        // If we have said we want a clean copy, then delete old copy before getting.
                        Log.Debug("Deleting " + this.WorkingDirectory);
                        this.deleteDirectory(this.WorkingDirectory);
                    }

                    if (workspaceExists(result))
                    {
                        if (DeleteWorkspace)
                        {
                            // We have asked for a new workspace every time, therefore delete the existing one.
                            Log.Debug("Removing existing workspace " + Workspace);
                            lookForErrorReturns(executor.Execute(DeleteWorkSpaceProcessInfo(result)));
                            
                            //Create Workspace
                            Log.Debug("Creating New Workspace " + Workspace);
                            lookForErrorReturns(executor.Execute(CreateWorkSpaceProcessInfo(result)));

                            //Map Workspace
                            Log.Debug(string.Format("Mapping Workspace {0} to {1}", Workspace, WorkingDirectory));
                            lookForErrorReturns(executor.Execute(MapWorkSpaceProcessInfo(result)));
                        }
                    }
                    else
                    {
                        //Create Workspace
                        Log.Debug("Creating New Workspace " + Workspace);
                        lookForErrorReturns(executor.Execute(CreateWorkSpaceProcessInfo(result)));

                        //Map Workspace
                        Log.Debug(string.Format("Mapping Workspace {0} to {1}", Workspace, WorkingDirectory));
                        lookForErrorReturns(executor.Execute(MapWorkSpaceProcessInfo(result)));
                    }

                    if (!workspaceIsMappedCorrectly(result))
                    {
                        //Map Workspace
                        Log.Debug(string.Format("Mapping Workspace {0} to {1}", Workspace, WorkingDirectory));
                        lookForErrorReturns(executor.Execute(MapWorkSpaceProcessInfo(result)));
                    }

                    Log.Debug("Getting Files in " + Workspace);
                    ProcessInfo pi = GetWorkSpaceProcessInfo(result);
                    pi.TimeOut = 3600000;
                    lookForErrorReturns(executor.Execute(pi));
                    
                }
                catch (Exception)
                {
                    throw;
                }               
            }
        }       
        
        #endregion ISourceControl Implementation

        #region Private Members  

        private bool projectExists(IIntegrationResult result)
        {
            try
            {
                //Check for  Workspace
                Log.Debug(String.Format("Checking if Project {0} exists", ProjectPath));
                ProcessResult pr = executor.Execute(CheckProjectProcessInfo(result));
                lookForErrorReturns(pr);

                string failedMessage = "No items match";

                return (!pr.StandardOutput.Contains(failedMessage));
            }
            catch (Exception)
            {
                throw;
            }
        }       

        private bool workspaceIsMappedCorrectly(IIntegrationResult result)
        {
            try
            {
                //Check for  Workspace
                Log.Debug(String.Format("Checking if Workspace {0} exists", Workspace));
                ProcessResult pr = executor.Execute(CheckWorkSpaceProcessInfo(result));
                lookForErrorReturns(pr);

                string expected = ProjectPath + ": " + WorkingDirectory;

                return (pr.StandardOutput.Contains(expected));
            }
            catch (Exception)
            {
                throw;
            }
        }
     
        /// <summary>
        ///   Makes sure we fail a build when TFS throws errors back
        /// </summary>
        private void lookForErrorReturns(ProcessResult pr)
        {
            if (pr.HasErrorOutput && pr.Failed)
            {
                Log.Error(pr.StandardError);
                throw new Exception(pr.StandardError);
            }
        }    

        /// <summary>
        ///   Delete a directory, even if it contains readonly files.
        /// </summary>
        private void deleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                this.MarkAllFilesReadWrite(path);
                Directory.Delete(path, true);
            }
        }

        private void MarkAllFilesReadWrite(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                file.IsReadOnly = false;
            }

            // Now recurse down the directories
            DirectoryInfo[] dirs = dirInfo.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                this.MarkAllFilesReadWrite(dir.FullName);
            }
        }

        private bool workspaceExists(IIntegrationResult result)
        {
            //Check for  Workspace
            Log.Debug(String.Format("Checking if Workspace {0} exists", Workspace));
            ProcessResult pr = executor.Execute(CheckWorkSpaceProcessInfo(result));

            lookForErrorReturns(pr);
            
            return !(pr.StandardOutput.Contains("No workspace matching"));
        }

        #region tf.exe process info creators

        // tf dir [/server:servername] itemspec [/version:versionspec] 
        // [/recursive] [/folders] [/deleted] 
        private ProcessInfo CheckProjectProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments(
                "dir",
                "/folders");
            buffer.Add("/server:", Server);
            buffer.AddQuote(ProjectPath);

            return NewProcessInfo(buffer, result);
        }


        // tf workfold [/map] [/s:servername] [/workspace: workspacename]
        //  repositoryfolder|localfolder
        private ProcessInfo MapWorkSpaceProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments(
                "workfold",
                "/map");
            buffer.AddQuote(ProjectPath);
            buffer.AddQuote(WorkingDirectory);
            buffer.Add("/server:", Server);
            buffer.Add("/workspace:", Workspace);
            return NewProcessInfo(buffer, result);
        }

        // tf get itemspec [/version:versionspec] [/all] [/overwrite] [/force] 
        // [/preview] [/recursive] [/noprompt]
        private ProcessInfo GetWorkSpaceProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments(
                "get",
                "/force",
                "/recursive",
                "/noprompt");
            buffer.AddQuote(WorkingDirectory);
            return NewProcessInfo(buffer, result);
        }

        // tf workspace /new [/noprompt] [/template:workspacename[;workspaceowner]]
        // [/computer:computername] [/comment:(“comment”|@comment file)]
        // [/s:servername] [workspacename[;workspaceowner]]
        private ProcessInfo CreateWorkSpaceProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments(
                "workspace",
                "/new");
            buffer.Add("/computer:", Environment.MachineName);
            buffer.AddQuote("/comment:", DEFAULT_WORKSPACE_COMMENT);
            buffer.Add("/server:", Server);
            buffer.AddQuote(Workspace);
            return NewProcessInfo(buffer, result);
        }

        // tf workspaces /delete [/owner:ownername] [/computer:computername] 
        // [/server:servername] workspacename
        private ProcessInfo DeleteWorkSpaceProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments(
                "workspace",
                "/delete");
            // buffer.Add("/computer:", Environment.MachineName);
            buffer.Add("-server:", Server);
            buffer.AddQuote(Workspace);
            return NewProcessInfo(buffer, result);
        }

        // tf workspaces [/computer:computername][/server:servername] workspacename
        private ProcessInfo CheckWorkSpaceProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments(
                "workspaces");
            buffer.Add("/computer:", Environment.MachineName);
            buffer.Add("-server:", Server);
            buffer.Add("/format:detailed");
            buffer.AddQuote(Workspace);
            return NewProcessInfo(buffer, result);
        }

        //	LABEL_COMMAND_FORMAT = "label [/server:servername] labelname[@scope] [/owner:ownername] 
        //  itemspec [/version:versionspec] [/comment:("comment"|@commentfile)] 
        //  [/child:(replace|merge)] [/recursive]"
        private ProcessInfo NewLabelProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments(
                "label");
            buffer.Add("/server:", Server);
            buffer.Add(result.Label);
            buffer.AddQuote(WorkingDirectory);
            buffer.Add("/recursive");           
            return NewProcessInfo(buffer, result);
        }

        //	HISTORY_COMMAND_FORMAT = "tf history -noprompt -server:http://tfsserver:8080 $/TeamProjectName/path
        //  -version:D2006-12-01T01:01:01Z~D2006-12-13T20:00:00Z -recursive
        // -format:detailed -login:DOMAIN\name,password"
        private ProcessInfo NewHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to)
        {
            var buffer = new PrivateArguments(
                "history",
                "-noprompt");
            buffer.Add("-server:", Server);
            buffer.AddQuote(ProjectPath);
            buffer.Add(String.Format("-version:D{0}~D{1}", FormatCommandDate(from.StartTime), FormatCommandDate(to.StartTime)));
            buffer.Add("-recursive");
            buffer.Add("-format:detailed");

            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password.PrivateValue))
            {
                if (!string.IsNullOrEmpty(Domain))
                {
                    Username = Domain + @"\" + Username;
                }

                buffer.Add("-login:" + this.Username, this.Password);
            }

            return NewProcessInfo(buffer, to);
        }

        #endregion

        private string FormatCommandDate(DateTime date)
        {
            return date.ToUniversalTime().ToString(UtcXmlDateFormat, CultureInfo.InvariantCulture);
        }

        private ProcessInfo NewProcessInfo(PrivateArguments args, IIntegrationResult result)
        {
            string workingDirectory = Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory));
            if (!Directory.Exists(workingDirectory)) Directory.CreateDirectory(workingDirectory);

            var processInfo = new ProcessInfo(Executable, args, workingDirectory);
            processInfo.StreamEncoding = Encoding.UTF8;
            return processInfo;
        }
       
        private string ReadTFFromRegistry()
        {
            string registryValue = null;

            registryValue = registry.GetLocalMachineSubKeyValue(VS2008_64_REGISTRY_PATH, VS_REGISTRY_KEY);

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2005_64_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2008_32_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2005_32_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                Log.Debug("Unable to find TF.exe and it was not defined in Executable Parameter");
                throw new Exception("Unable to find TF.exe and it was not defined in Executable Parameter");
            }

            return Path.Combine(registryValue, TF_EXE);
        }

      
        #endregion Private Members
    
    }
}
