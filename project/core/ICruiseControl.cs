using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface ICruiseControl : ICruiseServer
	{
		void WaitForExit();

		CruiseControlStatus Status { get; }

		IConfiguration Configuration { get; }
	}
}
