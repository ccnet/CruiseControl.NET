using NUnit.Framework;
using ThoughtWorks.CruiseControl.UnitTests.Core;

namespace ThoughtWorks.CruiseControl.Core.Label
{
    [TestFixture]
    public class FileLabellerTest : IntegrationFixture
    {
        [Test]
        public void ShouldGeneratedLabelWithPrefix()
        {
            FileLabeller labeller = new FileLabeller(new TestFileReader("001"));
            labeller.Prefix = "V0-";
            string label = labeller.Generate(InitialIntegrationResult());
            Assert.AreEqual("V0-001", label);
        }

        [Test]
        public void ShouldGenerateFirstLabel()
        {
            FileLabeller labeller = new FileLabeller(new TestFileReader("001"));
            labeller.AllowDuplicateSubsequentLabels = false;
            string label = labeller.Generate(InitialIntegrationResult());
            Assert.AreEqual("001", label);
        }

        [Test]
        public void ShouldGenerateDuplicateLabelWithSuffixForSubsequentDuplicateFileContent()
        {
            FileLabeller labeller = new FileLabeller(new TestFileReader("001"));
            string firstLabel = labeller.Generate(InitialIntegrationResult());
            string secondLabel = labeller.Generate(SuccessfulResult(firstLabel));
            Assert.AreEqual("001", secondLabel);
        }

        [Test]
        public void ShouldGenerateLabelWithSuffixForSubsequentDuplicateFileContent()
        {
            FileLabeller labeller = new FileLabeller(new TestFileReader("001"));
            labeller.AllowDuplicateSubsequentLabels = false;
            string firstLabel = labeller.Generate(InitialIntegrationResult());
            IntegrationResult integrationResult = SuccessfulResult(firstLabel);
            string secondLabel = labeller.Generate(integrationResult);
            Assert.AreEqual("001-1", secondLabel);
            IntegrationResult integrationResult2 = SuccessfulResult(secondLabel);
            string thirdLabel = labeller.Generate(integrationResult2);
            Assert.AreEqual("001-2", thirdLabel);
        }

        private class TestFileReader : FileLabeller.FileReader
        {
            private string label;

            public TestFileReader(string label)
            {
                this.label = label;
            }

            public override string GetLabel(string labelFilePath)
            {
                return label;
            }
        }
    }
}