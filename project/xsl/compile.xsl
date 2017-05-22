<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>

    <xsl:template match="/">
   		<xsl:variable name="messages" select="/cruisecontrol//buildresults//message" />
   		<xsl:if test="count($messages) > 0">   	
			    <xsl:variable name="error.messages" select="$messages[(contains(text(), 'error ') and not(contains(text(), 'warning ') )) or @level='Error'] | /cruisecontrol//builderror/message | /cruisecontrol//internalerror/message" />
	        <xsl:variable name="error.messages.count" select="count($error.messages)" />
	        <xsl:variable name="warning.messages" select="$messages[(contains(text(), 'warning ')) or @level='Warning']" />
	        <xsl:variable name="warning.messages.count" select="count($warning.messages)" />
	        <xsl:variable name="total" select="count($error.messages) + count($warning.messages)"/>

	        <xsl:if test="$error.messages.count > 0">
	            <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
	                <tr><td class="sectionheader">Errors: (<xsl:value-of select="$error.messages.count"/>)</td></tr>
					<xsl:apply-templates select="$error.messages"/>
	            </table>
	        </xsl:if>
	        <xsl:if test="$warning.messages.count > 0">
	            <table class="section-table" cellpadding="2" cellspacing="0" border="1" width="98%">
	                <tr><td class="sectionheader">Warnings: (<xsl:value-of select="$warning.messages.count"/>)</td></tr>
	                <xsl:apply-templates select="$warning.messages"/>
	            </table>
	        </xsl:if>
      </xsl:if>
    </xsl:template>

    <xsl:template match="message">
        <tr class="error"><td><xsl:value-of select="substring(text(),1,1024)"/></td></tr>
    </xsl:template>

</xsl:stylesheet>