<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>
    <xsl:variable name="tasklist" select="//target/task"/>
    <xsl:variable name="jar.tasklist" select="$tasklist[@name='csc']"/>
    <xsl:variable name="dist.count" select="count($jar.tasklist)"/>

    <xsl:template match="/">
        <table cellpadding="2" cellspacing="0" border="0" width="98%">

            <xsl:if test="$dist.count > 0">
                <tr>
                    <td class="distributables-sectionheader">
                         Deployments by this build: (<xsl:value-of select="$dist.count"/>)
                    </td>
                </tr>
                <xsl:apply-templates select="$jar.tasklist" />
            </xsl:if>

        </table>
    </xsl:template>

    <xsl:template match="task[@name='csc']/message">
        <tr>
            <xsl:if test="position() mod 2 = 0">
                <xsl:attribute name="class">distributables-oddrow</xsl:attribute>
            </xsl:if>
            <td class="distributables-data">
                <xsl:value-of select="text()"/>
            </td>
        </tr>
    </xsl:template>

</xsl:stylesheet>
