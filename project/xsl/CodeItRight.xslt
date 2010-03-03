<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:param name="applicationPath"/>
  <xsl:output method="html" indent="no"/>

  <xsl:key name="projects" match="Violation" use="ProjectName"/>
  <xsl:key name="files" match="Violation" use="FileName"/>

  <xsl:template match="/">
    <style>
      table.header tr th
      {
      text-align: left;
      }
      table.details
      {
      width: 100%;
      border-collapse:collapse;
      }
      table.details tr th
      {
      background-color:#2E8A2E;
      color:#ffffff;
      }
      tr.violation td
      {
      color: #2E8A2E;
      }
      img.expandable{
      width: 16px;
      height: 16px;
      }
      .open{
      background-image:url(<xsl:value-of select="applicationPath"/>/images/arrow_minus_small.gif);
      }
      .closed{
      background-image:url(<xsl:value-of select="applicationPath"/>/images/arrow_plus_small.gif);
      }
      .itemCol{
      width: 40%
      }
      .lineCol{
      width: 5%
      }
      .typeCol{
      width: 10%
      }
      .violCol{
      width: 30%
      }
      .catCol{
      width: 5%
      }
      .sevCol{
      width: 5%
      }
      .helpCol{
      width: 5%
      }
      div.section{
        border: solid 1px #888888;
        margin-left: 16px;
      }
    </style>
    <script type="text/javascript">
      jQuery(function(){
      jQuery('img.expandable').click(function(){
      var item = jQuery(this);
      item.toggleClass('open');
      item.toggleClass('closed');
      item.parent().next().toggle();
      });
      });
    </script>
    <xsl:for-each select="/cruisecontrol/build/CodeItRightReport">
      <h2>CodeIt.Right Analysis Report</h2>
      <xsl:apply-templates select="Violations">
        <xsl:sort order="ascending" select="@date"/>
      </xsl:apply-templates>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="Violations">
    <table class="header">
      <tr>
        <th>
          Solution:
        </th>
        <td>
          <xsl:value-of select="@solution"/>
        </td>
      </tr>
      <tr>
        <th>
          Analysis Date/Time:
        </th>
        <td>
          <xsl:value-of select="@date"/>
        </td>
      </tr>
    </table>
    <table class="details">
      <tr>
        <th class="itemCol">Item</th>
        <th class="lineCol">Line #</th>
        <th class="typeCol">Type</th>
        <th class="violCol">Violation</th>
        <th class="catCol">Category</th>
        <th class="sevCol">Severity</th>
        <th class="helpCol">Help</th>
      </tr>
    </table>
    <xsl:for-each select="Violation[generate-id() = generate-id(key('projects', ProjectName)[1])]">
      <xsl:sort select="ProjectName"/>
      <xsl:variable name="currentProject" select="ProjectName"/>
      <div>
        <img src="{$applicationPath}/images/shim.gif" class="expandable open"/>
        <xsl:value-of select="$currentProject" /> [<xsl:value-of select="count(key('projects', ProjectName))" />]
      </div>
      <div class="section">
        <xsl:for-each select="key('projects', ProjectName)">
          <xsl:sort select="FileName"/>
          <xsl:for-each select="current()[generate-id() = generate-id(key('files', FileName)[ProjectName=$currentProject][1])]">
            <xsl:variable name="currentFile" select="FileName"/>
            <div >
              <img src="{$applicationPath}/images/shim.gif" class="expandable open"/>
              <xsl:value-of select="$currentFile" /> [<xsl:value-of select="count(key('files', FileName)[ProjectName=$currentProject])" />]
            </div>
            <div class="section">
              <table class="details">
                <xsl:apply-templates select="key('files', FileName)"/>
              </table>
            </div>
          </xsl:for-each>
        </xsl:for-each>
      </div>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="Violation">
    <tr class="violation">
      <td class="itemCol">
        <xsl:value-of select="ItemName"/>
      </td>
      <td class="lineCol">
        <xsl:value-of select="LineNumber"/>
      </td>
      <td class="typeCol">
        <xsl:value-of select="ItemType"/>
      </td>
      <td class="violCol">
        <xsl:value-of select="Title"/>
      </td>
      <td class="catCol">
        <xsl:value-of select="RuleCategory"/>
      </td>
      <td class="sevCol">
        <xsl:value-of select="Severity"/>
      </td>
      <td class="helpCol">
        <xsl:if test="RuleSupportUrl">
          <a>
            <xsl:attribute name="href">
              <xsl:value-of select="RuleSupportUrl"/>
            </xsl:attribute>
            Details
          </a>
        </xsl:if>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
