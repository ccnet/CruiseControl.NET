using System;
using System.Reflection;
using NUnit.Framework;

namespace tw.ccnet.core.test
{
	[TestFixture]
	public class AssemblyIncrementerTest
	{
		public const int BUILD = 1137;
		public const int REVISION = 41003;

		[Test]
		public void TestIncrementNetReflectorRevision() 
		{
			AssemblyVersionIncrementer inc = new AssemblyVersionIncrementer(@"NetReflector.dll");

			Assertion.AssertEquals(String.Format("1.0.{0}.{1}", BUILD, REVISION + 1), inc.GetNextLabel());
		}

		[Test]
		public void TestIncrementBuildNumber() 
		{
			AssemblyVersionIncrementer inc = new AssemblyVersionIncrementer("NetReflector.dll",
                AssemblyVersionIncrementer.VersionPart.build);

			Assertion.AssertEquals(String.Format("1.0.{0}.{1}", BUILD + 1, REVISION), inc.GetNextLabel());
		}
	}
}
