using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[ReflectorType("mockpublisher")]
	public class MockPublisher : ITask
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

		public void Run(IIntegrationResult result)
		{
			_published = true;
			_result = result;
		}
	}
}
