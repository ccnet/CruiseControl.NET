<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                version="1.0">
  <xsl:output method="html" />
  <xsl:variable name="nunit2.result.list"
                select="//test-results" />
  <xsl:variable name="nunit2.suite.list"
                select="$nunit2.result.list//test-suite" />
  <xsl:variable name="nunit2.suite.count"
                select="count($nunit2.result.list//test-suite)" />
  <xsl:variable name="nunit2.case.list"
                select="$nunit2.suite.list/results/test-case" />
  <xsl:variable name="nunit2.case.count"
                select="count($nunit2.case.list)" />
  <xsl:variable name="nunit2.time"
                select="sum($nunit2.result.list/test-suite[position()=1]/@time)" />
  <xsl:variable name="nunit2.failure.list"
                select="$nunit2.case.list/failure" />
  <xsl:variable name="nunit2.failure.count"
                select="count($nunit2.failure.list)" />
  <xsl:variable name="nunit2.success.list"
                select="$nunit2.case.list[@success='True']" />
  <xsl:variable name="nunit2.success.count"
                select="count($nunit2.success.list)" />
  <xsl:variable name="nunit2.notrun.list"
                select="$nunit2.case.list/reason" />
  <xsl:variable name="nunit2.notrun.count"
                select="count($nunit2.notrun.list)" />
  <xsl:variable name="junit.suite.list"
                select="//testsuite" />
  <xsl:variable name="junit.suite.count"
                select="count($junit.suite.list)" />
  <xsl:variable name="junit.suite.error.list"
                select="$junit.suite.list/error" />
  <xsl:variable name="junit.suite.error.count"
                select="count($junit.suite.error.list)" />
  <xsl:variable name="junit.case.list"
                select="$junit.suite.list/testcase" />
  <xsl:variable name="junit.case.count"
                select="count($junit.case.list)" />
  <xsl:variable name="junit.time"
                select="sum($junit.case.list/@time)" />
  <xsl:variable name="junit.failure.list"
                select="$junit.case.list/failure" />
  <xsl:variable name="junit.failure.count"
                select="count($junit.failure.list)" />
  <xsl:variable name="junit.success.list"
                select="$junit.case.list[@success='True']" />
  <xsl:variable name="junit.success.count"
                select="count($junit.success.list)" />
  <xsl:variable name="junit.error.list"
                select="$junit.case.list/error" />
  <xsl:variable name="junit.error.count"
                select="count($junit.error.list)" />
  <xsl:variable name="total.time"
                select="$nunit2.time + $junit.time" />
  <xsl:variable name="total.notrun.count"
                select="$nunit2.notrun.count" />
  <xsl:variable name="total.suite.count"
                select="$nunit2.suite.count + $junit.suite.count" />
  <xsl:variable name="total.run.count"
                select="$nunit2.case.count + $junit.case.count - $total.notrun.count" />
  <xsl:variable name="total.failure.count"
                select="$nunit2.failure.count + $junit.failure.count + $junit.error.count + $junit.suite.error.count" />
  <xsl:template match="/">
    <table cellpadding="2"
           cellspacing="0"
           border="0">
      <!-- Unit Tests -->
      <tr>
        <td class="headernote"
            colspan="2">Suites run: 
        <xsl:value-of select="$total.suite.count" />, Tests run: 
        <xsl:value-of select="$total.run.count" />, Failures: 
        <xsl:value-of select="$total.failure.count" />, Not run: 
        <xsl:value-of select="$total.notrun.count" />, Time: 
        <xsl:value-of select="$total.time" />seconds</td>
      </tr>
      <xsl:choose>
        <xsl:when test="$total.run.count = 0">
          <tr>
            <td colspan="2"
                class="note">No Tests Run</td>
          </tr>
          <tr>
            <td colspan="2"
                class="failurenote">This project doesn't have any tests</td>
          </tr>
        </xsl:when>
        <xsl:when test="$total.failure.count = 0">
          <tr>
            <td colspan="2"
                class="successnote">All Tests Passed</td>
          </tr>
        </xsl:when>
      </xsl:choose>
      <xsl:apply-templates select="$junit.success.list | $nunit2.success.list" />
      <xsl:apply-templates select="$junit.suite.error.list" />
      <xsl:apply-templates select="$junit.error.list" />
      <xsl:apply-templates select="$junit.failure.list | $nunit2.failure.list" />
      <xsl:apply-templates select="$nunit2.notrun.list" />
    </table>
  </xsl:template>
  <!-- Unit Test Errors -->
  <xsl:template match="error">
    <tr>
      <td class="errornote">
        <xsl:value-of select="../@name" />
      </td>
      <td class="errornote">Error</td>
    </tr>
  </xsl:template>
  <!-- Unit Test Failures -->
  <xsl:template match="failure">
    <tr>
      <td class="failurenote">
        <xsl:value-of select="../@name" />
      </td>
      <td class="failurenote">Failure</td>
    </tr>
  </xsl:template>
  <!-- Unit Test Successes -->
<!-- no need to show hundreds of ok, focus on the not ok ones
  <xsl:template match="test-case[@success='True']">
    <tr>
      <xsl:if test="($junit.success.count + position()) mod 2 = 0">
        <xsl:attribute name="class">oddrow</xsl:attribute>
      </xsl:if>
      <td class="successnote">
        <xsl:value-of select="@name" />
      </td>
      <td class="successnote">Success</td>
    </tr>
  </xsl:template>
  -->
  <!-- Unit Test Warnings -->
  <xsl:template match="reason">
    <tr>
      <td class="warningnote">
        <xsl:value-of select="../@name" />
      </td>
      <td class="warningnote">Warning</td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
