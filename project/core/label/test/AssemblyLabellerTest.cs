using System;
using System.Reflection;
using NUnit.Framework;

namespace tw.ccnet.core.test
{
	[TestFixture]
	public class AssemblyLabellerTest
	{
		public const int BUILD = 1137;
		public const int REVISION = 41003;

		[Test]
		public void TestIncrementNetReflectorRevision() 
		{
			AssemblyVersionLabeller inc = new AssemblyVersionLabeller();
			inc.AssemblyPath = @"NetReflector.dll";

			Assertion.AssertEquals(String.Format("1.0.{0}.{1}", BUILD, REVISION + 1), 
				inc.Generate(new IntegrationResult()));
		}

		[Test]
		public void TestIncrementBuildNumber() 
		{
			AssemblyVersionLabeller inc = new AssemblyVersionLabeller();
			inc.AssemblyPath = @"NetReflector.dll";
			inc.VersionPartToIncrement =  AssemblyVersionLabeller.VersionPart.build;

			Assertion.AssertEquals(String.Format("1.0.{0}.{1}", BUILD + 1, REVISION), 
				inc.Generate(new IntegrationResult()));
		}
	}
}
