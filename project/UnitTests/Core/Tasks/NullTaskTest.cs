using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class NullTasklTest
	{
		private NullTask task;

		[SetUp]
		public void Setup()
		{
			task = new NullTask();
		}

		[Test]
		public void ShouldReturnUnchangedResult()
		{
			DynamicMock resultMock = new DynamicMock(typeof(IIntegrationResult));
			resultMock.Strict = true;

			task.Run((IIntegrationResult) resultMock.MockInstance);

			resultMock.Verify();
		}
	}
}
