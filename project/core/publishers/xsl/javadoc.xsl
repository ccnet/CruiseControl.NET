<?xml version="1.0"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns="http://www.w3.org/TR/html4/strict.dtd" >

    <xsl:output method="html"/>

    <xsl:variable name="tasklist" select="/cruisecontrol/build//target/task"/>
    <xsl:variable name="javadoc.tasklist" select="$tasklist[@name='Javadoc'] | $tasklist[@name='javadoc']"/>

    <xsl:template match="/">

        <xsl:variable name="javadoc.error.messages" select="$javadoc.tasklist/message[@priority='error']"/>
        <xsl:variable name="javadoc.warn.messages" select="$javadoc.tasklist/message[@priority='warn']"/>
        <xsl:variable name="total.errorMessage.count" select="count($javadoc.warn.messages)  + count($javadoc.error.messages)"/>

        <xsl:if test="$total.errorMessage.count > 0">
            <table class="section-table" align="center" cellpadding="2" cellspacing="0" border="0" width="98%">
                <tr>
                    <!-- NOTE: total.errorMessage.count is actually the number of lines of error
                     messages. This accurately represents the number of errors ONLY if the Ant property
                     build.compiler.emacs is set to "true" -->
                    <td class="javadoc-sectionheader">
                        &#160;Javadoc Errors/Warnings (<xsl:value-of select="$total.errorMessage.count"/>)
                    </td>
                </tr>
                <xsl:if test="count($javadoc.error.messages) > 0">
                    <tr><td class="javadoc-error"><xsl:apply-templates select="$javadoc.error.messages"/></td></tr>
                </xsl:if>
                <xsl:if test="count($javadoc.warn.messages) > 0">
                    <tr><td class="javadoc-warning"><xsl:apply-templates select="$javadoc.warn.messages"/></td></tr>
                </xsl:if>
            </table>
        </xsl:if>

    </xsl:template>

    <xsl:template match="message[@priority='error']">
        <xsl:value-of select="text()"/>
        <xsl:if test="count(./../message[@priority='error']) != position()">
            <br/>
        </xsl:if>
    </xsl:template>

    <xsl:template match="message[@priority='warn']">
        <xsl:value-of select="text()"/><br/>
    </xsl:template>

</xsl:stylesheet>
