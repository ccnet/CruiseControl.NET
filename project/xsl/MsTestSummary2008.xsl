<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>
    
    <xsl:template match="/">
    			<xsl:variable name="pass_count" select="sum(/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='ResultSummary']/*[local-name()='Counters']/@passed)"/>
    			<xsl:variable name="inconclusive_count" select="sum(/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='ResultSummary']/*[local-name()='Counters']/@inconclusive)"/>
    			<xsl:variable name="failed_count" select="sum(/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='ResultSummary']/*[local-name()='Counters']/@failed)"/>
				<xsl:variable name="total_count" select="sum(/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='ResultSummary']/*[local-name()='Counters']/@total)"/>

			<xsl:if test="$total_count != 0">
				<table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
					<tr>
						<td class="sectionheader" colspan="3">
							Tests run: <xsl:value-of select="$total_count"/>
							Failures: <xsl:value-of select="$failed_count" />
							Inconclusive: <xsl:value-of select="$inconclusive_count" />
						</td>
					</tr>
					<xsl:choose>
						<xsl:when test="$total_count = $pass_count">
							<tr>
								<td class="section-data" colspan="2">All tests passed.</td>
								<td class="section-data" colspan="2">Total count: <xsl:value-of select="$total_count"/></td>
								<td class="section-data" colspan="2">Passed count: <xsl:value-of select="$pass_count"/></td>
							</tr>
						</xsl:when>
						<xsl:otherwise>
							<xsl:apply-templates select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='Results']/*[local-name()='UnitTestResult'][@outcome!='Passed']" />
						</xsl:otherwise>
					</xsl:choose>
				</table>
			</xsl:if>
    </xsl:template>
    
    <xsl:template match="*[local-name()='UnitTestResult']">
			<tr>
				<xsl:choose>
					<xsl:when test="@outcome = 'Failed'">
						<td bgcolor="FF0000" align="center"> F </td>
					</xsl:when>
					<xsl:when test="@outcome = 'Inconclusive'">
						<td bgcolor="FFCC00" align="center"> I </td>
					</xsl:when>
					<xsl:otherwise>
						<td bgcolor="3399FF" align="center"> ? </td>
					</xsl:otherwise>
				</xsl:choose>
				<td>
					<xsl:value-of select="@testName"/>
				</td>
				<td>
					<xsl:value-of select="./*[local-name()='Output']/*[local-name()='ErrorInfo']/*[local-name()='Message']"/>
				</td>
			</tr>
    </xsl:template>
</xsl:stylesheet>
