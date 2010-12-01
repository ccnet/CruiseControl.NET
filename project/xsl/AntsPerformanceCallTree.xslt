<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="html" indent="no"/>

  <xsl:template match="/cruisecontrol">
    <style>
      .fullTable { width: 100%; }
      .cpu {}
      .wallClock { display: none; }
      .treeItem { margin-left: 14px; }
      .itemTitle { cursor: pointer; }
      .itemData { font-style:italic; }
      .itemOpen { padding-left: 14px; background-image:url(<xsl:value-of select="applicationPath"/>/images/minus.png); background-repeat:no-repeat; background-position:center left; }
      .itemClosed { padding-left: 14px; background-image:url(<xsl:value-of select="applicationPath"/>/images/plus.png); background-repeat:no-repeat; background-position:center left; }
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
      jQuery('div.itemToggle').click(function(){
      jQuery(this).toggleClass('itemOpen').toggleClass('itemClosed').next().toggle();
      });
      });
    </script>
    <h1>ANTS Performance: Call Tree</h1>
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
    <xsl:apply-templates select="build/PerformanceData[CallTree]">
      <xsl:sort select="CreationTime"/>
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="PerformanceData">
    <br/>
    <xsl:apply-templates select="CallTree/Method[@has-source]"/>
  </xsl:template>

  <xsl:template match="Method">
    <div class="itemOpen itemToggle">
      <div class="itemTitle">
        <xsl:value-of select="@class"/>.<xsl:value-of select="@name"/>(<xsl:value-of select="@parameters"/>)
      </div>
    </div>
    <div class="treeItem">
      <div class="itemData">
        <div>
          Hit count: <xsl:value-of select="HitCount"/>
        </div>
        <div>
          Time:
          <span class="cpu">
            <xsl:value-of select="format-number(number(substring(CPU/@time, 0, string-length(CPU/@time) - 2)), '#0.00')"/>ms
          </span>
          <span class="wallClock">
            <xsl:value-of select="format-number(number(substring(WithSelf/@percent-wallclock, 0, string-length(WithSelf/@percent-wallclock) - 2)), '#0.00')"/>ms
          </span>
        </div>
        <div>
          Percentage:
          <span class="cpu">
            <xsl:value-of select="format-number(number(substring(WithSelf/@percent-cpu, 0, string-length(WithSelf/@percent-cpu) - 2)), '#0.00')"/>%
          </span>
          <span class="wallClock">
            <xsl:value-of select="format-number(number(substring(Wallclock/@percent, 0, string-length(Wallclock/@percent) - 2)), '#0.00')"/>%
          </span>
        </div>
        <div>
          Percentage (with children):
          <span class="cpu">
            <xsl:value-of select="format-number(number(substring(CPU/@percent, 0, string-length(CPU/@percent) - 2)), '#0.00')"/>%
          </span>
          <span class="wallClock">
            <xsl:value-of select="format-number(number(substring(Wallclock/@percent, 0, string-length(Wallclock/@percent) - 2)), '#0.00')"/>%
          </span>
        </div>
      </div>
      <xsl:apply-templates select="Method[@has-source]"/>
    </div>
  </xsl:template>
</xsl:stylesheet>
