<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
    xmlns:lxslt="http://xml.apache.org/xslt">

    <xsl:output method="html"/>
    
	<xsl:variable name="totalnotrun" select="//test-results/@not-run"/>
	<xsl:variable name="nunit2result.list" select="//test-results"/>
	<xsl:variable name="nunit2testcount" select="$nunit2result.list/@total"/>
	<xsl:variable name="nunit2failures" select="$nunit2result.list/@failures"/>
	<xsl:variable name="nunit2notrun" select="$nunit2result.list/@not-run"/>
	<xsl:variable name="nunit2case.list" select="$nunit2result.list//test-case"/>
	<xsl:variable name="nunit2suite.list" select="$nunit2result.list//test-suite"/>
	<xsl:variable name="nunit2.failure.list" select="$nunit2case.list//failure"/>
	<xsl:variable name="nunit2.notrun.list" select="$nunit2case.list//reason"/>

   <xsl:variable name="testsuite.list" select="/cruisecontrol/build/buildresults//testsuite"/>
    <xsl:variable name="testcase.list" select="$testsuite.list/testcase"/>
    <xsl:variable name="testcase.error.list" select="$testcase.list/error"/>
    <xsl:variable name="testsuite.error.count" select="count($testcase.error.list)"/>
    <xsl:variable name="testcase.failure.list" select="$testcase.list/failure"/>
    <xsl:variable name="totalErrorsAndFailures" select="count($testcase.error.list) + count($testcase.failure.list) + count($nunit2.failure.list)"/>

    <xsl:template match="/">
        <table cellpadding="2" cellspacing="0" border="0" width="98%">

            <!-- Unit Tests -->
            <tr>
                <td style="font-size:12px; color:#000000; font-weight:bold;" colspan="2">
                   &#160;Unit Tests: (<xsl:value-of select="count($testcase.list) + count($nunit2case.list)"/>)
                </td>
            </tr>

            <xsl:choose>
                <xsl:when test="count($testsuite.list) + count($nunit2suite.list) = 0">
                    <tr>
                        <td colspan="2" style="font-size:9px; color:#000000;">
                            No Tests Run
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="font-size:9px; color:#FF3300;">
                            This project doesn't have any tests
                        </td>
                    </tr>
                </xsl:when>

                <xsl:when test="$totalErrorsAndFailures = 0">
                    <tr>
                        <td colspan="2" style="font-size:9px; color:#000000;">
                            All Tests Passed
                        </td>
                    </tr>
                </xsl:when>
            </xsl:choose>

            <xsl:apply-templates select="$testcase.error.list"/>
            <xsl:apply-templates select="$testcase.failure.list | $nunit2.failure.list"/>
            <xsl:apply-templates select="$nunit2.notrun.list"/>
            <tr/>
            <tr><td colspan="2">&#160;</td></tr>

            <xsl:if test="$totalErrorsAndFailures > 0">

              <tr>
                <td style="background-color:#000066; color:#FFFFFF;" colspan="2">
                    &#160;Unit Test Failure and Error Details:&#160;(<xsl:value-of select="$totalErrorsAndFailures"/>)
                </td>
              </tr>

              <!-- (PENDING) Why doesn't this work if set up as variables up top? -->
              <xsl:call-template name="testdetail">
                <xsl:with-param name="detailnodes" select="//testsuite/testcase[.//error]"/>
              </xsl:call-template>

              <xsl:call-template name="testdetail">
                <xsl:with-param name="detailnodes" select="//testsuite/testcase[.//failure]"/>
              </xsl:call-template>
              
              <xsl:call-template name="nunit2testdetail">
	        <xsl:with-param name="detailnodes" select="//test-suite//test-case[.//failure]"/>
              </xsl:call-template>


              <tr><td colspan="2">&#160;</td></tr>
            </xsl:if>
            
            
            
                       <xsl:if test="$nunit2notrun > 0">
	    
	                  <tr>
	                    <td style="font-size:12px; color:#000000; font-weight:bold;" colspan="2">
	                        &#160;Warning Details:&#160;(<xsl:value-of select="$nunit2notrun"/>)
	                    </td>
	                  </tr>
	    
	                  <!-- (PENDING) Why doesn't this work if set up as variables up top? -->
	                  <xsl:call-template name="nunit2testdetail">
	    	        <xsl:with-param name="detailnodes" select="//test-suite//test-case[.//reason]"/>
	                  </xsl:call-template>
	    
	    
	                  <tr><td colspan="2">&#160;</td></tr>
            </xsl:if>
            
            


        </table>
    </xsl:template>

    <!-- UnitTest Errors -->
    <xsl:template match="error">
        <tr>
            <xsl:if test="position() mod 2 = 0">
                <xsl:attribute name="style">background-color:#CCCCCC</xsl:attribute>
            </xsl:if>

            <td style="font-size:9px; color:#000000;">
                error
            </td>
            <td style="font-size:9px; color:#000000;">
                <xsl:value-of select="../@name"/>
            </td>
        </tr>
    </xsl:template>

    <!-- UnitTest Failures -->
    <xsl:template match="failure">
        <tr>
            <xsl:if test="($testsuite.error.count + position()) mod 2 = 0">
                <xsl:attribute name="style">background-color:#CCCCCC</xsl:attribute>
            </xsl:if>

            <td style="font-size:9px; color:#000000;">
                failure
            </td>
            <td style="font-size:9px; color:#000000;">
                <xsl:value-of select="../@name"/>
            </td>
        </tr>
    </xsl:template>
    
       <!-- UnitTest Warnings -->
        <xsl:template match="reason">
            <tr>
                <xsl:if test="($totalErrorsAndFailures + position()) mod 2 = 0">
                    <xsl:attribute name="style">background-color:#CCCCCC</xsl:attribute>
                </xsl:if>
    
                <td style="font-size:9px; color:#000000;">
                    warning
                </td>
                <td style="font-size:9px; color:#000000;">
                    <xsl:value-of select="../@name"/>
                </td>
            </tr>
    </xsl:template>

    <!-- UnitTest Errors And Failures Detail Template -->
    <xsl:template name="testdetail">
      <xsl:param name="detailnodes"/>

      <xsl:for-each select="$detailnodes">

        <tr>
            <td colspan="2" style="font-size:9px; color:#000000;">
                Test:&#160;<xsl:value-of select="@name"/>
            </td>
        </tr>

        <xsl:if test="error">
            <tr>
                <td colspan="2" style="font-size:9px; color:#000000;">
                    Type: <xsl:value-of select="error/@type" />
                </td>
            </tr>
        <tr>
            <td colspan="2" style="font-size:9px; color:#000000;">
                Message: <xsl:value-of select="error/@message" />
            </td>
        </tr>

        <tr>
            <td colspan="2" style="font-size:9px; color:#FF3300;">
                <PRE>
                    <xsl:call-template name="br-replace">
                        <xsl:with-param name="word" select="error" />
                    </xsl:call-template>
                </PRE>
            </td>
        </tr>
        </xsl:if>

        <xsl:if test="failure">
        <tr>
            <td colspan="2" style="font-size:9px; color:#000000;">
                Type: <xsl:value-of select="failure/@type" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="font-size:9px; color:#000000;">
                Message: <xsl:value-of select="failure/@message" />
            </td>
        </tr>

        <tr>
            <td colspan="2" style="font-size:9px; color:#FF3300;">
                <pre>
                    <xsl:call-template name="br-replace">
                        <xsl:with-param name="word" select="failure"/>
                    </xsl:call-template>
                </pre>
            </td>
        </tr>
        </xsl:if>

      </xsl:for-each>
    </xsl:template>
    
    
       <!-- UnitTest Errors And Failures Detail Template -->
        <xsl:template name="nunit2testdetail">
          <xsl:param name="detailnodes"/>
    
          <xsl:for-each select="$detailnodes">
    		<xsl:if test="failure">
            <tr>
                <td colspan="2" style="font-size:9px; color:#000000;">
                    Test:&#160;<xsl:value-of select="@name"/>
                </td>
            </tr>
    
            <tr>
                <td colspan="2" style="font-size:9px; color:#000000;">
                    Type: failure
                </td>
            </tr>
            <tr>
                <td colspan="2" style="font-size:9px; color:#000000;">
                    Message: <xsl:value-of select="failure//message" />
                </td>
            </tr>
    
            <tr>
                <td colspan="2" style="font-size:9px; color:#FF3300;">
                    <pre>
                        <xsl:value-of select="failure//stack-trace"/>
                    </pre>
                </td>
            </tr>
            </xsl:if>
            
               		<xsl:if test="reason">
	                <tr>
	                    <td colspan="2" style="font-size:9px; color:#000000;">
	                        Test:&#160;<xsl:value-of select="@name"/>
	                    </td>
	                </tr>
	        
	                <tr>
	                    <td colspan="2" style="font-size:9px; color:#000000;">
	                        Type: warning
	                    </td>
	                </tr>
	                <tr>
	                    <td colspan="2" style="font-size:9px; color:#000000;">
	                        Message: <xsl:value-of select="reason//message" />
	                    </td>
	                </tr>
	        
	                <tr>
	                    <td colspan="2" style="font-size:9px; color:#FF3300;">
	                        <pre>
	                            <xsl:call-template name="br-replace">
	                                <xsl:with-param name="word" select="/stack-trace"/>
	                            </xsl:call-template>
	                        </pre>
	                    </td>
	                </tr>
            </xsl:if>
    
          </xsl:for-each>
    </xsl:template>

    <xsl:template name="br-replace">
        <xsl:param name="word"/>
<!-- </xsl:text> on next line on purpose to get newline -->
<xsl:variable name="cr"><xsl:text>
</xsl:text></xsl:variable>
        <xsl:choose>
            <xsl:when test="contains($word,$cr)">
                <xsl:value-of select="substring-before($word,$cr)"/>
                <br/>
                <xsl:call-template name="br-replace">
                <xsl:with-param name="word" select="substring-after($word,$cr)"/>
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$word"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>


</xsl:stylesheet>
