<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>

    <xsl:variable name="modification.list" select="cruisecontrol/modifications/modification"/>

    <xsl:template match="/">
        <table cellpadding="2" cellspacing="0" border="0" width="98%">
            <!-- Modifications -->
            <tr>
                <td style="font-size:12px; color:#000000; font-weight:bold;" colspan="4">
                    &#160;Modifications since last build:&#160;
                    (<xsl:value-of select="count($modification.list)"/>)
                </td>
            </tr>

            <xsl:apply-templates select="$modification.list">
                <xsl:sort select="date" order="descending" data-type="text" />
            </xsl:apply-templates>
            
        </table>
    </xsl:template>

    <!-- Modifications template -->
    <xsl:template match="modification">
        <tr>
            <xsl:if test="position() mod 2=0">
                <xsl:attribute name="style">background-color:#CCCCCC</xsl:attribute>
            </xsl:if>
            <xsl:if test="position() mod 2!=0">
                <xsl:attribute name="style">background-color:#FFFFCC</xsl:attribute>
            </xsl:if>

            <td style="font-size:9px; color:#000000;" valign="top"><xsl:value-of select="@type"/></td>
            <td style="font-size:9px; color:#000000;" valign="top"><xsl:value-of select="user"/></td>
            <td style="font-size:9px; color:#000000;" valign="top"><xsl:value-of select="project"/>/<xsl:value-of select="filename"/></td>
            <td style="font-size:9px; color:#000000;" valign="top"><xsl:value-of select="comment"/></td>
        </tr>
    </xsl:template>


</xsl:stylesheet>
