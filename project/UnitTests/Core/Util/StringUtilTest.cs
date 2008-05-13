using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class StringUtilTest : CustomAssertion
	{
		[Test]
		public void TestContains()
		{
			string input = "abcde";

			Assert.IsTrue(StringUtil.Contains(input, "c"));
			Assert.IsFalse(StringUtil.Contains(input, "C"));
			Assert.IsFalse(StringUtil.Contains(input, "x"));
		}

		[Test]
		public void TestEqualsIgnoreCase()
		{
			string lower = "abcde";
			string upper = "ABCDE";
			string mixed = "aBcDe";
			string mixed2 = "AbCdE";

			Assert.IsTrue(StringUtil.EqualsIgnoreCase(lower, upper));
			Assert.IsTrue(StringUtil.EqualsIgnoreCase(lower, mixed));
			Assert.IsTrue(StringUtil.EqualsIgnoreCase(lower, mixed2));
			Assert.IsTrue(StringUtil.EqualsIgnoreCase(upper, mixed));
			Assert.IsTrue(StringUtil.EqualsIgnoreCase(upper, mixed2));
		}

		[Test]
		public void TestIsBlank()
		{
			Assert.IsTrue(StringUtil.IsBlank(null));
			Assert.IsTrue(StringUtil.IsBlank(string.Empty));
			Assert.IsFalse(StringUtil.IsBlank(" "));
			Assert.IsFalse(StringUtil.IsBlank("foo"));
		}

		[Test]
		public void TestIsWhitespace()
		{
			Assert.IsTrue(StringUtil.IsWhitespace(null));
			Assert.IsTrue(StringUtil.IsWhitespace(string.Empty));
			Assert.IsTrue(StringUtil.IsWhitespace(" "));
			Assert.IsFalse(StringUtil.IsWhitespace("foo"));
		}

		[Test]
		public void TestJoinUnique()
		{
			string[] x = new string[] {"a","b","b"};
			string[] y = new string[] {"n","m","b"};
			string actual = StringUtil.JoinUnique(": ", x, y);
			Assert.AreEqual("a: b: m: n", actual);
		}

		[Test]
		public void TestJoin()
		{
			string[] input = new string[] { "a", "b", "c" };
			
			string actual = StringUtil.JoinUnique(": ", input);
			Assert.AreEqual("a: b: c", actual);
		}

		[Test]
		public void TestGenerateHashCode()
		{
			string a = "a";
			string b = "b";
			Assert.AreEqual(
				a.GetHashCode() + b.GetHashCode(), StringUtil.GenerateHashCode(a, b));

			Assert.AreEqual(
				a.GetHashCode() + b.GetHashCode(), StringUtil.GenerateHashCode(a, b, null));
		}

		[Test]
		public void TestLastWord()
		{			
			string s = "this is a sentence without punctuation\n";
			Assert.AreEqual("punctuation", StringUtil.LastWord(s));
			
			s = "this is a sentence with punctuation.\n";
			Assert.AreEqual("punctuation", StringUtil.LastWord(s));

			s = "thisisoneword";
			Assert.AreEqual("thisisoneword", StringUtil.LastWord(s));

			s = "";
			Assert.AreEqual(String.Empty, StringUtil.LastWord(s));
			
			s = null;
			Assert.IsNull(StringUtil.LastWord(s));
		}

		[Test]
		public void TestLastWord_withSeps()
		{
			string s = "this#is$my%crazy*sentence!!!";
			Assert.AreEqual("sentence", StringUtil.LastWord(s, "#$%*!"));

			s = "!@#$%";
			Assert.AreEqual(String.Empty, StringUtil.LastWord(s, s));
		}

		[Test]
		public void TestInsert()
		{
			string[] original = new String[]{"a","b","c","d"};
			string[] augmented = StringUtil.Insert(original, "x", 2);
			Assert.AreEqual("a", augmented[0]);
			Assert.AreEqual("b", augmented[1]);
			Assert.AreEqual("x", augmented[2]);
			Assert.AreEqual("c", augmented[3]);
			Assert.AreEqual("d", augmented[4]);
		}

		[Test]
		public void TestStrip()
		{
			string input = "strip the word monkey and chinchilla or there's trouble";
			string actual = StringUtil.Strip(input, "monkey ", "chinchilla ");
			Assert.AreEqual("strip the word and or there's trouble", actual);

			input = "hey la monkey monkey chinchilla banana";
			actual = StringUtil.Strip(input, "monkey ", "chinchilla ");
			Assert.AreEqual("hey la banana", actual);
		}

		[Test]
		public void TestStripQuotes()
		{
			string input = "\"C:\foo\"";
			string actual = StringUtil.StripQuotes(input);

			Assert.AreEqual("C:\foo", actual);			
		}

        [Test]
        public void TestRemoveInvalidCharactersFromFileName()
        {
            string BadFileName = "Go Stand ? in the <*/:*?> corner.txt";
            string actual = StringUtil.RemoveInvalidCharactersFromFileName(BadFileName);

            Assert.AreEqual("Go Stand  in the  corner.txt", actual);
        }

		[Test]
		public void TestRemoveNulls()
		{
			Assert.AreEqual(StringUtil.RemoveNulls("\0\0hello"), "hello");
			Assert.AreEqual(StringUtil.RemoveNulls("\0\0hello\0\0"), "hello");
		}

		[Test]
		public void TestAutoDoubleQuoteString()
		{
			string nonQuotedString = "foo";
			string nonQuotedStringWithSpaces = "f o o";
			string quotedString = "\"foo\"";
			string quotedStringWithSpaces = "\"f o o\"";			
			
			Assert.AreEqual(StringUtil.AutoDoubleQuoteString(nonQuotedString), nonQuotedString);
			Assert.AreEqual(StringUtil.AutoDoubleQuoteString(quotedString), quotedString);
			Assert.AreEqual(StringUtil.AutoDoubleQuoteString(nonQuotedStringWithSpaces), quotedStringWithSpaces);
			Assert.AreEqual(StringUtil.AutoDoubleQuoteString(quotedStringWithSpaces), quotedStringWithSpaces);
		}

		[Test]
		public void TestRemoveTrailingPathDelimiter()
		{			
			string actual = "foo";
			string trailingSeparator = "foo" + Path.DirectorySeparatorChar;
			string trailingSeparator2 = "foo" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar;

			Assert.AreEqual(StringUtil.RemoveTrailingPathDelimeter(trailingSeparator), actual);
			Assert.AreEqual(StringUtil.RemoveTrailingPathDelimeter(trailingSeparator2), actual);
		}

		[Test]
		public void TestIntegrationPropertyToString()
		{
			int integer = 5;
			string integerString = integer.ToString();			
			BuildCondition buildCondition = BuildCondition.ForceBuild;
			IntegrationStatus integrationStatus = IntegrationStatus.Success;			
			ArrayList arrayList = new ArrayList();
			arrayList.Add("foo");
			arrayList.Add("5");
			arrayList.Add("bar");
			
			string customDelimiter = "-";
			string defaultConvertedArrayList = "foo" + StringUtil.DEFAULT_DELIMITER + "5" + StringUtil.DEFAULT_DELIMITER + "bar";
			string customConvertedArrayList = "foo" + customDelimiter + "5" + customDelimiter + "bar";
			
			Assert.AreEqual(StringUtil.IntegrationPropertyToString(integer), integerString);
			Assert.AreEqual(StringUtil.IntegrationPropertyToString(integerString), integerString);
			Assert.AreEqual(StringUtil.IntegrationPropertyToString(buildCondition), buildCondition.ToString());
			Assert.AreEqual(StringUtil.IntegrationPropertyToString(integrationStatus), integrationStatus.ToString());
			Assert.AreEqual(StringUtil.IntegrationPropertyToString(arrayList), defaultConvertedArrayList);

			Assert.AreEqual(StringUtil.IntegrationPropertyToString(integer, customDelimiter), integerString);
			Assert.AreEqual(StringUtil.IntegrationPropertyToString(integerString, customDelimiter), integerString);
			Assert.AreEqual(StringUtil.IntegrationPropertyToString(buildCondition, customDelimiter), buildCondition.ToString());
			Assert.AreEqual(StringUtil.IntegrationPropertyToString(integrationStatus, customDelimiter), integrationStatus.ToString());
			Assert.AreEqual(StringUtil.IntegrationPropertyToString(arrayList, customDelimiter), customConvertedArrayList);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestIntegrationPropertyToStringWithUnsupportedType()
		{
			StringUtil.IntegrationPropertyToString(new object());
			StringUtil.IntegrationPropertyToString(new object(), "-");
		}
	}
}