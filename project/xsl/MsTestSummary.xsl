<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>
    
    <xsl:template match="/">
    			<xsl:variable name="pass_toplevel" select="count(/cruisecontrol/build/Tests/UnitTestResult[outcome=10])"/>
    			<xsl:variable name="pass_multiples" select="count(/cruisecontrol/build/Tests/UnitTestResult/innerResults/element[outcome=10])"/>
    			<xsl:variable name="pass_count" select="$pass_toplevel + $pass_multiples"/>
    			<xsl:variable name="inconclusive_toplevel" select="count(/cruisecontrol/build/Tests/UnitTestResult[outcome=4])"/>
    			<xsl:variable name="inconclusive_multiples" select="count(/cruisecontrol/build/Tests/UnitTestResult/innerResults/element[outcome=4])"/>
    			<xsl:variable name="inconclusive_count" select="$inconclusive_toplevel + $inconclusive_multiples"/>
    			<xsl:variable name="failed_toplevel" select="count(/cruisecontrol/build/Tests/UnitTestResult[outcome=1])"/>
    			<xsl:variable name="failed_multiples" select="count(/cruisecontrol/build/Tests/UnitTestResult/innerResults/element[outcome=1])"/>
    			<xsl:variable name="failed_count" select="$failed_toplevel + $failed_multiples"/>
			<xsl:variable name="total_count" select="$failed_count + $pass_count + $inconclusive_count"/>

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
							</tr>
						</xsl:when>
						<xsl:otherwise>
							<xsl:apply-templates select="//Tests/UnitTestResult[outcome!=10]" />
						</xsl:otherwise>
					</xsl:choose>
				</table>
			</xsl:if>
    </xsl:template>
    
    <xsl:template match="UnitTestResult">
			<xsl:variable name="testId" select="/cruisecontrol/build/Tests/UnitTestResult/id/testId/id"/>
			<xsl:variable name="testDetails" select="/cruisecontrol/build/Tests/TestRun/tests/value[id=$testId]"/>
			<tr>
				<xsl:choose>
					<xsl:when test="outcome = 1">
						<td bgcolor="FF0000" align="center"> F </td>
					</xsl:when>
					<xsl:when test="outcome = 4">
						<td bgcolor="FFCC00" align="center"> I </td>
					</xsl:when>
					<xsl:otherwise>
						<td bgcolor="3399FF" align="center"> ? </td>
					</xsl:otherwise>
				</xsl:choose>
				<td>
					<script type="text/javascript">
						var str= &quot; <xsl:value-of select="$testDetails/testMethod/className"/> &quot;
						var pos=str.indexOf(",");
						if (pos>=0) { var cs = str.substring(0, pos); document.write(cs); }
						else { document.write("&lt; class name not specified&gt;"); }
					</script>.<xsl:value-of select="$testDetails/testMethod/name"/>
				</td>
				<td>
					<xsl:value-of select="errorInfo/message"/>
				</td>
			</tr>
    </xsl:template>
</xsl:stylesheet>
