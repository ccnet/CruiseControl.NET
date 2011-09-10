using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers.Statistics;
using ThoughtWorks.CruiseControl.Core.Util;


namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers.Statistics
{
    [TestFixture]
    public class StatisticsBuilderWithNamespacesTest
    {

        [Test]
        public void ShowOutputOfStatisticsInConsole()
        {
            StringBuilder buffer = new StringBuilder();
            System.IO.StreamReader reader = System.IO.File.OpenText(@"resources\UnitTestResults2.xml");
            var buildlog= reader.ReadToEnd();
            reader.Close();

            StatisticsBuilder builder = new StatisticsBuilder();

            var msb = new FirstMatch();
            msb.Name = "build";
            msb.Xpath = @"//build/@buildtime";
            builder.Add(msb);


            FirstMatch info = new FirstMatch();
            info.Name = "MS-Test";
            info.Xpath = @"//mstest:TestRun/@name";
            info.NameSpaces = new StatisticsNamespaceMapping[1];
            info.NameSpaces[0] = new StatisticsNamespaceMapping("mstest", @"http://microsoft.com/schemas/VisualStudio/TeamTest/2010");
            builder.Add(info);

            FirstMatch info2 = new FirstMatch();
            info2.Name = "TestsTotalCount";
            info2.Xpath = @"//mstest:TestRun/mstest:ResultSummary/mstest:Counters/@total";
            info2.NameSpaces = new StatisticsNamespaceMapping[1];           
            info2.NameSpaces[0] = new StatisticsNamespaceMapping("mstest", @"http://microsoft.com/schemas/VisualStudio/TeamTest/2010");
            builder.Add(info2);


            // seems impossible in xsl 1.0
            Statistic info3 = new Statistic();
            info3.Name = "TestsExecutedCount";
            info3.Xpath = @"/xmlns:cruisecontrol/mstest:TestRun/mstest:ResultSummary/mstest:Counters/@executed";
            info3.NameSpaces = new StatisticsNamespaceMapping[1];
            info3.NameSpaces[0] = new StatisticsNamespaceMapping("mstest", @"http://microsoft.com/schemas/VisualStudio/TeamTest/2010");
            
            builder.Add(info3);

            Statistic info4 = new Statistic();
            info4.Name = "TestsExecutedCount2";
            info4.Xpath = @"/cruisecontrol/mstest:TestRun/mstest:ResultSummary/mstest:Counters/@executed";
            info4.NameSpaces = new StatisticsNamespaceMapping[1];
            info4.NameSpaces[0] = new StatisticsNamespaceMapping("mstest", @"http://microsoft.com/schemas/VisualStudio/TeamTest/2010");

            builder.Add(info4);

            


            StatisticsResults results = builder.ProcessBuildResults(buildlog);

            foreach (var x in results)
            {
                Console.WriteLine("Statistic {0} : {1}", x.StatName, x.Value);
            }
            
        
        }
    
    
    
    }
}
