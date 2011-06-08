<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/xhtml1/strict">
  <xsl:output method="html"/>
  <xsl:template match="/">
    <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
      <tr>
        <td class="sectionheader" colspan="6">
          Microsoft Metrics
        </td>
      </tr>
      <tr>
        <th>Module</th>
        <th>Maintainability Index</th>
        <th>Cyclomatic Complexity</th>
        <th>Class Coupling</th>
        <th>Inheritance Depth</th>
        <th>Lines Of Code</th>
      </tr>
      <xsl:apply-templates select="//CodeMetricsReport/Targets/Target/Modules/Module" />
    </table>
  </xsl:template>

  <xsl:template match="Module">
    <tr>
      <td>
        <xsl:value-of select="@Name"/>
      </td>
      <td>
        <xsl:value-of select="Metrics/Metric[@Name='MaintainabilityIndex']/@Value"/>
      </td>
      <td>
        <xsl:value-of select="Metrics/Metric[@Name='CyclomaticComplexity']/@Value"/>
      </td>
      <td>
        <xsl:value-of select="Metrics/Metric[@Name='ClassCoupling']/@Value"/>
      </td>
      <td>
        <xsl:value-of select="Metrics/Metric[@Name='DepthOfInheritance']/@Value"/>
      </td>
      <td>
        <xsl:value-of select="Metrics/Metric[@Name='LinesOfCode']/@Value"/>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
