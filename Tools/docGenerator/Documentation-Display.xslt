<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <style type="text/css">
      .doc-Info
      {
      color: #000000;
      }
      .doc-Debug
      {
      color: #666666;
      font-style: italic;
      }
      .doc-Warning
      {
      color: #FF6600;
      }
      .doc-Error
      {
      color: #FF3333;
      font-weight: bold;
      }
      .header
      {
      border-bottom: solid 1px #888888;
      }
      .footer
      {
      margin-bottom: 1em;
      }
    </style>
    <h3>Documentation Generation</h3>
    <xsl:for-each select="//docGen">
      <div class="header">
        <b>Type: </b>
        <xsl:value-of select="@command"/>
      </div>
      <xsl:for-each select="message">
        <div>
          <xsl:attribute name="class">doc-<xsl:value-of select="@type"/></xsl:attribute>
          <xsl:value-of select="@time"/>: <xsl:value-of select="."/>
        </div>
      </xsl:for-each>
      <div class="footer"></div>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>
