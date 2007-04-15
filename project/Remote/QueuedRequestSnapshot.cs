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

		public QueuedRequestSnapshot(string projectName)
		{
			this.projectName = projectName;
		}

		public string ProjectName
		{
			get { return projectName; }
		}
	}
}
