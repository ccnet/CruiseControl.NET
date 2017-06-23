using System;
using System.Globalization;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Config;

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
    public class Vsts : ProcessSourceControl, IConfigurationValidation
    {
        #region Constants
        private const string VS2015_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\14.0";
        private const string VS2013_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\12.0";
        private const string VS2012_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\11.0";
        private const string VS2010_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\10.0";
        private const string VS2008_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\9.0";
        private const string VS2005_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\8.0";
        
        private const string VS2015_64_REGISTRY_PATH = @"Software\Wow6432Node\Microsoft\VisualStudio\14.0";
        private const string VS2013_64_REGISTRY_PATH = @"Software\Wow6432Node\Microsoft\VisualStudio\12.0";
        private const string VS2012_64_REGISTRY_PATH = @"Software\Wow6432Node\Microsoft\VisualStudio\11.0";
        private const string VS2010_64_REGISTRY_PATH = @"Software\Wow6432Node\Microsoft\VisualStudio\10.0";
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
        private string executable;

        private class TfsWorkspaceStatus
        {
            public bool WorkspaceIsMappedCorrectly { get; set; }
            public bool WorkspaceExists { get; set; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Vsts" /> class.	
        /// </summary>
        /// <param name="executor">The executor.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="registry">The registry.</param>
        /// <remarks></remarks>
        public Vsts(ProcessExecutor executor, IHistoryParser parser, IRegistry registry)
            : base(parser, executor)
        {
            this.registry = registry;
            this.executor = executor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vsts" /> class.	
        /// </summary>
        /// <remarks></remarks>
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
        public string Server { get; set; }

        /// <summary>
        /// The path to the executable
        /// </summary>
        /// <version>1.5</version>
        /// <default>From registry</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable
        {
            get { return executable ?? (executable = ReadTfFromRegistry()); }
            set { executable = value; }
        }

        /// <summary>
        /// The path to the project in source control, for example $\VSTSPlugins
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("project")]
        public string ProjectPath { get; set; }

        /// <summary>
        /// Whether this repository should be labeled.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("applyLabel", Required = false)]
        public bool ApplyLabel { get; set; }

        /// <summary>
        /// Whether to automatically get the source.
        /// </summary>
        /// <version>1.5</version>
        /// <default>falsea</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }

        /// <summary>
        /// Username that should be used.  Domain cannot be placed here, rather in domain property.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("username", Required = false)]
        public string Username { get; set; }

        /// <summary>
        /// The password in clear text of the domain user to be used.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("password", typeof(PrivateStringSerialiserFactory), Required = false)]
        public PrivateString Password { get; set; }

        /// <summary>
        ///  The domain of the user to be used.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("domain", Required = false)]
        public string Domain { get; set; }

        /// <summary>
        /// The working directory to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Whether to do a clean copy.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("cleanCopy", Required = false)]
        public bool CleanCopy { get; set; }

        /// <summary>
        /// Whether to force or not.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("force", Required = false)]
        public bool Force { get; set; }

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

                //if (string.IsNullOrEmpty(this.Username))
                //{
                //    return workspaceName + ";" + BuildTfsUsername();
                //}

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
        public bool DeleteWorkspace { get; set; }


        /// <summary>
        /// Encoding Code page to use for communicating with TFS
        /// </summary>
        /// <version>1.6</version>
        /// <default>Empty String, will default to UTF-8 encoding</default>
        [ReflectorProperty("codepage", Required = false)]
        public string CodePage { get; set; }


        #endregion NetReflectored Properties

        #region ISourceControl Implementation

        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            if (!ProjectExists(from))
            {
                Log.Error(string.Format(System.Globalization.CultureInfo.CurrentCulture, "[TFS] Project {0} is not valid on {1} TFS server", ProjectPath, Server));
                throw new CruiseControlException("Project Name is not valid on this TFS server");
            }

            Log.Debug("[TFS] Checking Team Foundation Server for Modifications");
            Log.Debug("[TFS] From: " + from.StartTime + " - To: " + to.StartTime);

            ProcessResult result = executor.Execute(NewHistoryProcessInfo(from, to));

            LookForErrorReturns(result);

            return ParseModifications(result, from.StartTime, to.StartTime);
        }

        /// <summary>
        /// Labels the source control.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public override void LabelSourceControl(IIntegrationResult result)
        {
            if (ApplyLabel && result.Succeeded)
            {
                LookForErrorReturns(executor.Execute(NewLabelProcessInfo(result)));
            }
        }

        /// <summary>
        /// Gets the source.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public override void GetSource(IIntegrationResult result)
        {
            if (!AutoGetSource || !ProjectExists(result)) return;

            this.WorkingDirectory = result.BaseFromWorkingDirectory(this.WorkingDirectory);

            if (CleanCopy)
            {
                // If we have said we want a clean copy, then delete old copy before getting.
                Log.Debug("[TFS] Deleting " + this.WorkingDirectory);
                this.DeleteDirectory(this.WorkingDirectory);
            }

            TfsWorkspaceStatus workspaceStatus = GetWorkspaceStatus(result);

            if (workspaceStatus.WorkspaceExists)
            {
                if (DeleteWorkspace)
                {
                    // We have asked for a new workspace every time, therefore delete the existing one.
                    Log.Debug("[TFS] Removing existing workspace " + Workspace);
                    LookForErrorReturns(executor.Execute(DeleteWorkSpaceProcessInfo(result)));

                    //Create Workspace
                    Log.Debug("[TFS] Creating New Workspace " + Workspace);
                    LookForErrorReturns(executor.Execute(CreateWorkSpaceProcessInfo(result)));

                    //Map Workspace
                    Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture, "[TFS] Mapping Workspace {0} to {1}", Workspace, WorkingDirectory));
                    LookForErrorReturns(executor.Execute(MapWorkSpaceProcessInfo(result)));
                }
            }
            else
            {
                //Create Workspace
                Log.Debug("[TFS] Creating New Workspace " + Workspace);
                LookForErrorReturns(executor.Execute(CreateWorkSpaceProcessInfo(result)));

                //Map Workspace
                Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture, "[TFS] Mapping Workspace {0} to {1}", Workspace, WorkingDirectory));
                LookForErrorReturns(executor.Execute(MapWorkSpaceProcessInfo(result)));
            }

            if (!workspaceStatus.WorkspaceIsMappedCorrectly)
            {
                //Map Workspace
                Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture, "[TFS] Mapping Workspace {0} to {1}", Workspace, WorkingDirectory));
                LookForErrorReturns(executor.Execute(MapWorkSpaceProcessInfo(result)));
            }

            Log.Debug("[TFS] Getting Files in " + Workspace);
            ProcessInfo pi = GetWorkSpaceProcessInfo(result);
            pi.TimeOut = 3600000;
            LookForErrorReturns(executor.Execute(pi));
        }

        #endregion ISourceControl Implementation

        #region Private Members

        private bool ProjectExists(IIntegrationResult result)
        {
            //Check for  Workspace
            Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture, "[TFS] Checking if Project {0} exists", ProjectPath));
            ProcessResult pr = executor.Execute(CheckProjectProcessInfo(result));
            LookForErrorReturns(pr);

            return (!pr.StandardOutput.Contains("No items match"));
        }

        private TfsWorkspaceStatus GetWorkspaceStatus(IIntegrationResult result)
        {
            Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture, "[TFS] Fetching Workspace {0} details", Workspace));
            ProcessResult pr = executor.Execute(CheckWorkSpaceProcessInfo(result));

            LookForErrorReturns(pr);

            TfsWorkspaceStatus status = new TfsWorkspaceStatus();
            status.WorkspaceIsMappedCorrectly = pr.StandardOutput.Contains(ProjectPath + ": " + WorkingDirectory);
            status.WorkspaceExists = !(pr.StandardOutput.Contains("No workspace matching"));

            return status;

        }

        /// <summary>
        ///   Makes sure we fail a build when TFS throws errors back
        /// </summary>
        private static void LookForErrorReturns(ProcessResult pr)
        {
            if (pr.HasErrorOutput && pr.Failed)
            {
                Log.Error(pr.StandardError);
                throw new CruiseControlException(pr.StandardError);
            }
        }

        /// <summary>
        ///   Delete a directory, even if it contains readonly files.
        /// </summary>
        private void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path)) return;

            this.MarkAllFilesReadWrite(path);
            Directory.Delete(path, true);
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

        #region tf.exe process info creators

        // tf dir [/server:servername] itemspec [/version:versionspec] 
        // [/recursive] [/folders] [/deleted] 
        private ProcessInfo CheckProjectProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments("dir", "/folders");
            buffer.Add("/server:", Server);
            buffer.AddQuote(ProjectPath);

            AppendSourceControlAuthentication(buffer);

            return NewProcessInfo(buffer, result);
        }


        // tf workfold [/map] [/s:servername] [/workspace: workspacename]
        //  repositoryfolder|localfolder
        private ProcessInfo MapWorkSpaceProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments("workfold", "/map");
            buffer.AddQuote(ProjectPath);
            buffer.AddQuote(WorkingDirectory);
            buffer.Add("/server:", Server);
            buffer.Add("/workspace:", this.Workspace, true);

            AppendSourceControlAuthentication(buffer);

            return NewProcessInfo(buffer, result);
        }

        // tf get itemspec [/version:versionspec] [/all] [/overwrite] [/force] 
        // [/preview] [/recursive] [/noprompt]
        private ProcessInfo GetWorkSpaceProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments(
                "get",
                "/recursive",
                "/noprompt");

            if (Force)
            {
                buffer.Add("/force");
            }

            buffer.AddQuote(WorkingDirectory);

            AppendSourceControlAuthentication(buffer);

            return NewProcessInfo(buffer, result);
        }

        // tf workspace /new [/noprompt] [/template:workspacename[;workspaceowner]]
        // [/computer:computername] [/comment:("comment"|@comment file)]
        // [/s:servername] [workspacename[;workspaceowner]]
        private ProcessInfo CreateWorkSpaceProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments("workspace", "/new");
            buffer.Add("/computer:", Environment.MachineName);
            buffer.AddQuote("/comment:", DEFAULT_WORKSPACE_COMMENT);
            buffer.Add("/server:", Server);
            buffer.AddQuote(Workspace);

            AppendSourceControlAuthentication(buffer);

            return NewProcessInfo(buffer, result);
        }

        // tf workspaces /delete [/owner:ownername] [/computer:computername] 
        // [/server:servername] workspacename
        private ProcessInfo DeleteWorkSpaceProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments("workspace", "/delete");
            buffer.Add("-server:", Server);
            buffer.AddQuote(Workspace);

            AppendSourceControlAuthentication(buffer);

            return NewProcessInfo(buffer, result);
        }

        // tf workspaces [/computer:computername][/server:servername] workspacename
        private ProcessInfo CheckWorkSpaceProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments("workspaces");
            buffer.Add("/computer:", Environment.MachineName);
            buffer.Add("-server:", Server);
            buffer.Add("/format:detailed");
            buffer.AddQuote(Workspace);

            AppendSourceControlAuthentication(buffer);

            return NewProcessInfo(buffer, result);
        }

        //	LABEL_COMMAND_FORMAT = "label [/server:servername] labelname[@scope] [/owner:ownername] 
        //  itemspec [/version:versionspec] [/comment:("comment"|@commentfile)] 
        //  [/child:(replace|merge)] [/recursive]"
        private ProcessInfo NewLabelProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments("label");
            buffer.Add("/server:", Server);
            buffer.AddQuote(result.Label, string.Format(System.Globalization.CultureInfo.CurrentCulture, "@{0}", ProjectPath));
            buffer.AddQuote(WorkingDirectory);
            buffer.Add("/recursive");
            buffer.Add("/comment:", "CCNet Build Label", true);

            AppendSourceControlAuthentication(buffer);

            return NewProcessInfo(buffer, result);
        }

        //	HISTORY_COMMAND_FORMAT = "tf history -noprompt -server:http://tfsserver:8080 $/TeamProjectName/path
        //  -version:D2006-12-01T01:01:01Z~D2006-12-13T20:00:00Z -recursive
        // -format:detailed /login:DOMAIN\name,password"
        private ProcessInfo NewHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to)
        {
            var buffer = new PrivateArguments("history", "-noprompt");
            buffer.Add("-server:", Server);
            buffer.AddQuote(ProjectPath);
            buffer.Add(string.Format(System.Globalization.CultureInfo.CurrentCulture, "-version:D{0}~D{1}", FormatCommandDate(from.StartTime), FormatCommandDate(to.StartTime)));
            buffer.Add("-recursive");
            buffer.Add("-format:detailed");

            AppendSourceControlAuthentication(buffer);

            return NewProcessInfo(buffer, to);
        }

        #endregion

        private void AppendSourceControlAuthentication(PrivateArguments buffer)
        {
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password.PrivateValue))
                buffer.Add("/login:" + this.BuildTfsAuthenticationString());
        }

        private string BuildTfsAuthenticationString()
        {
            return BuildTfsUsername() + "," + this.Password.ToString(SecureDataMode.Private);
        }

        private string BuildTfsUsername()
        {
            string username = this.Username;

            if (!string.IsNullOrEmpty(this.Domain))
            {
                return this.Domain + @"\" + username;
            }

            return username;
        }

        private static string FormatCommandDate(DateTime date)
        {
            return date.ToUniversalTime().ToString(UtcXmlDateFormat, CultureInfo.InvariantCulture);
        }

        private ProcessInfo NewProcessInfo(PrivateArguments args, IIntegrationResult result)
        {
            string workingDirectory = Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory));
            if (!Directory.Exists(workingDirectory)) Directory.CreateDirectory(workingDirectory);

            var processInfo = new ProcessInfo(Executable, args, workingDirectory);
            processInfo.StreamEncoding = Encoding.UTF8;

            if (!string.IsNullOrEmpty(CodePage))
            {
                int codePage;
                if (int.TryParse(CodePage, out codePage))
                {
                    processInfo.StreamEncoding = Encoding.GetEncoding(codePage);
                }
                else
                {
                    throw new CruiseControlException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Code page {0} could not be parsed to an encoding via instruction : Encoding.GetEncoding(codePage)", CodePage));
                }
            }

            processInfo.TimeOut = 600000;
            return processInfo;
        }


        private string ReadTfFromRegistry()
        {
            string registryValue = registry.GetLocalMachineSubKeyValue(VS2015_64_REGISTRY_PATH, VS_REGISTRY_KEY);

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2013_64_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2010_64_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2008_64_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2005_64_REGISTRY_PATH, VS_REGISTRY_KEY);
            }
            
            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2015_32_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2013_32_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2012_32_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2010_32_REGISTRY_PATH, VS_REGISTRY_KEY);
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
                Log.Debug("[TFS] Unable to find TF.exe and it was not defined in Executable Parameter");
                throw new CruiseControlException("Unable to find TF.exe and it was not defined in Executable Parameter");
            }

            return Path.Combine(registryValue, TF_EXE);
        }


        #endregion Private Members


        public void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            string ErrorInfo = "Invalid chars in TFS workspace name. Look at http://msdn.microsoft.com/en-us/library/aa980550.aspx#SourceControl for naming restrictions.";

            bool AllOk = true;
            System.Collections.Generic.List<string> badchars = new System.Collections.Generic.List<string>();

            badchars.Add("/");
            badchars.Add(":");
            badchars.Add("<");
            badchars.Add(">");
            badchars.Add("|");
            badchars.Add(@"\");
            badchars.Add("*");
            badchars.Add("?");
            badchars.Add(";");


            if (workspaceName.Length > 64)
            {
                AllOk = false;
                ErrorInfo += System.Environment.NewLine + "Max length is 64 chars";
            }
            if (workspaceName.EndsWith(" "))
            {
                AllOk = false;
                ErrorInfo += System.Environment.NewLine + "can not end with space";
            }


            foreach (string s in badchars)
            {
                if (workspaceName.Contains(s))
                {
                    AllOk = false;
                    ErrorInfo += System.Environment.NewLine + "can not contain character " + s;
                }
            }

            if (!AllOk) errorProcesser.ProcessError(ErrorInfo);
        }
    }
}
