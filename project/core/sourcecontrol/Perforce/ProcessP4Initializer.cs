using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
	public class ProcessP4Initializer : IP4Initializer
	{
		private readonly ProcessExecutor executor;

		public ProcessP4Initializer(ProcessExecutor executor)
		{
			this.executor = executor;
		}

		public void Initialize(P4 p4)
		{
			throw new NotImplementedException();
		}
	}
}
