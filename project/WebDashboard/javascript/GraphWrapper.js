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
This file contains a wrapper around the dojo graphing library which makes
it easier to add new statistics graphs.
*/

var Browser =   {
                  isIE:     !!(window.attachEvent && !window.opera),
                  isOpera:  !!window.opera,
                  isWebKit: navigator.userAgent.indexOf('AppleWebKit/')> -1,
                  isGecko:  navigator.userAgent.indexOf('Gecko')> -1 &&
                            navigator.userAgent.indexOf('KHTML') == -1
                };

var AxisType = {
                  x: 1,
                  y: 2
               };


//Include the required dojo libraries/namespaces
dojo.require("dojo.collections.Store");
dojo.require("dojo.collections.Queue");
dojo.require("dojo.charting.Chart");
dojo.require('dojo.json');

function Graph()
/// <summary>
/// Graph object that wraps the rendering of the graph and handles elements like the legend and ticks.
/// </summary>
{
                            
    var graphContainerArea = null;
    var graphContainer = null;
    var legendTable = null;
    var legendContainer = null;
    var heading = null;
    var store = new dojo.collections.Store();
    var dataSource = null;
    
    //Array that contains all the series arrays
    var series = [];
    var xTicks = [];
    var yAxisAttributeNames = [];
    
    this.numXTicks = 5;
    this.numYTicks = 5;
    this.yRange = null;
    this.xRange = null;
    this.dataType = 'integer';
    
    var determineRange = function(attributeName)
    /// <summary>
    /// Determines the upper and lower bounds for the summary data attribute
    /// </summary>
    {
        var range = { 
                        lower: min(dataSource, attributeName), 
                        upper: max(dataSource, attributeName) 
                    };

        return range;
    }

    this.determineMultiSourceRange = function(attributeNames)
    /// <summary>
    /// Using all the series values calculates the upper and lower
    /// bounds for the graph's y-axis
    /// </summary>
    {
         var ranges = dojo.lang.map(attributeNames, determineRange);
         
         return { lower: min(ranges, "lower"), upper: max(ranges, "upper") };
    }
    
    this.setDataSource = function(graphDataSource)
    /// <summary>
    /// Sets the graph's data source
    /// </summary>
    {
        store.setData(graphDataSource);
        dataSource = graphDataSource;
    }
    
    this.setContainer = function(containerElement)
    /// <summary>
    /// Sets the graph container and creates the heading and legend boxes
    /// </summary>
    {
        //If the container element is being changed then clear out the old legend and heading elements
        if (graphContainerArea != null && graphContainerArea != containerElement)
        {
            document.removeElement(heading);
            document.removeElement(legendContainer);
            document.removeElement(graphContainer);
        }
        
        graphContainerArea = containerElement;
        
        //Create the heading element
        function createContainerElements()
        {
            heading = document.createElement("h2");
            graphContainerArea.appendChild(heading);
            
            //Create a container for the "canvas"
            graphContainer = document.createElement("div");
            graphContainer.className = "GraphContainer";
            graphContainer.innerHTML = "Loading...";
            
            graphContainerArea.appendChild(graphContainer);
            
            //Create the legend box
            legendContainer = document.createElement("div");
            legendContainer.className = "Legend";
            graphContainerArea.appendChild(legendContainer);
            
            legendTable = document.createElement("table");
            legendContainer.appendChild(legendTable);
        }
        
        createContainerElements();
    }

    this.generatexAxisTickMarks = function(labelAttribute, numberTicks)
    /// <summary>
    /// Creates the xAxis tick marks based on the target label attribute
    /// </summary>
    /// <remarks>
    /// Get the ticks from the actual data source as opposed to deriving it from the range
    /// </remarks>
    {
        //Ensure that if there are too few data points then we only use the number available
        numberTicks = Math.min(numberTicks, dataSource.length);
        
        var labels = [];
        var tickIndexDelta = parseInt(dataSource.length / numberTicks);
        var tickIndex = 0;
        
        for (var i = 0; i <= dataSource.length; i += tickIndexDelta)
        {
            var tickIndex = Math.min(dataSource.length - 1, Math.round(i));
            var tickLabel = dataSource[tickIndex][labelAttribute].toString();
            
            labels.push({ label: tickLabel, value: tickIndex });
        }
        
        return labels;
    }
    
    this.generateyAxisTickMarks = function(range, numberTicks)
    /// <summary>
    /// Creates the yAxis tick marks based on the graph value range
    /// </summary>
    {
        var labels = [];
        var rangeDelta = range.upper - range.lower;
        var tickDelta = rangeDelta / numberTicks;
        var tickLabel;
        var tickDataType = this.dataType;
        
        //Make certain that the tick marks don't remain the same number for multiple ticks because they have
        //been truncated.
        if (tickDelta < 1)
        {
            tickDataType = 'decimal';
        }
        
        for (var tickValue = range.lower; tickValue <= range.upper; tickValue += tickDelta)
        {
            if (tickDataType == 'integer')
            {
                tickLabel = parseInt(tickValue);
            }
            //Round to 2 decimal places
            else
            {
                tickLabel = Math.round(tickValue * 100) / 100;
            }
                        
            labels.push({ label: tickLabel.toString(), value: tickValue });
        }
        
        return labels;
    }
    
    this.createAxis = function(axisValueAttributeNames, tickAttributeName, numberTicks, axisType)
    /// <summary>
    /// Creates an axis for the summary data using the attribute values in the summary data
    /// </summary>
    /// <remarks>
    /// Note that the number of ticks excludes the tick at the origin, so there will end
    /// up being N+1 ticks
    /// </remarks>
    {
        var axis = new dojo.charting.Axis();
        axis.showTicks = true;
        axis.showLines = true;
        axis.label = "";
        
        //Set the upper and lower data range values based on the number of days being viewed
        if (axisType == AxisType.x)
        { 
           axis.origin = "max";
           axis.range = this.xRange || this.determineMultiSourceRange(axisValueAttributeNames);
        }
        else if (axisType == AxisType.y)
        {
           axis.origin = "min";
           axis.range = this.yRange || this.determineMultiSourceRange(axisValueAttributeNames);
        }
        else
        {
           alert('Invalid axis type specified');
           return axis;
        }
        
        //If there are no values then don't bother
        if (dataSource.length == 0)
        {
           return axis;
        }
        
        var rangeDelta = axis.range.upper - axis.range.lower;
        
        //If there isn't more than one tick mark then simply add that to the graph
        if (rangeDelta == 0)
        {
           axis.range.lower = 0;
           
           axis.labels.push({ label: "0", value: 0 });
           axis.labels.push({ label: axis.range.upper.toString(), value: axis.range.upper });
           return axis;
        }
        
        //Use specialised tick generation logic instead of a generic approach
        if (axisType == AxisType.x)
        { 
           axis.labels = this.generatexAxisTickMarks(tickAttributeName, numberTicks);
        }
        else if (axisType == AxisType.y)
        {
           axis.labels = this.generateyAxisTickMarks(axis.range, numberTicks);
        }
        
        //The ticks in IE are normal HTML elements and therefore we can break the tick line appropriately
        if (Browser.isIE)
        {
            for (var i = 0; i < axis.labels.length; i++)
            {
                  axis.labels[i].label = axis.labels[i].label.replace("\n", "<br/>");
            }   
        }
        
        return axis;
    }
    
    this.setTitle = function(title)
    /// <summary>
    /// Sets the graph's title
    /// </summary>
    {
        heading.innerHTML = title;
    }
    
    this.addSeries = function(seriesName, seriesAttributeName, color)
    /// <summary>
    /// Adds the series values that will be used to draw the graph
    /// </summary>
    {
        //Store the attribute being used to create the series values to be used later to determine the axis range
        yAxisAttributeNames.push(seriesAttributeName);

        var seriesItem = new dojo.charting.Series({
                                                      dataSource: store,
                                                      bindings: { x: "index", y: seriesAttributeName },
                                                      label: seriesName,
                                                      color: color
                                                  });
                               
        series.push(seriesItem);

        //Add the legend information for the series
        var legendRow = legendTable.insertRow(legendTable.rows.length);
        var legendBoxCell = legendRow.insertCell(0);
        var legendTextCell = legendRow.insertCell(1);

        legendBoxCell.style.width = "14px";
        var colorBoxDiv = document.createElement("div");
        colorBoxDiv.className = "ColorBox";
        colorBoxDiv.style.backgroundColor = color;
        legendBoxCell.appendChild(colorBoxDiv);
        legendTextCell.innerHTML = seriesName;
    }
    
    this.draw = function() 
    /// <summary>
    /// Draws the graph using the supplied series
    /// </summary> 
    {
        var layoutOptions = {};
        
        //TODO: Since the x-axis does not change between graphs, this could be a global value that is reused
        var xAxis = this.createAxis([ "index" ], "label", this.numXTicks, AxisType.x);
        var yAxis = this.createAxis(yAxisAttributeNames, "", this.numYTicks, AxisType.y);
        
        //Create the actual graph with the x and y axes defined above
        var chartPlot = new dojo.charting.Plot(xAxis, yAxis);
        
        dojo.lang.forEach(series, function(seriesItem)
                                  {
                                      chartPlot.addSeries({ 
                                                              data: seriesItem, 
                                                              plotter: dojo.charting.Plotters.CurvedArea 
                                                          });
                                  });
        
        //Define the plot area
        var chartPlotArea = new dojo.charting.PlotArea();
        chartPlotArea.size = { width: 600, height: 250 };
        chartPlotArea.padding = { top: 20, right: 20, bottom: 50, left: 50 };
        
        //Add the plot to the area 
        chartPlotArea.plots.push(chartPlot);
        
        //Create the actual chart "canvas"
        var chart = new dojo.charting.Chart(null, "Statistics Chart", "");
        
        //Add the plot area at an offset of 10 pixels from the top left
        chart.addPlotArea({ x: 10, y: 10, plotArea: chartPlotArea });
        
        chart.node = graphContainer;
        chart.render();
    }
}

