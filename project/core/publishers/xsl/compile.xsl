<?xml version="1.0"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns="http://www.w3.org/TR/html4/strict.dtd" >

    <xsl:output method="html"/>

    <xsl:template match="/">

		<xsl:variable name="error.messages" select="/cruisecontrol/build/buildresults//message[@type='error']" />
		<xsl:variable name="error.messages.count" select="count($error.messages)" />
		
        <xsl:if test="$error.messages.count > 0">
            <table cellpadding="2" cellspacing="0" border="0" width="98%">
                <tr>
                    <td style="background-color:#000066; color:#FFFFFF;">
                        &#160;Errors/Warnings: (<xsl:value-of select="$error.messages.count"/>)
                    </td>
                </tr>
                <xsl:if test="$error.messages.count > 0">
					<tr>
						<td>
							<xsl:apply-templates select="$error.messages"/>
						</td>
					</tr>
                </xsl:if>
            </table>
        </xsl:if>

    </xsl:template>
	<xsl:template match="message[@type='error']">
			<p style="font-size:9px; color:#FF3300;">
				<xsl:value-of select="text()"/>
			</p>
	</xsl:template>
</xsl:stylesheet>
