/*
This file contains all the graph configuration information.

The graphs are configured using the graph configurations array two arrays (recent and historic) that 
hold a list of graph option objects.  The option objects need to have the following attributes defined on them:
+ graphName: The name/title of the graph 
+ dataSource: an array of objects containg the graph source, usually either _summarisedStatistics or _recentStatistics
+ series: An array of series objects with the following attributes:
            + name: Series name
            + attributeName: The name of the attribute on the data source objects that will be used for the series
            + color: the color of the series

Optional attributes:
+ numXTicks: The number of X Ticks to display
+ numYTicks: The number of Y Ticks to display

The graphs are rendered in the order they appear in the array (if they contain data)

File dependencies:
  + StatisticsGraphs.js
*/

//Note that this does not include the tick at the original (so there will be an N+1 ticks on the graph)
var _numberRecentGraphXTicks = 4;
var _numberHistoricGraphXTicks = 4;

//This option will fill in zero summary stats for days when no builds occurred
var _shouldDisplayGraphsAsTimelines = false;

var _recentGraphConfigurations =
    [
        //Build Duration
        {
            graphName: "Build Duration",
            dataSource: _recentStatistics,
            numXTicks: _numberRecentGraphXTicks,
            series: [
                        { name: "Build Duration (seconds)", attributeName: "DurationInSeconds", color: "green" }
                    ]
        },
        
        //Tests
        {
            graphName: "Tests",
            dataSource: _recentStatistics,
            numXTicks: _numberRecentGraphXTicks,
            series: [
                        { name: "Passed", attributeName: "TestsPassed", color: "green" },
                        { name: "Failed", attributeName: "TestFailures", color: "red" },
                        { name: "Ignored", attributeName: "TestIgnored", color: "yellow" }
                    ]
        },
        
        //Coverage
        {
            graphName: "Test Coverage",
            dataSource: _recentStatistics,
            numXTicks: _numberRecentGraphXTicks,
            yRange: { lower: 0, upper: 100 },
            dataType: 'decimal',
            series: [
                        { name: "Coverage", attributeName: "Coverage", color: "blue" }
                    ]
        },

        //Gendarme
        {
        graphName: "Gendarme",
        dataSource: _recentStatistics,
        numXTicks: _numberRecentGraphXTicks,
        series: [
                        { name: "Gendarme Defects", attributeName: "GendarmeDefects", color: "red" }
                    ]
       },
        
        //FxCop
        {
            graphName: "FxCop",
            dataSource: _recentStatistics,
            numXTicks: _numberRecentGraphXTicks,
            series: [
                        { name: "Errors", attributeName: "FxCop Errors", color: "red" },
                        { name: "Warnings", attributeName: "FxCop Warnings", color: "yellow" }
                    ]
        },
        
        //Statements
        {
            graphName: "Statements",
            dataSource: _recentStatistics,
            numXTicks: _numberRecentGraphXTicks,
            series: [
                        { name: "Statements", attributeName: "Statements", color: "blue" }
                    ]
        },
        
        //Complexity
        {
            graphName: "Complexity",
            dataSource: _recentStatistics,
            numXTicks: _numberRecentGraphXTicks,
            series: [
                        { name: "Average Complexity", attributeName: "AverageComplexity", color: "blue" }
                    ]
        }
        
    ];

