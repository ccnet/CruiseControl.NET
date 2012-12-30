/*
Copyright (c) 2007, the Eden Ridgway
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

    1. Redistributions of source code must retain the above copyright notice,
       this list of conditions and the following disclaimer.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

/*
This file contains the main control logic for the generation of the 
statistics graphs.  It summarises the data, creates tabs, graphs and
tables when necessary (some on demand).

File dependencies:
  + AggregateFunctions.js
  + GraphWrapper.js
  + StatisticsTables.js
*/

var version = "2.7";

var _summarisedStatistics = [];
var _recentStatistics = [];

//Variables used for delayed rendering of graphs to improve initial page load performance
var _hasDetailedTableBeenRendered = false;
var _hasSummaryTableBeenRendered = false;
var _haveHistoricGraphsBeenRendered = false;

var GraphTab = {
                  Recent: 1,
                  Historic: 2
               };

function convertTimeIntoSeconds(time)
/// <summary>
/// Converts a time in the format HH:mm:ss into seconds
/// </summary>
{
    var timeParts = time.split(":");
    return timeParts[0] * 3600 + timeParts[1] * 60 + parseInt(timeParts[2], 10);
}

function summariseStatistics()
/// <summary>
/// Summarises the build statistics per day
/// </summary>
{
    var lastDate = "";
    var statsGroupedByDay = {};
    
    //If there are no days then don't bother trying to summarise the statistics
    if (_statistics.length == 0)
    {
        return statsGroupedByDay;
    }
    
    //This days for which to generate statistics for
    var projectDays = distinct(_statistics, "Date");
    
    //Create a statistic summary placeholder for each day
    for (var dayIndex = 0; dayIndex < projectDays.length; dayIndex++)
    {
        var currentDate = projectDays[dayIndex];
        
        if (typeof(currentDate) == 'string')
        {
            currentDate = new Date(currentDate);
        }
        
        statsGroupedByDay[currentDate.toDateString()] = [];
    }
    
    var currentStatistic = [];
        
    //Group the _statistics by day in as array properties on the _statistics object (_statistics in not an array)
    for (var i = 0; i < _statistics.length; i++)
    {
        var statistic = _statistics[i];
        
        var statisticsDate = new Date(statistic.Date);
        var dayText = statisticsDate.toDateString();
        statsGroupedByDay[dayText].push(statistic);
    }
        
    return generateDailySummaries(statsGroupedByDay);
}

function getTimelineDays()
/// <summary>
/// Generate a range of dates over which to iterate so that missing days end up 
/// being filled in with zero values later.
/// </summary>
{
    var firstDate = new Date(_statistics[0].Date);
    var lastDate = new Date(_statistics[_statistics.length - 1].Date);
    
    return generateDateRange(firstDate, lastDate);
}

function prepareStatistics()
/// <summary>
/// Prepares the CC statistics by adding any additional properties to the statistics objects that may
/// be needed when summarising or displaying recent build data.  It also cleans the data so that there
/// are no non numeric values that are being summarised and displayed.
/// </summary>
{
    //Determine which statistic values are being used on the recent and summary graphs and ensure that they
    //do not contain non-numeric data
    var usedStats = getUsedStatisticAttributes();
    
    for (var i = 0; i < _statistics.length; i++)
    {
        var statistic = _statistics[i];
        
        //Prepare standard statistics
        statistic["index"] = i;
        statistic["DurationInSeconds"] = convertTimeIntoSeconds(statistic["Duration"]);
        statistic["TestsPassed"] = statistic["TestCount"] - statistic["TestFailures"] - statistic["TestIgnored"];
        
        //Prepare custom stats
        for (var attributeIndex = 0; attributeIndex < usedStats.length; attributeIndex++)
        {
            var attributeName = usedStats[attributeIndex];
            statistic[attributeName] = zeroIfInvalid(statistic[attributeName]);
        }
    }
}

