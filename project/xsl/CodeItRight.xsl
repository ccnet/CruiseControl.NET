<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:param name="applicationPath"/>
  <xsl:output method="html" indent="no"/>

  <xsl:template match="/">
    <style>
      table.cir-solution tr th
      {
      text-align: left;
      }
      tr.cir-violation td
      {
      color: #2E8A2E;
      text-align: left;
      }
      tr.cir-violation td.right{
      text-align: right;
      }
      .cir-hidden{
      display:none;
      }
      tr.cir-violation td.cir-icon{
      padding-left: 18px;
      }
      tr.cir-violation td.cir-CriticalError, tr.cir-violation td.cir-Error{
      background-image:url(<xsl:value-of select="applicationPath"/>/images/error.png);
      background-repeat:no-repeat;
      background-position:center left;
      }
      tr.cir-violation td.cir-CriticalWarning, tr.cir-violation td.cir-Warning{
      background-image:url(<xsl:value-of select="applicationPath"/>/images/warning.png);
      background-repeat:no-repeat;
      background-position:center left;
      }
    </style>
    <script type="text/javascript">
      jQuery(function(){
      jQuery('table.SortableGrid').initialiseProjectGrid({
      sortList: [[0,0], [1, 0]]
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
    <table class="cir-solution">
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
    <table class="SortableGrid">
      <thead>
        <tr>
          <th>Project</th>
          <th>File</th>
          <th>Item</th>
          <th>Line #</th>
          <th>Type</th>
          <th>Violation</th>
          <th>Category</th>
          <th>Severity</th>
        </tr>
      </thead>
      <tbody>
        <xsl:apply-templates select="Violation"/>
      </tbody>
    </table>
  </xsl:template>

  <xsl:template match="Violation">
    <tr class="cir-violation">
      <xsl:attribute name="id">projectData<xsl:value-of select="position()"/></xsl:attribute>
      <td>
        <xsl:attribute name="class">cir-icon cir-<xsl:value-of select="Severity"/></xsl:attribute>
        <xsl:value-of select="ProjectName"/>
      </td>
      <td>
        <xsl:value-of select="FileName"/>
      </td>
      <td>
        <xsl:value-of select="ItemName"/>
      </td>
      <td class="right">
        <xsl:value-of select="LineNumber"/>
      </td>
      <td>
        <xsl:value-of select="ItemType"/>
      </td>
      <td>
        <xsl:value-of select="Title"/>
      </td>
      <td>
        <xsl:value-of select="RuleCategory"/>
      </td>
      <td>
        <xsl:value-of select="Severity"/>
      </td>
    </tr>
    <tr class="buildStatus">
      <xsl:attribute name="id">link<xsl:value-of select="position()"/></xsl:attribute>
      <td colspan="8">
        <div>
          <xsl:value-of select="ViolationDescription"/>
        </div>
        <ol>
          <xsl:for-each select="CorrectDescriptions/CorrectDescription">
            <li>
              <xsl:value-of select="."/>
            </li>
          </xsl:for-each>
        </ol>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
