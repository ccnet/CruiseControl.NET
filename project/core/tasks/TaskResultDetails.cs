//-----------------------------------------------------------------------
// <copyright file="TaskResultDetails.cs" company="Craig Sutherland">
//     Copyright (c) 2009 Craig Sutherland. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines the details on a task result.
    /// </summary>
    public class TaskResultDetails
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResultDetails"/> class.
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="fileName">Name of the file the results are stored in.</param>
        public TaskResultDetails(string taskName, string taskType, string fileName)
        {
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

            // Validate the file name
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName is null or empty.", "fileName");
            }

            // Store the arguments
            this.TaskName = taskName;
            this.TaskType = taskType;
            this.FileName = fileName;
        }
        #endregion

        #region Public properties
        #region TaskName
        /// <summary>
        /// Gets or sets the name of the task.
        /// </summary>
        /// <value>The task name.</value>
        public string TaskName { get; private set; }
        #endregion

        #region TaskType
        /// <summary>
        /// Gets or sets the type of the task.
        /// </summary>
        /// <value>The task type.</value>
        public string TaskType { get; private set; }
        #endregion

        #region FileName
        /// <summary>
        /// Gets or sets the name of the file the results are stored in.
        /// </summary>
        /// <value>The file name.</value>
        public string FileName { get; private set; }
        #endregion
        #endregion
    }
}
