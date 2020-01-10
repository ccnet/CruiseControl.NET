using System.Threading;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils
{
	public class LatchHelper {
		private ManualResetEvent latch = new ManualResetEvent(false);
		public void WaitForSignal() {
			bool signalled = latch.WaitOne(2000, false);
			if(!signalled) {
				throw new CruiseControlException("Latch has not been signalled before the timeout expired!");
			}
		}
		public void SetLatch() {
			latch.Set();
		}
		public void ResetLatch() {
			latch.Reset();
		}
	}
}