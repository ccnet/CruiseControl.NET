using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Label;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Label
{
	[TestFixture]
	public class CustomLabellerTest : IntegrationFixture
	{
		private CustomLabeller labeller;

        private string exampleCode = @"

            if (
                integrationResult != null
                && integrationResult.LastIntegration != null
                && !integrationResult.LastIntegration.IsInitial()                
                )
            {
                if (integrationResult.LastIntegration.Status == ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Success)
                {
                    System.Version lastVersion = System.Version.Parse(integrationResult.LastIntegration.Label);
                    System.Version nextVersion = new System.Version(lastVersion.Major, lastVersion.Minor, lastVersion.Build, lastVersion.Revision + 1);
                    ret = nextVersion.ToString();
                }
                else
                {
                    ret = integrationResult.LastIntegration.Label;
                }
            }
            else
            {
                ret = new System.Version(1, 1, 1, 1).ToString();
            }


";

        [SetUp]
		public void SetUp()
		{
			labeller = new CustomLabeller();
        }

        [Test]
        public void GenerateInitialLabelWoCode()
        {
            Assert.AreEqual("0.0.0.0", labeller.Generate(InitialIntegrationResult()));
        }

        [Test]
        public void GenerateLabelWoCode()
        {
            Assert.AreEqual("0.0.0.0", labeller.Generate(SuccessfulResult("35")));
        }

        [Test]
		public void GenerateNextLabel()
		{
            labeller.CsCode = @"ret = ""somelabel"";";
            Assert.AreEqual("somelabel", labeller.Generate(SuccessfulResult("previouslabel")));
		}

        [Test]
        public void GenerateInitialLabel()
        {
            labeller.CsCode = @"ret = ""somelabel"";";
            Assert.AreEqual("somelabel", labeller.Generate(SuccessfulResult("previouslabel")));
        }

        [Test]
        public void GenerateInitialLabelWithExampleCode()
        {
            labeller.CsCode = exampleCode;
            Assert.AreEqual("1.1.1.1", labeller.Generate(InitialIntegrationResult()));
        }

        [Test]
        public void GenerateNextLabelWithExampleCode()
        {
            labeller.CsCode = exampleCode;
            Assert.AreEqual("1.2.3.5", labeller.Generate(SuccessfulResult("1.2.3.4")));
        }

        [Test]
        public void GenerateSameLabelWithExampleCode()
        {
            labeller.CsCode = exampleCode;
            Assert.AreEqual("1.2.3.4", labeller.Generate(FailedResult("1.2.3.4")));
        }
    }
}
