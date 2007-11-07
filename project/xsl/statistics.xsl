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
		<p><pre><strong>Note: </strong>Only builds run with the statistics publisher enabled will appear on this page!</pre></p>
			<xsl:variable name="day" select="//timestamp/@day"/>
			<xsl:variable name="month" select="//timestamp/@month"/>
			<xsl:variable name="year" select="//timestamp/@year"/>
			<xsl:variable name="totalCount" select="count(integration)"/>
			<xsl:variable name="successCount" select="count(integration[@status='Success'])"/>
			<xsl:variable name="failureCount" select="$totalCount - $successCount"/>
			<xsl:variable name="totalCountForTheDay" select="count(integration[@day=$day and @month=$month and @year=$year])"/>
			<xsl:variable name="successCountForTheDay" select="count(integration[@status='Success' and @day=$day and @month=$month and @year=$year])"/>
			<xsl:variable name="failureCountForTheDay" select="$totalCountForTheDay - $successCountForTheDay"/>
 			<xsl:variable name="okPercent" select="$successCount div $totalCount * 100 "/> 
			<xsl:variable name="nokPercent" select="$failureCount div $totalCount * 100 "/> 

		<p>
			Today is
			<xsl:value-of select="$day"/>/<xsl:value-of select="$month"/>/<xsl:value-of select="$year"/> <br />

			<table border="1" cellpadding="0" cellspacing="0">
				<tr>
					<th>Integration Summary</th>
					<th>For today</th>
					<th>Overall</th>
				</tr>
				<tr>
					<th align="left">Total Builds</th>
					<td align="center"><xsl:value-of select="$totalCountForTheDay"/></td>
					<td align="center"><xsl:value-of select="$totalCount"/></td>
				</tr>
				<tr>
					<th align="left">Number of Successful</th>
					<td align="center"><xsl:value-of select="$successCountForTheDay"/></td>
					<td align="center"><xsl:value-of select="$successCount"/></td>
				</tr>
				<tr>
					<th align="left">Number of Failed</th>
					<td align="center"><xsl:value-of select="$failureCountForTheDay"/></td>
					<td align="center"><xsl:value-of select="$failureCount"/></td>
				</tr>
			</table>
			
			<br/>
 			<table border="0" cellspacing="1" >
				<tr>
				  <td nowrap=""> Succesfull build rate : <xsl:value-of select="round($okPercent * 100) div 100" />%</td>
				  <td></td>
				  <td bgcolor="green" > 
						 <xsl:attribute name="width"><xsl:value-of select="$okPercent" /></xsl:attribute>				  				  
				  </td>

                  <td bgcolor="red"> 
						 <xsl:attribute name="width"><xsl:value-of select="$nokPercent" /></xsl:attribute>				  				  				  
				  </td>
         </tr>
       </table>
		</p>
		<br/>
		<table border="1" frame="above" rules="colls" cellpadding="2">		   
			<thead>
			<tr>
				<th>Build Label</th>
				<th>Status</th>
				<xsl:for-each select="./integration[last()]/statistic">
					<th>
						<xsl:value-of select="./@name" />
					</th>
				</xsl:for-each>
			</tr>
			</thead>			

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

					<xsl:if test="position() mod 2=0">
						 <xsl:attribute name="class">section-oddrow</xsl:attribute>
					 </xsl:if>
					 <xsl:if test="position() mod 2!=0">
						 <xsl:attribute name="class">section-evenrow</xsl:attribute>
					 </xsl:if>                                                 

					<th>
						<xsl:value-of select="./@build-label"/>
					</th>
					<th class="{$colorClass}">
						<xsl:value-of select="./@status"/>
					</th>
					<xsl:for-each select="./statistic">
						<td align="center">
							<xsl:value-of select="."/>
						</td>
					</xsl:for-each>
				</tr>
			</xsl:for-each>
		</table>
	</xsl:template>
</xsl:stylesheet>

  