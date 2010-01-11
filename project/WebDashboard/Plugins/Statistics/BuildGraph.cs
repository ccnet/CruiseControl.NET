using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Resources;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Statistics
{
	/// <summary>
	/// Provides functions for making a graph of the specified builds.
    /// These are HTML tables, so should not be browser specific.
	/// </summary>
    public class BuildGraph
    {
        private IBuildSpecifier[] mybuildSpecifiers;
        private ILinkFactory mylinkFactory;
        private Int32 myHighestAmountPerDay;

        private Int32 myOKBuildAmount;
        private Int32 myFailedBuildAmount;
        private Translations translations;
            
        public BuildGraph(IBuildSpecifier[] buildSpecifiers, ILinkFactory linkFactory, Translations translations)
        {
            mybuildSpecifiers = buildSpecifiers;
            mylinkFactory = linkFactory;
            this.translations = translations;
        }


        /// <summary>
        /// highest amount of builds found in 1 day for the entire graph
        /// used for calculating the height of the graph
        /// </summary>
        public Int32 HighestAmountPerDay
        {
            get
            {
                return myHighestAmountPerDay;
            }
        }


        /// <summary>
        /// Total amount of OK builds
        /// </summary>
        public Int32 AmountOfOKBuilds
        {
            get
            {
                return myOKBuildAmount;
            }
        }

        /// <summary>
        /// Total amount of failed builds
        /// </summary>
        public Int32 AmountOfFailedBuilds
        {
            get
            {
                return myFailedBuildAmount;
            }
        }

        public override bool Equals(object obj)
        {            
            if (obj.GetType() != this.GetType() )
                return false;

            BuildGraph Comparable = obj as BuildGraph;

            if (this.mybuildSpecifiers.Length != Comparable.mybuildSpecifiers.Length)
                {return false; }
        

            for (int i=0; i < this.mybuildSpecifiers.Length ; i++)
            {
                if (! this.mybuildSpecifiers[i].Equals(Comparable.mybuildSpecifiers[i]) )
                {return false; }
            }

            return true;
        }

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (mybuildSpecifiers != null) hashCode += 1000000007 * mybuildSpecifiers.GetHashCode(); 
				if (mylinkFactory != null) hashCode += 1000000009 * mylinkFactory.GetHashCode(); 
				hashCode += 1000000021 * myHighestAmountPerDay.GetHashCode();
				hashCode += 1000000033 * myOKBuildAmount.GetHashCode();
				hashCode += 1000000087 * myFailedBuildAmount.GetHashCode();
			}
			return hashCode;
		}
        
        /// <summary>
        //Returns a sorted list containing build information per buildday
        /// </summary>
        public ArrayList GetBuildHistory(Int32 maxAmountOfDays)
        {
            ArrayList Result;
            ArrayList DateSorter;
            Hashtable FoundDates;
            GraphBuildInfo CurrentBuildInfo;
            GraphBuildDayInfo CurrentBuildDayInfo;


            // adding the builds to a list per day
            FoundDates = new Hashtable();
            DateSorter = new ArrayList();

            foreach (IBuildSpecifier buildSpecifier in mybuildSpecifiers)
            {           
                CurrentBuildInfo = new GraphBuildInfo(buildSpecifier, mylinkFactory);

                if (!FoundDates.Contains(CurrentBuildInfo.BuildDate()))
                {
                    FoundDates.Add(CurrentBuildInfo.BuildDate(), new GraphBuildDayInfo(CurrentBuildInfo, this.translations) );
                    DateSorter.Add(CurrentBuildInfo.BuildDate());
                }
                else
                {
                    CurrentBuildDayInfo = FoundDates[CurrentBuildInfo.BuildDate()] as GraphBuildDayInfo;
                    CurrentBuildDayInfo.AddBuild(CurrentBuildInfo);

                    FoundDates[CurrentBuildInfo.BuildDate()] = CurrentBuildDayInfo;
                }                            
            }
 
            //making a sorted list of the dates where we have builds of
            //and limit to the amount specified in maxAmountOfDays
            DateSorter.Sort();
            while (DateSorter.Count > maxAmountOfDays)
            {
                DateSorter.RemoveAt(0);
            }

            //making final sorted arraylist
            Result = new ArrayList();
            myHighestAmountPerDay = 1;

            foreach (DateTime BuildDate in DateSorter)
            {
                CurrentBuildDayInfo = FoundDates[BuildDate] as GraphBuildDayInfo;
                Result.Add(CurrentBuildDayInfo);            

                if (CurrentBuildDayInfo.AmountOfBuilds > myHighestAmountPerDay) 
                {
                    myHighestAmountPerDay = CurrentBuildDayInfo.AmountOfBuilds; 
                }

                myOKBuildAmount += CurrentBuildDayInfo.AmountOfOKBuilds;
                myFailedBuildAmount += CurrentBuildDayInfo.AmountOfFailedBuilds;
            }

            return Result;
        }



        /// <summary>
        // Information about a certain build 
        // Wrapper around existing functions for ease of use in template
        /// </summary>
        public class GraphBuildInfo
        {
            private IBuildSpecifier mybuildSpecifier;
            private ILinkFactory mylinkFactory;

            public GraphBuildInfo(IBuildSpecifier buildSpecifier,  ILinkFactory linkFactory)
            {
                mybuildSpecifier = buildSpecifier;
                mylinkFactory = linkFactory;
            }
        
            //returns the day of the build (no time specification)            
            public DateTime BuildDate()
            {
                return new LogFile(mybuildSpecifier.BuildName).Date.Date;
            }

            public bool IsSuccesFull()
            {
                return new LogFile(mybuildSpecifier.BuildName).Succeeded;
            }

            public string LinkTobuild()
            {
                return mylinkFactory.CreateBuildLink( 
                    mybuildSpecifier,BuildReportBuildPlugin.ACTION_NAME).Url;                
            }

            public string Description()
            {               
                DefaultBuildNameFormatter BuildNameFormatter;
                BuildNameFormatter = new DefaultBuildNameFormatter();
                return BuildNameFormatter.GetPrettyBuildName(mybuildSpecifier);
            }

        }

        /// <summary>
        // structure containing all the builds on 1 day (YYYY-MM-DD)
        /// </summary>
        public class GraphBuildDayInfo
        {
            private DateTime myBuildDate; 
            private ArrayList myBuilds;

            private Int32 myOKBuildAmount;
            private Int32 myFailedBuildAmount;
            private Translations translations;

            public GraphBuildDayInfo(GraphBuildInfo buildInfo, Translations translations)
            {
                this.translations = translations;
                myBuildDate = buildInfo.BuildDate();
                myBuilds = new ArrayList();
                //myBuilds.Add(buildInfo);
                AddBuild(buildInfo);
            }

            
            //returns the day of the builds contained
            public DateTime BuildDate
            {
                get 
                { 
                    return myBuildDate; 
                }
            }

            public string BuildDateFormatted
            {
                get 
                {
                    return myBuildDate.Date.ToString("ddd", this.translations.UICulture)
                           + "<BR>" 
                           + myBuildDate.Year.ToString("0000") 
                           + "<BR>" 
                           + myBuildDate.Month.ToString("00")
                           + "<BR>"
                           + myBuildDate.Day.ToString("00"); 
                }
            }


            // the amount of builds in this day
            public Int32 AmountOfBuilds
            {
                get
                {
                    return myBuilds.Count;
                }
            }


            // the amount of ok builds in this day
            public Int32 AmountOfOKBuilds
            {
                get
                {
                    return myOKBuildAmount;
                }
            }

            // the amount of failed builds in this day
            public Int32 AmountOfFailedBuilds
            {
                get
                {
                    return myFailedBuildAmount;
                }
            }

            //retrieves a specific build in this day
            public GraphBuildInfo Build(Int32 index)
            {
                return myBuilds[index] as GraphBuildInfo;
            }

            // adds a build to this day
            public void AddBuild(GraphBuildInfo buildInfo)
            {
                myBuilds.Insert(0, buildInfo);
                if (buildInfo.IsSuccesFull())
                {
                    myOKBuildAmount++;
                }
                else
                {
                    myFailedBuildAmount++;
                }
            }
        }
	}
}
