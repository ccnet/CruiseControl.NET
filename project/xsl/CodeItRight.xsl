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
      cursor:pointer;
      }
      tr.cir-violation td.right{
      text-align: right;
      }
      .cir-hidden{
      display:none;
      }
      tr.cir-CriticalError td.cir-icon, tr.cir-Error td.cir-icon{
      background-image:url(<xsl:value-of select="applicationPath"/>/images/error.png);
      background-repeat:no-repeat;
      background-position:center left;
      }
      tr.cir-CriticalWarning td.cir-icon, tr.cir-Warning td.cir-icon{
      background-image:url(<xsl:value-of select="applicationPath"/>/images/warning.png);
      background-repeat:no-repeat;
      background-position:center left;
      }
      tr.cir-violation td.cir-icon{
      padding-left: 18px;
      }
    </style>
    <script type="text/javascript">
      function showHideSeverity(severity){
      jQuery('#show' + severity).change(function(){
      var show = jQuery('#show' + severity + ':checked').val();
      if (show){
      jQuery('tr.cir-violation.cir-' + severity).show();
      }else{
      jQuery('tr.cir-' + severity).hide();
      }
      });
      }

      jQuery(function(){
      jQuery('table.SortableGrid').initialiseProjectGrid({
      sortList: [[0,0], [1, 0]]
      });
      jQuery('tr.cir-violation td').click(function(){
      jQuery(this).parent().next().toggle();
      });
      showHideSeverity('CriticalError');
      showHideSeverity('Error');
      showHideSeverity('CriticalWarning');
      showHideSeverity('Warning');
      showHideSeverity('Information');
      });
    </script>
    <xsl:for-each select="/cruisecontrol/build/CodeItRightReport">
      <h2>CodeIt.Right Analysis Report</h2>
      <table class="cir-solution">
        <tr>
          <td>
            <input type="checkbox" id="showCriticalError" checked="checked"/>
            <label for="showCriticalError">Critical Errors</label>
          </td>
          <td>
            <input type="checkbox" id="showError" checked="checked"/>
            <label for="showError">Errors</label>
          </td>
          <td>
            <input type="checkbox" id="showCriticalWarning" checked="checked"/>
            <label for="showCriticalWarning">Critical Warnings</label>
          </td>
          <td>
            <input type="checkbox" id="showWarning" checked="checked"/>
            <label for="showWarning">Warnings</label>
          </td>
          <td>
            <input type="checkbox" id="showInformation" checked="checked"/>
            <label for="showInformation">Information</label>
          </td>
        </tr>
      </table>
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
    <tr>
      <xsl:attribute name="id">projectData<xsl:value-of select="position()"/></xsl:attribute>
      <xsl:attribute name="class">cir-violation cir-<xsl:value-of select="Severity"/></xsl:attribute>
      <td class="cir-icon">
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
    <tr>
      <xsl:attribute name="id">link<xsl:value-of select="position()"/></xsl:attribute>
      <xsl:attribute name="class">buildStatus cir-hidden cir-<xsl:value-of select="Severity"/></xsl:attribute>
      <td colspan="8">
        <div>
          <b>
            <xsl:value-of select="@ruleID"/>: <xsl:value-of select="Name"/>
          </b>
        </div>
        <div>
          <xsl:value-of select="ViolationDescription"/>
        </div>
        <div>
          Corrections:
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
