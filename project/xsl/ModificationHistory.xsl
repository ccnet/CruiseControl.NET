<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html"
      encoding="ISO-8859-1"
      standalone="yes"
      version="1.0"
      indent="yes"/>
    <xsl:param name="applicationPath"/>
    <xsl:param name="onlyShowBuildsWithModifications"/>

<xsl:template match="/">
       <script type="text/javascript">
           function toggleTr(imgId, trId)
            {
                eTr = document.getElementById(trId);
                eImg = document.getElementById(imgId);

                if ( eTr.style.display == "none" )
                {
                    /* Setting a TR to display:block doesn't work in proper browsers
                    but IE6's dodgy CSS implementation doesn't know table-row so
                    we need to try...catch it */
                    try
                    {
                        eTr.style.display="table-row";
                    }
                    catch(e)
                    {
                        eTr.style.display="block";
                    }
                    eImg.src="<xsl:value-of select="$applicationPath"/>/images/arrow_minus_small.gif";
                }
                else
                {
                    eTr.style.display = "none";
                    eImg.src="<xsl:value-of select="$applicationPath"/>/images/arrow_plus_small.gif";
                }
            }
       </script>

<H3>Modification History</H3>         
    <table width="98%">
        <xsl:for-each select="//Build">
            <xsl:sort select="@BuildDate" order="descending" data-type="text" />
                 <xsl:call-template name="ShowBuildRow" />
       </xsl:for-each>
    </table> 
 </xsl:template>


<xsl:template name="ShowBuildRow">

		<xsl:if test="count(modifications/modification)>0 or ( (count(modifications/modification)=0 and $onlyShowBuildsWithModifications=false() ))     ">

		      <xsl:variable name="Build_Id">
		          <xsl:value-of select="generate-id(@BuildDate)" />
		      </xsl:variable>
		
		      <tr> <!--  color the row according to success or failure -->
		          <xsl:call-template name="GetBuildRowColor">
		                  <xsl:with-param name="Succeeded" select="@Success" />
		          </xsl:call-template>
		
		          <th align="left">
		          <!--  add a clickable section to expand or collapse the changes within this build -->
		              <span>
		                    <xsl:attribute name="onclick">
		                         <xsl:text>toggleTr('img-</xsl:text>
		                         <xsl:value-of select="$Build_Id" />
		                         <xsl:text>','</xsl:text>
		                         <xsl:value-of select="$Build_Id" />
		                         <xsl:text>')</xsl:text>
		                    </xsl:attribute>
		                   <xsl:attribute name="class">
		                       <xsl:text>clickable</xsl:text>
		                    </xsl:attribute>
		
		                    <img src="{$applicationPath}/images/arrow_plus_small.gif" 
		                           alt="Toggle display of the changes within this build">  
		                           <xsl:attribute name="id">
		                              <xsl:text>img-</xsl:text>
		                              <xsl:value-of select="$Build_Id" />
		                           </xsl:attribute>
		                    </img>                      
		
		          <!--  show data of this build -->
		                    <xsl:text>&#0160;&#0160;&#0160;&#0160;</xsl:text>
		                    <xsl:value-of select="@BuildDate"/>    
		      
		                    <xsl:text>&#0160;</xsl:text>
		                    Label : <xsl:value-of select="@Label"/>
		
		                    <xsl:text>&#0160;&#0160;&#0160;&#0160;Changed Files: </xsl:text>
		                    <xsl:value-of select="count(modifications/modification)"  />

				    <xsl:text>&#0160;&#0160;&#0160;&#0160; </xsl:text>
		                    <xsl:value-of select="(modifications/modification/comment)"  />


                  </span>
		          </th>
		      </tr>
		      <tr>
		          <xsl:attribute name="id">
		               <xsl:value-of select="$Build_Id" />
		          </xsl:attribute>
		          <xsl:attribute name="style">
		             <xsl:text>display:none;</xsl:text>    
		          </xsl:attribute>                      
		
		          <td>
		              <table cellspacing="0" border="0" class="section-table" width="98%">
		                  <xsl:for-each select="modifications/modification">
		                    <tr>            
		                        <xsl:if test="position() mod 2=0">
		                             <xsl:attribute name="class">section-oddrow</xsl:attribute>
		                         </xsl:if>
		                         <xsl:if test="position() mod 2!=0">
		                             <xsl:attribute name="class">section-evenrow</xsl:attribute>
		                         </xsl:if>                                                 
		                       
		                         <td/>
		                         <td class="section-data"><xsl:attribute name="NOWRAP"/><xsl:value-of select="@type"/></td>                                                                           
		                         <td class="section-data"><xsl:attribute name="NOWRAP"/><xsl:value-of select="user"/></td>                                                                                   
		                         <td class="section-data"><xsl:value-of select="comment"/></td>
		                         <td class="section-data"><xsl:value-of select="project"/></td>
		                    </tr>
		                  </xsl:for-each>
		              </table>
		          </td>
		      </tr>               
		</xsl:if>
</xsl:template>



<xsl:template name="GetBuildRowColor">
    <xsl:param name="Succeeded" />
    
      <xsl:attribute name="bgcolor">
      <xsl:choose>
           <xsl:when test="$Succeeded='True'">
                  <xsl:text>#339900</xsl:text>
           </xsl:when>
           <xsl:otherwise>
                  <xsl:text>#FF0000</xsl:text>      
           </xsl:otherwise>
      </xsl:choose>
      
      </xsl:attribute>
</xsl:template>
  
</xsl:stylesheet>