//-----------------------------------------------------------------------
// <copyright file="TaskContext.cs" company="Craig Sutherland">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using ThoughtWorks.CruiseControl.Core.Publishers;
    using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Provides context for a task.
    /// </summary>
    public class TaskContext
    {
        #region Private fields
        #region fileSystem
        /// <summary>
        /// The <see cref="IFileSystem"/> to use when manipulating the file system.
        /// </summary>
        private readonly IFileSystem fileSystem;
        #endregion

        #region parentContext
        /// <summary>
        /// The parent context for this instance.
        /// </summary>
        private readonly TaskContext parentContext;
        #endregion

        #region contextId
        /// <summary>
        /// The identifier of the context.
        /// </summary>
        private readonly Guid contextId = Guid.NewGuid();
        #endregion

        #region result
        /// <summary>
        /// The integration result for the context.
        /// </summary>
        private readonly IIntegrationResult result;
        #endregion

        #region snapshotLock
        /// <summary>
        /// A lock object to use for generating snapshots.
        /// </summary>
        private readonly object snapshotLock = new object();
        #endregion

        #region associatedResult
        /// <summary>
        /// The associated <see cref="TaskResult"/>.
        /// </summary>
        private TaskResult associatedResult;
        #endregion

        #region buildFolder
        /// <summary>
        /// The integration result for the context.
        /// </summary>
        private string buildFolder;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskContext"/> class.
        /// </summary>
        /// <param name="project">The project configuration.</param>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="result">The associated result.</param>
        private TaskContext(ProjectConfiguration project, IFileSystem fileSystem, IIntegrationResult result)
            : this(project, fileSystem, result, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskContext"/> class.
        /// </summary>
        /// <param name="project">The project configuration.</param>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="result">The associated result.</param>
        /// <param name="parent">The parent context.</param>
        private TaskContext(ProjectConfiguration project, IFileSystem fileSystem, IIntegrationResult result, TaskContext parent)
        {
            this.Project = project;
            this.fileSystem = fileSystem;
            this.result = result;
            this.parentContext = parent;
            this.associatedResult = new TaskResult("project", project.Name);
        }
        #endregion

        #region Public properties
        #region Project
        /// <summary>
        /// Gets the configuration settings for the project.
        /// </summary>
        public ProjectConfiguration Project { get; private set; }
        #endregion

        #region IsFinialised
        /// <summary>
        /// Gets a value indicating whether this instance is finialised.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is finialised; otherwise, <c>false</c>.
        /// </value>
        public bool IsFinialised { get; private set; }
        #endregion

        #region BuildFolder
        /// <summary>
        /// Gets the build folder.
        /// </summary>
        /// <value>The build folder.</value>
        public string BuildFolder
        {
            get
            {
                if (this.buildFolder == null)
                {
                    this.buildFolder = this.result.BaseFromArtifactsDirectory(this.result.Label);
                }

                return this.buildFolder;
            }
        }
        #endregion
        #endregion

        #region Public methods
        #region FromProject()
        /// <summary>
        /// Starts a new task context from a project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="result">The result.</param>
        /// <returns>The new <see cref="TaskContext"/>.</returns>
        public static TaskContext FromProject(Project project, IIntegrationResult result)
        {
            var context = new TaskContext(ProjectConfiguration.FromProject(project), new SystemIoFileSystem(), result);
            return context;
        }

        /// <summary>
        /// Starts a new task context from a project.
        /// </summary>
        /// <param name="project">The project configuration.</param>
        /// <param name="result">The result.</param>
        /// <returns>The new <see cref="TaskContext"/>.</returns>
        public static TaskContext FromProject(ProjectConfiguration project, IIntegrationResult result)
        {
            var context = new TaskContext(project, new SystemIoFileSystem(), result);
            return context;
        }
        #endregion

        #region GetStreamName()
        /// <summary>
        /// Gets the filename of the stream.
        /// </summary>
        /// <param name="remoteStream">The remote stream.</param>
        /// <returns>The filename of the stream.</returns>
        public string GetStreamName(Stream remoteStream)
        {
            var fileStream = remoteStream as FileStream;
            if (fileStream != null)
            {
                return fileStream.Name;
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region ImportResult()
        /// <summary>
        /// Imports a result from an external source.
        /// </summary>
        /// <param name="resultToImport">The result to import.</param>
        public void ImportResult(TaskResult resultToImport)
        {
            this.associatedResult.Children.Add(resultToImport);
        }
        #endregion

        #region ImportResultFile()
        /// <summary>
        /// Imports a result file.
        /// </summary>
        /// <param name="filename">The filename of the results file.</param>
        /// <param name="resultName">Name of the result.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="deleteSourceFile">If set to <c>true</c> the source file will be deleted.</param>
        public void ImportResultFile(string filename, string resultName, string dataType, bool deleteSourceFile)
        {
            // Make sure this context has not been finialised
            if (this.IsFinialised)
            {
                throw new ApplicationException("Context has been finialised - no further actions can be performed using it");
            }

            // Validate the task type
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("filename is null or empty.", "filename");
            }

            // Validate the result name
            if (String.IsNullOrEmpty(resultName))
            {
                throw new ArgumentException("resultName is null or empty.", "resultName");
            }

            // Validate the data type
            if (String.IsNullOrEmpty(dataType))
            {
                throw new ArgumentException("dataType is null or empty.", "dataType");
            }

            // Generate the filename
            var actualFilename = this.GenerateDataFilename();
            this.fileSystem.EnsureFolderExists(actualFilename);

            // Add the result details
            var details = new TaskOutput(actualFilename, resultName, dataType);
            this.LockResults(
                t =>
                {
                    this.associatedResult.Output.Add(details);
                },
                "Unable to add new output - index is locked");

            // Move/copy the file
            if (deleteSourceFile)
            {
                File.Move(filename, actualFilename);
            }
            else
            {
                File.Copy(filename, actualFilename);
            }
        }

        public void ImportResultFile(Stream inputStream, string resultName, string dataType)
        {
            // Make sure this context has not been finialised
            if (this.IsFinialised)
            {
                throw new ApplicationException("Context has been finialised - no further actions can be performed using it");
            }

            // Validate the input stream
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream");
            }

            // Validate the result name
            if (String.IsNullOrEmpty(resultName))
            {
                throw new ArgumentException("resultName is null or empty.", "resultName");
            }

            // Validate the data type
            if (String.IsNullOrEmpty(dataType))
            {
                throw new ArgumentException("dataType is null or empty.", "dataType");
            }

            // Open the new stream and copy the data
            using (var newStream = this.CreateResultStream(resultName, dataType))
            {
                using (var stream = this.fileSystem.ResetStreamForReading(inputStream))
                {
                    var buffer = new byte[4096];
                    var length = 1;
                    while ((length = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        newStream.Write(buffer, 0, length);
                    }
                }
            }
        }
        #endregion

        #region CreateResultStream()
        /// <summary>
        /// Opens a new result stream for a task.
        /// </summary>
        /// <param name="resultName">Name of the result.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>
        /// The <see cref="Stream"/> for writing the task result.
        /// </returns>
        public virtual Stream CreateResultStream(string resultName, string dataType)
        {
            // Make sure this context has not been finialised
            if (this.IsFinialised)
            {
                throw new ApplicationException("Context has been finialised - no further actions can be performed using it");
            }

            // Validate the result name
            if (String.IsNullOrEmpty(resultName))
            {
                throw new ArgumentException("resultName is null or empty.", "resultName");
            }

            // Validate the data type
            if (String.IsNullOrEmpty(dataType))
            {
                throw new ArgumentException("dataType is null or empty.", "dataType");
            }

            // Generate the filename
            var fileName = this.GenerateDataFilename();
            this.fileSystem.EnsureFolderExists(fileName);

            // Add the result details
            var details = new TaskOutput(fileName, resultName, dataType);
            this.LockResults(
                t =>
                {
                    this.associatedResult.Output.Add(details);
                },
                "Unable to add new result - index is locked");

            // Generate the actual stream
            var stream = this.fileSystem.OpenOutputStream(fileName);
            return stream;
        }
        #endregion

        #region StartChildContext()
        /// <summary>
        /// Starts a new child <see cref="TaskContext"/> instance.
        /// </summary>
        /// <returns>
        /// The child <see cref="TaskContext"/> instance.
        /// </returns>
        public TaskContext StartChildContext()
        {
            // Start the new context
            var child = new TaskContext(this.Project, this.fileSystem, this.result, this);
            return child;
        }
        #endregion

        #region InitialiseResult()
        /// <summary>
        /// Initialises the associated result.
        /// </summary>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="identifier">The identifier (name) of the task.</param>
        public void InitialiseResult(string taskType, string identifier)
        {
            this.associatedResult = new TaskResult(taskType, identifier);
        }
        #endregion

        #region MergeChildContext()
        /// <summary>
        /// Merges a child <see cref="TaskContext"/>.
        /// </summary>
        /// <param name="childContext">The child <see cref="TaskContext"/> to merge.</param>
        /// <param name="status">The status.</param>
        public void MergeChildContext(TaskContext childContext, ItemBuildStatus status)
        {
            // Validate the child context first
            if (childContext == null)
            {
                throw new ArgumentNullException("childContext", "childContext is null.");
            }
            else if (childContext.parentContext.contextId != this.contextId)
            {
                throw new ArgumentException("Unable to merge - child does not belong to this context", "childContext");
            }

            // Finialise the context so noone else can modify it
            childContext.IsFinialised = true;
            childContext.associatedResult.TaskOutcome = status;

            // Merge all the child results
            this.LockResults(
                t =>
                {
                    this.associatedResult.Children.Add(childContext.associatedResult);
                },
                "Unable to merge results - index is locked");
        }
        #endregion

        #region GenerateLogFolder()
        /// <summary>
        /// Generates the log folder.
        /// </summary>
        /// <param name="logFolder">The default log folder.</param>
        /// <returns>The name of the log folder.</returns>
        /// <remarks>
        /// This method will also ensure that the log folder exists.
        /// </remarks>
        public string GenerateLogFolder(string logFolder)
        {
            // Generate the folder name
            var folder = logFolder ?? "buildlogs";
            if (!Path.IsPathRooted(folder))
            {
                folder = this.result.BaseFromArtifactsDirectory(folder);
            }

            // Ensure the folder exists
            this.fileSystem.EnsureFolderExists(folder, false);
            return folder;
        }
        #endregion

        #region GenerateLogFilename()
        /// <summary>
        /// Generates the log filename.
        /// </summary>
        /// <returns>The name of the log file.</returns>
        public string GenerateLogFilename()
        {
            var baseName = Util.StringUtil.RemoveInvalidCharactersFromFileName(new LogFile(this.result).Filename);
            var fullName = Path.Combine(this.result.BuildLogDirectory, baseName);
            return fullName;
        }
        #endregion

        #region Finialise()
        /// <summary>
        /// Finialises this instance.
        /// </summary>
        public void Finialise(ItemBuildStatus status)
        {
            // Mark this instance as finialised
            this.IsFinialised = true;
            this.associatedResult.TaskOutcome = status;

            // Generate the log folder
            var logLocation = this.GenerateLogFolder(this.Project.LogFolder);
            this.result.BuildLogDirectory = logLocation;

            // Start the log writer
            var logName = this.GenerateLogFilename();
            this.fileSystem.DeleteFile(logName);
            using (var writer = new StreamWriter(this.fileSystem.OpenOutputStream(logName)))
            {
                this.WriteCurrentLog(writer);
            }
        }
        #endregion

        #region WriteCurrentLog()
        /// <summary>
        /// Writes the current log to the writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void WriteCurrentLog(TextWriter writer)
        {
            using (var integrationWriter = new XmlIntegrationResultWriter(writer, this.GenerateResultsSnapshot()))
            {
                integrationWriter.Formatting = Formatting.Indented;
                integrationWriter.Write(this.result);
            }
        }
        #endregion

        #region GenerateResultsSnapshot()
        /// <summary>
        /// Generates a snapshot of the current results.
        /// </summary>
        /// <returns>
        /// The current results that have been generated.
        /// </returns>
        public TaskResult GenerateResultsSnapshot()
        {
            // Add the results from this context
            TaskResult snapshot = null;

            if (this.parentContext != null)
            {
                // Add the results from the parent and any ancestors
                snapshot = this.parentContext.GenerateResultsSnapshot();
            }
            else
            {
                this.LockResults(
                    t =>
                    {
                        snapshot = this.associatedResult;
                    },
                    "Unable to generate snapshot - unable to retrieve lock");
            }

            return snapshot;
        }
        #endregion

        #region RunTask()
        /// <summary>
        /// Runs a task in a child context.
        /// </summary>
        /// <param name="task">The task to run.</param>
        public void RunTask(ITask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task", "task is null.");
            }

            // Start a new child context and associate it with the task
            var child = this.StartChildContext();
            try
            {
                if (task is TaskBase)
                {
                    (task as TaskBase).AssociateContext(child);
                }

                task.Run(this.result);
            }
            finally
            {
                // Clean up
                if (task is TaskBase)
                {
                    this.MergeChildContext(child, (task as TaskBase).CurrentStatus.Status);
                }
                else
                {
                    this.MergeChildContext(child, ItemBuildStatus.Unknown);
                }
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region LockResults()
        /// <summary>
        /// Locks the results.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="errorMessage">The error message if the lock cannot be entered.</param>
        private void LockResults(Action<bool> action, string errorMessage)
        {
            // Attempt to lock the results
            if (Monitor.TryEnter(this.snapshotLock, 30000))
            {
                try
                {
                    action(true);
                }
                finally
                {
                    Monitor.Exit(this.snapshotLock);
                }
            }
            else
            {
                // Cannot lock because someone else is holding the lock - deadlock?
                throw new CruiseControlException(errorMessage);
            }
        }
        #endregion

        #region GenerateDataFilename()
        /// <summary>
        /// Generates a new data filename.
        /// </summary>
        /// <returns>The full path to the data file.</returns>
        private string GenerateDataFilename()
        {
            return Path.Combine(this.BuildFolder, Guid.NewGuid().ToString() + ".data");
        }
        #endregion
        #endregion
    }
}
