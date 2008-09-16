<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>
    	
	<xsl:template match="/">
	
		<xsl:variable name="report.root" select="cruisecontrol//FxCopReport" />
	 
		<xsl:if test="count($report.root/Targets/Target) + count($report.root/Exceptions/Exception) > 0" >
            <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
                <tr>
                    <td class="sectionheader" colspan="2">
                    FxCop Messages: <xsl:value-of select="count($report.root//Message)"/>, Warning Messages: <xsl:value-of select="count($report.root//Message[Issue/@Level='Warning' or Issue/@Level='CriticalWarning'])"/>, Error Messages: <xsl:value-of select="count($report.root//Message[Issue/@Level='Error' or Issue/@Level='CriticalError'])"/>
                    </td>
                </tr>
                <tr><td colspan="2"> </td></tr>
                <xsl:apply-templates select="$report.root/Exceptions/Exception" />
                <xsl:apply-templates select="$report.root/Targets/Target/Modules/Module" />
            </table>
		</xsl:if>
	
	</xsl:template>
	
    <xsl:template match="Exception">
        <xsl:variable name="type" select="./Type" />
        <xsl:variable name="message" select="./ExceptionMessage" />
        
        <tr><td class="section-error"><xsl:value-of select="concat ($type, ': ', $message)" /></td></tr>
    </xsl:template>
    
	<xsl:template match="Module">
		<xsl:variable name="name" select="./@Name"/>
		<xsl:variable name="errors" select="count(.//Message[Issue/@Level='Error' or Issue/@Level='CriticalError'])"/>
		<xsl:variable name="warnings" select="count(.//Message[Issue/@Level='Warning' or Issue/@Level='CriticalWarning'])"/>
		
		<tr><td colspan="2"><xsl:value-of select="concat ($name, ': ', $errors, ' Errors, ', $warnings, ' Warnings')" /></td></tr>
	</xsl:template>
	
</xsl:stylesheet>