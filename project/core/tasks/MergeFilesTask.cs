namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System.IO;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// <para>
    /// Merges external files into the build log.
    /// </para>
    /// <para>
    /// Most build processes interact with external tools that write their output to file (e.g. NUnit, FxCop, or NCover). To make the
    /// output of these tools available to CruiseControl.NET to be used in the build process or displayed in the CruiseControl.NET web page or
    /// included in CruiseControl.NET emails, these files need to be merged into the CruiseControl.NET integration.
    /// </para>
    /// <para type="tip">
    /// You should place your File Merge Tasks in the &lt;publishers /&gt; section of your <link>Project Configuration Block</link> before 
    /// your <link>Xml Log Publisher</link>.
    /// </para>
    /// </summary>
    /// <title>File Merge Task</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;merge&gt;
    /// &lt;files&gt;
    /// &lt;file&gt;&lt;!-- path to NUnit test file --&gt;&lt;/file&gt;
    /// &lt;file&gt;&lt;!-- path to FxCop file --&gt;&lt;/file&gt;
    /// &lt;/files&gt;
    /// &lt;/merge&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>Merge actions</heading>
    /// <para>
    /// Prior to CruiseControl.NET 1.5.0, all files specified in this task are merged into the build log (this is the default behaviour if no
    /// action is specified.) Since the build log is XML data, all data that is merged needed to be treated as XML data. If the data was XML,
    /// it would be merged without any problems, otherwise the data would be embedded inside a CDATA block.
    /// </para>
    /// <para>
    /// With CruiseControl.NET 1.5.0 it is now possible to control how the merge will work. There are three available actions:
    /// </para>
    /// <list type="1">
    /// <item>
    /// Merge: the default behaviour - merge to the build log as XML if possible, as CDATA if not possible
    /// </item>
    /// <item>
    /// CData: always merge to the build log in a CDATA block
    /// </item>
    /// <item>
    /// Copy: instead of merging the data into the build log, it will copy the specified files into a "build" folder under the artefacts 
    /// folder for the project
    /// </item>
    /// </list>
    /// <para>
    /// The following is an example of how to configure a "copy" action instead of merging:
    /// </para>
    /// <code>
    /// &lt;merge&gt;
    /// &lt;files&gt;
    /// &lt;file&gt;&lt;!-- path to NUnit test file --&gt;&lt;/file&gt;
    /// &lt;file action="Copy"&gt;&lt;!-- path to NUnit images --&gt;&lt;/file&gt;
    /// &lt;/files&gt;
    /// &lt;/merge&gt;
    /// </code>
    /// <para>
    /// The build folder will use the same name as the build label for the project. If this folder already exists, any files within this
    /// folder will be overwritten.
    /// </para>
    /// <heading>
    /// Why are the merged results not showing up in the Web Dashboard?
    /// </heading>
    /// <para>
    /// If you have set up the configuration for the File Merge Task as described above and you are still not ending up with the appropriate
    /// results showing up within the web application, please try the following steps:
    /// </para>
    /// <para>
    /// 1. Click the original log link and check to see if the merged content is included in the xml. If it is missing then got onto step 2.
    /// If it is present and is still not showing up in the web page then try emailing the CCNet users list .
    /// </para>
    /// <para>
    /// 2. Have you put the File Merge Tasks in the &lt;publishers /&gt; section of your Project Configuration Block before your Xml Log Publisher?
    /// </para>
    /// <para>
    /// 3. Check the folder that contains the files that should be merged. If they are not there then you need to dig into your build script
    /// to find out why they aren't getting created.
    /// </para>
    /// <para>
    /// 4. If the files are there but aren't getting merged, double-check your ccnet.config file. Is the configuration specified correctly as
    /// specified above? Remember that case matters in XML tag and attribute names.
    /// </para>
    /// <para>
    /// 5. Check the ccnet.log file. You should see Info-level log messages stating that the files have been merged.
    /// </para>
    /// <para>
    /// 6. Does the file contain valid XML data? The File Merge Task only expects to process XML files. It will attempt to clean up non-XML
    /// files and write errors to the ccnet.log file, but it isn't always successful.
    /// </para>
    /// </remarks>
    [ReflectorType("merge")]
	public class MergeFilesTask
        : TaskBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MergeFilesTask"/> class.
        /// </summary>
        public MergeFilesTask()
        {
            this.MergeFiles = new MergeFileInfo[0];
        }
        #endregion

        /// <summary>
        /// The folder to copy the files to.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Project Artefact Directory</default>
        [ReflectorProperty("target", Required = false)]
        public string TargetFolder { get; set; }

        /// <summary>
        /// The files to merge.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("files", typeof(MergeFileSerialiserFactory))]
        public MergeFileInfo[] MergeFiles { get; set; }

        /// <summary>
        /// Allows this task to interact with the file system in a testable way.
        /// </summary>
        public IFileSystem FileSystem { get; set; }

        /// <summary>
        /// Allows this task to interact with the logger in a testable way.
        /// </summary>
        public ILogger Logger { get; set; }

        protected override bool Execute(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Merging Files");

            var actualFileSystem = FileSystem ?? new SystemIoFileSystem();
            var actualLogger = Logger ?? new DefaultLogger();

            // Make sure the target folder is rooted
            var targetFolder = TargetFolder;
            if (!string.IsNullOrEmpty(targetFolder))
            {
                if (!Path.IsPathRooted(targetFolder))
                {
                    targetFolder = Path.Combine(
                        Path.Combine(result.ArtifactDirectory, result.Label),
                        targetFolder);
                }
            }
            else
            {
                targetFolder = Path.Combine(result.ArtifactDirectory, result.Label);
            }

			foreach (var mergeFile in MergeFiles)
			{
                // Get the name of the file
				string fullMergeFile = mergeFile.FileName;
                if (!Path.IsPathRooted(fullMergeFile))
                {
                    fullMergeFile = Path.Combine(result.WorkingDirectory, fullMergeFile);
                }

                // Merge each file
				WildCardPath path = new WildCardPath(fullMergeFile);
                foreach (var fileInfo in path.GetFiles())
                {
                    if (actualFileSystem.FileExists(fileInfo.FullName))
                    {
                        switch (mergeFile.MergeAction)
                        {
                            case MergeFileInfo.MergeActionType.Merge:
                            case MergeFileInfo.MergeActionType.CData:
                                // Add the file to the merge list
                                actualLogger.Info("Merging file '{0}'", fileInfo);
                                result.BuildProgressInformation.AddTaskInformation(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Merging file '{0}'", fileInfo));
                                result.AddTaskResult(new FileTaskResult(fileInfo, mergeFile.DeleteAfterMerge, actualFileSystem)
                                                         {WrapInCData = (mergeFile.MergeAction == MergeFileInfo.MergeActionType.CData)});
                                break;
                            case MergeFileInfo.MergeActionType.Copy:
                                // Copy the file to the target folder
                                actualFileSystem.EnsureFolderExists(targetFolder);
                                actualLogger.Info("Copying file '{0}' to '{1}'", fileInfo.Name, targetFolder);
                                result.BuildProgressInformation.AddTaskInformation(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Copying file '{0}' to '{1}'", fileInfo.Name, targetFolder));
                                actualFileSystem.Copy(fileInfo.FullName, Path.Combine(targetFolder, fileInfo.Name));
                                break;
                            default:
                                throw new CruiseControlException(
                                    string.Format(System.Globalization.CultureInfo.CurrentCulture,"Unknown file merge action '{0}'", mergeFile.MergeAction));
                        }
                    }
                    else
                    {
                        actualLogger.Warning("File not found '{0}", fileInfo);
                    }
                }
			}

            return true;
		}
	}
}
