<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="html" indent="no"/>

  <xsl:template match="/cruisecontrol">
    <style>
      .numberColumn{ text-align:right !important; width: 13%; }
      .fullTable { width: 100%; }
      .textColumn { text-align: left !important; width: 24%; }
      .cpu {}
      .wallClock { display: none; }
      #commands div { display:inline; }
    </style>
    <script type="text/javascript">
      function switchDisplay(newDisplay, oldDisplay){
      jQuery(newDisplay).show();
      jQuery(oldDisplay).hide();
      }
      jQuery(function(){
      jQuery('table.SortableGrid').initialiseProjectGrid({
      sortList: [[0,0], [1, 0]]
      });
      jQuery('#cpu').change(function(){
      switchDisplay('.cpu', '.wallClock');
      });
      jQuery('#wallClock').change(function(){
      switchDisplay('.wallClock', '.cpu');
      });
      });
    </script>
    <h1>ANTS Performance: All Methods</h1>
    <div id="commands">
      <div>
        <input type="radio" name="clockType" id="cpu" checked="checked"/>
        <label for="cpu">CPU Time</label>
      </div>
      <div>
        <input type="radio" name="clockType" id="wallClock"/>
        <label for="wallClock">Wall Clock Time</label>
      </div>
    </div>
    <xsl:apply-templates select="build/PerformanceData[AllMethods]">
      <xsl:sort select="CreationTime"/>
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="PerformanceData">
    <br/>
    <table class="fullTable SortableGrid">
      <thead>
        <tr>
          <th>Class</th>
          <th>Method</th>
          <th>Hit Count</th>
          <th class="cpu">Time</th>
          <th class="cpu">Percentage</th>
          <th class="cpu">Percentage (with children)</th>
          <th class="wallClock">Time</th>
          <th class="wallClock">Percentage</th>
          <th class="wallClock">Percentage (with children)</th>
        </tr>
      </thead>
      <tbody>
        <xsl:apply-templates select="AllMethods/Method[@has-source]"/>
      </tbody>
    </table>
  </xsl:template>

  <xsl:template match="Method">
    <tr>
      <td class="textColumn">
        <xsl:value-of select="@class"/>
      </td>
      <td class="textColumn">
        <xsl:value-of select="@name"/>(<xsl:value-of select="@parameters"/>)
      </td>
      <td class="numberColumn">
        <xsl:value-of select="number(HitCount)"/>
      </td>
      <td class="numberColumn cpu">
        <xsl:value-of select="format-number(number(substring(CPU/@time, 0, string-length(CPU/@time) - 2)), '#0.00')"/>ms
      </td>
      <td class="numberColumn cpu">
        <xsl:value-of select="format-number(number(substring(WithSelf/@percent-cpu, 0, string-length(WithSelf/@percent-cpu) - 2)), '#0.00')"/>%
      </td>
      <td class="numberColumn cpu">
        <xsl:value-of select="format-number(number(substring(CPU/@percent, 0, string-length(CPU/@percent) - 2)), '#0.00')"/>%
      </td>
      <td class="numberColumn wallClock">
        <xsl:value-of select="format-number(number(substring(WithSelf/@percent-wallclock, 0, string-length(WithSelf/@percent-wallclock) - 2)), '#0.00')"/>ms
      </td>
      <td class="numberColumn wallClock">
        <xsl:value-of select="format-number(number(substring(Wallclock/@percent, 0, string-length(Wallclock/@percent) - 2)), '#0.00')"/>%
      </td>
      <td class="numberColumn wallClock">
        <xsl:value-of select="format-number(number(substring(Wallclock/@percent, 0, string-length(Wallclock/@percent) - 2)), '#0.00')"/>%
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
