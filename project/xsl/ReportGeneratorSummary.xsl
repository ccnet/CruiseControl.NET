<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:output method="html"/>
  <xsl:template match="/">
    <xsl:apply-templates select="/cruisecontrol/build/CoverageReport" />
  </xsl:template>
  <xsl:template match="/cruisecontrol/build/CoverageReport">
    <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
      <tr>
        <td class="sectionheader" colspan="4">
          Code Coverage Summary
        </td>
      </tr>
      <tr>
        <td class="header-label" width="300px">
          Module
          <br/><img src="..\images\shim.gif" width="300px" height="1px" />
        </td>
        <td class="header-label" width="100px">
          Coverage %
          <br/><img src="..\images\shim.gif" width="100px" height="1px" />
        </td>
        <td class="header-label" width="110px">
          Acceptance %
          <br/><img src="..\images\shim.gif" width="110px" height="1px" />
        </td>
        <td class="header-label" width="100%">
          Verdict
        </td>
      </tr>
			<xsl:for-each select="./Assemblies/Assembly">
				<xsl:call-template name="AssemblySummary" />
			</xsl:for-each>
      <tr>
				<td class="header-label">
					Total
				</td>
				<td class="header-label">
					<xsl:value-of select="format-number(Summary/Coveredlines div Summary/Coverablelines, '0.0%')"/>
				</td>
				<td class="header-label">
					95%
				</td>
				<td class="header-label">
          <xsl:choose>
            <xsl:when test="(Summary/Coveredlines div Summary/Coverablelines) &lt; 0.95">
              <span style="color:red">FAIL</span>
            </xsl:when>
            <xsl:otherwise>
              <span style="color:green">PASS</span>
             </xsl:otherwise>
          </xsl:choose>
				</td>
			</tr>
			<tr>
				<td colspan="4">&#160;</td>
			</tr>
      <tr class="section-oddrow">
				<td>
					Covered lines: 
				</td>
 				<td>
					<xsl:value-of select="Summary/Coveredlines"/>
				</td>
				<td>
					Assemblies: 
				</td>
				<td>
					<xsl:value-of select="Summary/Assemblies"/>
				</td>
      </tr>
      <tr>
				<td>
					Coverable lines: 
				</td>
				<td>
					<xsl:value-of select="Summary/Coverablelines"/>
				</td>
				<td>
					Files: 
				</td>
				<td>
					<xsl:value-of select="Summary/Files"/>
				</td>
      </tr>
      <tr class="section-oddrow">
  			<td>
					Total Lines:
				</td>
				<td>
					<xsl:value-of select="Summary/Totallines"/>
				</td>
 				<td>
					&#160;
				</td>
				<td>
					&#160;
				</td>
      </tr>
		</table>
	</xsl:template>
	
	<!-- Display a summary of each assembly and whether it passed -->
	<xsl:template name="AssemblySummary">
    <tr>
      <xsl:if test="position() mod 2 = 1">
        <xsl:attribute name="class">section-oddrow</xsl:attribute>
      </xsl:if>
      <td>
        <xsl:value-of select="@name"/>
      </td>
      <td>
        <xsl:value-of select="format-number(@coverage, '0.0')"/>%
      </td>
      <td>
        80%
      </td>
      <td>
        <xsl:choose>
          <xsl:when test="@coverage &lt; 80">
            <span style="color:red">FAIL</span>
          </xsl:when>
          <xsl:otherwise>
            <span style="color:green">PASS</span>
           </xsl:otherwise>
        </xsl:choose>
      </td>
    </tr>
	</xsl:template>
</xsl:stylesheet>
