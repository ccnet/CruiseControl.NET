using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
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
