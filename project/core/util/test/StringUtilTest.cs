using System;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class StringUtilTest : CustomAssertion
	{
		public void TestJoinUnique()
		{
			string[] x = new string[] {"a","b","b"};
			string[] y = new string[] {"n","m","b"};
			string actual = StringUtil.JoinUnique(": ", x, y);
			AssertEquals("a: b: m: n", actual);
		}

		public void TestGenerateHashCode()
		{
			string a = "a";
			string b = "b";
			AssertEquals(
				a.GetHashCode() + b.GetHashCode(), StringUtil.GenerateHashCode(a, b));

			AssertEquals(
				a.GetHashCode() + b.GetHashCode(), StringUtil.GenerateHashCode(a, b, null));
		}

		public void TestLastWord()
		{			
			string s = "this is a sentence without punctuation\n";
			AssertEquals("punctuation", StringUtil.LastWord(s));
			
			s = "this is a sentence with punctuation.\n";
			AssertEquals("punctuation", StringUtil.LastWord(s));

			s = "thisisoneword";
			AssertEquals("thisisoneword", StringUtil.LastWord(s));

			s = "";
			AssertEquals(String.Empty, StringUtil.LastWord(s));
			
			s = null;
			AssertNull("expected null", StringUtil.LastWord(s));
		}

		public void TestLastWord_withSeps()
		{
			string s = "this#is$my%crazy*sentence!!!";
			AssertEquals("sentence", StringUtil.LastWord(s, "#$%*!"));

			s = "!@#$%";
			AssertEquals(String.Empty, StringUtil.LastWord(s, s));
		}

		public void TestInsert()
		{
			string[] original = new String[]{"a","b","c","d"};
			string[] augmented = StringUtil.Insert(original, "x", 2);
			AssertEquals("a", augmented[0]);
			AssertEquals("b", augmented[1]);
			AssertEquals("x", augmented[2]);
			AssertEquals("c", augmented[3]);
			AssertEquals("d", augmented[4]);
		}

		public void TestStrip()
		{
			string input = "strip the word monkey and chinchilla or there's trouble";
			string actual = StringUtil.Strip(input, "monkey ", "chinchilla ");
			AssertEquals("strip the word and or there's trouble", actual);

			input = "hey la monkey monkey chinchilla banana";
			actual = StringUtil.Strip(input, "monkey ", "chinchilla ");
			AssertEquals("hey la banana", actual);
		}
	}
}