<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/xhtml1/strict">
<xsl:output method="html"/>
	<xsl:template match="/">
		<xsl:apply-templates select="//coverage[count(module) != 0]" />					
	</xsl:template>
	
	<xsl:template match="coverage">
		<xsl:variable name="covered.lines" select="count(//coverage/module/method/seqpnt[@visitcount > 0])" />
		<xsl:variable name="uncovered.lines" select="count(//coverage/module/method/seqpnt[@visitcount = 0])" />
	
        <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
            <tr>
                <td class="sectionheader" colspan="3">
                   NCover results:
                </td>
            </tr>
            <tr>
				<td>
					Covered lines: <xsl:value-of select="$covered.lines"/>
				</td>
				<td>
					Uncovered lines: <xsl:value-of select="$uncovered.lines"/>
				</td>
				<td>
					Total coverage: <xsl:value-of select="round($covered.lines div ($uncovered.lines + $covered.lines) * 100)"/>%
				</td>
            </tr>
		</table>
	</xsl:template>
</xsl:stylesheet>
