<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                version="1.0">
  <xsl:output method="html"/>

  <xsl:template match="/">
      <style>
td
{
text-align: left;
}

.tst_dur
{
text-align: right;
}

.tst_nok
{   
	text-align: center; 
	font-weight: bold; 
	background-color: fireBrick; 
	color: white;
}

.tst_ok
{   
    text-align: center; 
    font-weight: bold; 
    background-color: forestGreen; 
    color: white;
}

.tst_inc
{   
	text-align: center; 
	font-weight: bold; 
	background-color: yellow; 
	color: black;
}

.tst_unk
{   
	text-align: center; 
	font-weight: bold; 
	background-color: lightblue; 
	color: black;
}
      </style>

    <xsl:apply-templates select="/cruisecontrol/build/*[local-name()='TestRun']" />
  </xsl:template>

  <xsl:template match="/cruisecontrol/build/*[local-name()='TestRun']">
    <h1>
      Test Run: <xsl:value-of select="@name" />
    </h1>

    <h2>Summary</h2>
    <p>
      Designated outcome :
      <xsl:choose>
        <xsl:when test="*[local-name()='ResultSummary']/@outcome ='Passed'">
          <span style="color: forestGreen; font-weight: bold;">
            <xsl:value-of select="*[local-name()='ResultSummary']/@outcome" />
          </span>
        </xsl:when>
        <xsl:otherwise>
          <span style="color: Red; font-weight: bold;">
            <xsl:value-of select="*[local-name()='ResultSummary']/@outcome" />
          </span>
        </xsl:otherwise>
      </xsl:choose>
    </p>

    <table border="1"
           cellSpacing="0"
           cellPadding="5" >
      <thead style="text-align: center;">
        <td>Number of Tests</td>
        <td style="background-color: forestGreen; color:white">Passed</td>
        <td style="background-color: fireBrick; color: white;">Failed</td>
        <td style="background-color: yellow; color:black;">Inconclusive</td>
        <td style="background-color: red; color: black;">Aborted</td>
        <td style="background-color: darkblue; color: white;">Timeout</td>
        <td style="background-color: darkorange; color: white;">Unknown</td>
        <td style="background-color: DarkSlateGray; color: white;">Started at</td>
        <td style="background-color: DimGrey; color: white;">Stopped at</td>
      </thead>
      <tr style="text-align: center;">
        <td>
          <xsl:value-of select="*[local-name()='ResultSummary']//*[local-name()='Counters']/@total" />
        </td>
        <td>
          <xsl:value-of select="*[local-name()='ResultSummary']//*[local-name()='Counters']/@passed" />
        </td>
        <td>
          <xsl:value-of select="*[local-name()='ResultSummary']//*[local-name()='Counters']/@failed" />
        </td>
        <td>
          <xsl:value-of select="*[local-name()='ResultSummary']//*[local-name()='Counters']/@inconclusive" />
        </td>
        <td>
          <xsl:value-of select="*[local-name()='ResultSummary']//*[local-name()='Counters']/@aborted" />
        </td>
        <td>
          <xsl:value-of select="*[local-name()='ResultSummary']//*[local-name()='Counters']/@timeout" />
        </td>
        <td>
          <xsl:value-of select="count(*[local-name()='Results']/*[local-name()='UnitTestResult'][not(@outcome)] )" />
        </td>		
        <td>
          <xsl:call-template name="formatDateTime">
            <xsl:with-param name="dateTime"
                            select="*[local-name()='Times']/@creation" />
          </xsl:call-template>
        </td>
        <td>
          <xsl:call-template name="formatDateTime">
            <xsl:with-param name="dateTime"
                            select="*[local-name()='Times']/@finish" />
          </xsl:call-template>
        </td>
      </tr>
    </table>

    <xsl:variable name="runinfos"
                  select="*[local-name()='ResultSummary']/*[local-name()='RunInfos']/*[local-name()='RunInfo']" />
    <xsl:if test="count($runinfos) > 0">
      <h3>Errors and Warnings</h3>
      <table width="100%"
             border="1"
             cellSpacing="0"
             style="font-size:small;">
        <xsl:apply-templates select="$runinfos" />
      </table>
    </xsl:if>

  <!-- bad ones  *[local-name()='ResultSummary']/@outcome ='Passed'      -->

	<xsl:variable name="count_badones"
                  select="count(*[local-name()='Results']/*[local-name()='UnitTestResult'][@outcome !=  'Passed'])" />
	<xsl:variable name="count_badones2"
                  select="count(*[local-name()='Results']/*[local-name()='UnitTestResult'][not(@outcome)] )  " />     
   <xsl:if test="$count_badones + $count_badones2 > 0">
  
       <h2>Failed tests </h2>
	    <table border="1"
	           cellPadding="2"
	           cellSpacing="0"
	           >
	      <thead style="text-align: center; font-size: large; font-weight:bold;background-color:#FF4000">
	        <td>Test List Name</td>
	        <td>Test Name</td>
	        <td>Test Result</td>
	        <td>Test Duration</td>
	        <td>Class Name</td>
	        <td>Category</td>	        
	      </thead>
	        <tr>
	             <xsl:apply-templates select="*[local-name()='Results']/*[local-name()='UnitTestResult'][@outcome='Failed']" >
		         </xsl:apply-templates>
	         </tr>
	         <tr>
	             <xsl:apply-templates select="*[local-name()='Results']/*[local-name()='UnitTestResult'][not(@outcome)]" >
		         </xsl:apply-templates>	         
	         </tr>			 
	    </table>
    </xsl:if>


	<h3>Longest running tests</h3>
	<!--
		<xsl:apply-templates select="*[local-name()='Results']/*[local-name()='UnitTestResult'] " >
		
		 </xsl:apply-templates>
