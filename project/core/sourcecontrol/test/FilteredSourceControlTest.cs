using System;
using NUnit.Framework;
using NMock;
using NMock.Constraints;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class FilteredSourceControlTest: CustomAssertion
	{
		private const string SourceControlXml =
			@"<sourcecontrol type=""filtered"">
				<sourceControlProvider type=""mocksourcecontrol"">
						<anOptionalProperty>foo</anOptionalProperty>
				</sourceControlProvider>
				<inclusionFilters>
					<pathFilter>
						<pattern>/sources/**/*.*</pattern>
					</pathFilter>
				</inclusionFilters>
                <exclusionFilters>
                    <pathFilter>
						<pattern>/sources/info/version.cs</pattern>
                    </pathFilter>
                </exclusionFilters>
              </sourcecontrol>";
		private FilteredSourceControl _filteredSourceControl;
		private DynamicMock _mockSC;

		[SetUp]
		public void SetUp()
		{
			_filteredSourceControl = new FilteredSourceControl();
			_mockSC = new DynamicMock(typeof(ISourceControl));
			_filteredSourceControl.SourceControlProvider = (ISourceControl)_mockSC.MockInstance;
		}

		[TearDown]
		public void TearDown()
		{
			_mockSC.Verify();			
		}

		[Test]
		public void ValuePopulation()
		{
			//// EXECUTE
			NetReflector.Read(SourceControlXml, _filteredSourceControl);

			//// VERIFY
			Assert(_filteredSourceControl.SourceControlProvider != null);

			string optionalProp = ((SourceControlMock)_filteredSourceControl.SourceControlProvider).AnOptionalProperty;
			AssertEquals("Didn't find expected source control provider", optionalProp, "foo");

			AssertEquals("Didn't find expected inclusion filter", _filteredSourceControl.InclusionFilters.Count, 1);

			string inclusionPattern = ((PathFilter)_filteredSourceControl.InclusionFilters[0]).Pattern;
			AssertEquals("Didn't find expected inclusion path pattern", inclusionPattern, "/sources/**/*.*");

			AssertEquals("Didn't find expected exclusion filter", _filteredSourceControl.ExclusionFilters.Count, 1);

			string exclusionPattern = ((PathFilter)_filteredSourceControl.ExclusionFilters[0]).Pattern;
			AssertEquals("Didn't find expected exclusion path pattern", exclusionPattern, "/sources/info/version.cs");
		}

		[Test]
		public void PassesThroughLabelSourceControl()
		{
			//// SETUP
			string label = "testLabel";
			DateTime dateTime = DateTime.Now;
			_mockSC.Expect("LabelSourceControl", label, dateTime);

			//// EXECUTE
			_filteredSourceControl.LabelSourceControl(label, dateTime);
		}

		[Test]
		public void PassesThroughGetSource()
		{
			//// SETUP
			IntegrationResult result = new IntegrationResult();
			_mockSC.Expect("GetSource", result);

			//// EXECUTE
			_filteredSourceControl.GetSource(result);
		}

		[Test]
		public void PassesThroughShouldRun()
		{
			//// SETUP
			IntegrationResult result = new IntegrationResult();
			_mockSC.ExpectAndReturn("ShouldRun", true, result);

			//// EXECUTE
			_filteredSourceControl.ShouldRun(result);
		}

		[Test]
		public void InvokesMethodsOnTemporaryLabeller() 
		{
			//// SETUP
			DynamicMock mockSC = new DynamicMock(typeof(ITemporaryLabeller));
			mockSC.Expect("CreateTemporaryLabel");
			mockSC.Expect("DeleteTemporaryLabel");

			//// EXECUTE
			_filteredSourceControl.CreateTemporaryLabel();
			_filteredSourceControl.DeleteTemporaryLabel();
		}

		[Test]
		public void InvokesRunOnSCProvider() 
		{
			//// SETUP
			IntegrationResult result = new IntegrationResult();
			_mockSC.ExpectAndReturn("GetModifications", Modifications, new IsTypeOf(typeof(DateTime)), new IsTypeOf(typeof(DateTime)));

			//// EXECUTE
			_filteredSourceControl.Run(result);
		}

		[Test]
		public void AppliesFiltersOnModifications()
		{
			//// SETUP
			DateTime dateTime1 = DateTime.Now;
			DateTime dateTime2 = dateTime1.AddDays(10);
			_mockSC.ExpectAndReturn("GetModifications", Modifications, dateTime1, dateTime2);

			NetReflector.Read(SourceControlXml, _filteredSourceControl);
			_filteredSourceControl.SourceControlProvider = (ISourceControl)_mockSC.MockInstance;

			//// EXECUTE
			Modification[] filteredResult = _filteredSourceControl.GetModifications(dateTime1, dateTime2);

			//// VERIFY
			AssertEquals(1, filteredResult.Length);
		}
		
		public static readonly Modification[] Modifications = new Modification[]
				{
					ModificationMother.CreateModification("project.info", "/"),
					ModificationMother.CreateModification("test.csproj", "/sources"),
					ModificationMother.CreateModification("version.cs", "/sources/info")
				};
	}
}
