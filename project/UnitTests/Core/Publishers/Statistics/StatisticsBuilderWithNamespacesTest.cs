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
            System.IO.StreamReader reader = System.IO.File.OpenText(System.IO.Path.Combine(@"resources", @"UnitTestResults2.xml"));
            var buildlog= reader.ReadToEnd();
            reader.Close();

            StatisticsBuilder builder = new StatisticsBuilder();

  
            Statistic info = new Statistic();
            info.Name = "Statistic TestsTotalCount no ns";
            info.Xpath = @"//TestRun/ResultSummary/Counters/@total";
            builder.Add(info);


            FirstMatch info2 = new FirstMatch();
            info2.Name = "FirstMatch TestsTotalCount no ns";
            info2.Xpath = @"//TestRun/ResultSummary/Counters/@total";
            builder.Add(info2);

            
            FirstMatch info1 = new FirstMatch();
            info1.Name = "FirstMatch TestsTotalCount with ns";
            info1.Xpath = @"//mstest:TestRun/mstest:ResultSummary/mstest:Counters/@total";
            info1.NameSpaces = new StatisticsNamespaceMapping[1];           
            info1.NameSpaces[0] = new StatisticsNamespaceMapping("mstest", @"http://microsoft.com/schemas/VisualStudio/TeamTest/2010");
            builder.Add(info1);



            Statistic info3 = new Statistic();
            info3.Name = "Statistic sum(TestsTotalCount) no ns";
            info3.Xpath = @"sum(//TestRun/ResultSummary/Counters/@total)";
            builder.Add(info3);

            Statistic info4 = new Statistic();
            info4.Name = "Statistic sum(TestsTotalCount) with ns";
            info4.Xpath = @"sum(//mstest:TestRun/mstest:ResultSummary/mstest:Counters/@total)";
            info4.NameSpaces = new StatisticsNamespaceMapping[1];
            info4.NameSpaces[0] = new StatisticsNamespaceMapping("mstest", @"http://microsoft.com/schemas/VisualStudio/TeamTest/2010");
            builder.Add(info4);


            // seems impossible in xsl 1.0
            Statistic imp01 = new Statistic();
            imp01.Name = "impossible : Statistic TestsExecutedCount xmlns";
            imp01.Xpath = @"/xmlns:cruisecontrol/mstest:TestRun/mstest:ResultSummary/mstest:Counters/@executed";
            imp01.NameSpaces = new StatisticsNamespaceMapping[1];
            imp01.NameSpaces[0] = new StatisticsNamespaceMapping("mstest", @"http://microsoft.com/schemas/VisualStudio/TeamTest/2010");
            
            builder.Add(imp01);

            Statistic imp02 = new Statistic();
            imp02.Name = "impossible : Statistic TestsExecutedCount2 not namespace";
            imp02.Xpath = @"/cruisecontrol/mstest:TestRun/mstest:ResultSummary/mstest:Counters/@executed";
            imp02.NameSpaces = new StatisticsNamespaceMapping[1];
            imp02.NameSpaces[0] = new StatisticsNamespaceMapping("mstest", @"http://microsoft.com/schemas/VisualStudio/TeamTest/2010");

            builder.Add(imp02);

            


            StatisticsResults results = builder.ProcessBuildResults(buildlog);

            foreach (var x in results)
            {
                Console.WriteLine("Result {0} : {1}", x.StatName, x.Value);
            }
            
        
        }
    
    
    
    }
}
