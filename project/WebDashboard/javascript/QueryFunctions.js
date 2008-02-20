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

function getLastValue(sourceArray, attribute)
/// <summary>
/// Returns the last item in the array	
/// </summary>
{
    
    if (sourceArray.length == 0)
    {
        return "";
    }
    
    return sourceArray[sourceArray.length - 1][attribute];
}

function average(sourceArray, attribute, predicate)
/// <summary>
/// Calculates the average value of an attribute on an object in an array.  The value is rounded
/// to 2 decimal places.
/// </summary>
{
    var filteredList = select(sourceArray, predicate);
    
    if (filteredList.length == 0)
    {
        return 0;
    }
    
    var itemCount = filteredList.length;
    var totalValue = sum(filteredList, attribute, null);
    var avg = (totalValue / itemCount);
    
    return Math.round(avg * 100) / 100;
}

function count(sourceArray, predicate)
/// <summary>
/// Counts the number of items in the array that match the predicate 
/// criteria (as defined by the predicate function)
/// </summary>
{
    var filteredList = getFilteredList(sourceArray, predicate);
    
    return filteredList.length;
}

function sum(sourceArray, attribute, predicate)
/// <summary>
/// Counts the number of items in the array that match the
/// filter criteria (as defined by the filter function)
/// </summary>
{
    
    var filteredList = getFilteredList(sourceArray, predicate);
    
    if (filteredList.length == 0)
    {
        return 0;
    }
    
    var total = 0;
    
    for (var i = 0; i < filteredList.length; i++)
    {
        var filteredListItem = filteredList[i];
        var itemValue = filteredListItem[attribute];
        
        if (isNaN(itemValue) || itemValue == '')
        {
            itemValue = 0;
        }
        
        total += parseFloat(itemValue);
    }
    
    return total;
}

function max(sourceArray, attribute, predicate)
/// <summary>
/// Determines the max numeric value of an item in an array
/// </summary>
{
    var filteredList = getFilteredList(sourceArray, predicate);
    
    if (filteredList.length == 0)
    {
        return 0;
    }
    
    var maxValue = 0;
    
    for (var i = 0; i < filteredList.length; i++)
    {
        maxValue = Math.max(maxValue, parseFloat(filteredList[i][attribute]));
    }
    
    return maxValue;
}

function min(sourceArray, attribute, predicate)
/// <summary>
/// Determines the min numeric value of an item in an array
/// </summary>
{
    var filteredList = getFilteredList(sourceArray, predicate);
    
    if (filteredList.length == 0)
    {
        return 0;
    }
    
    var minValue = 9999999999;
    
    for (var i = 0; i < filteredList.length; i++)
    {
        minValue = Math.min(minValue, parseFloat(filteredList[i][attribute]));
    }
    
    return minValue;
}

function distinct(sourceArray, attribute)
/// <summary>
/// Returns an array containing a distinct list of values from the source array	
/// </summary>
/// <remarks>
/// Previously this method was using the Dojo ArrayList and checking contains on each item,
/// but that caused performance issues.  By using an object attributes and checking for the
/// existance of the attribute, the performance difference was massive (in one instance it went from 3400ms to 0ms [no real firebug measurement])
/// </remarks>
{
    var distinctList = {};
    
    for (var i = 0; i < sourceArray.length; i++)
    {
        var value = sourceArray[i][attribute];
        
        if (distinctList[value] == null)
        {
            distinctList[value] = '';
        }
    }
    
    //Convert the object into an array
    var distinctArray = [];
    
    for (var attribute in distinctList)
    {
        distinctArray.push(attribute);
    }
    
    return distinctArray;
}

function select(sourceArray, predicate)
/// <summary>
/// Returns an array of all objects where the predicate returned true.	
/// </summary>
{
    var resultList = [];
    
    for (var i = 0; i < sourceArray.length; i++)
    {
        var item = sourceArray[i];
        
        if (predicate == null || predicate(item))
        {
            resultList.push(item);
        }
    }
    
    return resultList;
}

function getFilteredList(sourceArray, predicate)
/// <summary>
/// Filter out any objects that don't match the criteria if a filter has been specified
/// </summary>
{
    if (predicate != null)
    {
        return select(sourceArray, predicate); 
    }
    else
    {
        return sourceArray;
    }
}
