
using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class IntegrationSummary
	{
		public static readonly IntegrationSummary Initial = new IntegrationSummary(IntegrationStatus.Unknown, IntegrationResult.InitialLabel, IntegrationResult.InitialLabel, DateTime.MinValue);
		private IntegrationStatus status;
		private string label;
		private string lastSuccessfulIntegrationLabel;
		private DateTime startTime;
        private ArrayList failureUsers = new ArrayList();

		public IntegrationSummary(IntegrationStatus status, string label, string lastSuccessfulIntegrationLabel, DateTime startTime)
		{
			this.status = status;
			this.label = label;
			this.lastSuccessfulIntegrationLabel = lastSuccessfulIntegrationLabel;
			this.startTime = startTime;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (obj.GetType() != GetType()) return false;

			IntegrationSummary other = (IntegrationSummary) obj;
			return other.status.Equals(status) && other.label == label;
		}

		public override int GetHashCode()
		{
			return label.GetHashCode();
		}


		public override string ToString()
		{
			return string.Format("Status: {0}, Label: {1}", status, label);
		}

		public string Label
		{
			get { return label; }
		}

		public IntegrationStatus Status
		{
			get { return status; }
		}

		public string LastSuccessfulIntegrationLabel
		{
			get { return lastSuccessfulIntegrationLabel; }
		}

		public DateTime StartTime
		{
			get { return startTime; }
		}

		public bool IsInitial()
		{
			return label == IntegrationResult.InitialLabel;
		}

        public ArrayList FailureUsers
        {
            get { return failureUsers; }
            set { failureUsers = value; }
        }
	}
}