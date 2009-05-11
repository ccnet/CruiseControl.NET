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
		public void TestEqualsIgnoreCase()
		{
			const string lower = "abcde";
			const string upper = "ABCDE";
			const string mixed = "aBcDe";
			const string mixed2 = "AbCdE";

			Assert.IsTrue(StringUtil.EqualsIgnoreCase(lower, upper));
			Assert.IsTrue(StringUtil.EqualsIgnoreCase(lower, mixed));
			Assert.IsTrue(StringUtil.EqualsIgnoreCase(lower, mixed2));
			Assert.IsTrue(StringUtil.EqualsIgnoreCase(upper, mixed));
			Assert.IsTrue(StringUtil.EqualsIgnoreCase(upper, mixed2));
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
		public void TestGenerateHashCode()
		{
			const string a = "a";
			const string b = "b";
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
			const string input = "\"C:\foo\"";
			string actual = StringUtil.StripQuotes(input);

			Assert.AreEqual("C:\foo", actual);			
		}

        [Test]
        public void TestRemoveInvalidCharactersFromFileName()
        {
            const string BadFileName = "Go Stand ? in the <*/:*?> corner.txt";
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
			const string nonQuotedString = "foo";
			const string nonQuotedStringWithSpaces = "f o o";
			const string quotedString = "\"foo\"";
			const string quotedStringWithSpaces = "\"f o o\"";			
			
			Assert.AreEqual(StringUtil.AutoDoubleQuoteString(nonQuotedString), nonQuotedString);
			Assert.AreEqual(StringUtil.AutoDoubleQuoteString(quotedString), quotedString);
			Assert.AreEqual(StringUtil.AutoDoubleQuoteString(nonQuotedStringWithSpaces), quotedStringWithSpaces);
			Assert.AreEqual(StringUtil.AutoDoubleQuoteString(quotedStringWithSpaces), quotedStringWithSpaces);
		}

		[Test]
		public void TestRemoveTrailingPathDelimiter()
		{			
			const string actual = "foo";
			string trailingSeparator = "foo" + Path.DirectorySeparatorChar;
			string trailingSeparator2 = "foo" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar;

			Assert.AreEqual(StringUtil.RemoveTrailingPathDelimeter(trailingSeparator), actual);
			Assert.AreEqual(StringUtil.RemoveTrailingPathDelimeter(trailingSeparator2), actual);
		}

		[Test]
		public void TestIntegrationPropertyToString()
		{
			const int integer = 5;
			string integerString = integer.ToString();			
			const BuildCondition buildCondition = BuildCondition.ForceBuild;			
			const IntegrationStatus integrationStatus = IntegrationStatus.Success;			
			
			ArrayList arrayList = new ArrayList();
			arrayList.Add("foo");
			arrayList.Add("5");
			arrayList.Add("bar");			
			
			const string customDelimiter = "-";
			const string defaultConvertedArrayList = "\"foo" + StringUtil.DEFAULT_DELIMITER + "5" + StringUtil.DEFAULT_DELIMITER + "bar\"";
			const string customConvertedArrayList = "\"foo" + customDelimiter + "5" + customDelimiter + "bar\"";
			
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

			Assert.AreEqual(StringUtil.IntegrationPropertyToString(null), null);

			ArrayList arrayList2 = new ArrayList();
			arrayList2.Add("foo");
			Assert.AreEqual(StringUtil.IntegrationPropertyToString(arrayList2), "foo");
			Assert.AreEqual(StringUtil.IntegrationPropertyToString(arrayList2, customDelimiter), "foo");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestIntegrationPropertyToStringWithUnsupportedType()
		{
			StringUtil.IntegrationPropertyToString(new object());
			StringUtil.IntegrationPropertyToString(new object(), "-");
		}

		[Test]
		public void TestMakeBuildResult()
		{
			Assert.AreEqual(Environment.NewLine + "<buildresults>" + Environment.NewLine + "  <message>"
				+ "foo" + "</message>" + Environment.NewLine + "</buildresults>"
				+ Environment.NewLine, StringUtil.MakeBuildResult("foo", ""));
			Assert.AreEqual(Environment.NewLine + "<buildresults>" + Environment.NewLine + "  <message level=\"Error\">"
				+ "foo" + "</message>" + Environment.NewLine + "</buildresults>"
				+ Environment.NewLine, StringUtil.MakeBuildResult("foo", "Error"));
			Assert.AreEqual("", StringUtil.MakeBuildResult("", ""));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestMakeBuildResultThrowsArgumentNullException()
		{
			StringUtil.MakeBuildResult(null, "");
		}

		[Test]
		public void TestArrayToNewLineSeparatedString()
		{
			Assert.AreEqual("foo", StringUtil.ArrayToNewLineSeparatedString(new string[] { "foo" }));
			Assert.AreEqual("foo" + Environment.NewLine + "bar", StringUtil.ArrayToNewLineSeparatedString(new string[] {"foo", "bar"}));
			Assert.AreEqual("", StringUtil.ArrayToNewLineSeparatedString(new string[0]));
			Assert.AreEqual("", StringUtil.ArrayToNewLineSeparatedString(new string[1] { "" }));
			Assert.AreEqual("", StringUtil.ArrayToNewLineSeparatedString(new string[1] { null }));
		}

		[Test]
		public void TestNewLineSeparatedStringToArray()
		{
			Assert.AreEqual(new string[] { "foo" }, StringUtil.NewLineSeparatedStringToArray("foo"));
			Assert.AreEqual(new string[] { "foo", "bar" }, StringUtil.NewLineSeparatedStringToArray("foo" + Environment.NewLine + "bar"));
			Assert.AreEqual(new string[0], StringUtil.NewLineSeparatedStringToArray(""));
			Assert.AreEqual(new string[0], StringUtil.NewLineSeparatedStringToArray(null));
			Assert.AreEqual(new string[1] { "" }, StringUtil.NewLineSeparatedStringToArray(Environment.NewLine));
		}

        [Test]
        public void UrlEncodeNameCorrectlyEncodesNames()
        {
            Assert.AreEqual("cc.net%20rocks", StringUtil.UrlEncodeName("cc.net rocks"));
            Assert.AreEqual("http%3a%2f%2fserver%2fcc%20net", StringUtil.UrlEncodeName("http://server/cc net"));
            Assert.AreEqual("abcdefHIJKLMNopqrstUVWXYZ0123456789-_.~", StringUtil.UrlEncodeName("abcdefHIJKLMNopqrstUVWXYZ0123456789-_.~"));
            Assert.AreEqual("%60%21%40%23%24%25%5e%26%2a%28%29%3d%2b%3c%3e%3f%2f%5c%7c%7b%7d%5b%5d%3a%3b%22%27", StringUtil.UrlEncodeName("`!@#$%^&*()=+<>?/\\|{}[]:;\"'"));
            Assert.AreEqual("\x100\x200\x300\x400", StringUtil.UrlEncodeName("\x100\x200\x300\x400"));
        }
	}
}
