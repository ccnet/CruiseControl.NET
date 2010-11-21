
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Xml;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Contains all the results of a project's integration.
    /// </summary>
    [Serializable]
    public class IntegrationResult : IIntegrationResult
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string InitialLabel = "UNKNOWN";

        // immutable properties
        private string projectName;
        private string projectUrl;
        private string workingDirectory;
        private string artifactDirectory = string.Empty;
        private IntegrationRequest request = IntegrationRequest.NullRequest;
        private IntegrationSummary lastIntegration = IntegrationSummary.Initial;
        private string buildLogDirectory;
        private List<NameValuePair> parameters = new List<NameValuePair>();
        private List<NameValuePair> sourceControlData = new List<NameValuePair>();

        // mutable properties
        private IntegrationStatus status = IntegrationStatus.Unknown;
        private ArrayList failureUsers = new ArrayList();
        private string label = InitialLabel;
        private DateTime startTime;
        private DateTime endTime;
        private Modification[] modifications = new Modification[0];
        private Exception exception;

        private readonly List<ITaskResult> taskResults = new List<ITaskResult>();

        private readonly BuildProgressInformation buildProgressInformation = new BuildProgressInformation(string.Empty, string.Empty);

        private Exception sourceControlError;


        /// <summary>
        /// Gets the build progress information.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        [XmlIgnore]
        public BuildProgressInformation BuildProgressInformation
        {
            get { return buildProgressInformation; }
        }

        // Default constructor required for serialization
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationResult" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public IntegrationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationResult" /> class.	
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="artifactDirectory">The artifact directory.</param>
        /// <param name="request">The request.</param>
        /// <param name="lastIntegration">The last integration.</param>
        /// <remarks></remarks>
        public IntegrationResult(string projectName, string workingDirectory, string artifactDirectory, IntegrationRequest request, IntegrationSummary lastIntegration)
        {
            ProjectName = projectName;
            WorkingDirectory = workingDirectory;
            ArtifactDirectory = artifactDirectory;
            this.request = (lastIntegration.IsInitial()) ? new IntegrationRequest(BuildCondition.ForceBuild, request.Source, request.UserName) : request;
            this.lastIntegration = lastIntegration;
            if ((lastIntegration.Status == IntegrationStatus.Exception)
                || (lastIntegration.Status == IntegrationStatus.Failure))
                failureUsers = lastIntegration.FailureUsers;       // Inherit the previous build's failureUser list if it failed.

            buildProgressInformation = new BuildProgressInformation(artifactDirectory, projectName);
            
            this.label = this.LastIntegration.Label;
        }

        /// <summary>
        /// Gets or sets the name of the project.	
        /// </summary>
        /// <value>The name of the project.</value>
        /// <remarks></remarks>
        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
        }

        /// <summary>
        /// Gets or sets the project URL.	
        /// </summary>
        /// <value>The project URL.</value>
        /// <remarks></remarks>
        public string ProjectUrl
        {
            get { return projectUrl; }
            set { projectUrl = value; }
        }

        /// <summary>
        /// Gets or sets the build condition.	
        /// </summary>
        /// <value>The build condition.</value>
        /// <remarks></remarks>
        public BuildCondition BuildCondition
        {
            get { return request.BuildCondition; }
            set { request = new IntegrationRequest(value, "reloaded from state file", null); }
        }

        /// <summary>
        /// Gets or sets the label.	
        /// </summary>
        /// <value>The label.</value>
        /// <remarks></remarks>
        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        /// <summary>
        ///  The parameters used.
        /// </summary>
        public List<NameValuePair> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }


        /// <summary>
        /// Obtain the label as an integer if possible, otherwise zero. 
        /// </summary>
        /// <remarks>
        /// "0" is better than "-1" since build numbers are non-negative
        /// and "-" is a character frequently used to separate version components
        /// when represented in string form.  Thus "-1" might give someone
        /// "1-0--1", which might cause all sorts of havoc for them.  Best to
        /// avoid the "-" character.
        /// </remarks>
        public int NumericLabel
        {
            get
            {
                try
                {
                    string tempNumericLabel = Regex.Replace(Label, @".*?(\d+$)", "$1");
                    return int.Parse(tempNumericLabel, CultureInfo.CurrentCulture);
                }
                catch (FormatException)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the working directory.	
        /// </summary>
        /// <value>The working directory.</value>
        /// <remarks></remarks>
        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set { workingDirectory = value; }
        }

        /// <summary>
        /// Gets or sets the artifact directory.	
        /// </summary>
        /// <value>The artifact directory.</value>
        /// <remarks></remarks>
        public string ArtifactDirectory
        {
            get { return artifactDirectory; }
            set { artifactDirectory = value; }
        }


        /// <summary>
        /// Gets or sets the build log directory.	
        /// </summary>
        /// <value>The build log directory.</value>
        /// <remarks></remarks>
        public string BuildLogDirectory
        {
            get { return buildLogDirectory; }
            set { buildLogDirectory = value; }
        }


        /// <summary>
        /// Gets the integration artifact directory.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string IntegrationArtifactDirectory
        {
            get { return Path.Combine(ArtifactDirectory, Label); }
        }

        /// <summary>
        /// Gets the listener file.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string ListenerFile
        {
            get
            {
                return Path.Combine(artifactDirectory,
                  StringUtil.RemoveInvalidCharactersFromFileName(projectName) + "_ListenFile.xml");
            }
        }

        /// <summary>
        /// Gets or sets the status.	
        /// </summary>
        /// <value>The status.</value>
        /// <remarks></remarks>
        public IntegrationStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// Gets and sets the date and time at which the integration commenced.
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        /// <summary>
        /// Gets and sets the date and time at which the integration was completed.
        /// </summary>
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        /// <summary>
        /// Gets or sets the modifications.	
        /// </summary>
        /// <value>The modifications.</value>
        /// <remarks></remarks>
        [XmlIgnore]
        public virtual Modification[] Modifications
        {
            get { return modifications; }
            set { modifications = value; }
        }

        /// <summary>
        /// Gets the last modification date.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public DateTime LastModificationDate
        {
            get
            {
                //TODO: why set the date to yesterday's date as a default
                if (Modifications.Length == 0)
                {
                    //If there are no modifications then this should be set to the last modification date
                    // from the last integration (or 1/1/1980 if there is no previous integration).
                    return DateTime.Now.AddDays(-1.0);
                }

                DateTime latestDate = DateTime.MinValue;
                foreach (Modification modification in Modifications)
                {
                    latestDate = DateUtil.MaxDate(modification.ModifiedTime, latestDate);
                }
                return latestDate;
            }
        }

        /// <summary>
        /// Retrieves the change number of the last modification.
        /// </summary>
        /// <returns>The last change number if there are any changes, null otherwise.</returns>
        /// <remarks>
        /// Since ChangeNumbers are no longer numbers, this will return null if there are no 
        /// modifications.
        /// </remarks>
        public string LastChangeNumber
        {
            get
            {
                return Modification.GetLastChangeNumber(modifications);
            }
        }

        /// <summary>
        /// Determines whether this instance is initial.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsInitial()
        {
            return Label == InitialLabel;
        }

        /// <summary>
        /// Gets a value indicating the success of this integration.
        /// </summary>
        public bool Succeeded
        {
            get { return Status == IntegrationStatus.Success; }
        }

        /// <summary>
        /// Gets a value indicating whether this integration failed.
        /// </summary>
        public bool Failed
        {
            get { return Status == IntegrationStatus.Failure; }
        }

        /// <summary>
        /// Gets a value indicating whether this integration fixed a previously broken build.
        /// </summary>
        public bool Fixed
        {
            get { return Succeeded && LastIntegrationStatus == IntegrationStatus.Failure; }
        }

        /// <summary>
        /// Gets the time taken to perform the project's integration.
        /// </summary>
        public TimeSpan TotalIntegrationTime
        {
            get { return EndTime - StartTime; }
        }

        // Exceptions cannot be serialised because of permission attributes
        /// <summary>
        /// Gets or sets the exception result.	
        /// </summary>
        /// <value>The exception result.</value>
        /// <remarks></remarks>
        [XmlIgnore]
        public Exception ExceptionResult
        {
            get { return exception; }
            set
            {
                exception = value;
                if (exception != null)
                    Status = IntegrationStatus.Exception;
            }
        }

        /// <summary>
        /// Gets the task results.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        [XmlIgnore]
        public IList TaskResults
        {
            get { return taskResults; }
        }

        /// <summary>
        /// Adds the task result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public void AddTaskResult(string result)
        {
            AddTaskResult(new DataTaskResult(result));
        }

        /// <summary>
        /// Adds the task result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public void AddTaskResult(ITaskResult result)
        {
            taskResults.Add(result);
            if (Failed || Status == IntegrationStatus.Exception)
                return;

            Status = result.CheckIfSuccess() ? IntegrationStatus.Success : IntegrationStatus.Failure;
        }

        /// <summary>
        /// Marks the start time.	
        /// </summary>
        /// <remarks></remarks>
        public void MarkStartTime()
        {
            StartTime = DateTime.Now;
        }

        /// <summary>
        /// Marks the end time.	
        /// </summary>
        /// <remarks></remarks>
        public void MarkEndTime()
        {
            EndTime = DateTime.Now;
        }

        /// <summary>
        /// Determines whether this instance has modifications.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool HasModifications()
        {
            return Modifications.Length > 0;
        }

        /// <summary>
        /// Creates the initial integration result.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="artifactDirectory">The artifact directory.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static IntegrationResult CreateInitialIntegrationResult(string project, string workingDirectory, string artifactDirectory)
        {
            IntegrationRequest initialRequest = new IntegrationRequest(BuildCondition.ForceBuild, "Initial Build", null);
            IntegrationResult result = new IntegrationResult(project, workingDirectory, artifactDirectory, initialRequest, IntegrationSummary.Initial);
            result.StartTime = DateTime.Now.AddDays(-1);
            result.EndTime = DateTime.Now;
            return result;
        }

        /// <summary>
        /// Determines whether a build should run.  A build should run if there
        /// are modifications, and none have occurred within the modification
        /// delay.
        /// </summary>
        public bool ShouldRunBuild()
        {
            return BuildCondition.ForceBuild == BuildCondition || HasModifications();
        }

        /// <summary>
        /// Bases from artifacts directory.	
        /// </summary>
        /// <param name="pathToBase">The path to base.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string BaseFromArtifactsDirectory(string pathToBase)
        {
            return string.IsNullOrEmpty(pathToBase) ? ArtifactDirectory : Path.Combine(ArtifactDirectory, pathToBase);
        }

        /// <summary>
        /// Bases from working directory.	
        /// </summary>
        /// <param name="pathToBase">The path to base.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string BaseFromWorkingDirectory(string pathToBase)
        {
            return string.IsNullOrEmpty(pathToBase) ? WorkingDirectory : Path.Combine(WorkingDirectory, pathToBase);
        }

        /// <summary>
        /// Contains the output from the build process.  In the case of NAntBuilder, this is the 
        /// redirected StdOut of the nant.exe process.
        /// </summary>
        [XmlIgnore]
        public string TaskOutput
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (ITaskResult result in taskResults)
                    sb.Append(result.Data);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the last integration.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        [XmlIgnore]
        public IntegrationSummary LastIntegration
        {
            get { return lastIntegration; }
        }

        /// <summary>
        /// Gets or sets the last integration status.	
        /// </summary>
        /// <value>The last integration status.</value>
        /// <remarks></remarks>
        public IntegrationStatus LastIntegrationStatus
        {
            get { return lastIntegration.Status; }
            set { lastIntegration = new IntegrationSummary(value, lastIntegration.Label, LastSuccessfulIntegrationLabel, lastIntegration.StartTime); }		// used only for loading IntegrationResult from state file - to be removed
        }

        /// <summary>
        /// Gets or sets the last successful integration label.	
        /// </summary>
        /// <value>The last successful integration label.</value>
        /// <remarks></remarks>
        public string LastSuccessfulIntegrationLabel
        {
            get { return (Succeeded || lastIntegration.LastSuccessfulIntegrationLabel == null) ? Label : lastIntegration.LastSuccessfulIntegrationLabel; }
            set { lastIntegration = new IntegrationSummary(lastIntegration.Status, lastIntegration.Label, value, lastIntegration.StartTime); } // used only for loading IntegrationResult from state file - to be removed
        }

        /// <summary>
        /// The list of users who have contributed modifications to a sequence of builds that has failed.
        /// </summary>
        public ArrayList FailureUsers
        {
            get { return failureUsers; }
            set { failureUsers = value; }
        }

        /// <summary>
        /// Gets the integration request.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        [XmlIgnore]
        public IntegrationRequest IntegrationRequest
        {
            get { return request; }
        }

        /// <summary>
        /// Gets the integration properties.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        [XmlIgnore]
        public IDictionary IntegrationProperties
        {
            get
            {
                IDictionary fullProps = new SortedList();
                fullProps[IntegrationPropertyNames.CCNetProject] = projectName;
                if (projectUrl != null) fullProps[IntegrationPropertyNames.CCNetProjectUrl] = projectUrl;
                fullProps[IntegrationPropertyNames.CCNetWorkingDirectory] = workingDirectory;
                fullProps[IntegrationPropertyNames.CCNetArtifactDirectory] = artifactDirectory;
                fullProps[IntegrationPropertyNames.CCNetIntegrationStatus] = Status;
                fullProps[IntegrationPropertyNames.CCNetLabel] = Label;
                fullProps[IntegrationPropertyNames.CCNetBuildCondition] = BuildCondition;
                fullProps[IntegrationPropertyNames.CCNetNumericLabel] = NumericLabel;
                fullProps[IntegrationPropertyNames.CCNetBuildDate] = StartTime.ToString("yyyy-MM-dd", null);
                fullProps[IntegrationPropertyNames.CCNetBuildTime] = StartTime.ToString("HH:mm:ss", null);
                fullProps[IntegrationPropertyNames.CCNetLastIntegrationStatus] = LastIntegrationStatus;
                fullProps[IntegrationPropertyNames.CCNetListenerFile] = BuildProgressInformation.ListenerFile;
                fullProps[IntegrationPropertyNames.CCNetFailureUsers] = FailureUsers;
                fullProps[IntegrationPropertyNames.CCNetModifyingUsers] = GetModifiers();
                fullProps[IntegrationPropertyNames.CCNetUser] = request.UserName;
                if (IntegrationRequest != null) fullProps[IntegrationPropertyNames.CCNetRequestSource] = IntegrationRequest.Source;
                return fullProps;
            }
        }

        /// <summary>
        /// Equalses the specified obj.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override bool Equals(object obj)
        {
            IntegrationResult other = obj as IntegrationResult;
            if (other == null)
                return false;

            return ProjectName == other.ProjectName &&
                   Status == other.Status &&
                   Label == other.Label &&
                   StartTime == other.StartTime;
        }

        /// <summary>
        /// Gets the hash code.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetHashCode()
        {
            return (ProjectName + Label + StartTime.Ticks).GetHashCode();
        }

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture,"Project: {0}, Status: {1}, Label: {2}, StartTime: {3}", ProjectName, Status, Label, StartTime);
        }

        /// <summary>
        /// Any error that occurred during the get modifications stage of source control.
        /// </summary>
        /// <remarks>
        /// If there is no error then this property will be null.
        /// </remarks>
        [XmlIgnore]
        public Exception SourceControlError
        {
            get { return sourceControlError; }
            set
            {
                sourceControlError = value;
                HasSourceControlError = (value != null);
            }
        }

        #region HasSourceControlError
        /// <summary>
        /// Gets or sets a value indicating whether there was a source control error.
        /// </summary>
        [XmlAttribute("scmError")]
        public bool HasSourceControlError { get; set; }
        #endregion

        #region LastBuildStatus
        /// <summary>
        /// The last status from a build that progressed pass any source control checks.
        /// </summary>
        [XmlElement("lastBuild")]
        public IntegrationStatus LastBuildStatus { get; set; }
        #endregion

        private ArrayList GetModifiers()
        {
            ArrayList Result = new ArrayList();

            foreach (Modification mod in Modifications)
            {
                if (!Result.Contains(mod.UserName))
                {
                    Result.Add(mod.UserName);
                }
            }
            return Result;
        }

        #region Clone()
        /// <summary>
        /// Clones this integration result.
        /// </summary>
        /// <returns>Returns a clone of the result.</returns>
        public virtual IIntegrationResult Clone()
        {
            var clone = new IntegrationResult(projectName, workingDirectory, artifactDirectory, request, lastIntegration);
            clone.projectUrl = projectUrl;
            clone.buildLogDirectory = buildLogDirectory;
            clone.parameters = new List<NameValuePair>(parameters);
            clone.label = label;
            clone.modifications = (Modification[])modifications.Clone();
            return clone;
        }
        #endregion

        #region Merge()
        /// <summary>
        /// Merges another result.
        /// </summary>
        /// <param name="value">The result to merge.</param>
        public virtual void Merge(IIntegrationResult value)
        {
            // Apply some rules to the status merging - basically apply a hierachy of status codes
            if ((value.Status == IntegrationStatus.Exception) || (this.status == IntegrationStatus.Unknown))
            {
                this.status = value.Status;
            }
            else if (((value.Status == IntegrationStatus.Failure) ||
                (value.Status == IntegrationStatus.Cancelled)) &&
                (this.status != IntegrationStatus.Exception))
            {
                this.status = value.Status;
            }

            // Merge the exceptions
            if (value.ExceptionResult != null)
            {
                if (this.exception != null)
                {
                    var mutliple = this.exception as MultipleIntegrationFailureException;
                    if (mutliple == null)
                    {
                        mutliple = new MultipleIntegrationFailureException(this.exception);
                        this.exception = mutliple;
                    }
                    mutliple.AddFailure(value.ExceptionResult);
                }
                else
                {
                    this.exception = value.ExceptionResult;
                }
            }

            // Merge the results
            foreach (ITaskResult result in value.TaskResults)
            {
                AddTaskResult(result);
            }
        }
        #endregion

        #region SourceControlData
        /// <summary>
        /// Extended source control data.
        /// </summary>
        /// <remarks>
        /// It is up to the individual source control providers to decide what to store in here.
        /// </remarks>
        [XmlElement("SourceControl")]
        public List<NameValuePair> SourceControlData
        {
            get { return sourceControlData; }
        }
        #endregion
    }
}
