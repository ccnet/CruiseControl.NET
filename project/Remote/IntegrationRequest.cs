using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Remote
{
	[Serializable]
	public class IntegrationRequest
	{
		public static readonly IntegrationRequest NullRequest = new IntegrationRequest(BuildCondition.NoBuild, "NullRequest");
		private readonly BuildCondition buildCondition;
		private readonly string source;
        private bool publishOnSourceControlException;

		public IntegrationRequest(BuildCondition buildCondition, string source)
		{
			this.buildCondition = buildCondition;
			this.source = source;
		}

		public BuildCondition BuildCondition
		{
			get { return buildCondition; }
		}

		public string Source
		{
			get { return source; }
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
        public bool PublishOnSourceControlException
        {
            get { return publishOnSourceControlException; }
            set { publishOnSourceControlException = value; }
        }
	}
}