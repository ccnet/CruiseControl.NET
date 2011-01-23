<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="html"/>
<xsl:variable name="fit.result.list" select="//testResults/result"/>
<xsl:variable name="fit.wrongpagecount" select="count($fit.result.list/counts/wrong[text() > 0])" />
<xsl:variable name="fit.ignorespagecount" select="count($fit.result.list/counts/ignores[text() > 0])" />
<xsl:variable name="fit.exceptionspagecount" select="count($fit.result.list/counts/exceptions[text() > 0])" />
<xsl:variable name="fit.correctpagecount" select="count($fit.result.list/counts[wrong/text() = 0 and exceptions/text() = 0 and ignores/text() = 0])" />
<xsl:variable name="fit.correctcount" select="//testResults/finalCounts/right"/>
<xsl:variable name="fit.failures" select="//testResults/finalCounts/wrong"/>
<xsl:variable name="fit.notrun" select="//testResults/finalCounts/ignores"/>
<xsl:variable name="fit.exceptions" select="//testResults/finalCounts/exceptions"/>
<xsl:variable name="fit.case.list" select="$fit.result.list//test-case"/>
<xsl:variable name="fit.suite.list" select="$fit.result.list//test-suite"/>
<xsl:variable name="fit.failure.list" select="$fit.case.list//failure"/>
<xsl:variable name="fit.notrun.list" select="$fit.case.list//reason"/>
<xsl:variable name="colorClass">
	<xsl:choose>
		<xsl:when test="$fit.exceptionspagecount > 0">fiterror</xsl:when>
		<xsl:when test="$fit.ignorespagecount > 0">fitignore</xsl:when>
		<xsl:when test="$fit.wrongpagecount > 0" >fail</xsl:when>
		<xsl:otherwise>pass</xsl:otherwise>
	</xsl:choose>
</xsl:variable>
<xsl:variable name="fit.tests.present" select="count(//testResults/result) > 0 or count(/cruisecontrol/build/buildresults//testsuite) > 0" />
<xsl:template match="/">
<xsl:choose>
<xsl:when test="$fit.tests.present">
<style>
	*.pass{
		background-color: #AAFFAA;
	}
	*.fail{	
		background-color: #FFAAAA;
	}
	*.fit_label{	
		color: #AAAAAA;
	}
	*.fiterror{
		background-color: #FFFFAA;	
	}
	*.fitignore{
		background-color: #CCCCCC;	
	}
	*.fitheader{	
		border: solid 1px black;
		margin: 1px;
		padding: 2px;
	}
	*.line{	
		margin: 5px;
	}
</style>
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
	
	function exportDivToNewWindow( divId )
	{
		eDiv = document.getElementById( divId );
		newWin = window.open("", "_new");
		newWin.document.write("&lt;link href='fitnesse.css' type='text/css' rel='stylesheet'&gt;");
		newWin.document.write(eDiv.innerHTML);
		newWin.document.close();
	}	
</script>
<div>
<div class="{$colorClass} fitheader">
	<strong>FitNesse Summary -- Test Pages:</strong>
	<xsl:value-of select="$fit.correctpagecount"/> right, <xsl:value-of select="$fit.wrongpagecount"/> wrong,
	<xsl:value-of select="$fit.ignorespagecount"/> ignored, <xsl:value-of select="$fit.exceptionspagecount"/> exceptions
	<strong>Assertions:</strong> 
	<xsl:value-of select="$fit.correctcount"/> right, 
	<xsl:value-of select="$fit.failures"/> wrong,
	<xsl:value-of select="$fit.notrun"/> ignored, 
	<xsl:value-of select="$fit.exceptions"/> exceptions
</div>
<xsl:for-each select="$fit.result.list">
	<xsl:variable name="test.id" select="generate-id()" />
	<xsl:variable name="test.name" select="relativePageName" />
	<xsl:variable name="colorClass">
		<xsl:choose>
			<xsl:when test="counts/exceptions > 0">fiterror</xsl:when>
			<xsl:when test="counts/ignores > 0">fitignore</xsl:when>
			<xsl:when test="counts/wrong > 0" >fail</xsl:when>
			<xsl:otherwise>pass</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<div class="line">
		<span class="{$colorClass}" style="padding:2px;" >
			<xsl:value-of select="counts/right"/> right, <xsl:value-of select="counts/wrong"/> wrong,
			<xsl:value-of select="counts/ignores"/> ignored, <xsl:value-of select="counts/exceptions"/> exceptions
		</span>
		<input type="image" src="images/arrow_minus_small.gif">
			<xsl:attribute name="id">img<xsl:value-of select="$test.id"/></xsl:attribute>
			<xsl:attribute name="onclick">javascript:toggleDiv('img<xsl:value-of select="$test.id"/>', 'fitDetails<xsl:value-of select="$test.id"/>');</xsl:attribute>
		</input>&#160;
		<input type="button">
			<xsl:attribute name="value">Export Details</xsl:attribute>
			<xsl:attribute name="onclick">javascript:exportDivToNewWindow('fitDetails<xsl:value-of select="$test.id"/>');</xsl:attribute>
		</input>&#160;
		<xsl:value-of select="$test.name"/>
	</div>
	<span style="display:none">
		<xsl:attribute name="id">fitDetails<xsl:value-of select="$test.id"/></xsl:attribute>
		<blockquote>
			<xsl:value-of select="content" disable-output-escaping="yes"/>
		</blockquote>
	</span>
</xsl:for-each>
</div>
</xsl:when>
</xsl:choose>
</xsl:template>
</xsl:stylesheet>