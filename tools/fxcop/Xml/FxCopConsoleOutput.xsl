<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://www.w3.org/TR/xhtml1/strict">

<xsl:output method="text"/>

<xsl:template match="Message">
	<xsl:apply-templates select="SourceCode"/>
	<xsl:apply-templates select="Rule"/>
	<xsl:apply-templates select=".." mode="parent"/> : <xsl:apply-templates select="Resolution/Text" />
	<xsl:text disable-output-escaping="yes">&#xD;&#xA;</xsl:text>
</xsl:template>

<xsl:template match="SourceCode"><xsl:value-of select="@Path"/>\<xsl:value-of select="@File"/>(<xsl:value-of select="@Line"/>) : </xsl:template>

<xsl:template match="Rule"><xsl:value-of select="@TypeName"/> : </xsl:template>

<xsl:template match="Text">
	<xsl:value-of select="translate(normalize-space(text()),':','')"/>
</xsl:template>

<xsl:template match="Rules/Rule"/>
<xsl:template match="Note"/>

<xsl:template match="Messages" mode="parent">
	<xsl:apply-templates select=".." mode="parent" />
</xsl:template>

<xsl:template match="Namespace" mode="parent">
	<xsl:value-of select="@Name" />
	<xsl:apply-templates select=".." mode="parent" />
</xsl:template>

<xsl:template match="Namespaces" mode="parent">
	<xsl:if test="not(name(..)='FxCopReport')"><xsl:apply-templates select=".." mode="parent"/></xsl:if>
</xsl:template>

<xsl:template match="*" mode="parent">
	<xsl:for-each select="ancestor-or-self::*[ancestor-or-self::Module]">
		<xsl:choose>			
			<xsl:when test="self::Parameter">!Parameter[<xsl:value-of select="@Name" />]</xsl:when>
			<xsl:when test="self::Module"><xsl:value-of select="@Name" />, </xsl:when>
			<xsl:otherwise><xsl:value-of select="translate(@Name,':', '#')"/><xsl:if test="@Name"><xsl:if test="not(name(child::node())='Messages')">.</xsl:if></xsl:if></xsl:otherwise>
		</xsl:choose>
	</xsl:for-each>
</xsl:template>
			
</xsl:stylesheet>


