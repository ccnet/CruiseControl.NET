using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// A snapshot of the state of an item on a particular named queue at this moment in time.
	/// </summary>
	[Serializable]
	public class QueuedRequestSnapshot
	{
		private string projectName;
	    private ProjectActivity activity;
        private DateTime requestTime;

		public QueuedRequestSnapshot(string projectName, ProjectActivity activity)
            : this(projectName, activity, DateTime.MinValue) { }

        public QueuedRequestSnapshot(string projectName, ProjectActivity activity, DateTime requestTime)
		{
			this.projectName = projectName;
            this.activity = activity;
            this.requestTime = requestTime;
		}

		public string ProjectName
		{
			get { return projectName; }
		}

	    public ProjectActivity Activity
	    {
	        get { return activity; }
	    }

        public DateTime RequestTime
        {
            get { return requestTime; }
            set { requestTime = value; }
        }
	}
}
