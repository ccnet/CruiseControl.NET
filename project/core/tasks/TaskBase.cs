﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// An abstract base class to add parameters to a task
    /// </summary>
    public abstract class TaskBase
        : IParamatisedItem, IStatusSnapshotGenerator, ITask, IStatusItem
    {
        #region Private fields
        private IDynamicValue[] myDynamicValues = new IDynamicValue[0];
        private ItemStatus currentStatus;
        private List<TimeSpan> elapsedTimes = new List<TimeSpan>();
        private EnvironmentVariable[] myEnvironmentVariables = new EnvironmentVariable[0]; 
        #endregion

        #region Public properties
        #region DynamicValues
        /// <summary>
        /// The dynamic values to use for the task.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("dynamicValues", Required = false)]
        public IDynamicValue[] DynamicValues
        {
            get { return myDynamicValues; }
            set { myDynamicValues = value;}
        }
        #endregion

        #region Name
        /// <summary>
        /// The name of the task - by default this is the name of the type.
        /// </summary>
        public virtual string Name
        {
            get { return GetType().Name; }
        }
        #endregion

        #region Description
        /// <summary>
        /// Description used for the visualisation of the buildstage, if left empty the process name will be shown.
        /// </summary>
        /// <version>1.5</version>
        /// <default>The task/publisher name.</default>
        [ReflectorProperty("description", Required = false)]
        public string Description { get; set; }
        #endregion

        #region CurrentStatus
        /// <summary>
        /// The current status of the task.
        /// </summary>
        public ItemStatus CurrentStatus
        {
            get { return currentStatus; }
        }
        #endregion

        #region WasSuccessful
        /// <summary>
        /// Gets or sets a value indicating whether the task was successful.
        /// </summary>
        /// <value><c>true</c> if the task was successful; otherwise, <c>false</c>.</value>
        public bool WasSuccessful { get; private set; }
        #endregion


        /// <summary>                                                                                              
        /// A set of environment variables set for commands that are executed.                               
        /// </summary>                                                                                             
        /// <remarks>                                                                                              
        /// Each variable should be specified as <code>&lt;variable name="name" value="value"/&gt;</code>.         
        /// </remarks>                                                                                             
        [ReflectorProperty("environment", Required = false)]
        public EnvironmentVariable[] EnvironmentVariables
        {
            get { return myEnvironmentVariables; }
            set { myEnvironmentVariables = value; }
        }

        
        #endregion

        #region Public methods
        #region Run()
        /// <summary>
        /// Runs the task, given the specified <see cref="IIntegrationResult"/>, in the specified <see cref="IProject"/>.
        /// </summary>
        /// <param name="result"></param>
        public virtual void Run(IIntegrationResult result)
        {
            // Initialise the task
            this.WasSuccessful = false;
            if ((this.currentStatus == null) ||
                (this.currentStatus.Status != ItemBuildStatus.Running))
            {
                InitialiseStatus(ItemBuildStatus.Pending);
                currentStatus.Status = ItemBuildStatus.Running;
            }

            // Perform the actual run
            currentStatus.TimeOfEstimatedCompletion = CalculateEstimatedTime();
            currentStatus.TimeStarted = DateTime.Now;
            try
            {
                this.WasSuccessful = Execute(result);
            }
            catch (Exception error)
            {
                // Store the error message
                currentStatus.Error = error.Message;
                result.Status = IntegrationStatus.Exception;
                throw;
            }
            finally
            {
                // Clean up
                currentStatus.Status = (this.WasSuccessful) ? ItemBuildStatus.CompletedSuccess : ItemBuildStatus.CompletedFailed;
                currentStatus.TimeCompleted = DateTime.Now;

                switch (result.Status)
                {
                    case IntegrationStatus.Unknown:
                    case IntegrationStatus.Success:
                        result.Status = this.WasSuccessful ? IntegrationStatus.Success : IntegrationStatus.Failure;
                        break;
                }
            }
        }
        #endregion

        #region CalculateEstimatedTime()
        /// <summary>
        /// Calculate the estimated time of completion.
        /// </summary>
        /// <returns></returns>
        public virtual DateTime? CalculateEstimatedTime()
        {
            // Calculate the estimated completion time
            double seconds = 0;
            if (elapsedTimes.Count > 0)
            {
                for (var loop = 0; loop < elapsedTimes.Count; loop++)
                {
                    seconds += elapsedTimes[loop].TotalSeconds;
                }
                seconds /= elapsedTimes.Count;
            }
            return seconds > 0 ? (DateTime?)DateTime.Now.AddSeconds(seconds) : null;
        }
        #endregion

        #region GenerateSnapshot()
        /// <summary>
        /// Generates a snapshot of the current status.
        /// </summary>
        /// <returns></returns>
        public virtual ItemStatus GenerateSnapshot()
        {
            if (currentStatus == null) this.InitialiseStatus(ItemBuildStatus.Unknown);
            return currentStatus;
        }
        #endregion

        #region InitialiseStatus()
        /// <summary>
        /// Initialises the status.
        /// </summary>
        public void InitialiseStatus()
        {
            this.InitialiseStatus(ItemBuildStatus.Pending);
        }

        /// <summary>
        /// Initialise an <see cref="ItemStatus"/>.
        /// </summary>
        /// <param name="newStatus">The new status.</param>
        public virtual void InitialiseStatus(ItemBuildStatus newStatus)
        {
            // Store the last elapsed time
            if (currentStatus != null)
            {
                var elapsedTime = currentStatus.TimeCompleted - currentStatus.TimeStarted;
                if (elapsedTime.HasValue)
                {
                    if (elapsedTimes.Count >= 8)
                    {
                        elapsedTimes.RemoveAt(7);
                    }
                    elapsedTimes.Insert(0, elapsedTime.Value);
                }
            }

            // Initialise the status with the default value - do not re-initialise if the status is already set
            if ((currentStatus == null) || (currentStatus.Status != newStatus))
            {
                currentStatus = new ItemStatus
                {
                    Name = Name,
                    Description = Description,
                    Status = newStatus,
                    TimeCompleted = null,
                    TimeOfEstimatedCompletion = null,
                    TimeStarted = null
                };
            }
        }
        #endregion

        #region CancelStatus()
        /// <summary>
        /// Cancels the status.
        /// </summary>
        public void CancelStatus()
        {
            if ((this.currentStatus != null) && 
                ((this.currentStatus.Status == ItemBuildStatus.Running) ||
                 (this.currentStatus.Status == ItemBuildStatus.Pending)))
            {
                this.currentStatus.Status = ItemBuildStatus.Cancelled;
            }
        }
        #endregion

        #region ApplyParameters()
        /// <summary>
        /// Applies the input parameters to the task.
        /// </summary>
        /// <param name="parameters">The parameters to apply.</param>
        /// <param name="parameterDefinitions">The original parameter definitions.</param>
        public virtual void ApplyParameters(Dictionary<string, string> parameters, IEnumerable<ParameterBase> parameterDefinitions)
        {
            if (myDynamicValues != null)
            {
                foreach (IDynamicValue value in myDynamicValues)
                {
                    value.ApplyTo(this, parameters, parameterDefinitions);
                }
            }
        }
        #endregion

        #region PreprocessParameters()
        /// <summary>
        /// Preprocesses a node prior to loading it via NetReflector.
        /// </summary>
        /// <param name="typeTable">The type table.</param>
        /// <param name="inputNode">The input node.</param>
        /// <returns></returns>
        [ReflectionPreprocessor]
        public virtual XmlNode PreprocessParameters(NetReflectorTypeTable typeTable, XmlNode inputNode)
        {
            var node = DynamicValueUtility.ConvertXmlToDynamicValues(typeTable, inputNode);
            if (!string.IsNullOrEmpty(inputNode.NamespaceURI) &&
                inputNode.NamespaceURI.StartsWith("http://thoughtworks.org/ccnet/"))
            {
                var parts = inputNode.NamespaceURI.Split('/');
                var version = new Version(
                    Convert.ToInt32(parts[parts.Length - 2], CultureInfo.CurrentCulture),
                    Convert.ToInt32(parts[parts.Length - 1], CultureInfo.CurrentCulture));
                node = this.UpgradeConfiguration(version, node);
            }

            return node;
        }
        #endregion

        #region RetrieveDescriptionOrName()
        /// <summary>
        /// Retrieves the description if it is set, otherwise the name of the task.
        /// </summary>
        /// <returns>The description or name of the task.</returns>
        public virtual string RetrieveDescriptionOrName()
        {
            var value = string.IsNullOrEmpty(Description) ? Name : Description;
            return value;
        }
        #endregion
        #endregion

        #region Protected methods
        #region Execute()
        /// <summary>
        /// Execute the actual task functionality.
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns><c>true</c> if the task was successful; <c>false</c> otherwise.</returns>
        protected abstract bool Execute(IIntegrationResult result);
        #endregion

        #region UpgradeConfiguration()
        /// <summary>
        /// Upgrades the configuration for the node.
        /// </summary>
        /// <param name="configVersion">The version of the configuration.</param>
        /// <param name="node">The input node.</param>
        /// <returns>The upgraded node.</returns>
        protected virtual XmlNode UpgradeConfiguration(Version configVersion, XmlNode node)
        {
            return node;
        }
        #endregion
        #endregion
    }
}