function getArrayOfAttributeValues(sourceArray, attributeName)
/// <summary>
/// Determines whether or not any of the values in the supplied series have any values
/// </summary>
{
    var attributeValueArray = [];
    
    for (var i = 0; i < sourceArray.length; i++)
    {
        attributeValueArray.push(sourceArray[attributeName]);
    }
    
    return attributeValueArray;
}

function hasSeriesValues(dataSource, series)
/// <summary>
/// Determines whether or not any of the values in the supplied series have any values
/// </summary>
{
    //Check for just one non-zero value in the series
    for (var index = 0; index < dataSource.length; index++)
    {
        var statistic = dataSource[index];
        
        for (var i = 0; i < series.length; i++)
        {
            var value = zeroIfInvalid(statistic[series[i].attributeName]);
            
            if (value != 0)
            {
                return true;
            }
        }
    }
    
    return false;
}

function createGraph(options)
/// <summary>
/// Creates a graph using the option object supplied as the template.
/// </summary>
/// <remarks>
/// A graph will not be created if all the y-axis values for the series are zero.
/// It now also places a "loading" placeholder for the graph and queues it to be rendered.
/// </remarks>
{
    //Don't render blank graphs or graphs with just one value
    if (options.dataSource.length < 2 || !hasSeriesValues(options.dataSource, options.series))
    {
        return;
    }
    
    var dataSource = options.dataSource;
    var graphContainer = options.containerElement;
    
    var graph = new Graph();
    
    graph.setDataSource(dataSource);
    graph.setContainer(graphContainer);
    graph.setTitle(options.graphName);
    graph.numXTicks = options.numXTicks || 5;
    graph.numYTicks = options.numYTicks || 5;
    graph.dataType = options.dataType || 'integer';
    graph.xRange = options.xRange;
    graph.yRange = options.yRange;
    
    var series = {};
    
    if (typeof(options.chartType) != 'undefined')
    {
        graph.chartType = options.chartType;
    }
    
    //Create a placeholder array for each series that will be generated
    for (var i = 0; i < options.series.length; i++)
    {
        var seriesItem = options.series[i];
        graph.addSeries(seriesItem.name, seriesItem.attributeName, seriesItem.color);
    }
    
    //Add the graph to the rendering queue
    _graphProcessingQueue.enqueue(graph, graph.draw);
    
    return graph;
}

