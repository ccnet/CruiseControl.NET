<?xml version="1.0"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns="http://www.w3.org/TR/html4/strict.dtd" >

    <xsl:output method="html"/>

    <xsl:template match="/">
        <xsl:variable name="error.messages" select="/cruisecontrol/build/buildresults//message[contains(text(), 'error')]" />
        <xsl:variable name="error.messages.count" select="count($error.messages)" />
        <xsl:variable name="warning.messages" select="/cruisecontrol/build/buildresults//message[contains(text(), 'warning')]" />
        <xsl:variable name="warning.messages.count" select="count($warning.messages)" />
        <xsl:variable name="total" select="count($error.messages) + count($warning.messages)"/>

        <xsl:if test="$total > 0">
            <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
                <tr>
                    <td class="compile-sectionheader">
                        &#160;Errors/Warnings (<xsl:value-of select="$total"/>)
                    </td>
                </tr>
                <xsl:if test="$total > 0">
                    <tr>
                        <td>
                            <xsl:apply-templates select="$error.messages"/>
                            <xsl:apply-templates select="$warning.messages"/>
                        </td>
                    </tr>
                </xsl:if>
            </table>
        </xsl:if>
    </xsl:template>

    <xsl:template match="message[contains(text(), 'error')]|message[contains(text(), 'warning')]">
        <p class="compile-error-data">
            <xsl:value-of select="text()"/>
        </p>
    </xsl:template>

</xsl:stylesheet>