-->
     <ol>
     <xsl:for-each select="*[local-name()='Results']/*[local-name()='UnitTestResult']">
			      <xsl:sort select="@duration"  order="descending"/>
			      <xsl:if test="position() &lt; 21">
				      <li><xsl:value-of select= "@duration"/> - <xsl:value-of select= "@testName"/></li> 
			       </xsl:if>
     </xsl:for-each>
     </ol>
    
 <!-- full test overview -->
    <xsl:apply-templates select="*[local-name()='Results']">
    </xsl:apply-templates>

  </xsl:template>

  <xsl:template match="*[local-name()='RunInfo']">
    <tr>
      <td>
          <xsl:apply-templates select="*" />
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="*[local-name()='Results']">
    <h2>Test Results</h2>
    <table border="1"
           cellPadding="2"
           cellSpacing="0"
           width="98%">
      <thead style="text-align: center; font-size: large; font-weight:bold;background-color:#33CCFF">
        <td>Test List Name</td>
        <td>Test Name</td>
        <td>Test Result</td>
        <td>Test Duration</td>
        <!--	   
	   <td>Test Start</td>
       <td>Test End</td>
-->
        <td>Class Name</td>
		<td>Test Categories</td>
      </thead>
      <tr>
        <xsl:apply-templates select="./*" />
      </tr>
    </table>
  </xsl:template>

  <xsl:template match="*[local-name()='TestResultAggregation']">
    <tr>
      <td colspan="3">
        <center>
          <b>
            Composite Test : <xsl:value-of select="@testName "/>
          </b>
          <br />
          <table border="1"
                 style="text-align: center;"
                 cellSpacing="0"
                 cellpadding="5" >
            <thead>
              <td style="background-color: forestGreen; color:white">Passed</td>
              <td style="background-color: fireBrick; color:white;">Failed</td>
              <td style="background-color: yellow; color:black;">Inconclusive</td>
              <td style="background-color: red; color: black;">Aborted</td>
              <td style="background-color: darkblue; color:white;">Timeout</td>
            </thead>
            <tr>
              <td>
                <xsl:value-of select="*[local-name()='Counters']/@passed"/>
              </td>
              <td>
                <xsl:value-of select="*[local-name()='Counters']/@failed"/>
              </td>
              <td>
                <xsl:value-of select="*[local-name()='Counters']/@inconclusive"/>
              </td>
              <td>
                <xsl:value-of select="*[local-name()='Counters']/@aborted"/>
              </td>
              <td>
                <xsl:value-of select="*[local-name()='Counters']/@timeout"/>
              </td>
            </tr>
          </table>

        </center>
      </td>
    </tr>
    <xsl:apply-templates select="./*[local-name()='InnerResults']/*" />

    <tr>
      <td colspan="4"
          style="text-align: center; font-weight: bold;">
        End of composite test :  <xsl:value-of select="@testName "/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="*[local-name()='UnitTestResult']">
    <xsl:variable name="tstid"
                  select="@testListId" />
    <xsl:variable name="tstListName"
                  select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='TestLists']/*[local-name()='TestList'][@id=$tstid]/@name" />
    <xsl:variable name="testid"
                  select="@testId" />
    <xsl:variable name="tstClassname"
                  select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='TestDefinitions']/*[local-name()='UnitTest'][@id=$testid]/*[local-name()='TestMethod']/@className" />
    <xsl:variable name="cn"
                  select="substring-before($tstClassname,',')" />
				  
	<xsl:variable name="tstCategories">
		<xsl:for-each select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='TestDefinitions']/*[local-name()='UnitTest'][@id=$testid]/*[local-name()='TestCategory']/*[local-name()='TestCategoryItem']">
			<xsl:value-of select="@TestCategory" />
			<xsl:if test="position()!=last()">, </xsl:if>
		</xsl:for-each>
	</xsl:variable>				  

    <tr>
      <td>
        <xsl:value-of select="$tstListName"/>
      </td>
      <td>
        <xsl:value-of select="@testName"/>
      </td>
      <xsl:choose>
        <xsl:when test="@outcome = 'Passed'">
          <td class="tst_ok">Passed</td>
        </xsl:when>
        <xsl:when test="@outcome = 'Failed'">
          <td class="tst_nok">Failed</td>
        </xsl:when>
        <xsl:when test="@outcome = 'Inconclusive'">
          <td class="tst_inc">Inconclusive</td>
        </xsl:when>
        <xsl:otherwise>
          <td class="tst_unk ">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>
      <td class="tst_dur">
        <xsl:value-of select="@duration"/>
      </td>
      <!--
     <td style="text-align: right;">
       <xsl:value-of select="@startTime"/>
     </td>

     <td style="text-align: right;">
       <xsl:value-of select="@endTime"/>
     </td>
-->
      <td >
        <xsl:value-of select="$cn"/>
      </td>

		<td >
			<xsl:value-of select="$tstCategories"/>
		</td>
	
    </tr>
    <xsl:apply-templates select="./*[local-name()='Output']/*[local-name()='ErrorInfo']" />
  </xsl:template>

  <xsl:template match="*[local-name()='ErrorInfo']">
    <tr>
      <td colspan="6"
          bgcolor="#FF9900">
        <b>
          <xsl:value-of select="./*[local-name()='Message']" />
        </b>
        <br />
        <xsl:value-of select="./*[local-name()='StackTrace']" />
      </td>
    </tr>
  </xsl:template>

<!--
  <xsl:template match="*[local-name()='TestResult']">
    <xsl:variable name="tstid"
                  select="@testListId" />
    <xsl:variable name="tstListName"
                  select="/cruisecontrol/build/*[local-name()='TestRun']/*[local-name()='TestLists']/*[local-name()='TestList'][@id=$tstid]/@name" />
    <tr>
      <td>
        <xsl:value-of select="$tstListName"/>
      </td>
      <td>
        <xsl:value-of select="@testName"/>
      </td>
      <xsl:choose>
        <xsl:when test="@outcome = 'Passed'">
          <td style="text-align: center; font-weight: bold; background-color: forestGreen; color: white">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:when>
        <xsl:when test="@outcome = 'Failed'">
          <td style="text-align: center; font-weight: bold; background-color: fireBrick; color: white  ">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:when>
        <xsl:when test="@outcome = 'Inconclusive'">
          <td style="text-align: center; font-weight: bold; background-color: yellow; color: black;">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td style="text-align: center; font-weight: bold; background-color: lightblue; color: black; ">
            <xsl:value-of select="@outcome"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>
      <td style="text-align: right;">
        <xsl:value-of select="@duration"/>
      </td>
    </tr>
  </xsl:template>
-->

  <xsl:template name="formatDateTime">
    <xsl:param name="dateTime" />
    <xsl:variable name="date"
                  select="substring-before($dateTime, 'T')" />
    <xsl:variable name="year"
                  select="substring-before($date, '-')" />
    <xsl:variable name="month"
                  select="substring-before(substring-after($date, '-'), '-')" />
    <xsl:variable name="day"
                  select="substring-after(substring-after($date, '-'), '-')" />

    <xsl:variable name="alltime"
                  select="substring-after($dateTime, 'T')" />
    <xsl:variable name="time"
                  select="substring-before($alltime, '.')" />

    <xsl:value-of select="concat($day, '-', $month, '-', $year, ' ',$time)" />
  </xsl:template>

</xsl:stylesheet>

