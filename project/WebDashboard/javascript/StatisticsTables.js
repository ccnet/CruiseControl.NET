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
This file contains the logic to create the tables that display all the detailed 
and summarised statistics data.
*/

var _urlRoot = document.location.href.replace(/(ViewStatisticsReport.aspx)/gi, "");

function StringBuilder() 
/// <summary>
/// Makes it easier to use an array to concatenate a string
/// and the improved performance is significant
/// </summary>
{
    this.buffer = []; 
    
    this.append = function(string) 
                  { 
                     this.buffer.push(string); 
                  }; 
                  
    this.toString = function() 
                    { 
                       return this.buffer.join(""); 
                    }; 
} 

function createScrollableTable(tableId, sourceList, shouldWrapCellDelegate, cellRenderer, shouldDisplayDrillDownColumn, drillDownHandler)
/// <summary>
/// Uses string concatenation to create the table.  This is somewhere in the region of 3.5 times faster than
/// using DOM methods/element creation (which would have been my preference)
/// </summary>
{
    var html = new StringBuilder();
    
    html.append("<table id='" + tableId + "' cellpadding='0' cellspacing='0' border='0'><thead><tr class='TableHeader'>");
    
    //Add a drill down column at the front of the table
    if (shouldDisplayDrillDownColumn)
    {
        html.append("<td style='width: 30px;'>&nbsp;</td>");
    }
    
    var attributes = [];
    
    //Add the headings
    for (var attribute in sourceList[0])
    {
        attributes.push(attribute);
        
        var headingText = splitAndCapatilizeWords(attribute);
        html.append("<td style='" + getColumnStyle(null, attributeName, shouldWrapCellDelegate) + "'>");
        html.append(headingText);
        html.append("</td>");
    }
    
    html.append("</tr></thead><tbody>");
    
    //Add all the rows (in reverse order - latest first)
    for (var rowIndex = sourceList.length-1; rowIndex >= 0; rowIndex--)
    {
        var statistic = sourceList[rowIndex];
        var rowClass = getRowClass(statistic);
        
        if ((rowIndex + 1) % 2 == 0)
        {
            html.append("<tr class='AlternateRow " + rowClass + "'>");
        }
        else
        {
            html.append("<tr class='" + rowClass + "'>");
        }
        
        //Give the renderer the opportunity to 
        if (shouldDisplayDrillDownColumn)
        {
            html.append("<td class='DrillDown'><a title='Drill down' href='javascript:void(0)'>+</a></td>");
        }
    
        for (var columnIndex = 0; columnIndex < attributes.length; columnIndex++)
        {
            var attributeName = attributes[columnIndex];
            var cellValue = cellRenderer(statistic, attributeName);
            
            html.append("<td style='" + getColumnStyle(statistic, attributeName, shouldWrapCellDelegate) + "'>");
            html.append(cellValue);
            html.append("</td>");
        }
        
        html.append("</tr>");
    }
    
    html.append("</tbody></table>");
    
    var fragment = document.createDocumentFragment();
    
    //Have to do this in order to get the document fragment approach to work properly (which is crucial to getting the events to work)
    var scrollContainer = document.createElement("div");
    scrollContainer.className = "ScrollContainer";
    scrollContainer.innerHTML = html.toString();
    
    fragment.appendChild(scrollContainer);
    
    var table = scrollContainer.childNodes[0];
    
    //Hook up the drill down handler to the table so that the drill down cell can access it
    if (shouldDisplayDrillDownColumn)
    {
        var numberRows = table.rows.length;
        
        for (var rowIndex = 1; rowIndex < numberRows; rowIndex++)
        {
            var row = table.rows[rowIndex];
            
            //The statistics are displayed in reverse order (latest first)
            row.cells[0].childNodes[0].onclick = encloseDrillDownHandler(drillDownHandler, sourceList[numberRows - rowIndex - 1], row);
        }
    }
    
    return fragment;
}

function encloseDrillDownHandler(handler, rowData, row)
/// <summary>
/// Ensures that the drill down handler receives the correct event
/// </summary>
{
    return function() { handler(rowData, row) };
}

function getColumnStyle(currentRowData, attributeName, shouldWrapCellDelegate)
/// <summary>
/// Gets the style that must be applied to the current cell
/// </summary>
{  
    var shouldWrapColumn = shouldWrapCellDelegate(currentRowData, attributeName);
    
    var cellStyle = "white-space: nowrap";
    
    //Set the column whitespace wrapping appropriately (override any table wide styles)
    if (shouldWrapColumn)
    {
        cellStyle = "white-space: normal;";
    }
    
    return cellStyle;
}

function splitAndCapatilizeWords(name)
/// <summary>
/// Splits a name up based on capatilization and ensure the first letter is uppercase
/// </summary>
{
    var splitWord = name.replace(/([A-Z])/g, " $1");
    var capatalizedWord = splitWord.charAt(0).toUpperCase() + splitWord.substring(1);
    
    return capatalizedWord;
}

function generateStatisticsTable(tableContainerArea, reportName, sourceStatistics, cellRenderer, shouldDisplayDrillDown, drillDownHandler)
/// <summary>
/// Displays summary stats in a table format
/// </summary>
{
    //The container that will enclose the heading as well as the table
    var statisticsContainer = document.createElement("div");
    statisticsContainer.className = "StatisticsTable";
    tableContainerArea.appendChild(statisticsContainer);
    
    //Heading
    var summaryHeading = document.createElement("h2");
    summaryHeading.innerHTML = reportName;
    statisticsContainer.appendChild(summaryHeading);
    
    //The actual table
    var summaryTableContainer = createScrollableTable(reportName, sourceStatistics, shouldColumnWrap, cellRenderer, shouldDisplayDrillDown, drillDownHandler);
                                                      
    statisticsContainer.appendChild(summaryTableContainer);
}

