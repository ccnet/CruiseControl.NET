<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
    <xsl:output method="html"/>
    <xsl:template match="/">
		<xsl:variable name="errors" select="/cruisecontrol//msbuild//error" />
		<xsl:variable name="errors.count" select="count($errors)" />
		<xsl:variable name="warnings" select="/cruisecontrol//msbuild//warning" />
		<xsl:variable name="warnings.count" select="count($warnings)" />
        <xsl:if test="$errors.count > 0">
            <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
                <tr>
                    <td class="sectionheader">
                        Errors (<xsl:value-of select="$errors.count"/>)
                    </td>
                </tr>
                <tr>
					<td class="section-error">
						<xsl:apply-templates select="$errors"/>
					</td>
				</tr>
            </table>
        </xsl:if>
        <xsl:if test="$warnings.count > 0">
			<table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
				<tr>
					<td class="sectionheader">
						Warnings (<xsl:value-of select="$warnings.count"/>)
	                </td>
	            </tr>
	            <tr>
					<td class="section-warn">
						<xsl:apply-templates select="$warnings"/>
					</td>
				</tr>
			</table>
	    </xsl:if>
    </xsl:template>
	
	<xsl:template match="error">
		<div style="color:orangered">
			<xsl:if test="@file != ''" >
				<xsl:value-of select="@file"/> (<xsl:value-of select="@line"/>,<xsl:value-of select="@column"/>):
			</xsl:if>
			error<xsl:value-of select="@code"/>: <xsl:value-of select="text()" />
		</div>
	</xsl:template>

	<xsl:template match="warning">
		<div style="color:blue">
			<xsl:if test="@file != ''" >
				<xsl:value-of select="@file"/> (<xsl:value-of select="@line"/>,<xsl:value-of select="@column"/>): 
			</xsl:if>
			warning <xsl:value-of select="@code"/>: <xsl:value-of select="text()" />
		</div>
	</xsl:template>
</xsl:stylesheet>
