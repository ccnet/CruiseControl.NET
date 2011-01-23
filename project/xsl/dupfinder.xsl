<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html"/>

  <xsl:template match="/">
    <xsl:variable name="reports" select="//DuplicateReport" />
    <xsl:choose>
      <xsl:when test="count($reports) > 0">
        <h2>Duplicate Finder Analysis</h2>
        <hr/>
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
        -
        <xsl:value-of select="@DuplicateCount"/> duplicates found
      </div>
      <hr/>
      <xsl:for-each select="Duplicates">
        <div>
          Duplicate of length <xsl:value-of select="@Length"/> at:
        </div>
        <ul>
          <xsl:for-each select="Duplicate">
            <li>
              Line starting <xsl:value-of select="@LineNumber"/> in <xsl:value-of select="@FileName"/>
            </li>
          </xsl:for-each>
        </ul>
        <xsl:if test="code/line">
          <div style="background-color:#aaaaaa;">
            <xsl:for-each select="code/line">
              <div style="white-space:pre;">
                <xsl:value-of select="."/>
              </div>
            </xsl:for-each>
          </div>
        </xsl:if>
        <hr/>
      </xsl:for-each>
    </div>
  </xsl:template>
</xsl:stylesheet>