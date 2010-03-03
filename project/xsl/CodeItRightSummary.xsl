<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="html" indent="no"/>

  <xsl:key name="projects" match="Violation" use="ProjectName"/>

  <xsl:template match="/">
    <style>
      table.header tr th
      {
      text-align: left;
      }
      table.details
      {
      width: 100%;
      border-collapse:collapse;
      }
      table.details tr th
      {
      background-color:#2E8A2E;
      color:#ffffff;
      }
      tr.violation td
      {
      color: #2E8A2E;
      }
      td.number
      {
      width:15%;
      text-align:right;
      }
    </style>
    <xsl:for-each select="/cruisecontrol/build/CodeItRightReport">
      <h3>CodeIt.Right Analysis Summary</h3>
      <xsl:apply-templates select="Violations">
        <xsl:sort order="ascending" select="@date"/>
      </xsl:apply-templates>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="Violations">
    <table class="header">
      <tr>
        <th>
          Solution:
        </th>
        <td>
          <xsl:value-of select="@solution"/>
        </td>
      </tr>
    </table>
    <table class="details">
      <tr>
        <th>Project</th>
        <th>Errors</th>
        <th>Warnings</th>
        <th>Other</th>
        <th>Total</th>
      </tr>
      <xsl:for-each select="Violation[generate-id() = generate-id(key('projects', ProjectName)[1])]">
        <xsl:sort select="ProjectName"/>
        <xsl:variable name="total">
          <xsl:value-of select="count(key('projects', ProjectName))" />
        </xsl:variable>
        <xsl:variable name="errors">
          <xsl:value-of select="count(key('projects', ProjectName)[Severity='Error'])+count(key('projects', ProjectName)[Severity='CriticalError'])" />
        </xsl:variable>
        <xsl:variable name="warnings">
          <xsl:value-of select="count(key('projects', ProjectName)[Severity='Warning'])+count(key('projects', ProjectName)[Severity='CriticalWarning'])" />
        </xsl:variable>
        <tr>
          <td>
            <xsl:value-of select="ProjectName"/>
          </td>
          <td class="number">
            <xsl:value-of select="$errors"/>
          </td>
          <td class="number">
            <xsl:value-of select="$warnings"/>
          </td>
          <td class="number">
            <xsl:value-of select="$total - $errors - $warnings"/>
          </td>
          <td class="number">
            <xsl:value-of select="$total"/>
          </td>
        </tr>
      </xsl:for-each>
      </table>
  </xsl:template>
</xsl:stylesheet>
