using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;


namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
    [TestFixture]
    public class CronTriggerTest
    {
        [Test]
        public void TestX()
        {
            var c = new CronTrigger();

            c.CronExpression = "* * 1 1 *"; // first januari of each year

            c.Fire();

            DateTime expected;
            if (DateTime.Now.DayOfYear == 1)
            {
                expected = new DateTime(DateTime.Now.Year, 1, 1);
            }
            else
            {
                expected = new DateTime(DateTime.Now.Year + 1, 1, 1);
            }

            Assert.AreEqual(expected, c.NextBuild);

        }
    }
}
