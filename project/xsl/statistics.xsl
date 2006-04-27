<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html"/>
	<xsl:template match="/statistics">
	<style>
		*.pass{
			background-color: green;
		}
		*.fail{	
			background-color: red;
		}
		*.unknown{	
			background-color: yellow;
		}
	</style>

		<table>
			<tr>
				<th>Build Label</th>
				<xsl:for-each select="./integration[1]/statistic">
					<th>
						<xsl:value-of select="./@name" />
					</th>
				</xsl:for-each>
			</tr>
			<xsl:for-each select="./integration">
				<xsl:variable name="colorClass">
					<xsl:choose>
						<xsl:when test="./@status = 0">pass</xsl:when>
						<xsl:when test="./@status = 3" >unknown</xsl:when>
						<xsl:otherwise>fail</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<tr class="{$colorClass}">
					<th>
						<xsl:value-of select="./@build-label"/>
					</th>
					<xsl:for-each select="./statistic">
						<td>
							<xsl:value-of select="."/>
						</td>
					</xsl:for-each>
				</tr>
			</xsl:for-each>
		</table>
	</xsl:template>
</xsl:stylesheet>

  