using System;
using System.Collections;

namespace tw.ccnet.core
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICruiseControl
	{
		void AddProject(IProject project);
		IProject GetProject(string projectName);
		
		void Start();
		void Stop();
		void WaitForExit();

		ICollection Projects
		{
			get;
		}

		IList ProjectIntegrators
		{
			get;
		}

		bool Stopped
		{
			get;
//			set;
		}
	}
}
