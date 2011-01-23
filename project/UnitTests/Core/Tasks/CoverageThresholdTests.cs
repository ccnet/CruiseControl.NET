namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Tasks;

    [TestFixture]
    public class CoverageThresholdTests
    {
        [Test]
        public void ToParamStringGeneratesDefault()
        {
            var threshold = new CoverageThreshold();
            var expected = "SymbolCoverage:0";
            var actual = threshold.ToParamString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToParamStringGeneratesWithValue()
        {
            var threshold = new CoverageThreshold
                {
                    Metric = CoverageThreshold.NCoverMetric.BranchCoverage,
                    MinValue = 95
                };
            var expected = "BranchCoverage:95";
            var actual = threshold.ToParamString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToParamStringGeneratesWithPattern()
        {
            var threshold = new CoverageThreshold
            {
                Metric = CoverageThreshold.NCoverMetric.CyclomaticComplexity,
                MinValue = 95,
                ItemType = CoverageThreshold.NCoverItemType.Class,
                Pattern = "Test"
            };
            var expected = "CyclomaticComplexity:95:Class:Test";
            var actual = threshold.ToParamString();
            Assert.AreEqual(expected, actual);
        }
    }
}
