namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Tasks;

    [TestFixture]
    public class CoverageFilterTests
    {
        [Test]
        public void ToParamStringGeneratesDefault()
        {
            var threshold = new CoverageFilter();
            var expected = string.Empty;
            var actual = threshold.ToParamString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToParamStringGeneratesWithClass()
        {
            var threshold = new CoverageFilter
            {
                Data = "SomeData",
                ItemType = CoverageFilter.NCoverItemType.Class
            };
            var expected = "SomeData:Class";
            var actual = threshold.ToParamString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToParamStringGeneratesWithClassAndRegex()
        {
            var threshold = new CoverageFilter
            {
                Data = "SomeData",
                ItemType = CoverageFilter.NCoverItemType.Document,
                IsRegex = true
            };
            var expected = "SomeData:Document:true:false";
            var actual = threshold.ToParamString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToParamStringGeneratesWithClassAndInclude()
        {
            var threshold = new CoverageFilter
            {
                Data = "SomeData",
                ItemType = CoverageFilter.NCoverItemType.Document,
                IsInclude = true
            };
            var expected = "SomeData:Document:false:true";
            var actual = threshold.ToParamString();
            Assert.AreEqual(expected, actual);
        }
    }
}
