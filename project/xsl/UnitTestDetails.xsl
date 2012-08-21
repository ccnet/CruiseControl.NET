<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>

  <xsl:variable name="nunit2.result.list" select="//test-results"/>
  <xsl:variable name="nunit2.suite.list" select="$nunit2.result.list//test-suite"/>
  <xsl:variable name="nunit2.suite.count" select="count($nunit2.result.list//test-suite)"/>
  <xsl:variable name="nunit2.case.list" select="$nunit2.suite.list/results/test-case"/>
  <xsl:variable name="nunit2.case.count" select="count($nunit2.case.list)"/>
  <xsl:variable name="nunit2.time" select="sum($nunit2.result.list/test-suite[position()=1]/@time)"/>
  <xsl:variable name="nunit2.failure.list" select="$nunit2.case.list/failure"/>
  <xsl:variable name="nunit2.failure.count" select="count($nunit2.failure.list)"/>
  <xsl:variable name="nunit2.success.list" select="$nunit2.case.list[@success='True']"/>
  <xsl:variable name="nunit2.success.count" select="count($nunit2.success.list)"/>
  <xsl:variable name="nunit2.notrun.list" select="$nunit2.case.list/reason"/>
  <xsl:variable name="nunit2.notrun.count" select="count($nunit2.notrun.list)"/>
  
  <xsl:variable name="junit.suite.list" select="//testsuite"/>
  <xsl:variable name="junit.suite.count" select="count($junit.suite.list)"/>
  <xsl:variable name="junit.suite.error.list" select="$junit.suite.list/error"/>
  <xsl:variable name="junit.suite.error.count" select="count($junit.suite.error.list)"/>  
  <xsl:variable name="junit.case.list" select="$junit.suite.list/testcase"/>
  <xsl:variable name="junit.case.count" select="count($junit.case.list)"/>
  <xsl:variable name="junit.time" select="sum($junit.case.list/@time)"/>
  <xsl:variable name="junit.failure.list" select="$junit.case.list/failure"/>
  <xsl:variable name="junit.failure.count" select="count($junit.failure.list)"/>
  <xsl:variable name="junit.success.list" select="$junit.case.list[@success='True']"/>
  <xsl:variable name="junit.success.count" select="count($junit.success.list)"/>
  <xsl:variable name="junit.error.list" select="$junit.case.list/error"/>
  <xsl:variable name="junit.error.count" select="count($junit.error.list)"/>
  
  <xsl:variable name="total.time" select="$nunit2.time + $junit.time"/>
  <xsl:variable name="total.notrun.count" select="$nunit2.notrun.count"/>
  <xsl:variable name="total.suite.count" select="$nunit2.suite.count + $junit.suite.count"/>
  <xsl:variable name="total.run.count" select="$nunit2.case.count + $junit.case.count - $total.notrun.count"/>
  <xsl:variable name="total.failure.count" select="$nunit2.failure.count + $junit.failure.count + $junit.error.count + $junit.suite.error.count"/>

    <xsl:template match="/">      
        <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">

            <!-- Unit Tests -->
            <tr>
                <td class="sectionheader" colspan="2">
		          Suites run: <xsl:value-of select="$total.suite.count"/>,
		          Tests run: <xsl:value-of select="$total.run.count"/>, 
		          Failures: <xsl:value-of select="$total.failure.count"/>, 
		          Not run: <xsl:value-of select="$total.notrun.count"/>, 
		          Time: <xsl:value-of select="$total.time"/> seconds
                </td>
            </tr>

            <xsl:choose>
                <xsl:when test="$total.run.count = 0">
                    <tr><td colspan="2" class="section-data">No Tests Run</td></tr>
                    <tr><td colspan="2" class="section-error">This project doesn't have any tests</td></tr>
                </xsl:when>

                <xsl:when test="$total.failure.count = 0">
                    <tr><td colspan="2" class="section-data">All Tests Passed</td></tr>
                </xsl:when>
            </xsl:choose>

            <xsl:apply-templates select="$junit.success.list | $nunit2.success.list"/>
      		<xsl:apply-templates select="$junit.suite.error.list"/>
            <xsl:apply-templates select="$junit.error.list"/>
            <xsl:apply-templates select="$junit.failure.list | $nunit2.failure.list"/>
            <xsl:apply-templates select="$nunit2.notrun.list"/>

            <tr><td colspan="2"> </td></tr>

            <xsl:if test="$total.failure.count > 0">
                <tr>
                    <td class="sectionheader" colspan="2">
                        Unit Test Failure and Error Details (<xsl:value-of select="$total.failure.count"/>)
                    </td>
                </tr>

                <xsl:call-template name="junittestdetail">
                    <xsl:with-param name="detailnodes" select="//testsuite/testcase[.//error]"/>
                </xsl:call-template>

                <xsl:call-template name="junittestdetail">
                    <xsl:with-param name="detailnodes" select="//testsuite/testcase[.//failure]"/>
                </xsl:call-template>

                <xsl:call-template name="nunit2testdetail">
                    <xsl:with-param name="detailnodes" select="//test-suite/results/test-case[.//failure]"/>
                </xsl:call-template>

                <tr><td colspan="2"> </td></tr>
            </xsl:if>
            
            <xsl:if test="$nunit2.notrun.count > 0">
                <tr>
                    <td class="sectionheader" colspan="2">
                        Warning Details (<xsl:value-of select="$nunit2.notrun.count"/>)
                    </td>
                </tr>
                <xsl:call-template name="nunit2testdetail">
                    <xsl:with-param name="detailnodes" select="//test-suite/results/test-case[.//reason]"/>
                </xsl:call-template>
                <tr><td colspan="2"> </td></tr>
            </xsl:if>
        </table>
    </xsl:template>

    <!-- Unit Test Errors -->
    <xsl:template match="error">
        <tr>
            <xsl:if test="position() mod 2 = 0">
                <xsl:attribute name="class">section-oddrow</xsl:attribute>
            </xsl:if>
            <td class="section-data">Error</td>
            <td class="section-data"><xsl:value-of select="../@name"/></td>
        </tr>
    </xsl:template>

    <!-- Unit Test Failures -->
    <xsl:template match="failure">
        <tr>
            <xsl:if test="($junit.error.count + position()) mod 2 = 0">
                <xsl:attribute name="class">section-oddrow</xsl:attribute>
            </xsl:if>
            <td class="section-data">Failure</td>
            <td class="section-data"><xsl:value-of select="../@name"/></td>
        </tr>
    </xsl:template>

  <!-- Unit Test Successes -->
  <xsl:template match="test-case[@success='True'] | testcase[@success='True']">
    <tr>
      <xsl:if test="($junit.error.count + position()) mod 2 = 0">
        <xsl:attribute name="class">section-oddrow</xsl:attribute>
      </xsl:if>
      <td class="section-data">Success</td>
      <td class="section-data">
        <xsl:value-of select="@name"/>
      </td>
    </tr>
  </xsl:template>

  <!-- Unit Test Warnings -->
    <xsl:template match="reason">
        <tr>
            <xsl:if test="($total.failure.count + position()) mod 2 = 0">
                <xsl:attribute name="class">section-oddrow</xsl:attribute>
            </xsl:if>
            <td class="section-data">Warning</td>
            <td class="section-data"><xsl:value-of select="../@name"/></td>
        </tr>
    </xsl:template>

    <!-- JUnit Test Errors And Failures Detail Template -->
    <xsl:template name="junittestdetail">
      <xsl:param name="detailnodes"/>

      <xsl:for-each select="$detailnodes">

        <tr><td class="section-data">Test:</td><td class="section-data"><xsl:value-of select="@name"/></td></tr>
       
        <xsl:if test="error">
        <tr><td class="section-data">Type:</td><td class="section-data">Error</td></tr>
        <tr><td class="section-data">Message:</td><td class="section-data"><xsl:value-of select="error/@message"/></td></tr>
        <tr>
            <td></td>
            <td class="section-error">
                <pre><xsl:call-template name="br-replace">
                        <xsl:with-param name="word" select="error"/>
                    </xsl:call-template></pre>
            </td>
        </tr>
        </xsl:if>

        <xsl:if test="failure">
        <tr><td class="section-data">Type:</td><td class="section-data">Failure</td></tr>
        <tr><td class="section-data">Message:</td><td class="section-data"><xsl:value-of select="failure/@message"/></td></tr>
        <tr>
            <td></td>
            <td class="section-error">
                <pre><xsl:call-template name="br-replace">
                        <xsl:with-param name="word" select="failure"/>
                    </xsl:call-template></pre>
            </td>
        </tr>
        </xsl:if>

        <tr><td colspan="2"><hr size="1" width="100%" color="#888888"/></td></tr>
        
      </xsl:for-each>
    </xsl:template>

    <!-- NUnit Test Failures And Warnings Detail Template -->
    <xsl:template name="nunit2testdetail">
        <xsl:param name="detailnodes"/>

        <xsl:for-each select="$detailnodes">
        
            <xsl:if test="failure">
            <tr><td class="section-data">Test:</td><td class="section-data"><xsl:value-of select="@name"/></td></tr>
            <tr><td class="section-data">Type:</td><td class="section-data">Failure</td></tr>
            <tr><td class="section-data">Message:</td><td class="section-data"><xsl:value-of select="failure//message"/></td></tr>
            <tr>
                <td></td>
                <td class="section-error">
                    <pre><xsl:value-of select="failure//stack-trace"/></pre>
                </td>
            </tr>
            </xsl:if>

            <xsl:if test="reason">
            <tr><td class="section-data">Test:</td><td class="section-data"><xsl:value-of select="@name"/></td></tr>
            <tr><td class="section-data">Type:</td><td class="section-data">Warning</td></tr>
            <tr><td class="section-data">Message:</td><td class="section-data"><xsl:value-of select="reason//message"/></td></tr>
            </xsl:if>

            <tr><td colspan="2"><hr size="1" width="100%" color="#888888"/></td></tr>

        </xsl:for-each>
    </xsl:template>

    <xsl:template name="br-replace">
        <xsl:param name="word"/>
        <xsl:variable name="cr"><xsl:text>
        <!-- </xsl:text> on next line on purpose to get newline -->
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
