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

		public QueuedRequestSnapshot(string projectName, ProjectActivity activity)
		{
			this.projectName = projectName;
            this.activity = activity;
		}

		public string ProjectName
		{
			get { return projectName; }
		}

	    public ProjectActivity Activity
	    {
	        get { return activity; }
	    }
	}
}
