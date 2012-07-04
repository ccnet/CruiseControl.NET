<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.1" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >

<!--
  Cruisecontrol.NET dashboard xsl file for Sandcastle Help File Builder
  Jim Graham, Scimatic Software www.scimatic.com
  29/01/2009

  License: MS-Pl (see http://www.opensource.org/licenses/ms-pl.html)

  Based on TransformBuildLog.xsl, (see below for inline copyright in that file)
  in the Sandcastle Help File Builder v1.7.0.0, released under the MS-Pl
  license (see http://www.codeplex.com/SHFB, http://www.codeplex.com/SHFB/license)

// System  : Sandcastle Help File Builder
// File    : BuildLog.xsl
// Author  : Eric Woodruff
// Updated : 03/15/2008
// Note    : Copyright 2008, Eric Woodruff, All rights reserved
//
// This is used to convert a SHFB build log into a viewable HTML page.
-->

<xsl:output method="html"/>
<xsl:variable name="shfb.root" select="//shfbBuild"/>

<!-- Main template for the log -->
<xsl:template match="/">
  <div id="shfb-summary" xmlns="http://schemas.microsoft.com/intellisense/ie5">
    <script>
      // Expand/collapse a section
      function ExpandCollapse(showId, hideId)
      {
          var showSpan = document.getElementById(showId),
              hideSpan = document.getElementById(hideId);

        showSpan.style.display = "inline";
        hideSpan.style.display = "none";
      }


      // Expand or collapse all sections
      function ExpandCollapseAll(expand)
      {
          var spans = document.getElementsByTagName("span")
          var spanIdx, id;

        for(spanIdx = 0; spanIdx != spans.length - 1; spanIdx++)
        {
            id = spans[spanIdx].getAttribute('id');

              <xsl:text disable-output-escaping="yes">
              <![CDATA[
                if(id != null && id.substr(0, 4) == "col_")
              ]]>
              </xsl:text>
                {
                  if(expand)
                      ExpandCollapse("exp_" + id.substr(4), id);
                  else
                      ExpandCollapse(id, "exp_" + id.substr(4));
                }
          }
      }
    </script>
    <xsl:apply-templates select="$shfb.root"/>
  </div>
</xsl:template>


<xsl:template match="shfbBuild">
  <table class="section-table" cellSpacing="0" cellPadding="2" width="98%" border="0">
    <tr><td class="sectionheader"><xsl:value-of select="@product"/> <xsl:value-of select="@version"/> Build Log</td></tr>
    <tr><td>Project File: <xsl:value-of select="@projectFile"/></td></tr>
    <tr><td>Build Started: <xsl:value-of select="@started"/></td></tr>
    <tr><td><a href="#" onclick="javascript: ExpandCollapseAll(false);">Collapse All</a><xsl:text disable-output-escaping="yes"><![CDATA[ &nbsp;&nbsp;&nbsp;&nbsp; ]]></xsl:text><a href="#" onclick="javascript: ExpandCollapseAll(true);">Expand All</a></td></tr>

    <!-- Process the build steps -->
    <xsl:apply-templates select="//shfbBuild/buildStep" />

    <tr><td><a href="#" onclick="javascript: ExpandCollapseAll(false);">Collapse All</a><xsl:text disable-output-escaping="yes"><![CDATA[ &nbsp;&nbsp;&nbsp;&nbsp; ]]></xsl:text>
    <a href="#" onclick="javascript: ExpandCollapseAll(true);">Expand All</a></td></tr>

  </table>
</xsl:template>


<!-- Build step template -->
<xsl:template match="//shfbBuild/buildStep">
  <tr>
    <td>
      <span id="col_{@step}" style="display: none;">
        <span class="CollapseBox" onclick="javascript: ExpandCollapse('exp_{@step}', 'col_{@step}');">+</span>
        <span>
          <span class="sectionheader"><xsl:value-of select="@step"/></span><br/><br/>
        </span>
      </span>
      <span id="exp_{@step}" style="display: inline;">
        <span class="CollapseBox" onclick="javascript: ExpandCollapse('col_{@step}', 'exp_{@step}');">-</span>
        <span class="sectionheader"><xsl:value-of select="@step"/></span><br/><br/>
        <pre>
          <xsl:apply-templates />
        </pre>
      </span>
    </td>
  </tr>
</xsl:template>

<!-- Text template -->
<xsl:template match="text()">
  <xsl:value-of select="." disable-output-escaping="yes" />
</xsl:template>


</xsl:stylesheet>