function getUsedStatisticAttributes()
/// <summary>
/// Returns an array of all the attributes on the statistics object that are used (as defined in GraphConfiguration)
/// </summary>
{
    var usedStats = {};
    
    for (var configIndex = 0; configIndex < _recentGraphConfigurations.length; configIndex++)
    {
        var config = _recentGraphConfigurations[configIndex];
        
        for (var seriesIndex = 0; seriesIndex < config.series.length; seriesIndex++)
        {
            var series = config.series[seriesIndex];
            usedStats[series.attributeName] = '';
        }
    }
    
    //Convert the stats object into an array
    var attributes = [];
    
    for (var attribute in usedStats)
    {
        attributes.push(attribute);
    }
    
    return attributes;
}

function zeroIfInvalid(dataItem)
/// <summary>
/// Returns zero if the item is empty or undefined
/// </summary>
{
    if (dataItem == '' || typeof(dataItem) == 'undefined' || isNaN(dataItem))
    {
        return '0';
    }
    else
    {
        return dataItem;
    }
}

function getRecentStatistics(numberOfBuilds)
/// <summary>
/// Gets the last N number of builds (if there are that many)
/// </summary>
{
    var startIndex = Math.max(_statistics.length - numberOfBuilds, 0);
        
    //Create an array with the duration of the last N builds
    for (var i = startIndex; i < _statistics.length; i++)
    {
        var clonedStatistic = cloneObject(_statistics[i]);
                          
        clonedStatistic["index"] = _recentStatistics.length;
        clonedStatistic["label"] = clonedStatistic["BuildLabel"];
        
        _recentStatistics.push(clonedStatistic);
    }
}

function cloneObject(sourceObject)
/// <summary>
/// Creates a copy of the source object.  It does not clone the attributes on the object.
/// </summary>
{
    var clone = {};
                      
    for (var attribute in sourceObject)
    {
        clone[attribute] = sourceObject[attribute];
    }
    
    return clone;
}

function generateDateRange(startDate, endDate)
/// <summary>
/// Generates an array with dates between (inclusive) the start and end date
/// </summary>
{
    var dayDifference = 24 * 60 * 60 * 1000;
    var currentDate = startDate;
    var dateRange = [];
    
	//Ensure that daylight savings doesn't cause a problem with the end date
	endDate.setHours(23);
	endDate.setMinutes(59);
	
    while (currentDate <= endDate)
    {
        dateRange.push(currentDate);
        currentDate = new Date(currentDate.getTime() + dayDifference);
    }
    
    return dateRange;
}

function generateDailySummaries(statsGroupedByDay)
/// <summary>
/// Get the min max or average of the statistics per day
/// </summary>
{
    var lastBuildLabel = "";
    var index = 0;
    
    //Summarise the daily stats
    //Note that the day attributes on the statsGroupedByDay object are returned in insert order which will be chronologically
    for (var day in statsGroupedByDay)
    {
        var currentStatistics = statsGroupedByDay[day];
        
        var currentBuildLabel = getLastValue(currentStatistics, "BuildLabel");
        
        //Ensure that days without any stats still display the correct last build label
        if (currentBuildLabel.length == 0)
        {
            currentBuildLabel = lastBuildLabel;
        }
        
        var successfulBuilds = select(currentStatistics, successfulBuildsFilter);
        var failedBuilds = select(currentStatistics, failedBuildsFilter);
        
        var daySummary = {
                            day: day,
                            index: index++,
                            lastBuildLabel: currentBuildLabel
                         };
        
        //Add all the configured summary configuration entries by calling the summary function defined for each
        for (var attribute in _summaryConfiguration)
        {
            daySummary[attribute] = _summaryConfiguration[attribute](successfulBuilds, failedBuilds);
        }
        
        var dayDate = new Date(day);
        //daySummary.label = daySummary.lastBuildLabel + "\n(" + dayDate.getFullYear() + "/" + (dayDate.getMonth() + 1) + "/" + dayDate.getDate() + ")";
        daySummary.label = daySummary.lastBuildLabel + "\n(" + day + ")";
        
        _summarisedStatistics.push(daySummary);
        
        lastBuildLabel = currentBuildLabel;
    }
}

