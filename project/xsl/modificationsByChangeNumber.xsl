<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

 <xsl:output method="html"/>

 <xsl:param name="applicationPath" select="'.'" />
 <xsl:variable name="modification.list" select="/cruisecontrol/modifications/modification"/>
 <xsl:key name="modifications-by-changelist" match="modification" use="changeNumber" />

 <xsl:template match="/">
   <script language="javascript">
     function toggle (name, label)
     {
       var element = $(name);

       if (element.css("display") == "none") {
         element.css("display", "");
         $(label).text("Hide files");
       }
       else {
         element.css("display", "none");
         $(label).text("Show files");
       }
     }

     function hideAll()
     {
       $(".change-details").css("display", "none");
       $(".view-toggle").text("Show files");
     }

     function showAll()
     {
       $(".change-details").css("display", "");
       $(".view-toggle").text("Hide files");
     }
   </script>
   <table class="section-table" cellpadding="2" cellspacing="0"
                   border="0" width="98%" style="border-collapse:collapse;" >
     <!-- Modifications -->
     <tr>
       <td class="sectionheader" colspan="5">
         Modifications since last build (<xsl:value-of select="count($modification.list)"/>)
       </td>
     </tr>
     <tr>
       <td colspan="5" class="section-data">
         <a href="#" style="color: #00E;" onclick="hideAll(); return false;">Hide All Files</a>
         <span style="padding-left: 1em; padding-right: 1em;"> </span>
         <a href="#" style="color: #00E;" onclick="showAll(); return false;">Show All Files</a>
       </td>
     </tr>

     <!-- See http://www.jenitennison.com/xslt/grouping/muenchian.html
          for an explanation of this grouping trick. -->
     <xsl:for-each select="/cruisecontrol/modifications/modification[count(. | key('modifications-by-changelist',changeNumber)[1]) = 1]">
       <xsl:sort select="changeNumber" order="descending"/>
       <xsl:variable name="change" select="changeNumber"/>
       <tr>
         <td>
           <table width="100%" border="0">
             <tr style="font-weight: bold;">
               <td valign="top" width="25%">
                 <xsl:value-of select="changeNumber"/>
               </td>
               <td valign="top" width="25%">
                 <xsl:value-of select="user"/>
               </td>
               <td valign="top">
                 <xsl:value-of select="date"/>
               </td>
               <td valign="top" width="20%">
                 <xsl:value-of select="count(key('modifications-by-changelist', changeNumber))"/> Files
               </td>
             </tr>
             <tr style="border-bottom: 1px dotted gray;">
               <td class="section-data" valign="top" colspan="4">
                 <xsl:value-of select="comment" />
                 <br/><a href="#" style="color: #00E"
                         onclick="toggle('#change-{$change}','#toggle-{$change}'); return false;"
                         id="toggle-{$change}" class="view-toggle">Hide Files</a>
               </td>
             </tr>
           </table>
           <table width="100%" border="0" id="change-{$change}" class="change-details">
             <xsl:for-each select="key('modifications-by-changelist',changeNumber)">
               <xsl:sort select="project" />
               <tr>
                 <xsl:if test="position() mod 2=0">
                   <xsl:attribute name="class">section-evenrow</xsl:attribute>
                 </xsl:if>
                 <xsl:if test="position() mod 2!=0">
                   <xsl:attribute name="class">section-oddrow</xsl:attribute>
                 </xsl:if>
                 <td class="section-data">
                   <xsl:value-of select="project" />
                 </td>
                 <td class="section-data" width="30%">
                   <xsl:value-of select="filename" />
                 </td>
                 <td class="section-data" width="7%">
                   <xsl:value-of select="@type" />
                 </td>
               </tr>
             </xsl:for-each>
           </table>
         </td>
       </tr>
       <tr style="border-bottom: 1px solid;">
         <td colspan="0"></td>
       </tr>
     </xsl:for-each>
   </table>
 </xsl:template>
</xsl:stylesheet>