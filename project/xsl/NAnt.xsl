<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
    <xsl:output method="html"/>

    <xsl:template match="/">
		<xsl:apply-templates select="//buildresults" />
	</xsl:template>
	
	<xsl:template match="buildresults">
		<xsl:apply-templates select="message"/>
		<xsl:apply-templates select="task"/>
		<xsl:apply-templates select="target"/>
	</xsl:template>
	
	<xsl:template match="target">
		<p>
		<xsl:value-of select="@name"/>:
		</p>
		<xsl:apply-templates select="message"/>
		<xsl:apply-templates select="task"/>
	</xsl:template>
	
	<xsl:template match="task">
		<xsl:if test="count(message) > 0">
			<xsl:apply-templates select="message"/>
		</xsl:if>
		<xsl:apply-templates select="target"/>	
	</xsl:template>
	
	<xsl:template match="message">
		<span>
			<xsl:if test="@level='Error' or @level='Warning'">
				<xsl:attribute name="style">color:#F30</xsl:attribute>
			</xsl:if>
			<xsl:if test="../@name != ''">
				[<xsl:value-of select="../@name"/>] 
			</xsl:if>
			<xsl:value-of select="text()" />
		</span>
		<br/>
	</xsl:template>
</xsl:stylesheet>
