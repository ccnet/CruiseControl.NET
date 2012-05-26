using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Label;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Label
{
	[TestFixture]
	public class DefaultLabellerTest : IntegrationFixture
	{
		private DefaultLabeller labeller;

		[SetUp]
		public void SetUp()
		{
			labeller = new DefaultLabeller();
		}

		[Test]
		public void GenerateIncrementedLabel()
		{
			Assert.AreEqual("36", labeller.Generate(SuccessfulResult("35")));
		}

		[Test]
		public void GenerateInitialLabel()
		{
			Assert.AreEqual(DefaultLabeller.INITIAL_LABEL.ToString(), labeller.Generate(InitialIntegrationResult()));
		}

		[Test]
		public void GenerateInitialLabelWithInitialBuildLabelSet()
		{
			labeller.InitialBuildLabel = 10;
			Assert.AreEqual("10", labeller.Generate(InitialIntegrationResult()));
		}

		[Test]
		public void GenerateLabelWhenLastBuildFailed()
		{
			Assert.AreEqual("23", labeller.Generate(FailedResult("23")));
		}

		[Test]
		public void GenerateInitialPrefixedLabel()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample" + DefaultLabeller.INITIAL_LABEL.ToString(), labeller.Generate(InitialIntegrationResult()));
		}


        [Test]
        public void GenerateInitialPostfixedLabel()
        {
            labeller.LabelPostfix = "QA_Approved";
            Assert.AreEqual(DefaultLabeller.INITIAL_LABEL.ToString()+ "QA_Approved", labeller.Generate(InitialIntegrationResult()));
        }


		[Test]
		public void GeneratePrefixedLabelWhenLastBuildSucceeded()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample36", labeller.Generate(SuccessfulResult("35")));
		}


        [Test]
        public void GeneratePostfixedLabelWhenLastBuildSucceeded()
        {
            labeller.LabelPostfix = "Sample";
            Assert.AreEqual("36Sample", labeller.Generate(SuccessfulResult("35")));
        }


        [Test]
        public void GeneratePreAndPostfixedLabelWhenLastBuildSucceeded()
        {
            labeller.LabelPrefix = "Sample";
            labeller.LabelPostfix = "QA_OK";
            Assert.AreEqual("Sample36QA_OK", labeller.Generate(SuccessfulResult("35")));
            
        }


        [Test]
        public void GeneratePreAndPostfixedLabelWhenLastBuildSucceededPreAndPostFixContainingNumericParts()
        {
            labeller.LabelPrefix = "Numeric55Sample";
            labeller.LabelPostfix = "QA11OK";
            Assert.AreEqual("Numeric55Sample36QA11OK", labeller.Generate(SuccessfulResult("35")));

        }

        
        [Test]
		public void GeneratePrefixedLabelWhenLastBuildFailed()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("23", labeller.Generate(FailedResult("23")));
		}


        [Test]
        public void GeneratePostFixedLabelWhenLastBuildFailed()
        {
            labeller.LabelPostfix = "Sample";
            Assert.AreEqual("23", labeller.Generate(FailedResult("23")));
        }



		[Test]
		public void GeneratePrefixedLabelWhenLastBuildSucceededAndHasLabelWithPrefix()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample24", labeller.Generate(SuccessfulResult("Sample23")));
		}


        [Test]
        public void GeneratePostfixedLabelWhenLastBuildSucceededAndHasLabelWithPostfix()
        {
            labeller.LabelPostfix = "Sample";
            Assert.AreEqual("24Sample", labeller.Generate(SuccessfulResult("23Sample")));
        }


		[Test]
		public void GeneratePrefixedLabelWhenPrefixAndLastIntegrationLabelDontMatch()
		{
			labeller.LabelPrefix = "Sample";
			Assert.AreEqual("Sample24", labeller.Generate(SuccessfulResult("SomethingElse23")));
		}


        [Test]
        public void GeneratePostfixedLabelWhenPostfixAndLastIntegrationLabelDontMatch()
        {
            labeller.LabelPostfix = "Sample";
            Assert.AreEqual("24Sample", labeller.Generate(SuccessfulResult("23Dummy")));
        }

        
        [Test]
		public void GeneratePrefixedLabelWhenPrefixIsNumeric()
		{
			labeller.LabelPrefix = "R3SX";
			Assert.AreEqual("R3SX24", labeller.Generate(SuccessfulResult("R3SX23")));
		}


        [Test]
        public void GeneratePrefixedLabelWhenPostfixIsNumeric()
        {
            labeller.LabelPostfix = "R3";
            Assert.AreEqual("24R3", labeller.Generate(SuccessfulResult("23R3")));
        }



        [Test]
        public void GenerateInitialFormattedLabelWithPrefix()
        {
            labeller.LabelPrefix = "Sample";
            labeller.LabelFormat = "000";
            Assert.AreEqual("Sample" + DefaultLabeller.INITIAL_LABEL.ToString("000"), labeller.Generate(InitialIntegrationResult()));
        }


        [Test]
        public void GenerateInitialFormattedLabelWithPostfix()
        {
            labeller.LabelPostfix = "Sample";
            labeller.LabelFormat = "000";
            Assert.AreEqual(DefaultLabeller.INITIAL_LABEL.ToString("000") + "Sample" , labeller.Generate(InitialIntegrationResult()));
        }

        
        [Test]
        public void GenerateFormattedLabelWhenLastBuildSucceeded()
        {
            labeller.LabelPrefix = "Sample";
            labeller.LabelFormat = "000";
            Assert.AreEqual("Sample036", labeller.Generate(SuccessfulResult("35")));
        }

        [Test]
        public void GenerateFormattedLabelWhenLastBuildFailedWithPrefix()
        {
            labeller.LabelPrefix = "Sample";
            labeller.LabelFormat = "000";
            Assert.AreEqual("23", labeller.Generate(FailedResult("23")));
        }


        [Test]
        public void GenerateFormattedLabelWhenLastBuildFailedWithPostFix()
        {
            labeller.LabelPostfix = "Sample";
            labeller.LabelFormat = "000";
            Assert.AreEqual("23", labeller.Generate(FailedResult("23")));
        }



		[Test]
		public void IncrementLabelOnFailedBuildIfIncrementConditionIsAlways()
		{
			labeller.IncrementOnFailed = true;
			Assert.AreEqual("24", labeller.Generate(FailedResult("23")));
		}




		[Test]
		public void PopulateFromConfiguration()
		{
			string xml = @"<defaultLabeller initialBuildLabel=""35"" prefix=""foo"" incrementOnFailure=""true"" postfix=""bar"" />";
			NetReflector.Read(xml, labeller);
			Assert.AreEqual(35, labeller.InitialBuildLabel);
			Assert.AreEqual("foo", labeller.LabelPrefix);
			Assert.AreEqual(true, labeller.IncrementOnFailed);
            Assert.AreEqual("bar", labeller.LabelPostfix);
		}

		[Test]
		public void DefaultValues()
		{
			Assert.AreEqual(DefaultLabeller.INITIAL_LABEL, labeller.InitialBuildLabel);
			Assert.AreEqual(string.Empty, labeller.LabelPrefix);
			Assert.AreEqual(false, labeller.IncrementOnFailed);
            Assert.AreEqual(string.Empty, labeller.LabelPostfix);
		}


        [Test]
        public void GeneratePrefixedLabelFromLabelPrefixFileWhenLastBuildSucceeded()
        {
            string lblFile = "thelabelprefix.txt";
            System.IO.File.WriteAllText(lblFile, "1.3.4.");

            labeller.LabelPrefixFile = lblFile;

            Assert.AreEqual("1.3.4.36", labeller.Generate(SuccessfulResult("1.3.4.35")));
        }


        [Test]
        public void GeneratePrefixedLabelFromLabelPrefixFileAndLabelPrefixsFileSearchPatternWhenLastBuildSucceeded()
        {
            string lblFile = "thelabelprefix.txt";
            System.IO.File.WriteAllText(lblFile, "1.3.4.");

            labeller.LabelPrefixFile = lblFile;
            labeller.LabelPrefixsFileSearchPattern = @"\d+\.\d+\.\d+\.";

            Assert.AreEqual("1.3.4.36", labeller.Generate(SuccessfulResult("1.3.4.35")));
        }



        [Test]
        [ExpectedException(ExpectedMessage = "File DummyFile.txt does not exist")]
        public void MustThrowExceptionWhenSpecifyingNonExistentFile()
        {
            string lblFile = "DummyFile.txt"; 

            labeller.LabelPrefixFile = lblFile;

            labeller.Generate(SuccessfulResult("1.3.4.35"));
        }


        [Test]
        [ExpectedException(ExpectedMessage = "No valid prefix data found in file : thelabelprefix.txt")]
        public void MustThrowExceptionWhenContentsOfLabelPrefixFileDoesNotMatchLabelPrefixsFileSearchPattern()
        {
            string lblFile = "thelabelprefix.txt";
            System.IO.File.WriteAllText(lblFile, "ho ho ho");

            labeller.LabelPrefixFile = lblFile;
            labeller.LabelPrefixsFileSearchPattern = @"\d+\.\d+\.\d+\.";

            labeller.Generate(SuccessfulResult("1.3.4.35"));
        }

	}
}
