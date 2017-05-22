<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
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

.tst_nr
{   
	text-align: center; 
	font-weight: bold; 
	background-color: orange; 
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

    <xsl:apply-templates select="//test-results" />
  </xsl:template>
 
                    
   <xsl:template match="//test-results">
   
       <xsl:variable name="cwd" select="./environment/@cwd" />
        
   <h2><xsl:value-of select="substring(substring-after(@name, $cwd),2)"/></h2>
   <table border="1"
           cellSpacing="0"
           cellPadding="5" >
           <thead style="text-align: center;">
		        <td>Total</td>
		        <td class="tst_nok">Errors</td>        
		        <td class="tst_nok">Failures</td>        
		        <td class="tst_nr">Not run</td>        
		        <td class="tst_inc">Inconclusive</td>        
		        <td class="tst_nr">Ignored</td>        
		        <td class="tst_nr">Skipped</td>        
		        <td class="tst_nr">Invalid</td>        
		        <td>Duration</td>              	</thead>
       <tr>
        <td><xsl:value-of select="@total"/></td>
        <td ><xsl:value-of select="@errors"/></td>
        <td ><xsl:value-of select="@failures"/></td>
        <td ><xsl:value-of select="@not-run"/></td>
        <td ><xsl:value-of select="@inconclusive"/></td>
        <td ><xsl:value-of select="@ignored"/></td>
        <td ><xsl:value-of select="@skipped"/></td>
        <td ><xsl:value-of select="@invalid"/></td>
        <td><xsl:value-of select="./test-suite/@time"/></td>        
      </tr>
   </table>
      
       <xsl:variable name="FailedOnes"
                  select=".//test-case[@success!='True']" />
                  
	    <xsl:if test="count($FailedOnes) > 0">
	      <h3>Errors and Failures</h3>
	      <table width="100%"
	             border="1"
	             cellSpacing="0">

			      <thead style="text-align: center; font-size: large; font-weight:bold;background-color:#FF4000">
			        <td>Test Duration</td>
			        <td>Test Result</td>
			        <td>Test Name</td>
			      </thead>
	        <xsl:apply-templates select="$FailedOnes" />
	      </table>
	    </xsl:if>
	   
   
   	<h3>Longest running tests</h3>
	     <ol>
	     <xsl:for-each select=".//test-case">
				      <xsl:sort select="@time"  order="descending" data-type="number"/>
				      <xsl:if test="position() &lt; 21">
					      <li><xsl:value-of select= "@time"/> - <xsl:value-of select= "@name"/></li> 
				       </xsl:if>
	     </xsl:for-each>
	     </ol>
      
     <h2>Test Overview</h2>
        <table border="1"
           cellPadding="2"
           cellSpacing="0" >
	      <thead style="text-align: center; font-size: large; font-weight:bold;background-color:#33CCFF">
		        <td>Test Duration</td>
		        <td>Test Result</td>
		        <td>Test Name</td>
		      </thead>            	<xsl:apply-templates select=".//test-case"/>
       </table>
       
	</xsl:template>

     
    <xsl:template match="test-case" >
         
			<tr >
	        <xsl:if test="@executed = 'False' ">
				  <xsl:attribute name="Title">
				    <xsl:value-of select="./reason/message" />			    
				  </xsl:attribute>			
			  </xsl:if>

			  <td class="tst_dur"><xsl:value-of select="@time"/></td>

			   <xsl:choose>
			     <xsl:when test="@executed = 'False' ">
			               <td class="tst_nr">Not Run</td>
			     </xsl:when>
			     <xsl:otherwise>
			     			<xsl:choose>
			     				<xsl:when test="@success = 'True' ">
			     						<td class="tst_ok">Passed</td>
			     				</xsl:when>
			     			   <xsl:otherwise>
			     			   		<td class="tst_nok">Failed</td>
			     			   </xsl:otherwise>
			     			</xsl:choose>
			     </xsl:otherwise>
			   </xsl:choose>

	 		<td>
	        <xsl:value-of select="@name"/>
	      </td>
		
		 </tr>		 
		            
        <xsl:if test="@success != 'True' ">
     				<tr bgcolor="#FF9900"><td colspan="3"><xsl:value-of select="./failure/message"/></td></tr>
			</xsl:if>

		 		                 
	 </xsl:template>
		                 
</xsl:stylesheet>                