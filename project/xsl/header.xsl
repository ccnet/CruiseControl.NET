<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>

    <xsl:template match="/">
        <xsl:variable name="modification.list" select="/cruisecontrol/modifications/modification"/>

        <table class="section-table" cellpadding="2" cellspacing="0" border="0">

            <xsl:if test="/cruisecontrol/exception">
                <tr><td class="header-title" colspan="2">BUILD EXCEPTION</td></tr>
                <tr>
                    <td class="header-label" valign="top"><nobr>Error Message:</nobr></td>
                    <td class="header-data-error"><xsl:value-of select="/cruisecontrol/exception"/></td>
                </tr>
            </xsl:if>
            
            <xsl:if test="/cruisecontrol/build/@error">
                <tr><td class="header-title" colspan="2">BUILD FAILED</td></tr>
            </xsl:if>
            
            <xsl:if test="not (/cruisecontrol/build/@error) and not (/cruisecontrol/exception)">
                <tr><td class="header-title" colspan="2">BUILD SUCCESSFUL</td></tr>
            </xsl:if>

			<tr>
                <td class="header-label"><nobr>Project:</nobr></td>
                <td class="header-data"><xsl:value-of select="/cruisecontrol/@project"/></td>
			</tr>
            <tr>
                <td class="header-label"><nobr>Date of build:</nobr></td>
                <td class="header-data"><xsl:value-of select="/cruisecontrol/build/@date"/></td>
            </tr>
            <tr>
                <td class="header-label"><nobr>Running time:</nobr></td>
                <td class="header-data"><xsl:value-of select="/cruisecontrol/build/@buildtime"/></td>
            </tr>
            <tr>
                <td class="header-label"><nobr>Integration Request:</nobr></td>
                <td class="header-data">
					<xsl:value-of select="/cruisecontrol/request" />
                </td>
            </tr>

          <xsl:if test="/cruisecontrol/integrationProperties/CCNetForceBuildReason">
            <tr>
              <td class="header-label">
                <nobr>Reason for Force Build:</nobr>
              </td>
              <td class="header-data">
                <xsl:value-of select="/cruisecontrol/integrationProperties/CCNetForceBuildReason"/>
              </td>
            </tr>
          </xsl:if>
            
            <xsl:apply-templates select="$modification.list">
                <xsl:sort select="date" order="descending" data-type="text" />
            </xsl:apply-templates>
            
        </table>
    </xsl:template>

    <!-- Last Modification template -->
    <xsl:template match="/cruisecontrol/modifications/modification">
        <xsl:if test="position() = 1">
            <tr>
                <td class="header-label"><nobr>Last changed:</nobr></td>
                <td class="header-data"><xsl:value-of select="date"/></td>
            </tr>
 	    <xsl:if test="string-length(issueUrl)!=0">
              <tr>
                <td class="header-label" valign="top"><nobr>Link to issue:</nobr></td>
                <td class="header-data"><A><xsl:attribute name="HREF"><xsl:value-of select="issueUrl"/></xsl:attribute><xsl:value-of select="issueUrl"/></A></td>
              </tr>
 	    </xsl:if>
 	    <tr>
                <td class="header-label" valign="top"><nobr>Last log entry:</nobr></td>
                <td class="header-data"><xsl:value-of select="comment"/></td>
            </tr>
        </xsl:if>
    </xsl:template>
</xsl:stylesheet>
