<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html"/>

  <xsl:param name="highCoverage"
             select="65" />
  <xsl:param name="mediumCoverage"
             select="40" />

  <xsl:template match="/">
      <style>

.mhc
{  #method  high coverage
   text-align: left;
   background-color:#86ed60;
}

.mmc
{  #method medium coverage
   text-align: left;
   background-color:#ffff99;
}

.mbc
{  #method  bad coverage
   text-align: left;
   background-color:#eb4848;
}


.chc
{  #class high coverage
   text-align: left; 
   border-right: 1px solid #649cc0; 
   border-bottom: 1px solid #649cc0; 
   width: 100; 
   font-weight: bold;
   background-color:#86ed60;
   margin-right:16px;
}

.cmc
{  #class high coverage
   text-align: left; 
   border-right: 1px solid #649cc0; 
   border-bottom: 1px solid #649cc0; 
   width: 100; 
   font-weight: bold;
   background-color:#ffff99;
   margin-right:16px;
}

.cbc
{  #class high coverage
   text-align: left; 
   border-right: 1px solid #649cc0; 
   border-bottom: 1px solid #649cc0; 
   width: 100; 
   font-weight: bold;
   background-color:#eb4848;
   margin-right:16px;
}
 </style>


    <xsl:variable name="root"
                  select="//CoverageDSPriv" />

    <xsl:if test="$root">
        <h3>Code Coverage Summary (MsTest)</h3>

      <table class="section-table"
             cellpadding="2"
             cellspacing="0"
             border="1">
        <tr><td class="mhc">highCoverage &gt;= <xsl:value-of select="$highCoverage" /> %</td></tr>
        <tr><td class="mmc">mediumCoverage &gt;= <xsl:value-of select="$mediumCoverage" /> %</td></tr>
        <tr><td class="mbc">bad &lt; <xsl:value-of select="$mediumCoverage" /> % </td></tr>
      </table>

      <br/>

      <table cellpadding="4"
             cellspacing="0">
        <tr>
          <th style="border: 1px solid #649cc0; background-color: #a9d9f7; text-align: left;">Assembly</th>
          <th style="border-top: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-right: 1px solid #649cc0; background-color: #a9d9f7;">Blocks Covered</th>
          <th style="border-top: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-right: 1px solid #649cc0; background-color: #a9d9f7;">Blocks Not Covered</th>
          <th style="border-top: 1px solid #649cc0; border-bottom: 1px solid #649cc0; border-right: 1px solid #649cc0; background-color: #a9d9f7;">Coverage</th>
        </tr>

        <xsl:for-each select="$root/Module">
          <tr>
            <td style="border-left: 1px solid #649cc0; border-right: 1px solid #649cc0; border-bottom: 1px solid #649cc0; padding-right: 10px">
              <xsl:value-of select="ModuleName" />
            </td>
            <td style="text-align: right; border-right: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
              <xsl:value-of select="BlocksCovered" />
            </td>
            <td style="text-align: right; border-right: 1px solid #649cc0; border-bottom: 1px solid #649cc0;">
              <xsl:value-of select="BlocksNotCovered" />
            </td>
            <td>
              <xsl:variable name="pctCovered"
                            select="(BlocksCovered div (BlocksCovered + BlocksNotCovered)) * 100" />
              <xsl:attribute name="style">
                text-align: right; border-right: 1px solid #649cc0; border-bottom: 1px solid #649cc0; width: 100; font-weight: bold;
                <xsl:choose>
                  <xsl:when test="number($pctCovered &gt;= $highCoverage)">background-color:#86ed60;</xsl:when>
                  <xsl:when test="number($pctCovered &gt;= $mediumCoverage)">background-color:#ffff99;</xsl:when>
                  <xsl:otherwise>background-color:#eb4848;</xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>

              <xsl:if test="$pctCovered > 0">
                <xsl:value-of select="format-number($pctCovered, '###.##')" />%
              </xsl:if>
              <xsl:if test="$pctCovered = 0">
                0.00%
              </xsl:if>
            </td>
          </tr>
        </xsl:for-each>
      </table>
      <xsl:apply-templates select="//CoverageDSPriv/Module" />
    </xsl:if>

  </xsl:template>

  <xsl:template match="Module">
    <h3>
      <xsl:value-of select="ModuleName"/>
    </h3>
    <ul>
      <xsl:apply-templates select="./NamespaceTable" />
    </ul>
  </xsl:template>

  <xsl:template match="NamespaceTable">
    <xsl:apply-templates select="./Class" />
  </xsl:template>

  <xsl:template match="Class">
    <li>
      <xsl:variable name="pctCovered"
                    select="(BlocksCovered div (BlocksCovered + BlocksNotCovered)) * 100" />
           <span><xsl:attribute name="class"><xsl:choose>
                <xsl:when test="number($pctCovered &gt;= $highCoverage)">chc</xsl:when>
                <xsl:when test="number($pctCovered &gt;= $mediumCoverage)">cmc</xsl:when>
                <xsl:otherwise>cbc</xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
            <xsl:value-of select="ClassName"/>
          </span>
            <xsl:if test="$pctCovered > 0"><xsl:value-of select="format-number($pctCovered, '###.##')" />%</xsl:if>
            <xsl:if test="$pctCovered = 0">0.00%</xsl:if>

       <table>
          <xsl:apply-templates select="./Method" />
       </table>
    </li>

  </xsl:template>

  <xsl:template match="Method">
    <xsl:variable name="pctCovered"
                  select="(BlocksCovered div (BlocksCovered + BlocksNotCovered)) * 100" />

      <tr><td width="50">
          <xsl:if test="$pctCovered > 0"><xsl:value-of select="format-number($pctCovered, '###.##')" />%</xsl:if>
          <xsl:if test="$pctCovered = 0">0.00%</xsl:if></td><td>
          <xsl:attribute name="class"><xsl:choose>
              <xsl:when test="number($pctCovered &gt;= $highCoverage)">mhc</xsl:when>
              <xsl:when test="number($pctCovered &gt;= $mediumCoverage)">mmc</xsl:when>
              <xsl:otherwise>mbc</xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
          <xsl:value-of select="MethodName"/></td></tr>     
  </xsl:template>


</xsl:stylesheet>