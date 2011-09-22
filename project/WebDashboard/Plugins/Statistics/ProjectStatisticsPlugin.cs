using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Statistics
{
    /// <summary>
    /// The Project Statistics Plugin displays the statistics for the project. 
    /// Since version 1.4 the graphs of Eden Ridgway are incorporated into CCNet.
    /// <para>
    /// LinkDescription : View Statistics.
    /// </para>
    /// <example>
    /// <code title="the old statistics page of CCNet">
    /// &lt;projectStatisticsPlugin xslFileName="xsl\statistics.xsl" /&gt;      
    /// </code>
    /// <code title="the new statistics page with the graphs of Eden, this is the default of CCNet now">
    /// &lt;projectStatisticsPlugin xslFileName="xsl\StatisticsGraphs.xsl" /&gt;  
    /// </code>
    /// </example>
    /// <heading>
    /// Configuration of the graphs
    /// </heading>
    /// <code>
    /// For a full reference, you can check out Eden's Blog at : 
    /// http://ridgway.co.za/archive/2007/05/21/adding-custom-graphs-to-the-cruisecontrol.net-statistics-replacement.aspx
    /// Below the information from Eden's Blog :
    /// 
    /// In version 2.7 of CCNet graphs I made some further improvements to the manner in which graphs are configured.  
    /// Now any customisations that need to be done are housed in the GraphConfiguration.jsfile (in the webdashboard\javascript folder).  
    /// The logic in the graph generation is now also resilient to problems such as blank or non-numeric data in the custom nodes.
    /// 
    /// If you want to include custom data in your report you should read the Statistics Publisher Wiki page 
    /// to get the it included in the report.xml file (which you will find in your artifacts folder for each project).
    /// 
    /// *Configuration Setup*
    /// 
    /// There are 3 areas of configuration, the:
    /// 
    ///     * Recent Graphs Tab- the details of which are stored in the _recentGraphConfigurations array of configuration/option objects.  
    ///        The datasource of each refers to the _recentStatistics object array, that contains up to the last 20 build statistics.
    ///     * Manner in which Summary Data is calculated- this is stored in the _summaryConfiguration object that contains 
    ///        functions that accept successful and failed build arrays for a day and return an numeric value for each.
    ///     * Historic Builds Tab- is configured in the same way as the Recent Graphs Tab, 
    ///        except is defined in the _historicGraphConfigurations array and each configuration object uses the _summarisedStatistics datasource.  
    ///        This datasource contains, the item index, DurationInSeconds, TestsPassed (calculated properties not present in the report.xml file) 
    ///        and the summary data properties defined in _summaryConfiguration.
    /// 
    /// Example Customisation
    /// 
    /// Lets say that one wanted to add a complexity graph to the recent and historic tabs, the changes required would be this:
    /// 
    ///     One would edit the _recentGraphConfigurations array and add the following (assuming that the xml element node 
    ///      [in the report.xml file] that contains the data is called AverageComplexity):
    ///     var _recentGraphConfigurations =
    ///         [
    ///             //... Other configuration objects excluded for brevity
    ///             {
    ///                 graphName: "Complexity",
    ///                 dataSource: _recentStatistics,
    ///                 numXTicks: _numberRecentGraphXTicks,
    ///                 series: [
    ///                          { name: "Average Complexity", attributeName: "AverageComplexity", color: "blue" {color:#000000}}
    ///                         ]
    ///             }
    ///         ];
    /// 
    ///     Then the manner in which you want to summarise the data on a daily basis needs to be defined in summaryConfiguration.  
    ///     Note that all the standard summary functions are contained in the QueryFunctions.js file and include methods such as 
    ///     (getLastValue, select, distinct, sum, average, count, min and max).  
    ///     Say we wanted to display the average complexity value for the day, the configuration would be defined like this:
    ///     var _summaryConfiguration =
    ///         {
    ///             //Other attributes...
    ///             averageComplexity: function(successfulBuilds, failedBuilds) { return average(successfulBuilds, "AverageComplexity") }
    ///         };
    ///         
    ///     Lastly the summarised data collected by the function above for each day must be configured for the history tab, like this:
    ///     var _historicGraphConfigurations =
    ///         [
    ///             //Other configuration objects...
    ///             {
    ///                 graphName: "Complexity",
    ///                 dataSource: _summarisedStatistics,
    ///                 numXTicks: _numberHistoricGraphXTicks,
    ///                 series: [
    ///                            { name: "Average Complexity", attributeName: "averageComplexity", color: "blue" {color:#000000}}
    ///                         ]
    ///             }
    ///         ];
    ///     Note how the attribute name here corresponds to the attribute name defined in _summaryConfiguration and not the 
    ///     original statistic configuration element.
    /// </code>
    /// </summary>
    /// <title>Project Statistics Plugin</title>
    [ReflectorType("projectStatisticsPlugin")]
    public class ProjectStatisticsPlugin : ICruiseAction, IPlugin
    {
        public static readonly string ACTION_NAME = "ViewStatisticsReport";
        private readonly IFarmService farmService;
        private readonly IPhysicalApplicationPathProvider pathProvider;
        private readonly ITransformer transformer;
        private string xslFileName;

        public ProjectStatisticsPlugin(IFarmService farmService, IPhysicalApplicationPathProvider pathProvider)
        {
            this.farmService = farmService;
            this.pathProvider = pathProvider;
            transformer = new XslTransformer();
        }

        [ReflectorProperty("xslFileName")]
        public string XslFileName
        {
            get { return xslFileName; }
            set { xslFileName = value; }
        }

        #region ICruiseAction Members

        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            if (xslFileName == null)
            {
                throw new ApplicationException("XSL File Name has not been set for XSL Report Action");
            }
            Hashtable xsltArgs = new Hashtable();
            xsltArgs["applicationPath"] = cruiseRequest.Request.ApplicationPath;

            string xslFile = pathProvider.GetFullPathFor(XslFileName);
            string statisticsDocument = farmService.GetStatisticsDocument(cruiseRequest.ProjectSpecifier, cruiseRequest.RetrieveSessionToken());
            Log.Debug(statisticsDocument);
            string htmlFragment;
            try
            {
                htmlFragment = transformer.Transform(statisticsDocument, xslFile, xsltArgs);
            }
            catch (CruiseControlException)
            {
                htmlFragment = "Missing/Invalid statistics reports. Please check if you have enabled the Statistics Publisher, and statistics have been collected atleast once after that.";
            }
            return new HtmlFragmentResponse(htmlFragment);
        }

        #endregion

        #region IPlugin Members

        public INamedAction[] NamedActions
        {
            get { return new INamedAction[] {new ImmutableNamedAction(ACTION_NAME, this)}; }
        }

        public string LinkDescription
        {
            get { return "View Statistics"; }
        }

        #endregion
    }
}