using System;
using System.Collections;

namespace tw.ccnet.core.test
{
	/// <summary>
	/// Mock implementation of ICruiseControl, for unit testing.
	/// </summary>
	public class MockCruiseControl : ICruiseControl
	{
		public MockCruiseControl()
		{
		}

		public void AddProject(IProject project)
		{
		}

		public IProject GetProject_ReturnValue = null;
		public string GetProject_projectName = null;
		public int GetProject_CallCount = 0;
		public IProject GetProject(string projectName)
		{
			GetProject_CallCount++;
			GetProject_projectName = projectName;
			return GetProject_ReturnValue;
		}

		public void Start()
		{
		}

		public void Stop()
		{
		}

		public void WaitForExit()
		{
		}

		public ICollection Projects
		{
			get
			{
				return null;
			}
		}

		public IList ProjectIntegrators
		{
			get
			{
				return null;
			}
		}

		public bool Stopped
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
	}
}
