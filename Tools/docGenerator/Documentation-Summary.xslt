<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <xsl:if test="//docGen">
      <style type="text/css">
        .info
        {
        color: #000000;
        }
        .warning
        {
        color: #FF6600;
        }
        .error
        {
        color: #FF3333;
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
        <xsl:for-each select="message[@type='Info']">
          <xsl:if test="position()=1">
            <div class="info">
              <xsl:value-of select="@time"/>: <xsl:value-of select="."/>
            </div>
          </xsl:if>
          <xsl:if test="position()=last()">
            <div class="info">
              <xsl:value-of select="@time"/>: <xsl:value-of select="."/>
            </div>
          </xsl:if>
        </xsl:for-each>
        <xsl:if test="message[@type='Error']">
          <div class="error">
            <xsl:value-of select="count(message[@type='Error'])"/> error(s)
          </div>
        </xsl:if>
        <xsl:if test="message[@type='Warning']">
          <div class="warning">
            <xsl:value-of select="count(message[@type='Warning'])"/> warning(s)
          </div>
        </xsl:if>
        <div class="footer"></div>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
