<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html" />

	<xsl:variable name="gendarme.root" select="//gendarme-output"/>

	<xsl:template match="/">
		<xsl:if test="count($gendarme.root/files/file) + count($gendarme.root/results/rule) > 0">
			<table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
				<tr>
					<td class="sectionheader" colspan="2">
						Gendarme Report: found <xsl:value-of select="count($gendarme.root//rule/target/defect)" /> potential defects using <xsl:value-of select="count($gendarme.root/rules/rule)" /> rules.
					</td>
				</tr>
				<tr>
					<td colspan="2"> </td>
				</tr>
				<xsl:apply-templates select="$gendarme.root/files/file" />
			</table>
		</xsl:if>
	</xsl:template>

	<xsl:template match="file">
		<xsl:variable name="name" select="./@Name"/>
		<xsl:variable name="defects" select="count(//target[@Assembly = $name])"/>

		<tr>
			<td colspan="2">
				<xsl:value-of select="concat ($name, ': ', $defects, ' defects')" />
			</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>
