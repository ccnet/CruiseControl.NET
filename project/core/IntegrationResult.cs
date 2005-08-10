using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Contains all the results of a project's integration.
	/// </summary>
	[Serializable]
	public class IntegrationResult : IIntegrationResult
	{
		public const string InitialLabel = "UNKNOWN";
		private string lastSuccessfulIntegrationLabel;
		private DateTime startTime;
		private DateTime endTime;
		private Modification[] modifications = new Modification[0];
		private Exception exception;
		private ArrayList taskResults = new ArrayList();
		private IDictionary properties = new SortedList();

		// Default constructor required for serialization
		public IntegrationResult()
		{
			BuildCondition = BuildCondition.NoBuild;
			Label = InitialLabel;
			Status = IntegrationStatus.Unknown;
			LastIntegrationStatus = IntegrationStatus.Unknown;
		}

		public IntegrationResult(string projectName, string workingDirectory) : this()
		{
			ProjectName = projectName;
			WorkingDirectory = workingDirectory;
		}

		public string ProjectName
		{
			get { return Convert(properties["ccnet.project"]); }
			set { properties["ccnet.project"] = value; }
		}

		public string ProjectUrl
		{
			get { return Convert(properties["ccnet.project.url"]); }
			set { properties["ccnet.project.url"] = value; }
		}

		public BuildCondition BuildCondition
		{
			get { return (BuildCondition) properties["ccnet.buildcondition"]; }
			set { properties["ccnet.buildcondition"] = value; }
		}

		public string Label
		{
			get { return Convert(properties["ccnet.label"]); }
			set { properties["ccnet.label"] = value; }
		}

		public string WorkingDirectory
		{
			get { return Convert(properties["ccnet.working.directory"]); }
			set { properties["ccnet.working.directory"] = value; }
		}

		public string ArtifactDirectory
		{
			get { return Convert(properties["ccnet.artifact.directory"]); }
			set { properties["ccnet.artifact.directory"] = value; }
		}

		public IntegrationStatus Status
		{
			get { return (IntegrationStatus) properties["ccnet.integration.status"]; }
			set { properties["ccnet.integration.status"] = value; }
		}

		public IntegrationStatus LastIntegrationStatus
		{
			get { return (IntegrationStatus) properties["ccnet.lastintegration.status"]; }
			set { properties["ccnet.lastintegration.status"] = value; }
		}

		public string LastSuccessfulIntegrationLabel
		{
			get { return (Succeeded || lastSuccessfulIntegrationLabel == null) ? Label : lastSuccessfulIntegrationLabel; }
			set { lastSuccessfulIntegrationLabel = value; }
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

		[XmlIgnore]
		public virtual Modification[] Modifications
		{
			get { return modifications; }
			set { modifications = value; }
		}

		public DateTime LastModificationDate
		{
			get
			{
				DateTime latestDate = DateTime.MinValue;
				if (Modifications.Length == 0) //TODO: why set the date to yesterday's date as a default
				{ //If there are no modifications then this should be set to the last modification date
					latestDate = DateTime.Now; // from the last integration (or 1/1/1980 if there is no previous integration).
					latestDate = latestDate.AddDays(-1.0);
				}

				foreach (Modification modification in Modifications)
				{
					latestDate = DateUtil.MaxDate(modification.ModifiedTime, latestDate);
				}
				return latestDate;
			}
		}

		public int LastChangeNumber
		{
			get
			{
				int lastChangeNumber = 0;
				foreach (Modification modification in modifications)
				{
					if (modification.ChangeNumber > lastChangeNumber)
						lastChangeNumber = modification.ChangeNumber;
				}
				return lastChangeNumber;
			}
		}

		/// <summary>
		/// Identifies if this is the first integration ever performed for this project (ie. there are no previous integrations).
		/// </summary>
		public bool IsInitial()
		{
			return (LastIntegrationStatus == IntegrationStatus.Unknown) && (Status == IntegrationStatus.Unknown);
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
		/// Gets a value indicating whether this integration is either successful or in an unknown state.
		/// </summary>
		public bool Working
		{
			get { return Status == IntegrationStatus.Unknown || Succeeded; }
		}

		/// <summary>
		/// Gets the time taken to perform the project's integration.
		/// </summary>
		public TimeSpan TotalIntegrationTime
		{
			get { return EndTime - StartTime; }
		}

		// Exceptions cannot be serialised because of permission attributes
		[XmlIgnore]
		public Exception ExceptionResult
		{
			get { return exception; }
			set
			{
				exception = value;
				if (exception != null)
				{
					Status = IntegrationStatus.Exception;
				}
			}
		}

		[XmlIgnore]
		public IList TaskResults
		{
			get { return taskResults; }
		}

		public void AddTaskResult(string result)
		{
			AddTaskResult(new DataTaskResult(result));
		}

		public void AddTaskResult(ITaskResult result)
		{
			taskResults.Add(result);
			if (! Failed) Status = result.Succeeded() ? IntegrationStatus.Success : IntegrationStatus.Failure;
		}

		public void MarkStartTime()
		{
			StartTime = DateTime.Now;
		}

		public void MarkEndTime()
		{
			EndTime = DateTime.Now;
		}

		public bool HasModifications()
		{
			return Modifications.Length > 0;
		}

		public override bool Equals(object obj)
		{
			IntegrationResult other = obj as IntegrationResult;
			if (other == null)
			{
				return false;
			}
			return this.ProjectName == other.ProjectName &&
				this.Status == other.Status &&
				this.Label == other.Label &&
				this.StartTime == other.StartTime;
		}

		public override int GetHashCode()
		{
			return (ProjectName + Label + StartTime.Ticks).GetHashCode();
		}

		public static IntegrationResult CreateInitialIntegrationResult(string project, string workingDirectory)
		{
			IntegrationResult result = new IntegrationResult(project, workingDirectory);
			result.StartTime = DateTime.Now.AddDays(-1);
			result.EndTime = DateTime.Now;
			return result;
		}

		/// <summary>
		/// Determines whether a build should run.  A build should run if there
		/// are modifications, and none have occurred within the modification
		/// delay.
		/// </summary>
		public bool ShouldRunBuild(int modificationDelaySeconds)
		{
			if (BuildCondition.ForceBuild == BuildCondition)
				return true;
			else
				return (HasModifications() && ! DoModificationsExistWithinModificationDelay(modificationDelaySeconds));
		}

		public string BaseFromArtifactsDirectory(string pathToBase)
		{
			if (StringUtil.IsBlank(pathToBase)) return ArtifactDirectory;
			return Path.Combine(ArtifactDirectory, pathToBase);
		}

		public string BaseFromWorkingDirectory(string pathToBase)
		{
			if (StringUtil.IsBlank(pathToBase)) return WorkingDirectory;
			return Path.Combine(WorkingDirectory, pathToBase);
		}

		/// <summary>
		/// Checks whether modifications occurred within the modification delay.  If the
		/// modification delay is not set (has a value of zero or less), this method
		/// will always return false.
		/// </summary>
		private bool DoModificationsExistWithinModificationDelay(int modificationDelaySeconds)
		{
			if (modificationDelaySeconds <= 0)
				return false;

			//TODO: can the last mod date (which is the time on the SCM) be compared with now (which is the time on the build machine)?
			TimeSpan diff = DateTime.Now - LastModificationDate;
			if (diff.TotalSeconds < modificationDelaySeconds)
			{
				Log.Info("Changes found within the modification delay");
				return true;
			}

			return false;
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
				StringBuilder builder = new StringBuilder();
				foreach (ITaskResult result in taskResults)
				{
					builder.Append(result.Data);
				}
				return builder.ToString();
			}
		}

		[XmlIgnore]
		public IDictionary IntegrationProperties
		{
			get
			{
				IDictionary fullProps = new Hashtable(properties);
				fullProps["ccnet.build.date"] = StartTime.ToShortDateString();
				fullProps["ccnet.build.time"] = StartTime.ToLongTimeString();
				return fullProps;
			}
		}

		
		private string Convert(object obj)
		{
			return (obj == null) ? null : obj.ToString();
		}

		public override string ToString()
		{
			return string.Format("Project: {0}, Status: {1}, Label: {2}, StartTime: {3}", ProjectName, Status, Label, StartTime);
		}
	}
}