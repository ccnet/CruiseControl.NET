<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">	
	<xsl:output method="html"/>

	<xsl:template match="/">
		<div name="StyleCopViolations">
		<style type="text/css">
			div[name=StyleCopViolations] div[name=header]{ font-weight: bold; padding: 10px 0 10px 0;}
			li { line-height: 1.5em; padding-bottom: 5px;}
			li span[name=info] {color: #AA0000; font-weight: bold;}
			*[name=missingInfo] { font-weight: bold;}
			div[name=StyleCopViolations] .sectionheader { padding: 2px;}
		</style>
		<!-- Use a table and not a div, as (at least) Outlook does not render the width correctly -->
		<table class="sectionheader" width="98%"><tr><td>StyleCop Report</td></tr></table>
		<xsl:choose>
			<xsl:when test="count(//StyleCopViolations) = 0">
				<div name="header">No StyleCop information in build logs.</div>
			</xsl:when>
		</xsl:choose>
		<xsl:apply-templates select="//StyleCopViolations"/>
		</div>
	</xsl:template>
	
	<xsl:template match="//StyleCopViolations">
            <div name="header">
				<xsl:choose>
					<xsl:when test="count(./Violation) > 0">
						<xsl:attribute name="class">error</xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name="class">success</xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
				<span name="label">Number of Violations: </span><xsl:value-of select="count(./Violation)" />
			</div>
			<ol name="errorList">
				<xsl:for-each select="Violation">
					<xsl:sort select="@Source" />
					<xsl:sort select="@LineNumber" />
					<li>
						<span name="info"><xsl:value-of select="concat(@Source, ':', @LineNumber, ' (', @RuleId, ' - ', @Rule, ')')" /></span><br/>
						<span name="errorDetails"><xsl:value-of select="." /></span>
					</li>
				</xsl:for-each>
			</ol>
    </xsl:template>
	
</xsl:stylesheet>
