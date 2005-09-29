<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>
    
    <xsl:template match="/">
		<xsl:variable name="mstest.resultnodes" select="//Tests/TestRun/result" />
		
		<xsl:if test="count($mstest.resultnodes)>0">
			<xsl:variable name="mstest.testcount" select="sum($mstest.resultnodes/totalTestCount)" />		
			<xsl:variable name="mstest.executedcount" select="sum($mstest.resultnodes/executedTestCount)" />		
			<xsl:variable name="mstest.failurecount" select="$mstest.executedcount - sum($mstest.resultnodes/passedTestCount)" />		
			<table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
				<tr>
					<td class="sectionheader" colspan="2">
						Tests run: <xsl:value-of select="$mstest.executedcount"/>
						Failures: <xsl:value-of select="$mstest.failurecount" />
						Not run: <xsl:value-of select="$mstest.testcount - sum($mstest.resultnodes/executedTestCount)" />
					</td>
				</tr>
				<xsl:choose>
					<xsl:when test="$mstest.failurecount = 0">
						<tr>
							<td class="section-data" colspan="2">All tests passed.</td>
						</tr>
					</xsl:when>
					<xsl:otherwise>
						<xsl:apply-templates select="//Tests/UnitTestResult[outcome/value__ = 1]" />
					</xsl:otherwise>
				</xsl:choose>
			</table>
		</xsl:if>		
    </xsl:template>
    
    <xsl:template match="UnitTestResult">
		<tr>
			<td class="section-data" valign="top">Test:</td>
			<td class="section-data"><xsl:value-of select="testName" /></td>
		</tr>
		<tr>
			<td class="section-data" valign="top">Message:</td>
			<td class="section-data"><xsl:value-of select="errorInfo/message" /></td>
		</tr>
		<tr>
			<td class="section-data" valign="top">Stacktrace:</td>
			<td class="section-data"><pre><xsl:value-of select="errorInfo/stackTrace" /></pre></td>
		</tr>
    </xsl:template>
</xsl:stylesheet>