function ProcessingQueue()
/// <summary>
/// Processes requests one after each other allowing the UI to render any changes before starting
/// the next item.
/// </summary>
{
    this.queue = new dojo.collections.Queue();
    this.isTimerActive = false;
    
    this.enqueue = function(targetObject, targetFunction)
    /// <summary>
    /// Queues the target object for processing and calls the target function on the object
    /// when it is processed.
    /// </summary>
    {
        this.queue.enqueue({ 
                              targetObject: targetObject, 
                              targetFunction: targetFunction 
                           });
        
        if (!this.isTimerActive)
        {
            var processingQueue = this;
            window.setTimeout(function() { processingQueue.process() }, 150);
            this.isTimerActive = true;
        }
    }
                   
    this.process = function()
    /// <summary>
    /// The logic that processes the queue in a FIFO manner and sets up the next queue call.
    /// </summary>
    {
       if (this.queue.count > 0)
       {
           var queueItem = this.queue.dequeue();
           queueItem.targetFunction.apply(queueItem.targetObject);
       }
       
       //Determine whether or not any more queued calls are necessary
       if (this.queue.count == 0)
       {
           this.isTimerActive = false;
       }
       else
       {
           var processingQueue = this;
           window.setTimeout(function() { processingQueue.process() }, 150);
       }
    }
}


var _graphProcessingQueue = new ProcessingQueue();
