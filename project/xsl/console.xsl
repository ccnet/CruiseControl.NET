<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

    <xsl:output method="html"/>
	
	<xsl:template match="message" >
		<span>
			<xsl:if test="@level = 'Error'">
				<xsl:attribute name="style">color:red;</xsl:attribute>
			</xsl:if>
		<xsl:value-of select="text()" /></span><br />
	</xsl:template>
	
	<xsl:template match="/">
		<h1>Console Log</h1>
		<p style="font-family: consolas, monospace;font-size: 10pt;">
		<pre>
			<xsl:apply-templates select="/cruisecontrol/build/buildresults/message" />
		</pre>
		</p>
	</xsl:template>

</xsl:stylesheet>