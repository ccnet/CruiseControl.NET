using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests
{
	public class CustomAssertion
	{
		public static void AssertContains(string search, string target)
		{
			string message = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Search substring: {0} is not contained in target: {1}", search, target);
			Assert.IsTrue(target.IndexOf(search) >= 0, message);
		}

		public static void AssertNotContains(string search, string target)
		{
			string message = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Search substring: {0} should not be contained in target: {1}", search, target);
			Assert.IsFalse(target.IndexOf(search) >= 0, message);
		}

		public static void AssertContainsInArray(object search, object[] target)
		{
			foreach (object a in target)
			{
				if (a.Equals(search)) return;
			}
			string message = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Did not find {0} in the array", search);
			Assert.Fail(message);
		}

		public static void AssertStartsWith(string expected, string actual)
		{
			string message = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<{0}> does not start wth \n<{1}>", actual, expected);
			Assert.IsTrue(actual != null && actual.StartsWith(expected), message);
		}

		public static void AssertMatches(string pattern, string actual)
		{
			string message = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Pattern string <{0}> does not match \nactual string <{1}>", pattern, actual);
			Assert.IsTrue(Regex.IsMatch(actual, pattern), message);
		}

		public static void AssertMatchCount(int expectedCount, string pattern, string actual)
		{
			int actualCount = Regex.Matches(actual, pattern).Count;
			string message = string.Format(System.Globalization.CultureInfo.CurrentCulture, 
			                               "Actual string <{0}> \nmatches pattern <{1}> \n {2} times, expected was {3} times", 
			                               actual, pattern, actualCount, expectedCount);
			Assert.AreEqual(expectedCount, actualCount, message);
		}

		public static void AssertFalse(bool assert)
		{
			Assert.IsTrue(!assert);
		}

		public static void AssertFalse(string message, bool assert)
		{
			Assert.IsTrue(!assert, message);
		}

		/// <summary>
		/// Verifies that two objects are of the same Type. Objects are the same Type if the actual 
		/// object is the expected Type, or an instance of the expected Type.
		/// </summary>
		/// <param name="expectedType"></param>
		/// <param name="actual"></param>
		public static void AssertEquals(Type expectedType, object actual)
		{
			if (expectedType == null)
			{
				Assert.AreEqual(expectedType, actual);
				return;
			}
			Assert.IsNotNull(actual, string.Format(System.Globalization.CultureInfo.CurrentCulture,"object of expected type {0} is null", expectedType.FullName));
			Type actualType = (actual is Type) ? (Type) actual : actual.GetType();
			Assert.AreEqual(expectedType, actualType, "object of the wrong type");
		}

		public static void AssertNotEquals(object expected, object actual)
		{
			string message = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Values ({0}) and ({1}) should not be equal", expected, actual);
			Assert.IsTrue(!expected.Equals(actual), message);
			Assert.IsTrue(!actual.Equals(expected), message);
		}

		public static void AssertApproximatelyEqual(double expected, double actual, double tolerance)
		{
			AssertApproximatelyEqual(string.Empty, expected, actual, tolerance);
		}

		public static void AssertApproximatelyEqual(string message, double expected, double actual, double tolerance)
		{
			string expectation = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Expected {0}, but was {1}", expected, actual);
			Assert.IsTrue(Math.Abs(expected - actual) < tolerance, message + expectation);
		}

		public static void AssertEqualArrays(Array expected, Array actual)
		{
			Assert.AreEqual(expected.Length, actual.Length, "Arrays should have same length");

			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected.GetValue(i), actual.GetValue(i), "Comparing array index " + i);
			}
		}

		public static void AssertXPathExists(string xml, string xpath)
		{
			Assert.IsTrue(SelectNodeIterator(xpath, xml).Count > 0,
			              string.Format(System.Globalization.CultureInfo.CurrentCulture,"Unable to locate xpath expression <{0}>\n\t in xml <{1}>", xpath, xml));
		}

		public static void AssertXPathNodeValue(string expectedValue, string xml, string xpath)
		{
			string actual = SelectNodeIterator(xpath, xml).Current.Value;
			Assert.AreEqual(expectedValue, actual,
			                string.Format(System.Globalization.CultureInfo.CurrentCulture,"Expected value <{0}> does not equal <{1}>\n\t in xml <{2}>", xpath, actual, xml));
		}

		private static XPathNodeIterator SelectNodeIterator(string xpath, string xml)
		{
			try
			{
				XPathDocument document = new XPathDocument(new XmlTextReader(new StringReader(xml)));
				return document.CreateNavigator().Select(xpath);
			}
			catch (Exception e)
			{
				throw new AssertionException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Unable to parse xml <{0}> or xpath expression <{1}>\n\t{2}", xml, xpath, e), e);
			}
		}
	}
}