<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="xml" version="1.0" encoding="iso-8859-1" indent="yes"/>

  <!-- default template (identity transform) -->
  <xsl:template match="*|@*">
	<xsl:copy>
	    <xsl:apply-templates select="node()|@*"/>
	</xsl:copy>
  </xsl:template>

  <!-- switch sourcecontrol element to lower case -->
  <xsl:template match="/cruisecontrol/project/sourceControl">
  	<sourcecontrol><xsl:apply-templates select="node()|@*"/></sourcecontrol>
  </xsl:template>

  <!-- replace name with type attribute -->
  <xsl:template match="/cruisecontrol/project/sourceControl/@name">
	<xsl:attribute name="type">
		<xsl:value-of select="."/>
	</xsl:attribute>
  </xsl:template>

  <!-- add type attribute to build element -->
  <xsl:template match="/cruisecontrol/project/build">
  	<build>
		<xsl:attribute name="type">
			<xsl:text>nant</xsl:text>
		</xsl:attribute>
		<xsl:apply-templates/>
  	</build>
  </xsl:template>
  
  <!-- remove buildTimeout from build element -->
  <xsl:template match="/cruisecontrol/project/build/buildTimeout"/>
  
  <!-- rename sleep element to buildTimeout element -->
  <xsl:template match="/cruisecontrol/project/sleep">
  	<buildTimeout><xsl:value-of select="."/></buildTimeout>
  </xsl:template>

  <!-- remove logDir element -->
  <!-- <xsl:template match="/cruisecontrol/project/logDir"/> -->
  
  <!-- move projectUrl to emailPublisher element -->
  <xsl:template match="/cruisecontrol/project/publishers/email">
  	<xsl:copy>
	    <xsl:apply-templates select="node()|@*"/>  	
            <xsl:copy-of select="/cruisecontrol/project/@projectUrl"/>
            <xsl:copy-of select="/cruisecontrol/project/projectUrl"/>
  	</xsl:copy>
  </xsl:template>
  <xsl:template match="/cruisecontrol/project/@projectUrl"/>
  <xsl:template match="/cruisecontrol/project/projectUrl"/>

</xsl:stylesheet>
