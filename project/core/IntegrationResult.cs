using System;
using System.Collections;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Contains all the results of a project's integration.
    /// </summary>
    [Serializable]
    public class IntegrationResult
    {
        private Modification[] _modifications = new Modification[0];
        private string _output;
        private Exception _exception;
        private ArrayList _taskResults = new ArrayList();

        // Default constructor required for serialization
        public IntegrationResult()
        {
        }

        public IntegrationResult(string projectName)
        {
            ProjectName = projectName;
        }

        public string ProjectName;
		public BuildCondition BuildCondition;
        public IntegrationStatus Status = IntegrationStatus.Unknown;
		public IntegrationStatus LastIntegrationStatus = IntegrationStatus.Unknown;
        public string Label;

        /// <summary>
        /// Gets and sets the date and time at which the integration commenced.
        /// </summary>
        public DateTime StartTime;

        /// <summary>
        /// Gets and sets the date and time at which the integration was completed.
        /// </summary>
        public DateTime EndTime;

		[XmlIgnore]
        public virtual Modification[] Modifications
        {
            get { return _modifications; }
            set { _modifications = value; }
        }

        public DateTime LastModificationDate
        {
            get
            {
                DateTime latestDate = DateTime.MinValue;
                if (Modifications.Length == 0)			//TODO: why set the date to yesterday's date as a default
                {										//If there are no modifications then this should be set to the last modification date
                    latestDate = DateTime.Now;			// from the last integration (or 1/1/1980 if there is no previous integration).
                    latestDate = latestDate.AddDays(-1.0);
                }

                foreach (Modification modification in Modifications)
                {
                    latestDate = DateUtil.MaxDate(modification.ModifiedTime, latestDate);
                }
                return latestDate;
            }
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

        /// <summary>
        /// Contains the output from the build process.  In the case of NAntBuilder, this is the 
        /// redirected StdOut of the nant.exe process.
        /// </summary>
        [XmlIgnore] 
		public virtual string Output
        {
            get { return _output; }
            set { _output = value; }
        }

        [XmlIgnore] // Exceptions cannot be serialised because of permission attributes
        public Exception ExceptionResult
        {
            get { return _exception; }
            set
            {
                _exception = value;
                if (_exception != null)
                {
                    Status = IntegrationStatus.Exception;
                }
            }
        }

		[XmlIgnore]
		public IList TaskResults
		{
			get { return _taskResults; }
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
            return this.ProjectName == other.ProjectName && this.Status == other.Status && this.Label == other.Label && this.StartTime == other.StartTime && this.EndTime == other.EndTime;
        }

        public override int GetHashCode()
        {
            return (ProjectName + Label + StartTime.Ticks).GetHashCode();
        }

    	public static IntegrationResult Initial
    	{
			get
			{
				IntegrationResult result = new IntegrationResult();
				result.StartTime = DateTime.Now.AddDays(-1);
				result.EndTime = DateTime.Now;
				return result;
			}
    	}

    	public bool IsInitial()
    	{
    		return ProjectName == null;
    	}
    }
}