<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:cb="urn:ccnet.config.builder"    
    xmlns:env="environment"
    xmlns:exsl="http://exslt.org/common"
    exclude-result-prefixes="msxsl cb env exsl"
>
  <xsl:output method="xml" standalone="yes" omit-xml-declaration="no" indent="yes"/>
  <xsl:strip-space elements="*"/>
  
  <!-- Identity template, copies input to output -->
  <xsl:template match="node()" priority="-3">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template name="EvalTextConstants">
    <xsl:variable name="text">
      <xsl:variable name="repl_text" select="env:eval_text_constants(.)"/>
      <xsl:choose>
        <xsl:when test="$repl_text = .">
          <xsl:value-of select="."/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates select="env:eval_text_constants($repl_text)"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <!-- Unwind the stack used for cycle detection 
         (variable names are pushed onto the stack inside the extension methods)
    -->
    <xsl:for-each select="env:unwind_eval_stack()"/>
    <xsl:value-of select="normalize-space($text)"/>    
  </xsl:template>
  
  <!-- Text nodes get copied, substituting constant values in for $(xxx) names -->
  <xsl:template match="text()">
    <xsl:call-template name="EvalTextConstants"/>
  </xsl:template>

  <!-- Attributes get copied, substituting constant values in for $(xxx) names
       in the attribute value.-->
  <xsl:template match="@*">
    <xsl:variable name="text">
      <xsl:call-template name="EvalTextConstants"/>
    </xsl:variable>
    <xsl:attribute name="{name()}">
      <xsl:value-of select="normalize-space($text)"/>
    </xsl:attribute>
  </xsl:template>

  <!-- Include another document specified by a URI, substituting constant values in 
       for $(xxx) names in the URI. -->
  <xsl:template match="cb:include">
    <!-- Expand any defined values ("$(...)") in the URI -->
    <xsl:variable name="docName" >
      <xsl:for-each select="@href">
        <xsl:call-template name="EvalTextConstants"/>
      </xsl:for-each>
    </xsl:variable>
    <!-- Recurse into the included doc -->
    <xsl:apply-templates select="env:push_include($docName)"/>
    <xsl:for-each select="env:pop_include()"/>
  </xsl:template>

  <!-- Text constant definition, value is text node -->
  <xsl:template match="cb:define[not(*) and @name]">
    <xsl:for-each select="env:define_text_constant(@name,.)"/>
  </xsl:template>

  <!-- Text constant definition(s).
    Names are attribute names, values are attribute values -->
  <xsl:template match="cb:define[count(*)=0 and @* and not(@name)]">
    <xsl:for-each select="@*">
      <xsl:for-each select="env:define_text_constant(local-name(),.)"/>
    </xsl:for-each>
  </xsl:template>

  <!-- Nodeset constant definition -->
  <xsl:template match="cb:define[@name and *]">
    <xsl:for-each select="env:define_nodeset_constant(@name,*)"/>
  </xsl:template>

  <!-- Fallthrough template for invalid definiton syntax-->
  <xsl:template match="cb:define">
    <xsl:message terminate="yes">
      Bad symbol/macro definition for define[@name='<xsl:value-of select="@name"/>']
    </xsl:message>
  </xsl:template>

  <!-- Introduction of a new scope (stack frame )-->
  <xsl:template match="cb:scope">
    <xsl:for-each select="env:push_stack()"/>
    <!-- Define text scope's attributes as text constants -->
    <xsl:for-each select="@*">
      <xsl:for-each select="env:define_text_constant(local-name(),.)"/>
    </xsl:for-each>
    <!-- Recurse -->
    <xsl:apply-templates select="node()"/>
    <xsl:for-each select="env:pop_stack()"/>
  </xsl:template>

  <!-- Eat and recurse -->
  <xsl:template match="cb:config-template">    
    <xsl:apply-templates/>    
  </xsl:template>

  <!-- Eat -->
  <xsl:template match="cb:ignore|comment()[substring(.,1,1)='#']">    
  </xsl:template>

  <!-- Constant expansion (nodeset) -->
  <xsl:template match="cb:*" priority="-2">
    <xsl:for-each select="env:push_stack()"/>
    <!-- Define macro call attributes as text constants -->
    <xsl:for-each select="@*">
      <xsl:for-each select="env:define_text_constant(local-name(),.)"/>
    </xsl:for-each>
    <xsl:apply-templates select="cb:*"/>
    <xsl:apply-templates select="env:eval_constant(local-name())"/>
    <xsl:for-each select="env:pop_stack()"/>
  </xsl:template>  

</xsl:stylesheet>