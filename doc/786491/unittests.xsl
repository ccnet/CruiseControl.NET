<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>

	<!-- CppUnit Cases (DEmon) -->
	<xsl:variable name="cppunit.result.list" select="/cruisecontrol/build/TestRun"/>
	<xsl:variable name="cppunit.failed.list" select="$cppunit.result.list/FailedTests/FailedTest"/>
	<xsl:variable name="cppunit.failed.count" select="count($cppunit.failed.list)"/>
	<xsl:variable name="cppunit.passed.list" select="$cppunit.result.list/SuccessfulTests/Test"/>
	<xsl:variable name="cppunit.passed.count" select="count($cppunit.passed.list)"/>
	<!-- CppUnit Statistics (DEmon) -->
	<xsl:variable name="cppunit.case.count" select="$cppunit.result.list/Statistics/Tests"/>
	<xsl:variable name="cppunit.failure.count" select="$cppunit.result.list/Statistics/Failures"/>
	<xsl:variable name="cppunit.error.count" select="$cppunit.result.list/Statistics/Errors"/>

    <xsl:variable name="nunit2.result.list" select="//test-results"/>
    <xsl:variable name="nunit2.suite.list" select="$nunit2.result.list//test-suite"/>
    <xsl:variable name="nunit2.case.list" select="$nunit2.suite.list/results/test-case"/>
    <xsl:variable name="nunit2.case.count" select="count($nunit2.case.list)"/>
    <xsl:variable name="nunit2.time" select="sum($nunit2.result.list/test-suite[position()=1]/@time)"/>
    <xsl:variable name="nunit2.failure.list" select="$nunit2.case.list/failure"/>
    <xsl:variable name="nunit2.failure.count" select="count($nunit2.failure.list)"/>
    <xsl:variable name="nunit2.notrun.list" select="$nunit2.case.list/reason"/>
    <xsl:variable name="nunit2.notrun.count" select="count($nunit2.notrun.list)"/>

    <xsl:variable name="junit.suite.list" select="//testsuite"/>
    <xsl:variable name="junit.case.list" select="$junit.suite.list/testcase"/>
    <xsl:variable name="junit.case.count" select="count($junit.case.list)"/>
    <xsl:variable name="junit.time" select="sum($junit.case.list/@time)"/>
    <xsl:variable name="junit.failure.list" select="$junit.case.list/failure"/>
    <xsl:variable name="junit.failure.count" select="count($junit.failure.list)"/>
    <xsl:variable name="junit.error.list" select="$junit.case.list/error"/>
    <xsl:variable name="junit.error.count" select="count($junit.error.list)"/>

    <!-- "Old" <xsl:variable name="total.time" select="$nunit2.time + $junit.time"/>
    <xsl:variable name="total.notrun.count" select="$nunit2.notrun.count"/>
    <xsl:variable name="total.run.count" select="$nunit2.case.count + $junit.case.count - $total.notrun.count"/>
    <xsl:variable name="total.failure.count" select="$nunit2.failure.count + $junit.failure.count + $junit.error.count"/> -->
    
    <!-- Added CppUnit (DEmon)-->
    <xsl:variable name="total.time" select="$nunit2.time + $junit.time"/>
    <xsl:variable name="total.notrun.count" select="$nunit2.notrun.count"/>
    <xsl:variable name="total.run.count" select="$cppunit.case.count + $nunit2.case.count + $junit.case.count - $total.notrun.count"/>
    <xsl:variable name="total.failure.count" select="$cppunit.failed.count + $nunit2.failure.count + $junit.failure.count + $junit.error.count"/>

    <xsl:template match="/">
        <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">

            <!-- Unit Tests -->
            <tr>
                <td class="sectionheader" colspan="2">
                   Tests run: <xsl:value-of select="$total.run.count"/>, Failures: <xsl:value-of select="$total.failure.count"/>, Not run: <xsl:value-of select="$total.notrun.count"/>, Time: <xsl:value-of select="$total.time"/> seconds
                </td>
            </tr>

            <xsl:choose> <!-- Обрабатываем крайние ситуации -->
                <xsl:when test="$total.run.count = 0"> <!-- Тестов нет вообще -->
                    <tr><td colspan="2" class="section-data">No Tests Run</td></tr>
                    <tr><td colspan="2" class="section-error">This project doesn't have any tests</td></tr>
                </xsl:when>

                <xsl:when test="$total.failure.count = 0"> <!-- Все тесты прошли -->
                    <tr><td colspan="2" class="section-data">All Tests Passed</td></tr>
                </xsl:when>
            </xsl:choose>

			<!-- Краткая информация об ошибках (DEmon)-->
			<xsl:apply-templates select="$cppunit.failed.list"/>
			
            <xsl:apply-templates select="$junit.error.list"/>
            <xsl:apply-templates select="$junit.failure.list | $nunit2.failure.list"/>
            <xsl:apply-templates select="$nunit2.notrun.list"/>


            <tr><td colspan="2"> </td></tr>

			<!-- Выводим ошибки -->
            <xsl:if test="$total.failure.count > 0"> 
                <tr>
                    <td class="sectionheader" colspan="2">
                        Unit Test Failure and Error Details (<xsl:value-of select="$total.failure.count"/>)
                    </td>
                </tr>

				<!-- Детальная информация об ошибках (DEmon)-->
				<xsl:call-template name="cppunittestdetail">
                    <xsl:with-param name="detailnodes" select="$cppunit.failed.list"/>
                </xsl:call-template>
				
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

	<!-- CppUnit Failed Test (DEmon)-->
	<xsl:template match="FailedTest">
        <tr>
            <xsl:if test="position() mod 2 = 0">
                <xsl:attribute name="class">section-oddrow</xsl:attribute>
            </xsl:if>
            <td class="section-data">CppUnit <xsl:value-of select="FailureType"/></td>
            <td class="section-data"><xsl:value-of select="Name"/></td>
        </tr>
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

    <!-- CppUnit Failed Test Detail Template (DEmon) -->
    <xsl:template name="cppunittestdetail">
        <xsl:param name="detailnodes"/>

        <xsl:for-each select="$detailnodes">
        
            <tr><td class="section-data">CppTest:</td><td class="section-data"><xsl:value-of select="Name"/></td></tr>
            <tr><td class="section-data">Type:</td><td class="section-data"><xsl:value-of select="FailureType"/></td></tr>
            <tr><td class="section-data">Message:</td><td class="section-data"><pre style="font-size=120%"><xsl:value-of select="Message"/></pre></td></tr>
            
            <xsl:if test="count(Location) > 0">
            <tr>
                <td></td>
                <td class="section-error">
                    <pre><xsl:value-of select="Location/File"/>: Line <xsl:value-of select="Location/Line"/></pre>
                </td>
            </tr>
            </xsl:if>

            <tr><td colspan="2"><hr size="1" width="100%" color="#888888"/></td></tr>

        </xsl:for-each>
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
