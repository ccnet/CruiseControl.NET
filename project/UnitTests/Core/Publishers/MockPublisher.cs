using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[ReflectorType("mockpublisher")]
	public class MockPublisher : PublisherBase
	{
		private bool _published = false;
		private IIntegrationResult _result;

		public bool Published
		{
			get { return _published; }
		}

		public IIntegrationResult Result
		{
			get { return _result; }
		}

		public override void PublishIntegrationResults(IIntegrationResult result)
		{
			_published = true;
			_result = result;
		}
	}
}
