<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/html4/strict.dtd" >

    <xsl:output method="html"/>

    <xsl:template match="/">
    <div id="testdata">
    	<script>
		function ToggleVisible(blockId)
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

        <table cellpadding="2" cellspacing="0" border="0" width="98%">
            <tr>
                <td class="unittests-sectionheader" colspan="3">
                    Test Results: 
                </td>
            </tr>
			<tr>
				<th>Test Fixture</th>
				<th>Status</th>
				<th>Progress</th>
			</tr>
			<xsl:apply-templates select="//test-suite[results/test-case/@success='False']">
				<xsl:sort select="@name" order="ascending" data-type="text" />
			</xsl:apply-templates>
			<xsl:apply-templates select="//test-suite[results/test-case/@executed='False']">
				<xsl:sort select="@name" order="ascending" data-type="text" />
			</xsl:apply-templates>
			<xsl:apply-templates select="//test-suite[results/test-case]" mode="success">
				<xsl:sort select="@name" order="ascending" data-type="text" />
			</xsl:apply-templates>
		</table>
	</div>
	</xsl:template>

	<xsl:template match="//test-suite" mode="success">
		<xsl:if test="count(results/test-case[@success='False']) + count(results/test-case[@executed='False']) = 0">
			<xsl:apply-templates select="."/>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="//test-suite">
		<xsl:variable name="passedtests.list" select="results/test-case[@success='True']"/>
		<xsl:variable name="ignoredtests.list" select="results/test-case[@executed='False']"/>
		<xsl:variable name="failedtests.list" select="results/test-case[@success='False']"/>
		
		<xsl:variable name="tests.count" select="count(results/test-case)"/>
		<xsl:variable name="passedtests.count" select="count($passedtests.list)"/>
		<xsl:variable name="ignoredtests.count" select="count($ignoredtests.list)"/>
		<xsl:variable name="failedtests.count" select="count($failedtests.list)"/>
		<xsl:variable name="has.failures" select="$failedtests.count > 0 or $ignoredtests.count > 0" />
		
		<xsl:variable name="storyId" select="generate-id()"/>
		
		<tr>
			<td valign="top" style="cursor:pointer">
				<xsl:if test="$has.failures">
					<xsl:attribute name="onClick">javascript:ToggleVisible('<xsl:value-of select="$storyId"/>');</xsl:attribute>
				</xsl:if>
				<nobr>
				<span><xsl:attribute name="id"><xsl:value-of select="$storyId"/>.plus</xsl:attribute>+ </span>
				<xsl:value-of select="@name" />
				</nobr>
			</td>
			<td width="100%">
				<table border="1" cellspacing="1" width="100%">
				<tr>
				<xsl:if test="$passedtests.count > 0">
					<xsl:variable name="passedtests.countpercent" select="($passedtests.count * 100) div $tests.count"/>
					<td bgcolor="green">
						<xsl:attribute name="width"><xsl:value-of select="$passedtests.countpercent"/>%</xsl:attribute>
						&#160;
					</td>
				</xsl:if>
				<xsl:if test="$ignoredtests.count > 0">
					<xsl:variable name="ignoredtests.countpercent" select="($ignoredtests.count * 100) div $tests.count"/>
					<td bgcolor="yellow">
						<xsl:attribute name="width"><xsl:value-of select="$ignoredtests.countpercent"/>%</xsl:attribute>
						&#160;
					</td>
				</xsl:if>
				<xsl:if test="$failedtests.count > 0">
					<xsl:variable name="failedtests.countpercent" select="($failedtests.count * 100) div $tests.count"/>
					<td bgcolor="red">
						<xsl:attribute name="width"><xsl:value-of select="$failedtests.countpercent"/>%</xsl:attribute>
						&#160;
					</td>
				</xsl:if>
				</tr>
				</table>
				<xsl:if test="$has.failures">
					<div style="display:none">
					<xsl:attribute name="id"><xsl:value-of select="$storyId"/></xsl:attribute>
						<table border="2" cell-padding="6" cell-spacing="0" width="100%">
							<xsl:apply-templates select="$failedtests.list"/>
							<xsl:apply-templates select="$ignoredtests.list"/>
						</table>
					</div>
				</xsl:if>
			</td>
			<td valign="top">
				(<xsl:value-of select="$passedtests.count"/>/<xsl:value-of select="$tests.count"/>)
			</td>
		</tr>
	</xsl:template>
	
	<xsl:template match="test-case[@success='False']">
		<tr>
            <xsl:if test="position() mod 2 = 0">
                <xsl:attribute name="class">unittests-oddrow</xsl:attribute>
            </xsl:if>
            <td bgcolor="red"><b>Failed</b></td>
            <td>
			<xsl:call-template name="getTestName">
				<xsl:with-param name="name" select="@name"/>
			</xsl:call-template>
			</td>
			<td>
				<pre><xsl:value-of select="failure/message"/></pre>
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="test-case[@executed='False']">
		<tr>
            <xsl:if test="position() mod 2 = 0">
                <xsl:attribute name="class">unittests-oddrow</xsl:attribute>
            </xsl:if>
            <td bgcolor="yellow"><b>Ignored</b></td>
            <td>
			<xsl:call-template name="getTestName">
				<xsl:with-param name="name" select="@name"/>
			</xsl:call-template>
			</td>
			<td>
				<pre><xsl:value-of select="reason/message"/></pre>
			</td>
		</tr>
	</xsl:template>
	
	<xsl:template name="getTestName">
        <xsl:param name="name"/>
        <xsl:choose>
			<xsl:when test="contains($name, '.')">
				<xsl:call-template name="getTestName">
					<xsl:with-param name="name" select="substring-after($name, '.')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$name"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
