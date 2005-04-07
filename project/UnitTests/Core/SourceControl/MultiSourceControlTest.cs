using System;
using System.Collections;
using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class MultiSourceControlTest : CustomAssertion
	{
		public static string SourceControlXml = @"<sourcecontrol type=""multi"">
	<sourceControls>
		<mocksourcecontrol>
			<anOptionalProperty>foo</anOptionalProperty>
		</mocksourcecontrol>
		<mocksourcecontrol>
			<anOptionalProperty>bar</anOptionalProperty>
		</mocksourcecontrol>
	</sourceControls>
</sourcecontrol>
";

		[Test]
		public void ValuePopulation()
		{
			//// SETUP
			MultiSourceControl multiSourceControl = new MultiSourceControl();

			//// EXECUTE
			NetReflector.Read(SourceControlXml, multiSourceControl);

			//// VERIFY
			Assert.IsTrue(multiSourceControl.SourceControls.Length == 2);

			string optionalProp0 = ((SourceControlMock)multiSourceControl.SourceControls[0]).AnOptionalProperty;
			string optionalProp1 = ((SourceControlMock)multiSourceControl.SourceControls[1]).AnOptionalProperty;

			bool fooFound = optionalProp0 == "foo" || optionalProp1 == "foo";
			bool barFound = optionalProp0 == "bar" || optionalProp1 == "bar";

			Assert.IsTrue(fooFound && barFound);
		}

		[Test]
		public void PassesThroughLabelSourceControl()
		{
			//// SETUP
			string label = "testLabel";
			IntegrationResult result = new IntegrationResult();

			DynamicMock mockSC1 = new DynamicMock(typeof(ISourceControl));
			mockSC1.Expect("LabelSourceControl", label, result);

			DynamicMock mockSC2 = new DynamicMock(typeof(ISourceControl));
			mockSC2.Expect("LabelSourceControl", label, result);

			ISourceControl[] sourceControls = new ISourceControl[] { (ISourceControl) mockSC1.MockInstance, (ISourceControl) mockSC2.MockInstance };

			MultiSourceControl multiSourceControl = new MultiSourceControl();
			multiSourceControl.SourceControls = sourceControls;

			//// EXECUTE
			multiSourceControl.LabelSourceControl(label, result);

			//// VERIFY
			mockSC1.Verify();
			mockSC2.Verify();
		}

		[Test]
		public void PassesThroughGetSourceControlAndCombinesResults()
		{
			//// SETUP
			DateTime dateTime1 = DateTime.Now;
			DateTime dateTime2 = dateTime1.AddDays(10);

			Modification mod1 = new Modification();
			mod1.Comment = "Testing Multi";
			Modification mod2 = new Modification();
			mod2.Comment = "More Multi";
			Modification mod3 = new Modification();
			mod3.Comment = "Yet More Multi";

			ArrayList mocks = new ArrayList();
			mocks.Add( CreateModificationsSourceControlMock( new Modification[] { mod1, mod2 }, dateTime1, dateTime2 ) );
			mocks.Add( CreateModificationsSourceControlMock( new Modification[] { mod3 }, dateTime1, dateTime2 ) );
			mocks.Add( CreateModificationsSourceControlMock( new Modification[0], dateTime1, dateTime2 ) );
			mocks.Add( CreateModificationsSourceControlMock( null, dateTime1, dateTime2 ) );

			ArrayList scList = new ArrayList();
			foreach (DynamicMock mock in mocks)
			{
				scList.Add(mock.MockInstance);
			}

			MultiSourceControl multiSourceControl = new MultiSourceControl();
			multiSourceControl.SourceControls = (ISourceControl[]) scList.ToArray(typeof (ISourceControl));

			//// EXECUTE
			ArrayList returnedMods = new ArrayList( multiSourceControl.GetModifications(dateTime1, dateTime2) );

			//// VERIFY
			foreach (DynamicMock mock in mocks)
			{
				mock.Verify();
			}

			Assert.IsTrue(returnedMods.Contains(mod1));
			Assert.IsTrue(returnedMods.Contains(mod2));
			Assert.IsTrue(returnedMods.Contains(mod3));
		}

		[Test]
		public void TemporaryLabellersHaveMethodsInvoked()
		{
			//// SETUP
			DynamicMock sourceControl = new DynamicMock(typeof(ISourceControl));
			DynamicMock tempLabeller = new DynamicMock(typeof(ITemporaryLabeller));

			tempLabeller.Expect( "CreateTemporaryLabel" );
			tempLabeller.Expect( "DeleteTemporaryLabel" );
			ArrayList mocks = new ArrayList();
			mocks.Add( sourceControl );
			mocks.Add( tempLabeller );
			ArrayList scList = new ArrayList( 2 );
			scList.Add( sourceControl.MockInstance );
			scList.Add( tempLabeller.MockInstance );
			MultiSourceControl multiSourceControl = new MultiSourceControl();
			multiSourceControl.SourceControls = (ISourceControl[]) scList.ToArray(typeof (ISourceControl));

			//// EXECUTE
			multiSourceControl.CreateTemporaryLabel();
			multiSourceControl.DeleteTemporaryLabel();

			//// VERIFY
			foreach (DynamicMock mock in mocks)
			{
				mock.Verify();
			}
		}

		[Test]
		public void ShouldInstructAggregatedSourceControlsToGetSource()
		{
			IntegrationResult result = new IntegrationResult();
			IMock mockSC1 = new DynamicMock(typeof(ISourceControl));
			IMock mockSC2 = new DynamicMock(typeof(ISourceControl));
			mockSC1.Expect("GetSource", result);
			mockSC2.Expect("GetSource", result);

			MultiSourceControl multiSourceControl = new MultiSourceControl();
			multiSourceControl.SourceControls = new ISourceControl[] { (ISourceControl) mockSC1.MockInstance, (ISourceControl) mockSC2.MockInstance };
			multiSourceControl.GetSource(result);

			mockSC1.Verify();
			mockSC2.Verify();
		}

		private DynamicMock CreateModificationsSourceControlMock(Modification[] mods, DateTime dt1, DateTime dt2)
		{
			DynamicMock mock = new DynamicMock(typeof(ISourceControl));
			mock.ExpectAndReturn("GetModifications", mods, dt1, dt2);
			return mock;
		}
	} 
}