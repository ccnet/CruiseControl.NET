using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
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
			new XmlPopulator().Populate(XmlUtil.CreateDocumentElement(SourceControlXml), multiSourceControl);

			//// VERIFY
			Assert(multiSourceControl.SourceControls.Count == 2);

			string optionalProp0 = ((SourceControlMock)multiSourceControl.SourceControls[0]).AnOptionalProperty;
			string optionalProp1 = ((SourceControlMock)multiSourceControl.SourceControls[1]).AnOptionalProperty;

			bool fooFound = optionalProp0 == "foo" || optionalProp1 == "foo";
			bool barFound = optionalProp0 == "bar" || optionalProp1 == "bar";

			Assert("Didn't find both foo and bar Source Controls", fooFound && barFound);
		}

		[Test]
		public void TestPassesThroughLabelSourceControl()
		{
			//// SETUP
			string label = "testLabel";
			DateTime dateTime = DateTime.Now;

			DynamicMock mockSC1 = new DynamicMock(typeof(ISourceControl));
			mockSC1.Expect("LabelSourceControl", label, dateTime);

			DynamicMock mockSC2 = new DynamicMock(typeof(ISourceControl));
			mockSC2.Expect("LabelSourceControl", label, dateTime);

			ArrayList scList = new ArrayList();
			scList.Add(mockSC1.MockInstance);
			scList.Add(mockSC2.MockInstance);

			MultiSourceControl multiSourceControl = new MultiSourceControl();
			multiSourceControl.SourceControls = scList;

			//// EXECUTE
			multiSourceControl.LabelSourceControl(label, dateTime);

			//// VERIFY
			mockSC1.Verify();
			mockSC2.Verify();
		}

		[Test]
		public void TestPassesThroughGetSourceControlAndCombinesResults()
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
			mocks.Add( createModificationsSourceControlMock( new Modification[] { mod1, mod2 }, dateTime1, dateTime2 ) );
			mocks.Add( createModificationsSourceControlMock( new Modification[] { mod3 }, dateTime1, dateTime2 ) );
			mocks.Add( createModificationsSourceControlMock( new Modification[0], dateTime1, dateTime2 ) );
			mocks.Add( createModificationsSourceControlMock( null, dateTime1, dateTime2 ) );

			ArrayList scList = new ArrayList();
			foreach (DynamicMock mock in mocks)
			{
				scList.Add(mock.MockInstance);
			}

			MultiSourceControl multiSourceControl = new MultiSourceControl();
			multiSourceControl.SourceControls = scList;

			//// EXECUTE
			ArrayList returnedMods = new ArrayList( multiSourceControl.GetModifications(dateTime1, dateTime2) );

			//// VERIFY
			foreach (DynamicMock mock in mocks)
			{
				mock.Verify();
			}

			Assert(returnedMods.Contains(mod1));
			Assert(returnedMods.Contains(mod2));
			Assert(returnedMods.Contains(mod3));
		}

		[Test]
		public void ShouldRun()
		{
			FileSourceControl sc = new FileSourceControl();
			Assert(sc.ShouldRun(new IntegrationResult()));
			Assert(sc.ShouldRun(IntegrationResultMother.CreateSuccessful()));
			AssertFalse(sc.ShouldRun(IntegrationResultMother.CreateFailed()));
			AssertFalse(sc.ShouldRun(IntegrationResultMother.CreateExceptioned()));
		}

		private DynamicMock createModificationsSourceControlMock(Modification[] mods, DateTime dt1, DateTime dt2)
		{
			DynamicMock mock = new DynamicMock(typeof(ISourceControl));
			mock.ExpectAndReturn("GetModifications", mods, dt1, dt2);
			return mock;
		}
	} 
}