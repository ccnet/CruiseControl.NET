using System;
using NUnit.Framework;

namespace tw.ccnet.acceptance
{
	/// <summary>
	/// Summary description for NUnitTest.
	/// </summary>
	public class NUnitTest : TestCase
	{
		public NUnitTest(string name) : base (name)
		{
		}

		public void TestFail()
		{
			Fail("We want to test failures");
		}

		public void TestError()
		{
			AssertEquals("false should be false", false,false);
		}

		public void TestPassPlease()
		{
			Assert("true if you don't mind", true);
		}

	}
}
