<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/xhtml1/strict">
  <xsl:output method="html"/>
  <xsl:template match="/">
    <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
      <tr>
        <td class="sectionheader" colspan="8">
          Microsoft Metrics
        </td>
      </tr>
      <tr>
        <th colspan="3">Item</th>
        <th>Maintainability Index</th>
        <th>Cyclomatic Complexity</th>
        <th>Class Coupling</th>
        <th>Inheritance Depth</th>
        <th>Lines Of Code</th>
      </tr>
      <xsl:apply-templates select="//CodeMetricsReport/Targets/Target/Modules/Module">
        <xsl:sort select="@Name"/>
      </xsl:apply-templates>
    </table>
  </xsl:template>

  <xsl:template match="Metrics">
    <td>
      <xsl:value-of select="Metric[@Name='MaintainabilityIndex']/@Value"/>
    </td>
    <td>
      <xsl:value-of select="Metric[@Name='CyclomaticComplexity']/@Value"/>
    </td>
    <td>
      <xsl:value-of select="Metric[@Name='ClassCoupling']/@Value"/>
    </td>
    <td>
      <xsl:value-of select="Metric[@Name='DepthOfInheritance']/@Value"/>
    </td>
    <td>
      <xsl:value-of select="Metric[@Name='LinesOfCode']/@Value"/>
    </td>
  </xsl:template>

  <xsl:template match="Module">
    <tr>
      <td colspan="3">
        <xsl:value-of select="@Name"/>
      </td>
      <xsl:apply-templates select="Metrics" />
    </tr>
    <xsl:apply-templates select="Namespaces/Namespace">
      <xsl:sort select="@Name"/>
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="Namespace">
    <tr>
      <td>
        <span style="width:10px;"/>
      </td>
      <td colspan="2">
        <xsl:value-of select="@Name"/>
      </td>
      <xsl:apply-templates select="Metrics" />
    </tr>
    <xsl:apply-templates select="Types/Type">
      <xsl:sort select="@Name"/>
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="Type">
    <tr>
      <td>
        <span style="width:10px;"/>
      </td>
      <td>
        <span style="width:10px;"/>
      </td>
      <td>
        <xsl:value-of select="@Name"/>
      </td>
      <xsl:apply-templates select="Metrics" />
    </tr>
  </xsl:template>
</xsl:stylesheet>
