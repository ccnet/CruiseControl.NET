using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// A snapshot of the state of an item on a particular named queue at this moment in time.
	/// Note we are copying a subset of values rather than adding the original objects just to minimize the serialized message size.
	/// </summary>
	[Serializable]
	public class QueuedItemSnapshot
	{
		private string queueName;
		private string projectName;
		private int queuePriority;
		private BuildCondition buildCondition;
		private string requestSource;

		public QueuedItemSnapshot(string queueName, string projectName, int queuePriority, BuildCondition buildCondition, string requestSource)
		{
			this.queueName = queueName;
			this.projectName = projectName;
			this.queuePriority = queuePriority;
			this.buildCondition = buildCondition;
			this.requestSource = requestSource;
		}

		public string QueueName
		{
			get { return queueName; }
		}

		public string ProjectName
		{
			get { return projectName; }
		}

		public int QueuePriority
		{
			get { return queuePriority; }
		}

		public BuildCondition BuildCondition
		{
			get { return buildCondition; }
		}

		public string RequestSource
		{
			get { return requestSource; }
		}
	}
}
