<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/html4/strict.dtd" >

	<xsl:output method="html"/>

	<xsl:variable name="fxcop.root" select="//FxCopReport"/>
	<xsl:variable name="fxcop.version" select="$fxcop.root/@Version" />
	<xsl:variable name="message.list" select="$fxcop.root//Messages"/>

	<xsl:template match="/">

		<xsl:variable name="modifications.list" select="/cruisecontrol/modifications" />
		<xsl:variable name="modifications.list.count" select="count($modifications.list)" />
		<xsl:variable name="message.list.count" select="count($message.list)"/>

		<xsl:if test="($message.list.count > 0) and ($modifications.list.count > 0)">
			<div id="fxcop-summary">
				<script>
				function toggleRuleVisiblity(blockId)
				{
					var block = document.getElementById(blockId);
					var plus = document.getElementById(blockId + '.plus');
					if (block.style.display=='none') {
						block.style.display='block';
						plus.innerText='- ';
					} else {
						block.style.display='none';
						plus.innerText='+ ';
					}
				}
				</script>

				<table class="section-table" cellSpacing="0" cellPadding="2" width="98%" border="0">
					<tr><td class="fxcop-summary-sectionheader" colSpan="4">FxCop <xsl:value-of select="$fxcop.version" /> Summary</td></tr>
					<xsl:apply-templates select="$modifications.list" />
				</table>
			</div>
		</xsl:if>
		
	</xsl:template>
	
	<xsl:template match="modification">
		<xsl:variable name="filename" select="filename" />
		<xsl:variable name="messages" select="$message.list[Message//SourceCode/@File=$filename]" />
		<xsl:variable name="messages.count" select="count($messages)" />
				
		<xsl:if test="$messages.count > 0">
			<tr style="cursor:pointer">
				<xsl:attribute name="onClick">toggleRuleVisiblity('<xsl:value-of select="$filename"/>');</xsl:attribute>
				<td colspan="2" class="fxcop-summary-data">
					<span><xsl:attribute name="id"><xsl:value-of select="$filename"/>.plus</xsl:attribute>+ </span>
					<xsl:value-of select="$filename" /> (<xsl:value-of select="$messages.count" />)
					<div style="display:none; border: 1px solid gray">
						<xsl:attribute name="id"><xsl:value-of select="$filename"/></xsl:attribute>
						<xsl:apply-templates select="$messages">
							<xsl:sort select="Message//@Level" />
						</xsl:apply-templates>
					</div>
				</td>
			</tr>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="Message">
			<xsl:variable name="level" select=".//@Level" />
			<xsl:variable name="certainty" select=".//@Certainty" />
			<xsl:variable name="ruleName" select="Rule/@TypeName" />
			<xsl:variable name="line" select=".//SourceCode/@Line" />
			<xsl:variable name="resolution" select=".//Resolution/Text" />

			<div class="fxcop-summary-data" style="margin-left:10px">
				<img>
					<xsl:if test="$level='CriticalError'">
						<xsl:attribute name="src">images/fxcop-critical-error.gif</xsl:attribute>
					</xsl:if>
					<xsl:if test="$level='Error'">
						<xsl:attribute name="src">images/fxcop-error.gif</xsl:attribute>
					</xsl:if>
					<xsl:if test="$level='Warning'">
						<xsl:attribute name="src">images/fxcop-warning.gif</xsl:attribute>
					</xsl:if>
					<xsl:if test="$level='CriticalWarning'">
						<xsl:attribute name="src">images/fxcop-critical-warning.gif</xsl:attribute>
					</xsl:if>
					<xsl:attribute name="alt"><xsl:value-of select="$level"/> (<xsl:value-of select="$certainty"/>% certainty)</xsl:attribute>
				</img>
				<a>
					<xsl:attribute name="href"><xsl:value-of select="$fxcop.root/Rules/Rule[@TypeName=$ruleName]/Url"/></xsl:attribute>
					<xsl:value-of select="$ruleName"/>
				</a>
				(line: <xsl:value-of select="$line"/>)
				<div style="margin-left:18px; margin-bottom:4px">
					<xsl:value-of select="$resolution"/>
				</div>
			</div>
	</xsl:template>

</xsl:stylesheet>
