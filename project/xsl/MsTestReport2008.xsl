<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>
    
    <xsl:template match="/">
     
        <xsl:variable name="pass_count" select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='ResultSummary']/*[local-name()='Counters']/@passed"/>
    	<xsl:variable name="inconclusive_count" select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='ResultSummary']/*[local-name()='Counters']/@inconclusive"/>
    	<xsl:variable name="failed_count" select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='ResultSummary']/*[local-name()='Counters']/@failed"/>
		<xsl:variable name="total_count" select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='ResultSummary']/*[local-name()='Counters']/@total"/>

        <table class="section-table" width="100%">
            <tr>
                <td class="sectionheader">
                    Tests run: <xsl:value-of select="$total_count"/>, Failures: <xsl:value-of select="$failed_count"/>, Inconclusive: <xsl:value-of select="$inconclusive_count"/>
                </td>
            </tr>

            <tr>
                <td>
                    <table>
                        <tr>
                            <td width="100">
                                <b>Passed:</b>
                            </td>
                            <td>
                                <xsl:value-of select="$pass_count"/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>Failed:</b>
                            </td>
                            <td>
                                <xsl:value-of select="$failed_count"/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>Inconclusive:</b>
                            </td>
                            <td>
                                <xsl:value-of select="$inconclusive_count"/>
                            </td>
                        </tr> 
										</table>
                </td>
            </tr>
             
            <tr>
                <td width="100%">
                    <table border="1" width="100%">
                        <tr height="20">
                            <xsl:choose>
                                <xsl:when test="$total_count=$pass_count">
                                    <td bgcolor="00FF33" align="center">
                                        <b>PASSED</b>
                                    </td>
                                </xsl:when>
                                <xsl:otherwise>
                                    <td bgcolor="FF0000" align="center">
                                        <b>FAILED</b>
                                    </td>
                                </xsl:otherwise>
                            </xsl:choose>
                        </tr>
                    </table>
                </td>
            </tr>
            
            <tr>
                <td>
                    <table border="1" width="100%" frame="box">
                        <tr bgcolor="#2288FF">
                            <th align="center" width="40">Code</th>
                            <th align="left">Method</th>
                            <th align="left">Message</th>
                            <th align="left">Duration</th>
                        </tr>
						<xsl:apply-templates select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='Results']/*[local-name()='UnitTestResult']" />
					</table>
                </td>
            </tr>
        </table>
    </xsl:template>

	<xsl:template match="*[local-name()='UnitTestResult']">
		<tr>
	        <xsl:choose>
	             <xsl:when test="@outcome = 'Passed'">
					<td bgcolor="00FF33" align="center"> P </td>
				</xsl:when>
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
				<xsl:value-of select="*[local-name()='Output']/*[local-name()='ErrorInfo']/*[local-name()='Message']"/>
			</td>
			 <td>
				<xsl:value-of select="concat(substring-before(@duration,'.'),'.',substring(substring-after(@duration,'.'),1,7))"/>
			</td>
		</tr>
	 </xsl:template>
</xsl:stylesheet>
