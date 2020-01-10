using System;
using Exortech.NetReflector;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
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
		private Mock<ISourceControl> _mockSC;

		[SetUp]
		public void SetUp()
		{
			_filteredSourceControl = new FilteredSourceControl();
			_mockSC = new Mock<ISourceControl>();
			_filteredSourceControl.SourceControlProvider = (ISourceControl)_mockSC.Object;
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
			Assert.IsTrue(_filteredSourceControl.SourceControlProvider != null);

			string optionalProp = ((SourceControlMock)_filteredSourceControl.SourceControlProvider).AnOptionalProperty;
			Assert.AreEqual(optionalProp, "foo", "Didn't find expected source control provider");

			Assert.AreEqual(_filteredSourceControl.InclusionFilters.Length, 1);

			string inclusionPattern = ((PathFilter)_filteredSourceControl.InclusionFilters[0]).Pattern;
			Assert.AreEqual(inclusionPattern, "/sources/**/*.*", "Didn't find expected inclusion path pattern");

			Assert.AreEqual(_filteredSourceControl.ExclusionFilters.Length, 1);

			string exclusionPattern = ((PathFilter)_filteredSourceControl.ExclusionFilters[0]).Pattern;
			Assert.AreEqual(exclusionPattern, "/sources/info/version.cs", "Didn't find expected exclusion path pattern");
		}

		[Test]
		public void PassesThroughLabelSourceControl()
		{
			//// SETUP
			IntegrationResult result = new IntegrationResult();
			_mockSC.Setup(sc => sc.LabelSourceControl(result)).Verifiable();

			//// EXECUTE
			_filteredSourceControl.LabelSourceControl(result);
		}

		[Test]
		public void PassesThroughGetSource()
		{
			//// SETUP
			IntegrationResult result = new IntegrationResult();
			_mockSC.Setup(sc => sc.GetSource(result)).Verifiable();

			//// EXECUTE
			_filteredSourceControl.GetSource(result);
		}

		[Test]
		public void AppliesFiltersOnModifications()
		{
			//// SETUP
			IntegrationResult from = IntegrationResult(DateTime.Now);
			IntegrationResult to = IntegrationResult(DateTime.Now.AddDays(10));
			_mockSC.Setup(sc => sc.GetModifications(from, to)).Returns(Modifications).Verifiable();

			NetReflector.Read(SourceControlXml, _filteredSourceControl);
			_filteredSourceControl.SourceControlProvider = (ISourceControl)_mockSC.Object;

			//// EXECUTE
			Modification[] filteredResult = _filteredSourceControl.GetModifications(from, to);

			//// VERIFY
			Assert.AreEqual(1, filteredResult.Length);
		}

		private IntegrationResult IntegrationResult(DateTime dateTime1)
		{
			return IntegrationResultMother.CreateSuccessful(dateTime1);
		}

		public static readonly Modification[] Modifications = new Modification[]
				{
					ModificationMother.CreateModification("project.info", "/"),
					ModificationMother.CreateModification("test.csproj", "/sources"),
					ModificationMother.CreateModification("version.cs", "/sources/info")
				};
        public static readonly Modification[] ModificationsWithCVS = new Modification[]
                {
                    ModificationMother.CreateModification("x.cs", "/working/sources"),
                    ModificationMother.CreateModification("Entries", "/working/sources/CVS"),
                    ModificationMother.CreateModification("x.build", "/working/build"),
                    ModificationMother.CreateModification("x.dll", "/working/build/target/sources")                 
                };
                
        private const string SourceControlXmlWithCVS =
            @"<sourcecontrol type=""filtered"">
                <sourceControlProvider type=""mocksourcecontrol"">
                        <anOptionalProperty>foo</anOptionalProperty>
                </sourceControlProvider>
                <inclusionFilters>
                    <pathFilter>
                        <pattern>**/sources/**/*.*</pattern>
                    </pathFilter>
                    <pathFilter>
                        <pattern>**/build/**/*.*</pattern>
                    </pathFilter>
                </inclusionFilters>
                <exclusionFilters>
                    <pathFilter>
                        <pattern>**/CVS/**/*.*</pattern>
                    </pathFilter>
                    <pathFilter>
                        <pattern>**/target/**/*.*</pattern>
                    </pathFilter>
                </exclusionFilters>
              </sourcecontrol>";
              
        [Test]
        public void AppliesInclusionExclusionOnModifications()
        {
            // Setup
            IntegrationResult from = IntegrationResult(DateTime.Now);
            IntegrationResult to = IntegrationResult(DateTime.Now.AddDays(10));
            _mockSC.Setup(sc => sc.GetModifications(from, to)).Returns(ModificationsWithCVS).Verifiable();
            
            NetReflector.Read(SourceControlXmlWithCVS, _filteredSourceControl);
            _filteredSourceControl.SourceControlProvider = (ISourceControl)_mockSC.Object;

            //// EXECUTE
            Modification[] filteredResult = _filteredSourceControl.GetModifications(from, to);

            //// VERIFY
            Assert.AreEqual(2, filteredResult.Length);     
            Assert.AreEqual("x.cs", filteredResult[0].FileName);
            Assert.AreEqual("x.build", filteredResult[1].FileName);
        }
    }
}
