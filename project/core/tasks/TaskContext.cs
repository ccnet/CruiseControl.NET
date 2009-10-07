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
    using System.Xml;
    using ThoughtWorks.CruiseControl.Core.Util;

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

        #region resultDetails
        /// <summary>
        /// The result details for the tasks that have have streams created.
        /// </summary>
        private readonly List<TaskResultDetails> resultDetails = new List<TaskResultDetails>();
        #endregion

        #region contextId
        /// <summary>
        /// A unique identifier for the context.
        /// </summary>
        private readonly Guid contextId;
        #endregion

        #region parentContext
        /// <summary>
        /// The parent context for this instance.
        /// </summary>
        private readonly TaskContext parentContext;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskContext"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="artifactFolder">The artifact folder.</param>
        public TaskContext(IFileSystem fileSystem, string artifactFolder)
            : this(fileSystem, artifactFolder, null, Guid.NewGuid())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskContext"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="artifactFolder">The artifact folder.</param>
        /// <param name="parent">The parent context.</param>
        /// <param name="contextId">The context id.</param>
        private TaskContext(IFileSystem fileSystem, string artifactFolder, TaskContext parent, Guid contextId)
        {
            this.fileSystem = fileSystem;
            this.ArtifactFolder = artifactFolder;
            this.parentContext = parent;
            this.contextId = contextId;
        }
        #endregion

        #region Public properties
        #region ArtifactFolder
        /// <summary>
        /// Gets the artifact folder for the task.
        /// </summary>
        /// <value>The artifact folder.</value>
        public string ArtifactFolder { get; private set; }
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
        #endregion

        #region Public methods
        #region CreateResultStream()
        /// <summary>
        /// Opens a new result stream for a task.
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <returns>
        /// The <see cref="Stream"/> for writing the task result.
        /// </returns>
        public virtual Stream CreateResultStream(string taskName, string taskType)
        {
            return this.CreateResultStream(taskName, taskType, false);
        }

        /// <summary>
        /// Opens a new result stream for a task.
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="ignoreExtension">If set to <c>true</c> then the extension is not changed.</param>
        /// <returns>
        /// The <see cref="Stream"/> for writing the task result.
        /// </returns>
        public virtual Stream CreateResultStream(string taskName, string taskType, bool ignoreExtension)
        {
            // Make sure this context has not been finialised
            if (this.IsFinialised)
            {
                throw new ApplicationException("Context has been finialised - no further actions can be performed using it");
            }

            // Validate the task name
            if (String.IsNullOrEmpty(taskName))
            {
                throw new ArgumentException("taskName is null or empty.", "taskName");
            }

            // Validate the task type
            if (String.IsNullOrEmpty(taskType))
            {
                throw new ArgumentException("taskType is null or empty.", "taskType");
            }

            // Generate the filename
            var extension = Path.GetExtension(taskName);
            if (!ignoreExtension || (extension.Length == 0))
            {
                extension = ".xml";
            }

            // Make sure the filename does not exist
            var fileName = this.GenerateUniqueFileName(
                Path.GetFileNameWithoutExtension(taskName) + extension);

            // Add the result details
            var details = new TaskResultDetails(taskName, taskType, fileName);
            this.resultDetails.Add(details);

            // Generate the actual stream
            var stream = this.fileSystem.OpenOutputStream(fileName);
            return stream;
        }
        #endregion

        #region StartChildContext()
        /// <summary>
        /// Starts a new child <see cref="TaskContext"/> instance.
        /// </summary>
        /// <returns>The child <see cref="TaskContext"/> instance.</returns>
        public TaskContext StartChildContext()
        {
            // Generate the child's id here, so it can be used to generate the new folder
            var childId = Guid.NewGuid();

            // Generate the new folder for the child artifacts
            var childFolder = Path.Combine(this.ArtifactFolder, childId.ToString());
            this.fileSystem.EnsureFolderExists(Path.Combine(childFolder, "temp"));

            // Start the new context
            var child = new TaskContext(this.fileSystem, childFolder, this, childId);
            return child;
        }
        #endregion

        #region MergeChildContext()
        /// <summary>
        /// Merges a child <see cref="TaskContext"/>.
        /// </summary>
        /// <param name="childContext">The child <see cref="TaskContext"/> to merge.</param>
        public void MergeChildContext(TaskContext childContext)
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

            // Merge all the child results
            foreach (var childResult in childContext.resultDetails)
            {
                var newFilePath = this.GenerateUniqueFileName(
                    Path.GetFileName(childResult.FileName));
                this.fileSystem.MoveFile(childResult.FileName, newFilePath);
                this.resultDetails.Add(
                    new TaskResultDetails(childResult.TaskName, childResult.TaskType, newFilePath));
            }
        }
        #endregion

        #region Finialise()
        /// <summary>
        /// Finialises this instance.
        /// </summary>
        public void Finialise()
        {
            // Mark this instance as finialised
            this.IsFinialised = true;

            // Write out the index
            var indexPath = Path.Combine(this.ArtifactFolder, "ccnet-task-index.xml");
            using (var indexStream = this.fileSystem.OpenOutputStream(indexPath))
            {
                var settings = new XmlWriterSettings
                {
                    CheckCharacters = true,
                    CloseOutput = true,
                    ConformanceLevel = ConformanceLevel.Document,
                    Encoding = UTF8Encoding.UTF8,
                    Indent = false,
                    NewLineHandling = NewLineHandling.None,
                    NewLineOnAttributes = false,
                    OmitXmlDeclaration = true
                };
                using (var document = XmlWriter.Create(indexStream, settings))
                {
                    document.WriteStartElement("task");
                    foreach (var result in this.resultDetails)
                    {
                        document.WriteStartElement("result");
                        document.WriteAttributeString("file", result.FileName);
                        document.WriteAttributeString("name", result.TaskName);
                        document.WriteAttributeString("type", result.TaskType);
                        document.WriteEndElement();
                    }

                    document.WriteEndElement();
                    document.Flush();
                    document.Close();
                }
            }
        }
        #endregion

        #region RunTask()
        /// <summary>
        /// Runs a task in a child context.
        /// </summary>
        /// <param name="task">The task to run.</param>
        /// <param name="result">The result to use.</param>
        public void RunTask(TaskBase task, IIntegrationResult result)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task", "task is null.");
            }

            // Start a new child context and associate it with the task
            var child = this.StartChildContext();
            try
            {
                task.AssociateContext(this);
                task.Run(result);
            }
            finally
            {
                // Clean up
                this.MergeChildContext(child);
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region GenerateUniqueFileName()
        /// <summary>
        /// Generates a unique file name.
        /// </summary>
        /// <param name="fileName">The base name of the file.</param>
        /// <returns>A unique file name within the artifacts folder.</returns>
        private string GenerateUniqueFileName(string fileName)
        {
            var baseFileName = Path.Combine(
                this.ArtifactFolder,
                Path.GetFileNameWithoutExtension(fileName));
            var extension = Path.GetExtension(fileName);

            var actualFileName = baseFileName + extension;
            var copy = 0;
            while (this.fileSystem.FileExists(actualFileName))
            {
                actualFileName = baseFileName + "-" + (++copy).ToString() + extension;
            }

            return actualFileName;
        }
        #endregion
        #endregion
    }
}
