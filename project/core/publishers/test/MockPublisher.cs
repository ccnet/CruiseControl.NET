using System;
using System.Collections;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	[ReflectorType("mockpublisher")]
	public class MockPublisher : PublisherBase
	{
		private bool _published = false;
		private IntegrationResult _result;

		public bool Published
		{
			get { return _published; }
		}

		public IntegrationResult Result
		{
			get { return _result; }
		}

		public override void PublishIntegrationResults(IProject project, IntegrationResult result)
		{
			_published = true;
			_result = result;
		}
	}
}
