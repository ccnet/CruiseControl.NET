<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/xhtml1/strict">
<xsl:output method="html"/>
	<xsl:template match="/">
		<xsl:apply-templates select="//coverage" />					
	</xsl:template>
	<xsl:template match="coverage">
		<html>
			<head>
				<title>Code Coverage Report</title>
			</head>
			<body>
				<div style="font-size: 12pt; font-weight: bold;">Code Coverage Report</div>
				<p/>
				<xsl:apply-templates select="module"/>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="module">
		<div style="font-size: 10pt">
			<xsl:value-of select="@assembly"/>
		</div>
		<p/>
		<xsl:apply-templates select="method"/>
	</xsl:template>
	<xsl:template match="method">
		<div style="color: maroon; font-size: 10pt; font-weight: bold;">
			<xsl:value-of select="@class"/>.<xsl:value-of select="@name"/>
		</div>
		<table border="1" cellpadding="3" cellspacing="0" bordercolor="black">
			<tr>
				<td bgcolor="DDEEFF">Visit Count</td>
				<td bgcolor="DDEEFF">Line</td>
				<td bgcolor="DDEEFF">Column</td>
				<td bgcolor="DDEEFF">End Line</td>
				<td bgcolor="DDEEFF">End Column</td>
				<td bgcolor="DDEEFF">Document</td>
			</tr>
			<xsl:apply-templates select="seqpnt"/>
		</table>
		<p/>
	</xsl:template>
	<xsl:template match="seqpnt">
		<tr>
			<td bgcolor="#FFFFEE">
				<xsl:attribute name="bgcolor">
					<xsl:choose>
						<xsl:when test="@visitcount = 0">#FFCCCC</xsl:when>
						<xsl:otherwise>#FFFFEE</xsl:otherwise>
					</xsl:choose>
				</xsl:attribute>
				<xsl:value-of select="@visitcount"/>
			</td>
			<td bgcolor="#FFFFEE">
				<xsl:value-of select="@line"/>
			</td>
			<td bgcolor="#FFFFEE">
				<xsl:value-of select="@column"/>
			</td>
			<td bgcolor="#FFFFEE">
				<xsl:value-of select="@endline"/>
			</td>
			<td bgcolor="#FFFFEE">
				<xsl:value-of select="@endcolumn"/>
			</td>
			<td bgcolor="#FFFFEE">
				<xsl:value-of select="@document"/>
			</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>
