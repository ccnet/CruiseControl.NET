<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://www.w3.org/TR/xhtml1/strict">

<xsl:output method="text"/>

<xsl:template match="/">
	<xsl:apply-templates select="//Issue" />
</xsl:template>

<xsl:template match="Issue">
	<xsl:apply-templates select=".." mode="parentMessage" /><xsl:value-of select="translate(normalize-space(text()),':','')" /><xsl:text disable-output-escaping="yes">&#xD;&#xA;</xsl:text>
</xsl:template>

<xsl:template match="Message" mode="parentMessage">	
<xsl:value-of select="@TypeName"/> : <xsl:apply-templates select=".." mode="signature"/> : </xsl:template>


<xsl:template match="Text">
	<xsl:value-of select="translate(normalize-space(text()),':','')"/>
</xsl:template>

<xsl:template match="Rules/Rule"/>
<xsl:template match="Note"/>

<xsl:template match="*" mode="signature">
  <xsl:choose>			
    <xsl:when test="self::Module"><xsl:value-of select="@Name" />, </xsl:when>
    <xsl:when test="self::Messages"><xsl:apply-templates select=".." mode="signature" /></xsl:when>
    <xsl:when test="self::Namespace"><xsl:apply-templates select=".." mode="parent" /><xsl:value-of select="@Name" /></xsl:when>
    <xsl:when test="self::Namespaces"><xsl:if test="not(name(..)='FxCopReport')"><xsl:apply-templates select=".." mode="parent"/></xsl:if></xsl:when>
    <xsl:when test="self::Target"><xsl:value-of select="@Name" />, </xsl:when>  
    <xsl:when test="@Name"><xsl:apply-templates select=".." mode="parent" /><xsl:value-of select="translate(@Name,':', '#')"/></xsl:when>  
    <xsl:otherwise><xsl:apply-templates select=".." mode="signature" /></xsl:otherwise>
  </xsl:choose>
</xsl:template>

<xsl:template match="*" mode="parent">
  <xsl:choose>			
    <xsl:when test="self::Module"><xsl:value-of select="@Name" />, </xsl:when>
    <xsl:when test="self::Messages"><xsl:apply-templates select=".." mode="parent" /></xsl:when>
    <xsl:when test="self::Namespace"><xsl:apply-templates select=".." mode="parent" /><xsl:value-of select="@Name" />.</xsl:when>
    <xsl:when test="self::Namespaces"><xsl:if test="not(name(..)='FxCopReport')"><xsl:apply-templates select=".." mode="parent"/></xsl:if></xsl:when>
    <xsl:when test="self::Target"><xsl:value-of select="@Name" />, </xsl:when>    
    <xsl:otherwise><xsl:apply-templates select=".." mode="parent" /><xsl:if test="@Name"><xsl:value-of select="translate(@Name,':', '#')"/>.</xsl:if></xsl:otherwise>
  </xsl:choose>
</xsl:template>
			
</xsl:stylesheet>


