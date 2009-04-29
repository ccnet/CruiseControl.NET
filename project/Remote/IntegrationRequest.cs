using System;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Remote
{
	[Serializable]
	public class IntegrationRequest
	{
		public static readonly IntegrationRequest NullRequest = new IntegrationRequest(BuildCondition.NoBuild, "NullRequest");
		private readonly BuildCondition buildCondition;
		private readonly string source;
        private readonly DateTime requestTime;
        private Dictionary<string, string> parameterValues = new Dictionary<string,string>();

		public IntegrationRequest(BuildCondition buildCondition, string source)
		{
			this.buildCondition = buildCondition;
			this.source = source;
            this.requestTime = DateTime.Now;
		}

		public BuildCondition BuildCondition
		{
			get { return buildCondition; }
		}

		public string Source
		{
			get { return source; }
		}

        public Dictionary<string, string> BuildValues
        {
            get { return parameterValues; }
            set { parameterValues = value; }
        }

        public DateTime RequestTime
        {
            get { return requestTime; }
        }

		public override bool Equals(object obj)
		{
			IntegrationRequest other = obj as IntegrationRequest;
			return other != null && other.BuildCondition == BuildCondition && other.Source == Source;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0} triggered a build ({1})", Source, BuildCondition);
		}

        /// <summary>
        /// Should the results of a failed source control exception be published?
        /// </summary>
        public bool PublishOnSourceControlException { get; set; }
	}
}