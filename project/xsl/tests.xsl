<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/html4/strict.dtd">
	
	<xsl:output method="html"/>
	
	<xsl:template match="/">
		<div id="master">
			<xsl:apply-templates select="cruisecontrol/test-results[.//test-suite]" />
		</div>
	</xsl:template>
	
	<xsl:template match="test-results">

		<xsl:variable name="test.suite.id" select="generate-id()" />
		<xsl:variable name="test.suite.name" select="./@name"/>
		<xsl:variable name="failure.count" select="count(.//results/test-case[@success='False'])" />
		<xsl:variable name="ignored.count" select="count(.//results/test-case[@executed='False'])" />
				
		<div>
			<script>
				function toggleDiv( imgId, divId )
				{
					eDiv = document.getElementById( divId );
					eImg = document.getElementById( imgId );
					
					if ( eDiv.style.display == "none" )
					{
						eDiv.style.display = "block";
						eImg.src = "images/arrow_minus_small.gif";
					}
					else
					{
						eDiv.style.display = "none";
						eImg.src = "images/arrow_plus_small.gif";
					}
				}
			</script>
			<table cellpadding="2" cellspacing="0" border="0" width="98%">
				<tr>
					<td class="yellow-sectionheader" colspan="3" valign="top">
						<xsl:choose>
							<xsl:when test="$failure.count > 0">
								<img src="images/fxcop-critical-error.gif"/>
							</xsl:when>
							<xsl:when test="$ignored.count > 0">
								<img src="images/fxcop-error.gif"/>
							</xsl:when>
							<xsl:otherwise>
								<img src="images/check.jpg" width="16" height="16"/>
							</xsl:otherwise>
						</xsl:choose>
				
						<input type="image" src="images/arrow_minus_small.gif">
							<xsl:attribute name="id">img<xsl:value-of select="$test.suite.id"/></xsl:attribute>
							<xsl:attribute name="onclick">javascript:toggleDiv('img<xsl:value-of select="$test.suite.id"/>', 'divDetails<xsl:value-of select="$test.suite.id"/>');</xsl:attribute>
						</input>&#160;<xsl:value-of select="$test.suite.name"/>
             </td>
				</tr>
			</table>
			<div>
				<xsl:attribute name="id">divDetails<xsl:value-of select="$test.suite.id"/></xsl:attribute>
				<blockquote>
					<table>
						<tr>
							<th>Test Fixture</th>
							<th>Status</th>
							<th>Progress</th>
						</tr>
						<xsl:apply-templates select=".//test-suite[@success='False'][results/test-case]">
							<xsl:sort select="@name" order="ascending" data-type="text"/>
						</xsl:apply-templates>
						<xsl:apply-templates select=".//test-suite[results/test-case/@executed='False']">
							<xsl:sort select="@name" order="ascending" data-type="text"/>
						</xsl:apply-templates>
						<xsl:apply-templates select=".//test-suite[@success='True'][results/test-case/@executed='True']" mode="success">
							<xsl:sort select="@name" order="ascending" data-type="text"/>
						</xsl:apply-templates>
					</table>
				</blockquote>
			</div>
		</div>
	</xsl:template>

	<xsl:template match="test-suite" mode="success">
		<xsl:if test="count(results/test-case[@success='False']) + count(results/test-case[@executed='False']) = 0">
			<xsl:apply-templates select="."/>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="test-suite">
		<xsl:variable name="passedtests.list" select="results/test-case[@success='True']"/>
		<xsl:variable name="ignoredtests.list" select="results/test-case[@executed='False']"/>
		<xsl:variable name="failedtests.list" select="results/test-case[@success='False']"/>
		<xsl:variable name="tests.count" select="count(results/test-case)"/>
		<xsl:variable name="passedtests.count" select="count($passedtests.list)"/>
		<xsl:variable name="ignoredtests.count" select="count($ignoredtests.list)"/>
		<xsl:variable name="failedtests.count" select="count($failedtests.list)"/>

		<xsl:variable name="testName" select="@name"/>
		<tr>
			<td valign="top" width="30%">
				<input type="image" src="images/arrow_plus_small.gif">
					<xsl:attribute name="id">imgTestCase_<xsl:value-of select="@name"/></xsl:attribute>
					<xsl:attribute name="onClick">javascript:toggleDiv('imgTestCase_<xsl:value-of select="$testName"/>', 'divTest_<xsl:value-of select="$testName"/>');</xsl:attribute>
				</input>
				<a>
					<xsl:attribute name="name"><xsl:value-of select="@name"/></xsl:attribute>
					<xsl:value-of select="@name"/>
				</a>
			</td>
			<td width="70%">
				<table border="0" cellspacing="1" width="100%">
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
				<!--xsl:if test="$failedtests.count > 0 or $ignoredtests.count > 0"-->
				<div style="display:none">
					<xsl:attribute name="id">divTest_<xsl:value-of select="$testName"/></xsl:attribute>
					<table border="0" cell-padding="6" cell-spacing="0" width="100%">
						<xsl:apply-templates select="$failedtests.list"/>
						<xsl:apply-templates select="$ignoredtests.list"/>
						<xsl:apply-templates select="$passedtests.list"/>
					</table>
				</div>
				<!--/xsl:if-->
			</td>
			<td valign="top">
				(<xsl:value-of select="$passedtests.count"/>/<xsl:value-of select="$tests.count"/>)
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="test-case[@success='True']">
		<tr>
			<xsl:if test="position() mod 2 = 0">
				<xsl:attribute name="class">section-oddrow</xsl:attribute>
			</xsl:if>
			<td>
				<img src="images\check.jpg" width="16" height="16"/>
			</td>
			<td>
				<xsl:call-template name="getTestName">
					<xsl:with-param name="name" select="@name"/>
				</xsl:call-template>
			</td>
			<td>
				<xsl:value-of select="substring-after(failure/message, '-')"/>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="test-case[@success='False']">
		<tr>
			<xsl:if test="position() mod 2 = 0">
				<xsl:attribute name="class">section-oddrow</xsl:attribute>
			</xsl:if>
			<td>
				<img src="images\fxcop-critical-error.gif"/>
			</td>
			<td>
				<xsl:call-template name="getTestName">
					<xsl:with-param name="name" select="@name"/>
				</xsl:call-template>
			</td>
			<td bgcolor="gainsboro">
				<xsl:value-of select="substring-after(failure/message, '-')"/><br/>
				<xsl:value-of select="failure/message"/>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="test-case[@executed='False']">
		<tr>
			<xsl:if test="position() mod 2 = 0">
				<xsl:attribute name="class">section-oddrow</xsl:attribute>
			</xsl:if>
			<td>
				<img src="images\fxcop-error.gif"/>
			</td>
			<td>
				<xsl:call-template name="getTestName">
					<xsl:with-param name="name" select="@name"/>
				</xsl:call-template>
			</td>
			<td>
				<xsl:value-of select="substring-after(reason/message, '-')"/>
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
