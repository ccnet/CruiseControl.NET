using System;
using NUnit.Framework;

namespace tw.ccnet.core.util
{
	public class CustomAssertion : Assertion
	{
		public static void AssertContains(string search, string target)
		{
			Assert(String.Format("Search substring: {0} is not contained in target: {1}", search, target), target.IndexOf(search) > 0);
		}

		public static void AssertFalse(bool assert)
		{
			AssertEquals(false, assert);
		}

		public static void AssertFalse(string message, bool assert)
		{
			AssertEquals(message, false, assert);
		}
	}
}
