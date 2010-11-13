using System;
using System.Collections;
using NUnit.Framework;
using NMock;
using ThoughtWorks;
using ThoughtWorks.CruiseControl.WebDashboard.Resources;


namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.Statistics
{
	[TestFixture]
	public class BuildGraphTest
	{
       
        [Test]
        public void BuildGraphsAreTheSame()
        {            
            CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph bg01;            
            CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph bg02;          

            bg01 = GetBuildGraph(2, true);      
            bg02 = GetBuildGraph(2, true);      

            Assert.IsTrue(bg01.Equals(bg02));

            bg01 = GetBuildGraph(2, false);      
            bg02 = GetBuildGraph(2, false);      

            Assert.IsTrue(bg01.Equals(bg02));

        }


        [Test]
        public void BuildGraphsAreDifferent()
        {            
            CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph bg01;            
            CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph bg02;          

            bg01 = GetBuildGraph(1, true);      
            bg02 = GetBuildGraph(5, true);      

            Assert.IsFalse(bg01.Equals(bg02));

            bg01 = GetBuildGraph(1, false);      
            bg02 = GetBuildGraph(5, false);      

            Assert.IsFalse(bg01.Equals(bg02));
        
        }


        [Test]
        public void BuildGraphDataConsistency01()
        {            
            CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph bg01;                        
            CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph.GraphBuildDayInfo GraphInfoDay1;

            bg01 = GetBuildGraph(1, true);

            var Result = bg01.GetBuildHistory(15); // get all builds, but limit them to the last 15 days
            
            Assert.AreEqual(1,bg01.HighestAmountPerDay, "builds in same day must result in HighestAmountPerDay set to 1");
            Assert.AreEqual(1,Result.Count, "1 build must result in HighestAmountPerDay set to 1");
            
            GraphInfoDay1 = Result[0] as CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph.GraphBuildDayInfo;
            
            Assert.AreEqual(1, GraphInfoDay1.AmountOfBuilds, "1 build must have 1 build result in 1 day");
           
        }

        [Test]
        public void BuildGraphDataConsistency02()
        {            
            CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph bg01;                        
            CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph.GraphBuildDayInfo GraphInfoDay1;

            bg01 = GetBuildGraph(10, true);

            var Result = bg01.GetBuildHistory(15); // get all builds, but limit them to the last 15 days
            
            Assert.AreEqual(10, bg01.HighestAmountPerDay, "incorrect amount of HighestAmountPerDay");
            Assert.AreEqual(1,Result.Count, "builds are in the same day, must have 1 day-build result");
            
            GraphInfoDay1 = Result[0] as CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph.GraphBuildDayInfo;
            
            Assert.AreEqual(10, GraphInfoDay1.AmountOfBuilds, "day-build must have 10 build results");
           
        }

        [Test]
        public void BuildGraphDataConsistency03()
        {            
            CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph bg01;                        
            CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph.GraphBuildDayInfo GraphInfoDay1;

            bg01 = GetBuildGraph(10, false);

            var Result = bg01.GetBuildHistory(15); // get all builds, but limit them to the last 15 days
            
            Assert.AreEqual(1,bg01.HighestAmountPerDay, "1 build must result in HighestAmountPerDay set to 1");
            Assert.AreEqual(10,Result.Count, "incorrect amount of build days");
            
            GraphInfoDay1 = Result[0] as CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph.GraphBuildDayInfo;
            
            Assert.AreEqual(1, GraphInfoDay1.AmountOfBuilds, "day-build must have 10 build results");
           
        }




        private CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph 
                GetBuildGraph(int amountOfBuilds, Boolean buildsAreInSameDay)
        {        
            CruiseControl.Core.Reporting.Dashboard.Navigation.DefaultBuildSpecifier[] builds;
            CruiseControl.Core.Reporting.Dashboard.Navigation.DefaultProjectSpecifier ProjectSpecifier;
            CruiseControl.Core.Reporting.Dashboard.Navigation.DefaultServerSpecifier ServerSpecifier;
            CruiseControl.WebDashboard.Dashboard.DefaultLinkFactory LinkFactory;
            CruiseControl.WebDashboard.Dashboard.DefaultBuildNameFormatter BuildNameFormatter;
            CruiseControl.Core.Reporting.Dashboard.Navigation.DefaultUrlBuilder UrlBuilder;
            CruiseControl.Core.Reporting.Dashboard.Navigation.DefaultCruiseUrlBuilder CruiseUrlBuilder;
            CruiseControl.Core.Project project;

            project = new CruiseControl.Core.Project();
            project.Name = "TestProject";

            ServerSpecifier = new CruiseControl.Core.Reporting.Dashboard.Navigation.DefaultServerSpecifier("local");

            ProjectSpecifier = new CruiseControl.Core.Reporting.Dashboard.Navigation.DefaultProjectSpecifier(ServerSpecifier,project.Name);
            
            builds = new CruiseControl.Core.Reporting.Dashboard.Navigation.DefaultBuildSpecifier[amountOfBuilds];

            for(int i=0; i < amountOfBuilds; i++)
            {
                if (buildsAreInSameDay)                
                    builds[i] = new CruiseControl.Core.Reporting.Dashboard.Navigation.DefaultBuildSpecifier( ProjectSpecifier , string.Format(System.Globalization.CultureInfo.CurrentCulture,"log20050801015223Lbuild.0.0.0.{0}.xml", i) );
                  else
                    builds[i] = new CruiseControl.Core.Reporting.Dashboard.Navigation.DefaultBuildSpecifier( ProjectSpecifier , string.Format(System.Globalization.CultureInfo.CurrentCulture,"log200508{0}015223Lbuild.0.0.0.{1}.xml", (i+1).ToString("00"), i) );
            }

            UrlBuilder = new CruiseControl.Core.Reporting.Dashboard.Navigation.DefaultUrlBuilder();
            CruiseUrlBuilder = new CruiseControl.Core.Reporting.Dashboard.Navigation.DefaultCruiseUrlBuilder(UrlBuilder);
            BuildNameFormatter = new CruiseControl.WebDashboard.Dashboard.DefaultBuildNameFormatter();
            
            LinkFactory = new  CruiseControl.WebDashboard.Dashboard.DefaultLinkFactory(UrlBuilder,CruiseUrlBuilder,BuildNameFormatter);
        
            return new CruiseControl.WebDashboard.Plugins.Statistics.BuildGraph(builds, LinkFactory, new Translations("en-US"));
        }

	}
}
