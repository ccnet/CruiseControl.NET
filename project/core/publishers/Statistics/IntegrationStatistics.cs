using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	public class IntegrationStatistics
	{
		private string buildLabel;
		private IntegrationStatus integrationStatus;
		private TimeSpan timeSpan;
		private string projectName;

		public string BuildLabel
		{
			get { return buildLabel; }
			set { buildLabel = value; }
		}

		public IntegrationStatus IntegrationStatus
		{
			get { return integrationStatus; }
			set { integrationStatus = value; }
		}

		public TimeSpan IntegrationTime
		{
			get { return timeSpan; }
			set { timeSpan = value; }
		}

		public string ProjectName
		{
			get { return projectName; }
			set { projectName = value; }
		}

		public Hashtable ConfiguredStats
		{
			get { throw new NotImplementedException(); }
		}
	}
}