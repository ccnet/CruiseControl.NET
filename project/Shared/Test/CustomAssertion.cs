using System;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Shared.Test
{
	public class CustomAssertion : Assertion
	{
		public static void AssertContains(string search, string target)
		{
			string message = string.Format("Search substring: {0} is not contained in target: {1}", search, target);
			Assert(message, target.IndexOf(search) > 0);
		}

		public static void AssertFalse(bool assert)
		{
			Assert(!assert);
		}

		public static void AssertFalse(string message, bool assert)
		{
			Assert(message, !assert);
		}

		public static void AssertNotEquals(object expected, object actual)
		{
			string message = string.Format("Values ({0}) and ({1}) should not be equal", expected, actual);
			Assert(message, !expected.Equals(actual));
			Assert(message, !actual.Equals(expected));
		}

		public static void AssertApproximatelyEqual(double expected, double actual, double tolerance)
		{
			AssertApproximatelyEqual(string.Empty, expected, actual, tolerance);
		}

		public static void AssertApproximatelyEqual(string message, double expected, double actual, double tolerance)
		{
			string expectation = string.Format("Expected {0}, but was {1}", expected, actual);
			Assert(message + expectation, Math.Abs(expected - actual) < tolerance);
		}

		public static void AssertEqualArrays(Array expected, Array actual)
		{
			AssertEquals("Arrays should have same length", actual.Length, expected.Length);
			
			for (int i=0; i<expected.Length; i++)
			{
				AssertEquals("Comparing array index " + i, expected.GetValue(i), actual.GetValue(i));
			}
		}
	}
}
