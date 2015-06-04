<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:output method="html"/>
  <xsl:template match="/">
    <xsl:apply-templates select="/cruisecontrol/build/CoverageReport" />
  </xsl:template>
  <xsl:template match="/cruisecontrol/build/CoverageReport">
    <link rel="stylesheet" type="text/css" href="/ccnet/ReportGenerator.css" />
	<script src="/ccnet/javascript/ReportGenerator.js"></script>
    <h1 class="sectionheader">
      Code Coverage Report
    </h1>
    <table class="overview">
      <colgroup>
        <col width="130" />
        <col />
      </colgroup>
      <tr>
        <td class="sectionheader">
          Generated on:
        </td>
        <td>
          <xsl:value-of select="Summary/Generatedon"/>
        </td>
      </tr>
      <tr>
        <td class="sectionheader">
          Parser:
        </td>
        <td>
          <xsl:value-of select="Summary/Parser"/>
        </td>
      </tr>
      <tr>
        <td class="sectionheader">
          Assemblies:
        </td>
        <td>
          <xsl:value-of select="Summary/Assemblies"/>
        </td>
      </tr>
      <tr>
        <td class="sectionheader">
          Files:
        </td>
        <td>
          <xsl:value-of select="Summary/Files"/>
        </td>
      </tr>
      <tr>
        <td class="sectionheader">
          Coverage:
        </td>
        <td>
          <xsl:value-of select="format-number(Summary/Coveredlines div Summary/Coverablelines, '0.0%')"/>
        </td>
      </tr>
      <tr>
        <td class="sectionheader">
          Covered lines:
        </td>
        <td>
          <xsl:value-of select="Summary/Coveredlines"/>
        </td>
      </tr>
      <tr>
        <td class="sectionheader">
          Coverable lines:
        </td>
        <td>
          <xsl:value-of select="Summary/Coverablelines"/>
        </td>
      </tr>
      <tr>
        <td class="sectionheader">
          Total lines:
        </td>
        <td>
          <xsl:value-of select="Summary/Totallines"/>
        </td>
      </tr>
    </table>
    <h1 class="sectionheader">
      Assemblies
    </h1>
    <p class="toggleClasses">
      <a id="collapseAllClasses" style="text-decoration: none;color:red;font-size:10px" href="#">Collapse all classes</a> | <a id="expandAllClasses" style="text-decoration: none;color:red;font-size:10px" href="#">Expand all classes</a>
    </p>
    <table class="overview">
      <colgroup>
        <col />
        <col width="60" />
        <col width="105" />
      </colgroup>
      <xsl:for-each select="./Assemblies/Assembly">
        <xsl:sort select="@name"/>
        <xsl:variable name="ModulenameVariable" select="@name"/>
        <tr class="expanded">
          <th class="expanded">
            <a href="#" class="toggleClassesInAssembly" style="text-decoration: none;color:red;font-size:10px" title="Collapse/Expand classes"/>
            <xsl:value-of select="@name"/>
            <a href="#" class="toggleDetails" style="text-decoration: none;color:red;font-size:10px" title="Show details of assembly">Details</a>
            <div class="detailspopup">
              <table class="overview">
                <colgroup>
                  <col width="130" />
                  <col />
                </colgroup>
                <tr>
                  <td class="sectionheader">
                    Assembly:
                  </td>
                  <td>
                    <xsl:value-of select="@name"/>
                  </td>
                </tr>
                <tr>
                  <td class="sectionheader">
                    Classes:
                  </td>
                  <td>
                    <xsl:value-of select="@classes"/>
                  </td>
                </tr>
                <tr>
                  <td class="sectionheader">
                    Coverage:
                  </td>
                  <td>
                    <xsl:value-of select="@coverage"/>%
                  </td>
                </tr>
                <tr>
                  <td class="sectionheader">
                    Covered lines:
                  </td>
                  <td>
                    <xsl:value-of select="@coveredlines"/>
                  </td>
                </tr>
                <tr>
                  <td class="sectionheader">
                    Coverable lines:
                  </td>
                  <td>
                    <xsl:value-of select="@coverablelines"/>
                  </td>
                </tr>
                <tr>
                  <td class="sectionheader">
                    Total lines:
                  </td>
                  <td>
                    <xsl:value-of select="@totallines"/>
                  </td>
                </tr>
              </table>
            </div>
          </th>		  
          <th title="LineCoverage">
            <xsl:value-of select="@coverage"/>%
          </th>
          <td>
            <xsl:variable name="width" select="@coverage"/>
            <table class="coverage">
              <tr>
                <td class="green" style="width: {$width}px;">
                  &#160;
                </td>
                <td class="red" style="width: {100-$width}px;">
                  &#160;
                </td>
              </tr>
            </table>
          </td>
        </tr>
        <xsl:for-each select="Class">
          <tr class="classrow">
            <td>
              <xsl:value-of select="@name"/>
              <a href="#" class="toggleDetails" style="text-decoration: none;color:red;font-size:10px" title="Show details of class">Details</a>
              <div class="detailspopup">
                <table class="overview">
                  <colgroup>
                    <col width="130" />
                    <col />
                  </colgroup>
                  <tr>
                    <td class="sectionheader">
                      Class:
                    </td>
                    <td>
                      <xsl:value-of select="@name"/>
                    </td>
                  </tr>
                  <tr>
                    <td class="sectionheader">
                      Coverage:
                    </td>
                    <td>
                      <xsl:value-of select="@coverage"/>%
                    </td>
                  </tr>
                  <tr>
                    <td class="sectionheader">
                      Covered lines:
                    </td>
                    <td>
                      <xsl:value-of select="@coveredlines"/>
                    </td>
                  </tr>
                  <tr>
                    <td class="sectionheader">
                      Coverable lines:
                    </td>
                    <td>
                      <xsl:value-of select="@coverablelines"/>
                    </td>
                  </tr>
                  <tr>
                    <td class="sectionheader">
                      Total lines:
                    </td>
                    <td>
                      <xsl:value-of select="@totallines"/>
                    </td>
                  </tr>
                </table>
              </div>
            </td>
            <td title="LineCoverage">
              <xsl:value-of select="@coverage"/>%
            </td>
            <td>
              <table class="coverage">
                <tr width="100px">
                  <xsl:variable name="width" select="round(@coverage)"/>
                  <xsl:choose>
                    <xsl:when test="$width &lt;= 0">
                      <td class="red" style="width: 103px;">
                        &#160;
                      </td>
                    </xsl:when>
                    <xsl:when test="$width &gt;= 100">
                      <td class="green" style="width: 103px;">
                        &#160;
                      </td>
                    </xsl:when>
                    <xsl:otherwise>
                      <td class="green" style="width: {$width}px;">
                        &#160;
                      </td>
                      <td class="red" style="width: {100-$width}px;">
                        &#160;
                      </td>
                    </xsl:otherwise>
                  </xsl:choose>
                </tr>
              </table>
            </td>
          </tr>
        </xsl:for-each>
      </xsl:for-each>
    </table>
  </xsl:template>
</xsl:stylesheet>
