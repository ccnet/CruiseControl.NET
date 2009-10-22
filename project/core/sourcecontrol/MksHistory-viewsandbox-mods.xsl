<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <modifications>
      <xsl:apply-templates select="//WorkItem"/>
    </modifications>
  </xsl:template>

  <xsl:template match="WorkItem">
    <xsl:if test="@modelType!='si.FormerMember'">
      <xsl:if test="Field[@name='revsyncdelta']/Item/Field[@name='isDelta' and Value='true']">
        <modification>
          <project>
            <xsl:value-of select="@context"/>
          </project>
          <name>
            <xsl:value-of select="Field[@name='canonicalMember']/Value"/>
          </name>
          <fullname>
            <xsl:value-of select="Field[@name='name']/Value"/>
          </fullname>
          <workingrev>
            <xsl:value-of select="Field[@name='revsyncdelta']/Item[@id='delta']/Field[@name='workingRev']/Item/@id"/>
          </workingrev>
          <memberrev>
            <xsl:value-of select="Field[@name='revsyncdelta']/Item[@id='delta']/Field[@name='memberRev']/Item/@id"/>
          </memberrev>
          <modificationtype>
            <xsl:choose>
              <xsl:when test="Field[@name='revsyncdelta']/Item[@id='delta']/Field[@name='isWorkingRevUnknown']/Value = 'true'">
                <xsl:text>add</xsl:text>
              </xsl:when>
              <xsl:otherwise>
                <xsl:text>change</xsl:text>
              </xsl:otherwise>
            </xsl:choose>
          </modificationtype>
        </modification>
      </xsl:if>
    </xsl:if>
    <xsl:if test="@modelType='si.FormerMember'">
      <modification>
        <project>Deleted Item: Project Context Not Available.</project>
        <name>
          <xsl:value-of select="Field[@name='canonicalMember']/Value"/>
        </name>
        <fullname>
          <xsl:value-of select="Field[@name='name']/Value"/>
        </fullname>
        <workingrev>NA</workingrev>
        <memberrev>NA</memberrev>
        <modificationtype>
          <xsl:if test="Field[@name='type']/Value='dropped'">
            <xsl:text>deleted</xsl:text>
          </xsl:if>
        </modificationtype>
      </modification>
    </xsl:if>
    <xsl:if test="@modelType='si.FormerSandbox'">
      <modification>
        <project>
          <xsl:value-of select="@context"/>
        </project>
        <name>
          <xsl:value-of select="Field[@name='canonicalMember']/Value"/>
        </name>
        <fullname>
          <xsl:value-of select="Field[@name='name']/Value"/>
        </fullname>
        <workingrev>NA</workingrev>
        <memberrev>NA</memberrev>
        <modificationtype>deleted</modificationtype>
      </modification>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
