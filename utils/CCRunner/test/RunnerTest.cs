using System;
using NUnit.Framework;
using NMock;
using tw.ccnet.remote;

namespace CCRunner.test
{
	[TestFixture]
	public class RunnerTest : Assertion
	{
		private IMock _mockManager;

		[Test]
		public void SendScheduleToCruiseManager()
		{
			ICruiseManager manager = CreateMockManager();
			manager.Run("myProject", new Schedule());

		}

		private ICruiseManager CreateMockManager()
		{
			_mockManager = new DynamicMock(typeof(ICruiseManager));
			return (ICruiseManager)_mockManager.MockInstance;
		}

	}
}