//Defines how the statistics will be summarised.  The summary function will be called for each
//property specified passing in both the Successful builds and Failed builds arrays.  The function
//must return a numeric value otherwise it will cause graph rendering issues etc.
var _summaryConfiguration =
    {
        //Success/failed counts
        successfulBuildCount: function(successfulBuilds, failedBuilds) { return count(successfulBuilds) },
        failedBuildCount: function(successfulBuilds, failedBuilds) { return count(failedBuilds) },
        
        //Duration
        averageBuildDuration: function(successfulBuilds, failedBuilds) { return average(successfulBuilds, "DurationInSeconds") },
        minBuildDuration: function(successfulBuilds, failedBuilds) { return min(successfulBuilds, "DurationInSeconds") },
        maxBuildDuration: function(successfulBuilds, failedBuilds) { return max(successfulBuilds, "DurationInSeconds") },
        
        //Coverage
        averageCoverage: function(successfulBuilds, failedBuilds) { return average(successfulBuilds, "Coverage") },
        
        //Tests
        testCount: function(successfulBuilds, failedBuilds) { return average(successfulBuilds, "TestCount") },
        testsPassed: function(successfulBuilds, failedBuilds) { return average(successfulBuilds, "TestsPassed") },
        testFailures: function(successfulBuilds, failedBuilds) { return average(successfulBuilds, "TestFailures") },
        testsIgnored: function(successfulBuilds, failedBuilds) { return average(successfulBuilds, "TestIgnored") },

        //Gendarme
        gendarmeDefects: function(successfulBuilds, failedBuilds) { return average(successfulBuilds, "GendarmeDefects") },
        
        //FxCop
        fxCopWarnings: function(successfulBuilds, failedBuilds) { return average(successfulBuilds, "FxCop Warnings") },
        fxCopErrors: function(successfulBuilds, failedBuilds) { return average(successfulBuilds, "FxCop Errors") },
        
        //Statements
        averageStatements: function(successfulBuilds, failedBuilds) { return average(successfulBuilds, "Statements") },
        
        //Complexity
        averageComplexity: function(successfulBuilds, failedBuilds) { return average(successfulBuilds, "AverageComplexity") }
        
    };

//Defines what historic graphs are displayed and their data
var _historicGraphConfigurations =
    [
        //Build Report
        {
            graphName: "Build Report",
            dataSource: _summarisedStatistics,
            numXTicks: _numberHistoricGraphXTicks,
            series: [
                        { name: "Successful Builds", attributeName: "successfulBuildCount", color: "green" },
                        { name: "Failed Builds", attributeName: "failedBuildCount", color: "red" }
                     ]
        },
    
        //Build Duration
        {
            graphName: "Build Duration",
            dataSource: _summarisedStatistics,
            numXTicks: _numberHistoricGraphXTicks,
            series: [
                        { name: "Max Duration (seconds)", attributeName: "maxBuildDuration", color: "blue" },
                        { name: "Average Duration (seconds)", attributeName: "averageBuildDuration", color: "green" }
                    ]
        },
        
        //Test Summary
        {
            graphName: "Test Summary",
            dataSource: _summarisedStatistics,
            numXTicks: _numberHistoricGraphXTicks,
            series: [
                        { name: "Average Tests Passed", attributeName: "testsPassed", color: "green" },
                        { name: "Average Test Failures", attributeName: "testFailures", color: "red" },
                        { name: "Average Tests Ignored", attributeName: "testsIgnored", color: "yellow" }
                    ]
        },
        
        //Coverage
        {
            graphName: "Test Coverage",
            dataSource: _summarisedStatistics,
            numXTicks: _numberHistoricGraphXTicks,
            dataType: 'decimal',
            yRange: { lower: 0, upper: 100 },
            series: [
                        { name: "Coverage", attributeName: "averageCoverage", color: "blue" }
                    ]
        },

        //Gendarme
        {
        graphName: "Gendarme",
        dataSource: _summarisedStatistics,
        numXTicks: _numberHistoricGraphXTicks,
        series: [
                        { name: "Average Defects", attributeName: "gendarmeDefects", color: "red" }
                    ]
        },
       
        //FxCop Errors/Warnings
        {
            graphName: "FxCop Errors/Warnings",
            dataSource: _summarisedStatistics,
            numXTicks: _numberHistoricGraphXTicks,
            series: [
                        { name: "Average Warnings", attributeName: "fxCopWarnings", color: "yellow" },
                        { name: "Average Errors", attributeName: "fxCopErrors", color: "red" }
                    ]
        },
        
        //Statements
        {
            graphName: "Statements",
            dataSource: _summarisedStatistics,
            numXTicks: _numberHistoricGraphXTicks,
            series: [
                        { name: "Average Statements", attributeName: "averageStatements", color: "blue" }
                    ]
        },
        
        //Complexity
        {
            graphName: "Complexity",
            dataSource: _summarisedStatistics,
            numXTicks: _numberHistoricGraphXTicks,
            series: [
                        { name: "Average Complexity", attributeName: "averageComplexity", color: "blue" }
                    ]
        }
    ];