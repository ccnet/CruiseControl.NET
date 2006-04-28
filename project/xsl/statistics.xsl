<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html"/>
	<xsl:template match="/statistics">
	<style>
		*.pass{
			background-color: #33ff99;
		}
		*.fail{	
			background-color: #ff6600;
		}
		*.unknown{	
			background-color: #ffffcc;
		}
	</style>
		<p>
			Today is
			<xsl:variable name="day" select="//timestamp/@day"/>
			<xsl:variable name="month" select="//timestamp/@month"/>
			<xsl:variable name="year" select="//timestamp/@year"/>
			<xsl:value-of select="$day"/>/<xsl:value-of select="$month"/>/<xsl:value-of select="$year"/> <br />
			<xsl:variable name="totalCount" select="count(integration)"/>
			<xsl:variable name="successCount" select="count(integration[@status='Success'])"/>
			<xsl:variable name="failureCount" select="$totalCount - $successCount"/>
			<xsl:variable name="totalCountForTheDay" select="count(integration[@day=$day and @month=$month and @year=$year])"/>
			<xsl:variable name="successCountForTheDay" select="count(integration[@status='Success' and @day=$day and @month=$month and @year=$year])"/>
			<xsl:variable name="failureCountForTheDay" select="$totalCountForTheDay - $successCountForTheDay"/>
			<table border="1" cellpadding="0" cellspacing="0">
				<tr>
					<th>Integration Summary</th>
					<th>For today</th>
					<th>Overall</th>
				</tr>
				<tr>
					<th align="left">Total Builds</th>
					<td><xsl:value-of select="$totalCountForTheDay"/></td>
					<td><xsl:value-of select="$totalCount"/></td>
				</tr>
				<tr>
					<th align="left">Number of Successful</th>
					<td><xsl:value-of select="$successCountForTheDay"/></td>
					<td><xsl:value-of select="$successCount"/></td>
				</tr>
				<tr>
					<th align="left">Number of Failed</th>
					<td><xsl:value-of select="$failureCountForTheDay"/></td>
					<td><xsl:value-of select="$failureCount"/></td>
				</tr>
			</table>
		</p>
		<p><pre><strong>Note: </strong>Only builds run with the statistics publisher enabled will appear on this page!</pre></p>
		<table>
			<tr>
				<th>Build Label</th>
				<th>Status</th>
				<xsl:for-each select="./integration[1]/statistic">
					<th>
						<xsl:value-of select="./@name" />
					</th>
				</xsl:for-each>
			</tr>
			<xsl:for-each select="./integration">
				<xsl:sort select="position()" data-type="number" order="descending"/>
				<xsl:variable name="colorClass">
					<xsl:choose>
						<xsl:when test="./@status = 'Success'">pass</xsl:when>
						<xsl:when test="./@status = 'Unknown'" >unknown</xsl:when>
						<xsl:otherwise>fail</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<tr>
					<th>
						<xsl:value-of select="./@build-label"/>
					</th>
					<th class="{$colorClass}">
						<xsl:value-of select="./@status"/>
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

  