using System;
using NUnit.Framework;

namespace tw.ccnet.core.util.test
{
	[TestFixture]
	public class StringUtilTest
	{
		public void TestJoinUnique()
		{
			string[] x = new string[] {"a","b","b"};
			string[] y = new string[] {"n","m","b"};
			string actual = StringUtil.JoinUnique(": ", x, y);
			Assertion.AssertEquals("a: b: m: n", actual);
		}

		public void TestGenerateHashCode()
		{
			string a = "a";
			string b = "b";
			Assertion.AssertEquals(
				a.GetHashCode() + b.GetHashCode(), StringUtil.GenerateHashCode(a, b));

			Assertion.AssertEquals(
				a.GetHashCode() + b.GetHashCode(), StringUtil.GenerateHashCode(a, b, null));
		}

		public void TestLastWord()
		{			
			string s = "this is a sentence without punctuation\n";
			Assertion.AssertEquals("punctuation", StringUtil.LastWord(s));
			
			s = "this is a sentence with punctuation.\n";
			Assertion.AssertEquals("punctuation", StringUtil.LastWord(s));

			s = "thisisoneword";
			Assertion.AssertEquals("thisisoneword", StringUtil.LastWord(s));

			s = "";
			Assertion.AssertEquals(String.Empty, StringUtil.LastWord(s));
			
			s = null;
			Assertion.AssertNull("expected null", StringUtil.LastWord(s));
		}

		public void TestLastWord_withSeps()
		{
			string s = "this#is$my%crazy*sentence!!!";
			Assertion.AssertEquals("sentence", StringUtil.LastWord(s, "#$%*!"));

			s = "!@#$%";
			Assertion.AssertEquals(String.Empty, StringUtil.LastWord(s, s));
		}

		public void TestInsert()
		{
			string[] original = new String[]{"a","b","c","d"};
			string[] augmented = StringUtil.Insert(original, "x", 2);
			Assertion.AssertEquals("a", augmented[0]);
			Assertion.AssertEquals("b", augmented[1]);
			Assertion.AssertEquals("x", augmented[2]);
			Assertion.AssertEquals("c", augmented[3]);
			Assertion.AssertEquals("d", augmented[4]);
		}

		public void TestStrip()
		{
			string input = "strip the word monkey and chinchilla or there's trouble";
			string actual = StringUtil.Strip(input, "monkey ", "chinchilla ");
			Assertion.AssertEquals("strip the word and or there's trouble", actual);

			input = "hey la monkey monkey chinchilla banana";
			actual = StringUtil.Strip(input, "monkey ", "chinchilla ");
			Assertion.AssertEquals("hey la banana", actual);
		}
	}
}