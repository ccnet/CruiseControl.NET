using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
    [TestFixture]
    public class SystemPathTest
    {
        [Test]
        public void ShouldConvertPathSeparatorForWindows()
        {
            DynamicMock mockWindows = new DynamicMock(typeof (IExecutionEnvironment));
            mockWindows.SetupResult("DirectorySeparator", '\\');
            Assert.AreEqual(@"c:\temp\files", new SystemPath("c:/temp/files", (IExecutionEnvironment) mockWindows.MockInstance).ToString());
        }

        [Test]
        public void ShouldConvertPathSeparatorForMono()
        {
            DynamicMock mockMono = new DynamicMock(typeof (IExecutionEnvironment));
            mockMono.SetupResult("DirectorySeparator", '/');
            Assert.AreEqual(@"/home/build/files", new SystemPath(@"\home\build\files", (IExecutionEnvironment) mockMono.MockInstance).ToString());
        }
    }
}