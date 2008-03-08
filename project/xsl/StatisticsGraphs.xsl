<?xml version="1.0" encoding="utf-8" ?>
<!--
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
-->
<xsl:stylesheet version="1.0" xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
  <xsl:output method="html" encoding="UTF-8"/>
  
  <xsl:template match="/">
    <xsl:apply-templates select="//statistics" />
  </xsl:template>
  
  <xsl:template match="//statistics">
      <xsl:comment>CruiseControl Summary Graphs - Written by Eden Ridgway</xsl:comment>
             
    <style type="text/css">
    body
    {
        font-family: Verdana;
    }

    .Legend
    {
        border: 1px solid silver;
        margin: 10px 0px 10px 0px;
        width: 200px;
        position: relative;
        left: 195px;
    }
    
    .Legend td
    {
        font-size: 10px;
    }
    
    .Legend .ColorBox
    {
        width: 10px;
        height: 10px;
        border: 1px solid black;
    }
    
    .ScrollContainer
    {
        height: 500px;
        width: 95%;
        overflow: auto;
        border: 1px solid #666;
    }
    
    .ScrollContainer table
    {
        width: 100%;
        border-bottom: 1px solid #7DAAEA;
    }
    
    .ScrollContainer a
    {
        text-decoration: none;
    }
    
    .ScrollContainer thead
    {
        font-weight: bold;
        background-color: #3d80df;
        color: white;
    }
    
    /* Detail table inside of summary table */
    .ScrollContainer .DetailSubTable thead
    {
        font-weight: bold;
        background-color: #d6e2f4;
        color: black;
    }
    
    .ScrollContainer .DetailSubTable
    {
        border-top: 1px solid black;
        border-bottom: 1px solid black;
    }
    
    .ScrollContainer td
    {
        font-family: Verdana;
        font-size: 10px;
        text-align: center;
        border-left: 1px solid #346DBE;
        border-right: 1px solid #7DAAEA;
    }
    
    .ScrollContainer .AlternateRow
    {
        background-color: #eee;
    }
    
    .GraphContainer
    {
        padding: 10px;
        background-Color: #F0F0FF;
        border: 1px solid black;
        margin: 4px;
        font-family: Verdana;
        width: 600px;
        height: 250px;
    }
    
    #MainTabContainer
    {
        font-family: Verdana;
        font-size: 12px;
        width: 100%; 
        height: 600px;
    }
    
    /* Graph/Table Headers */
    #MainTabContainer h2
    {
        font-size: 16px;
    }
  
    div.TabContentsScrollContainer
    {
        overflow: auto; 
        height: 560px; 
        width: 100%;
        position: relative;
        font-family: Verdana;
        font-size: 12px;
    }
    
    /* Pads the inside of each tab */
    .dojoTabPaneWrapper 
    {
      padding : 10px 10px 10px 10px;
    }
    
    tr.Success
    {
        color: black;
    }
    
    tr.Failure
    {
        color: red;
    }
    
    tr.Exception
    {
        color: #B88A00;
    }
    
    td.DrillDown a
    {
        width: 10px;
        height: 10px;
    }
    
    tr.HideDetails
    {
        display: none;
    }
    
    </style>
    
      <xsl:comment><![CDATA[[if IE]>
      <style type="text/css">
      
      .GraphContainer
      {
          width: 620px;
          height: 270px;
      }
      
      </style>
      <![endif]]]></xsl:comment>
    
      <script type="text/javascript" src="../../../../javascript/dojo/dojo.js"></script>
      <script type="text/javascript" src="../../../../javascript/QueryFunctions.js"></script>
      <script type="text/javascript" src="../../../../javascript/StatisticsTables.js"></script>
      <script type="text/javascript" src="../../../../javascript/GraphWrapper.js"></script>
      
      <script type="text/javascript">
      dojo.require("dojo.widget.TabContainer");
      dojo.require("dojo.widget.LinkPane");
      dojo.require("dojo.widget.ContentPane");
      dojo.require("dojo.widget.LayoutContainer");
      </script>
    
      <script type="text/javascript">
      
      //Convert the statistics data into JavaScript objects
      var _statistics = 
      [<xsl:apply-templates select="integration" />
      ];
      </script>
      <script type="text/javascript" src="../../../../javascript/StatisticsGraphs.js"></script>
      <script type="text/javascript" src="../../../../javascript/GraphConfiguration.js"></script>
  
      <!-- The tab container -->
      <div id="MainTabContainer" dojoType="TabContainer" selectedTab="RecentBuildsTabWidget" >
      
          <div id="RecentBuildsTab" widgetId="RecentBuildsTabWidget" dojoType="ContentPane" label="Recent Build Graphs" >
              <div class="TabContentsScrollContainer">
                  <div id="RecentBuildsContainerArea"></div>
              </div>
          </div>
          
          <div id="HistoricalBuildsTab" widgetId="HistoricalTabWidget" dojoType="ContentPane" label="Historic Build Graphs">
              <div class="TabContentsScrollContainer">
                  <div id="HistoricGraphContainerArea"></div>
              </div>
          </div>

          <div id="DetailedTabularDataTab" widgetId="DetailedDataTabWidget" dojoType="ContentPane" label="Detailed Data">
              <div class="TabContentsScrollContainer">
                  <div id="DetailedTableStatisticsContainerArea"></div>
              </div>
          </div>

          <div id="SummarisedTabularDataTab" widgetId="SummarisedDataTabWidget" dojoType="ContentPane" label="Summarised Data">
              <div class="TabContentsScrollContainer">
                  <div id="SummaryTableStatisticsContainerArea"></div>
              </div>
          </div>
      </div>
    
  </xsl:template>
  
  <!-- Replace one string with another -->
  <xsl:template name="replace-string">
    <xsl:param name="text"/>
    <xsl:param name="replace"/>
    <xsl:param name="with"/>
    <xsl:choose>
      <xsl:when test="contains($text,$replace)">
        <xsl:value-of select="substring-before($text,$replace)"/>
        <xsl:value-of select="$with"/>
        <xsl:call-template name="replace-string">
          <xsl:with-param name="text" select="substring-after($text,$replace)"/>
          <xsl:with-param name="replace" select="$replace"/>
          <xsl:with-param name="with" select="$with"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  
  <!-- Ensures that there are no characters in the text that will cause JavaScript problems (like \u) -->
  <xsl:template match="text()">
    <xsl:call-template name="replace-string">
      <xsl:with-param name="text" select='normalize-space(translate(., "&apos;", "`"))'/>
      <xsl:with-param name="replace" select="'\'"/>
      <xsl:with-param name="with" select="'\\'"/>
    </xsl:call-template>
  </xsl:template>
  
  <!-- Create JSON statistic representation for each build -->
  <xsl:template match="statistics/integration" xml:space="preserve">
    {'BuildLabel' : '<xsl:apply-templates select="@build-label" />', 'Status' : '<xsl:value-of select="@status"/>', <xsl:for-each select="statistic[@name != '']">'<xsl:apply-templates select="@name" />' : '<xsl:apply-templates select="." />', </xsl:for-each> 'Date': '<xsl:value-of select="@day"/> <xsl:value-of select="@month"/> <xsl:value-of select="@year"/>'}<xsl:if test="position() != count(//integration)">,</xsl:if></xsl:template>
  
</xsl:stylesheet>
