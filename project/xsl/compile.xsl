<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>

    <xsl:template match="/">
   	<xsl:variable name="messages" select="/cruisecontrol//buildresults//message | //builderror//message" />
   	<xsl:if test="count($messages) > 0">   	
		<xsl:variable name="error.messages" select="$messages[(contains(text(), 'error') and not(contains(text(), '0 error') or contains(text(), 'Build complete'))) or @level='Error']" />
	        <xsl:variable name="error.messages.count" select="count($error.messages)" />
	        <xsl:variable name="warning.messages" select="$messages[(contains(text(), 'warning') and not(contains(text(), '0 warning') or contains(text(), 'Build complete'))) or @level='Warning']" />
	        <xsl:variable name="warning.messages.count" select="count($warning.messages)" />
	        <xsl:variable name="total" select="count($error.messages) + count($warning.messages)"/>

	        <xsl:if test="$total > 0">
	            <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
	                <tr>
	                    <td class="compile-sectionheader">
	                        Errors/Warnings (<xsl:value-of select="$total"/>)
	                    </td>
	                </tr>
	                <xsl:if test="$total > 0">
		                <tr><td><xsl:apply-templates select="$error.messages"/></td></tr>
		                <tr><td><xsl:apply-templates select="$warning.messages"/></td></tr>
	                </xsl:if>
	            </table>
	        </xsl:if>
        </xsl:if>
    </xsl:template>

    <xsl:template match="message">
        <pre class="compile-error-data"><xsl:value-of select="text()"/></pre>
    </xsl:template>

</xsl:stylesheet>
