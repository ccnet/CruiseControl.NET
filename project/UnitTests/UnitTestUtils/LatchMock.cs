using System;
using System.Collections;
using System.Threading;
using NMock;

namespace ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils
{
	public class LatchMock : DynamicMock
	{
		private ManualResetEvent latch = new ManualResetEvent(false);
		private ArrayList signalMethods = new ArrayList();
		private VerifyException ex;

		public LatchMock(Type type) : base(type)
		{}

		public void SetupResultAndSignal(string methodName, object returnVal, params Type[] argTypes)
		{
			base.SetupResult(methodName, returnVal, argTypes);
			signalMethods.Add(methodName);
		}

		public void ExpectAndSignal(string methodName, params object[] args)
		{
			base.Expect(methodName, args);
			signalMethods.Add(methodName);
		}

		public void ExpectAndReturnAndSignal(string methodName, object result, params object[] args)
		{
			base.ExpectAndReturn(methodName, result, args);
			signalMethods.Add(methodName);
		}

		public void ExpectAndThrowAndSignal(string methodName, Exception e, params object[] args)
		{
			base.ExpectAndThrow(methodName, e, args);
			signalMethods.Add(methodName);
		}

		public override object Invoke(string methodName, params object[] args)
		{
			try
			{
				return base.Invoke(methodName, args);
			}
			catch (VerifyException verifyException)
			{
				ex = verifyException;
				throw;
			}
			finally
			{
				if (signalMethods.Contains(methodName))
				{
					latch.Set();
				}
			}
		}

		public void WaitForSignal()
		{
			bool signalled = latch.WaitOne(2000, false);
			if (! signalled)
			{
				throw new Exception("Latch has not been signalled before the timeout expired!");
			}
			if (ex != null)
			{
				throw ex;
			}
		}

		public void ResetLatch()
		{
			latch.Reset();
		}
	}
}