using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class CustomAssertion : Assertion
	{
		public static void AssertContains(string search, string target)
		{
			string message = string.Format("Search substring: {0} is not contained in target: {1}", search, target);
			Assert(message, target.IndexOf(search) >= 0);
		}

		public static void AssertStartsWith(string expected, string actual)
		{
			string message = string.Format("<{0}> does not start wth \n<{1}>", actual, expected);
			Assert(message, actual != null && actual.StartsWith(expected));
		}

		public static void AssertMatches(string pattern, string actual)
		{
			string message = string.Format("Pattern string <{0}> does not match \nactual string <{1}>", pattern, actual);
			Assert(message, Regex.IsMatch(actual, pattern));
		}

		public static void AssertFalse(bool assert)
		{
			Assert(!assert);
		}

		public static void AssertFalse(string message, bool assert)
		{
			Assert(message, !assert);
		}

		public static void AssertEquals(Type type, object obj)
		{
			if (type == null)
			{
				Assertion.AssertEquals(type, obj);
				return;
			}
			AssertNotNull(string.Format("object of expected type {0} is null", type.FullName), obj);
			Type actualType = (obj is Type) ? (Type)obj : obj.GetType();
			AssertEquals("object of the wrong type", type, actualType);
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
