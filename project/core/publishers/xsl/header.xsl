<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
    xmlns:lxslt="http://xml.apache.org/xslt">

    <xsl:output method="html"/>

    <xsl:template match="/">
        <xsl:variable name="modification.list" select="cruisecontrol/modifications/modification"/>

        <table cellpadding="2" cellspacing="0" border="0" width="98%">

            <xsl:if test="cruisecontrol/build/@error">
                <tr><td style="font-size:12px; color:#000000; font-weight:bold;">BUILD FAILED</td></tr>
                <tr><td style="color:#000000;">
                    <span style="font-weight:bold;">Error Message:</span>
                    <xsl:value-of select="cruisecontrol/build/@error"/>
                </td></tr>
            </xsl:if>

            <xsl:if test="not (cruisecontrol/build/@error)">
                <tr><td style="font-size:12px; color:#000000; font-weight:bold;">BUILD COMPLETE
                </td></tr>
            </xsl:if>

            <tr><td style="color:#000000;">
                <span style="font-weight:bold;">Date of build:&#160;</span>
                <xsl:value-of select="cruisecontrol/build/@date"/>
            </td></tr>
            <tr><td style="color:#000000;">
                <span style="font-weight:bold;">Time to build:&#160;</span>
                <xsl:value-of select="cruisecontrol/build/@buildtime"/>
            </td></tr>
            <xsl:apply-templates select="$modification.list">
                <xsl:sort select="date" order="descending" data-type="text" />
            </xsl:apply-templates>
        </table>
    </xsl:template>

    <!-- Last Modification template -->
    <xsl:template match="modification">
        <xsl:if test="position() = 1">
            <tr><td style="color:#000000;">
                <span style="font-weight:bold;">Last changed:&#160;</span>
                <xsl:value-of select="date"/>
            </td></tr>
            <tr><td style="color:#000000;">
                <span style="font-weight:bold; ">Last log entry:&#160;</span>
                <xsl:value-of select="comment"/>
            </td></tr>
        </xsl:if>
    </xsl:template>
</xsl:stylesheet>
