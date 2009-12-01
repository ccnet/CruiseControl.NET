using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.State
{
    /// <summary>
    /// The File State Manager is a State Manager that saves the state for one project to a file. The 
    /// filename should be stored in either the working directory for the project or in the explicitly
    /// specified directory. The filename will match the project name, but will have the extension .state.
    /// </summary>
    /// <title>File State Manager</title>
    /// <version>1.0</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;state type="state" /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;state type="state" directory="C:\CCNetState" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("state")]
    public class FileStateManager : IStateManager
	{
		private readonly IFileSystem fileSystem;
		private readonly IExecutionEnvironment executionEnvironment;
		private string stateFileDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStateManager"/> class.
        /// </summary>
		public FileStateManager() : this(new SystemIoFileSystem(), new ExecutionEnvironment())
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStateManager"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="executionEnvironment">The execution environment.</param>
		public FileStateManager(IFileSystem fileSystem, IExecutionEnvironment executionEnvironment)
		{
			this.fileSystem = fileSystem;
			this.executionEnvironment = executionEnvironment;

			stateFileDirectory = this.executionEnvironment.GetDefaultProgramDataFolder(ApplicationType.Server);
			fileSystem.EnsureFolderExists(stateFileDirectory);
		}

        /// <summary>
        /// The directory to save the state file to.
        /// </summary>
        /// <version>1.0</version>
        /// <default>The directory CCNet was launched from.</default>
        [ReflectorProperty("directory", Required=false)]
        public string StateFileDirectory
		{
			get { return stateFileDirectory; }
			set
			{
                if (!string.IsNullOrEmpty(value))
					fileSystem.EnsureFolderExists(value);

				stateFileDirectory = value;
			}
		}

        /// <summary>
        /// Determines whether the project has previous state.
        /// </summary>
        /// <param name="project">The name of project.</param>
        /// <returns>
        /// <c>true</c> if the project has previous state; otherwise, <c>false</c>.
        /// </returns>
		public bool HasPreviousState(string project)
		{
			return fileSystem.FileExists(GetFilePath(project));
		}

        /// <summary>
        /// Loads the state of the project.
        /// </summary>
        /// <param name="project">The name of the project.</param>
        /// <returns>An <see cref="IIntegrationResult"/> containing the current state.</returns>
		public IIntegrationResult LoadState(string project)
		{
			var stateFile = LoadStateIntoDocument(project);
            if (stateFile.DocumentElement.Name == "IntegrationResult")
            {
                using (var reader = new StringReader(stateFile.OuterXml))
                {
                    return LoadState(reader);
                }
            }
            else
            {
                throw new CruiseControlException("State file contains invalid data");
            }
		}

        /// <summary>
        /// Loads the state from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="stateFileReader">The state file reader.</param>
        /// <returns>An <see cref="IIntegrationResult"/> containing the current state.</returns>
        public IIntegrationResult LoadState(TextReader stateFileReader)
		{
			XmlSerializer serializer = new XmlSerializer(typeof (IntegrationResult));
			return (IntegrationResult) serializer.Deserialize(stateFileReader);
		}

        /// <summary>
        /// Loads the state into an <see cref="XmlDocument"/>.
        /// </summary>
        /// <param name="project">The name of the project.</param>
        /// <returns>An <see cref="XmlDocument"/> containing the state.</returns>
		private XmlDocument LoadStateIntoDocument(string project)
		{
            var document = new XmlDocument();
			string stateFilePath = GetFilePath(project);
			try
			{
                document.Load(
                    fileSystem.Load(stateFilePath));
				return document;
			}
			catch (Exception e)
			{
				throw new CruiseControlException(string.Format("Unable to read the specified state file: {0}.  The path may be invalid.", stateFilePath), e);				
			}
		}

        /// <summary>
        /// Write the state to disk, ensuring that it gets there in its entirety.
        /// </summary>
        /// <param name="result">The integration to save the state for.</param>
		public void SaveState(IIntegrationResult result)
		{
			XmlSerializer serializer = new XmlSerializer(typeof (IntegrationResult));
			StringWriter buffer = new Utf8StringWriter();
			serializer.Serialize(buffer, result);

			string path = GetFilePath(result.ProjectName);
			try
			{
				fileSystem.AtomicSave(path, buffer.ToString());
			}
			catch (SystemException e)
			{
				throw new CruiseControlException(
					string.Format("Unable to save the IntegrationResult to the specified directory: {0}{1}{2}",
                        path, Environment.NewLine, buffer),
                    e);
			}
		}

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <param name="project">The name of project.</param>
        /// <returns>The file path to a state file.</returns>
		private string GetFilePath(string project)
		{
			return Path.Combine(StateFileDirectory, StateFilename(project));
		}

        /// <summary>
        /// Generates the state filename for a project.
        /// </summary>
        /// <param name="project">The name of project.</param>
        /// <returns>The name of the state file for the project.</returns>
		private string StateFilename(string project)
		{
			StringBuilder strBuilder = new StringBuilder();
			foreach (string token in project.Split(' '))
			{
				strBuilder.Append(token.Substring(0, 1).ToUpper());
				if (token.Length > 1)
				{
					strBuilder.Append(token.Substring(1));
				}
			}
			return strBuilder.Append(".state").ToString();
		}
    }
}