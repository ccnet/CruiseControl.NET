using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class StringUtilTest : CustomAssertion
	{
		public void TestJoinUnique()
		{
			string[] x = new string[] {"a","b","b"};
			string[] y = new string[] {"n","m","b"};
			string actual = StringUtil.JoinUnique(": ", x, y);
			Assert.AreEqual("a: b: m: n", actual);
		}

		public void TestGenerateHashCode()
		{
			string a = "a";
			string b = "b";
			Assert.AreEqual(
				a.GetHashCode() + b.GetHashCode(), StringUtil.GenerateHashCode(a, b));

			Assert.AreEqual(
				a.GetHashCode() + b.GetHashCode(), StringUtil.GenerateHashCode(a, b, null));
		}

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

		public void TestLastWord_withSeps()
		{
			string s = "this#is$my%crazy*sentence!!!";
			Assert.AreEqual("sentence", StringUtil.LastWord(s, "#$%*!"));

			s = "!@#$%";
			Assert.AreEqual(String.Empty, StringUtil.LastWord(s, s));
		}

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

		public void TestStrip()
		{
			string input = "strip the word monkey and chinchilla or there's trouble";
			string actual = StringUtil.Strip(input, "monkey ", "chinchilla ");
			Assert.AreEqual("strip the word and or there's trouble", actual);

			input = "hey la monkey monkey chinchilla banana";
			actual = StringUtil.Strip(input, "monkey ", "chinchilla ");
			Assert.AreEqual("hey la banana", actual);
		}
	}
}