using NMock;
using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	public class ConfigurationStub : IConfiguration
	{
		private ProjectIntegratorList _integrators = new ProjectIntegratorList();
		private ArrayList _mockIntegrators = new ArrayList();

		public ConfigurationStub(int integrators)
		{
			for (int i = 0; i < integrators; i++)
			{
				IMock mockIntegrator = new DynamicMock(typeof(IProjectIntegrator));
				mockIntegrator.ExpectAndReturn("Name", "project" + (i + 1));
				_mockIntegrators.Add(mockIntegrator);
				_integrators.Add(mockIntegrator.MockInstance as IProjectIntegrator);
			}
		}

		public IProjectList Projects { get { return null; } }
		public IProjectIntegratorList Integrators 
		{ 
			get { return _integrators; }
		}

		public IMock GetIntegratorMock(int index)
		{
			return (IMock)_mockIntegrators[index];
		}

		public void Verify()
		{
			foreach (IMock mock in _mockIntegrators)
			{
				mock.Verify();
			}
		}
	}
}
