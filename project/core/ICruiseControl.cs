using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICruiseControl : ICruiseServer
	{
		void WaitForExit();

		ICollection Projects { get;	}

		CruiseControlStatus Status { get; }

		IConfiguration Configuration { get; }
	}
}