function getRowClass(currentRowData)
/// <summary>
/// Determines the class that should be applied to the table row
/// </summary>
{
    var status = currentRowData["Status"];
    
    return status || "";
}

function summaryTableDrillDown(rowData, row)
/// <summary>
/// Displays the detail rows associated with a summary row in a table below the summary row
/// </summary>
{
    var hasDetails = (typeof(row.hasDetails) != 'undefined');
    
    if (!hasDetails)
    {
        setupDetailSubTable(rowData, row);
    }
    else
    {
        var table = row.parentNode.parentNode;
        var detailRow = table.rows[row.rowIndex + 1];
        
        if (detailRow.className == '')
        {
            detailRow.className = 'HideDetails';
            row.cells[0].childNodes[0].innerHTML = "+";
        }
        else
        {
            detailRow.className = ''; 
            row.cells[0].childNodes[0].innerHTML = "-";
        }
    }
}

function setupDetailSubTable(rowData, row)
/// <summary>
/// Creates the detail sub table that is displayed in a new row below the summary row
/// </summary>
{
    var table = row.parentNode.parentNode;
    
    var detailRow = table.insertRow(row.rowIndex + 1);
    var indentCell = detailRow.insertCell(0);
    indentCell.innerHTML = "&nbsp;";
    
    var detailCell = detailRow.insertCell(1);
    detailCell.colSpan = row.cells.length;
    detailRow.className = '';
    row.hasDetails = true;
    
    var day = new Date(rowData["day"]);
    
    //Get the detail rows associated with this summary row
    var detailStatistics = select(_statistics, function(item)
                                               {
                                                  var itemDate = new Date(item["Date"]);
                                                  return (itemDate.valueOf() == day.valueOf());
                                               });
    
    //Add the detail table 
    var scrollTable = createScrollableTable("", detailStatistics, shouldColumnWrap, cellRenderer, false, null);

    scrollTable.childNodes[0].className = "DetailSubTable";
    detailCell.appendChild(scrollTable);
    
    //Change the + to a - sign
    row.cells[0].childNodes[0].innerHTML = "-";
}

function cellRenderer(currentRowData, attributeName)
/// <summary>
/// Determines how the contents of a cell should be rendered
/// </summary>
{
    var cellValue = currentRowData[attributeName];
    
    if (typeof(cellValue) == 'string' && cellValue.length == 0)
    {
        cellValue = "&nbsp;";
    }
    
    var cellHtml;
    
    //Change the color of the status text
    switch (attributeName)
    {
        case "Status": 
                
                var textColorStyle = '';
                 
                if (cellValue == 'Success')
                {
                    textColorStyle = 'color: green';
                }
                
                cellHtml = "<span style='" + textColorStyle + "'>" + cellValue + "</span>";
                break;
        
        case "BuildLabel":
                var wasSuccessful = (currentRowData["Status"] == 'Success');
                var buildLabel = currentRowData["BuildLabel"];
                var startTimeText = currentRowData["StartTime"];
                var buildDateText = currentRowData["Date"];
                
                //Use the build date instead of the start time because the start time may be in a
                //local other than the US and therfore will be created as the wrong date
                var timeRegEx = /\d{1,2}:\d{1,2}:\d{1,2}\s*((AM)|(PM)|())/gi;
                var timeMatch = startTimeText.match(timeRegEx);
                var buildDate = new Date(buildDateText + " " + timeMatch);
                
                var buildTime = buildDate.getFullYear().toString() +
                                zeroPadValue(buildDate.getMonth() + 1, 2) +
                                zeroPadValue(buildDate.getDate(), 2) +
                                zeroPadValue(buildDate.getHours(), 2) +
                                zeroPadValue(buildDate.getMinutes(), 2) +
                                zeroPadValue(buildDate.getSeconds(), 2);
                var url;
                
                if (wasSuccessful)
                {
                    url = _urlRoot + "build/log" + buildTime + "Lbuild." + buildLabel + ".xml/ViewBuildReport.aspx";
                }
                else
                {
                    url = _urlRoot + "build/log" + buildTime + ".xml/ViewBuildReport.aspx";
                }
                
                url = url.replace(/'/g, "%27");
                
                cellHtml = "<a href='" + url + "'>" + cellValue + "</a>";
                
                break;
        
        default: 
                cellHtml = cellValue;
                break;
    }
    
    return cellHtml;
}

function zeroPadValue(value, numDigits)
/// <summary>
/// Puts zeros at the front of numbers
/// </summary>
{
    value = value.toString();
    
    var numDigitsRequired = numDigits - value.length;
    
    for (var i = 0; i < numDigitsRequired; i++)
    {
        value = "0" + value;
    }
    
    return value;
}

function shouldColumnWrap(currentRowData, attributeName)
/// <summary>
/// Ensures that the columns don't become too wide
/// </summary>
{
    if (attributeName == 'BuildErrorMessage')
    {
        return true;
    }

	// The attribute in question may not be present in all instances of the data.             
	// In places where the attribute is not present, no wrapping will be necessary.           
	var attribute;                                                                            
	                                                                                          
	if (currentRowData != null)                                                               
	{                                                                                         
		attribute = currentRowData[attributeName];                                        
	}                                                                                         
	                                                                                          
	if ( attribute == null )                                                                  
	{                                                                                         
		return false;                                                                     
	}                                                                                         
                                                                                                 
    return (attribute.toString().length > 35);                                                   
 
}