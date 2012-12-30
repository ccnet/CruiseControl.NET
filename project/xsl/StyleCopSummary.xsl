<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">	
	<xsl:output method="html"/>

	<xsl:template match="/">
		<xsl:if test="count(//StyleCopViolations) &gt; 0">
      <div name="StyleCopViolations">
        <style type="text/css">
          div[name=StyleCopViolations] div[name=header]{ font-weight: bold; padding: 10px 0 10px 0;}
          li { line-height: 1.5em; padding-bottom: 5px;}
          li span[name=info] {color: #AA0000; font-weight: bold;}
          *[name=missingInfo] { font-weight: bold;}
          div[name=StyleCopViolations] .sectionheader { padding: 2px;}
        </style>

        <!-- Use a table and not a div, as (at least) Outlook does not render the width correctly -->
        <table class="sectionheader" width="98%">
          <tr>
            <td>StyleCop Report</td>
          </tr>
        </table>
        <xsl:apply-templates select="//StyleCopViolations"/>
      </div>
    </xsl:if>
	</xsl:template>

	<xsl:template match="//StyleCopViolations">
    <xsl:if test="count(./Violation) &gt; 0">
      <div name="header">
				<span name="label">Number of Violations: </span>
        <xsl:value-of select="count(./Violation)" />
      </div>
		</xsl:if>
  </xsl:template>
</xsl:stylesheet>
