using System;
using System.Collections;
using tw.ccnet.remote;

namespace tw.ccnet.core
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