function successfulBuildsFilter(item)
/// <summary>
/// A predicate that returns the successful builds
/// </summary>
{
    return (item["Status"] == "Success");
}

function failedBuildsFilter(item)
/// <summary>
/// A predicate that returns the failed builds
/// </summary>
{
    var status = item["Status"];
    
    return (status == "Failure" || status == "Exception");
}
                                 
function processGraphList(configurationList, containerElement)
/// <summary>
/// Creates the graphs specified using the option list supplied
/// </summary>
{
    for (var i = 0; i < configurationList.length; i++)
    {
        var graphOptions = configurationList[i];
        graphOptions.containerElement = containerElement;
        createGraph(graphOptions);
    }
}

function createRecentGraphs()
/// <summary>
/// Creates the recent build statistics graphs
/// </summary>
{
    processGraphList(_recentGraphConfigurations, dojo.byId("RecentBuildsContainerArea"));
    
    //Add any non-standard (i.e. graphs that are not simple area fill graphs) here
}

function createHistoricGraphs()
/// <summary>
/// Creates the historic build statistics graphs
/// </summary>
{
    processGraphList(_historicGraphConfigurations, dojo.byId("HistoricGraphContainerArea"));
    
    //Add any non-standard (i.e. graphs that are not simple area fill graphs) here
}

function summaryDataTabChangeHandler() 
/// <summary>
/// If the tables for the tab have not been rendered then renders them
/// </summary>
{
    if (!_hasSummaryTableBeenRendered)
    {
        ensureStatisticsHaveBeenSummarised();
        
        var tableContainerArea = dojo.byId("SummaryTableStatisticsContainerArea");
        generateStatisticsTable(tableContainerArea, "Build Summary Statistics", _summarisedStatistics, cellRenderer, true, summaryTableDrillDown);
        
        _hasSummaryTableBeenRendered = true;
    }
}

function detailedDataTabChangeHandler()
/// <summary>
/// If the tables for the tab have not been rendered then renders them
/// </summary> 
{
    if (!_hasDetailedTableBeenRendered)
    {
        var tableContainerArea = dojo.byId("DetailedTableStatisticsContainerArea");
        generateStatisticsTable(tableContainerArea, "Build Detailed Statistics", _statistics, cellRenderer, false, null);
        
        _hasDetailedTableBeenRendered = true;
    }
}

function historicGraphsTabChangeHandler(evt) 
/// <summary>
/// If the historic graphs have not been rendered then renders them
/// </summary>
{
    if (!_haveHistoricGraphsBeenRendered)
    {
        ensureStatisticsHaveBeenSummarised();
        
        createHistoricGraphs();
        _haveHistoricGraphsBeenRendered = true;
    }
}

function ensureStatisticsHaveBeenSummarised()
/// <summary>
/// Makes sure that the statistics have been summarised
/// </summary>
{
    if (_summarisedStatistics.length == 0)
    {
        summariseStatistics();
    }
}

function setupLazyTabInitialization()
/// <summary>
/// To improve performance sets up hooks that allow for the tab contents to be rendered only
/// if the user wants to view it.
/// </summary> 
{
    var historicalTabWidget = dojo.widget.byId("HistoricalTabWidget");
    var detailedTabularTabWidget = dojo.widget.byId("DetailedDataTabWidget");
    var summarisedTabularTabWidget = dojo.widget.byId("SummarisedDataTabWidget");
    
    dojo.event.connect("before", historicalTabWidget, "show", historicGraphsTabChangeHandler);
    dojo.event.connect("before", detailedTabularTabWidget, "show", detailedDataTabChangeHandler);
    dojo.event.connect("before", summarisedTabularTabWidget, "show", summaryDataTabChangeHandler);
}

    
//Setup the chart to be added to the DOM on load
dojo.addOnLoad(function()
               {
                   prepareStatistics();
                   getRecentStatistics(20);
                   setupLazyTabInitialization();
                   window.setTimeout("createRecentGraphs()", 100);
               });