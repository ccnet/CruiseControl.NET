using System;
using System.Collections;
using Exortech.NetReflector;

namespace tw.ccnet.core.publishers.test
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
