<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html"/>

  <xsl:template match="/">
    <xsl:variable name="reports" select="//DuplicateReport" />
    <xsl:choose>
      <xsl:when test="count($reports) > 0">
        <xsl:apply-templates select="$reports" />
      </xsl:when>
      <xsl:otherwise>
        <h2>Log does not contain any XML output from DupFinder.</h2>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="DuplicateReport">
    <div>
      <xsl:for-each select="Message">
        <div>
          <xsl:value-of select="."/>
        </div>
      </xsl:for-each>
      <div>
        <xsl:value-of select="@FileCount"/> files processed
      </div>
      <div>
        <xsl:value-of select="@DuplicateCount"/> duplicates found
      </div>
      <xsl:for-each select="Duplicates">
        <div>
          Duplicate of length <xsl:value-of select="@Length"/> at:
        </div>
        <xsl:for-each select="Duplicate">
          <div>
            Line starting <xsl:value-of select="@LineNumber"/> in <xsl:value-of select="@FileName"/>
          </div>
        </xsl:for-each>
      </xsl:for-each>
    </div>
  </xsl:template>
</xsl:stylesheet>