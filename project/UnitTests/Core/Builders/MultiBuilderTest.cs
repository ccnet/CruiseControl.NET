using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.builder;
using ThoughtWorks.CruiseControl.Core.Builder.Test;
using ThoughtWorks.CruiseControl.Core.Test;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Builders
{
	[TestFixture]
	public class MultiBuilderTest
	{
		private MultiBuilder builder;

		[SetUp]
		protected void SetUp()
		{
			builder = new MultiBuilder();	
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = @"
    <multiBuilder>
		<builders>
			<mockbuildrunner />
			<mockbuildrunner />
		</builders>
    </multiBuilder>";

			NetReflector.Read(xml, builder);
			Assert.AreEqual(2, builder.Builders.Length);
			Assert.IsTrue(builder.Builders[0] is MockBuilder);
			Assert.IsTrue(builder.Builders[1] is MockBuilder);
		}

		[Test]
		public void InvokingRunShouldCallRunOnBuilders()
		{
			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			DynamicMock mockBuilder1 = new DynamicMock(typeof(IBuilder));
			mockBuilder1.Expect("Run", result);
			DynamicMock mockBuilder2 = new DynamicMock(typeof(IBuilder));
			mockBuilder2.Expect("Run", result);
			builder.Builders = new IBuilder[] { (IBuilder)mockBuilder1.MockInstance, (IBuilder)mockBuilder2.MockInstance };
			
			builder.Run(result);
			mockBuilder1.Verify();
			mockBuilder2.Verify();
		}
	}
}
