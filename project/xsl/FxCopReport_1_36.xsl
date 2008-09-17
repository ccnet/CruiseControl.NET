<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/html4/strict.dtd">	
	<xsl:output method="html"/>
	
	<xsl:template match="/">
		<xsl:variable name="report.root" select="cruisecontrol//FxCopReport" />
		<div id="report">
			<script>
				function toggleDiv( imgId, divId )
				{
					eDiv = document.getElementById( divId );
					eImg = document.getElementById( imgId );
					
					if ( eDiv.style.display == "none" )
					{
						eDiv.style.display = "block";
						eImg.src = "/ccnet/images/arrow_minus_small.gif";
					}
					else
					{
						eDiv.style.display = "none";
						eImg.src = "/ccnet/images/arrow_plus_small.gif";
					}
				}
			</script>
			<h1>FxCop Results</h1>
			<div id="summary">
				<h3>Summary</h3>
				<table>
					<tbody>
						<tr>
							<td>Assemblies checked:</td>
							<td><xsl:value-of select="count($report.root/Targets/Target/Modules/Module)"/></td>
						</tr>
						<tr>
							<td>Warnings:</td>
							<td><xsl:value-of select="count($report.root//Message[Issue/@Level='Warning'])"/></td>
						</tr>
						<tr>
							<td>Critical Warnings:</td>
							<td><xsl:value-of select="count($report.root//Message[Issue/@Level='CriticalWarning'])"/></td>
						</tr>
						<tr>
							<td>Errors:</td>
							<td><xsl:value-of select="count($report.root//Message[Issue/@Level='Error'])"/></td>
						</tr>
						<tr>
							<td>Critical Errors:</td>
							<td><xsl:value-of select="count($report.root//Message[Issue/@Level='CriticalError'])"/></td>
						</tr>
						<tr>
							<td>Total Messages:</td>
							<td><xsl:value-of select="count($report.root//Message)"/></td>
						</tr>
					</tbody>
				</table>
			</div>
			<div id="details">
				<h3>FxCop Details:</h3>
				<xsl:apply-templates select="$report.root/Targets/Target/Modules/Module" />
			</div>
		</div>
	</xsl:template>
	
	<xsl:template match="Module">
		<xsl:variable name="coverage.module.id" select="generate-id()" />
		<xsl:variable name="coverage.module.name" select="./@Name"/>
		
		<table cellpadding="2" cellspacing="0" border="0" width="98%">
			<tr>
				<td class="yellow-sectionheader" colspan="3" valign="top">
					<xsl:call-template name="ErrorsAndWarningsIcon">
						<xsl:with-param name="messages" select=".//Message" />
					</xsl:call-template>
					
					<input type="image" src="/ccnet/images/arrow_plus_small.gif">
						<xsl:attribute name="id">img<xsl:value-of select="$coverage.module.id"/></xsl:attribute>
						<xsl:attribute name="onclick">javascript:toggleDiv('img<xsl:value-of select="$coverage.module.id"/>', 'divDetails_<xsl:value-of select="$coverage.module.id"/>');</xsl:attribute>
					</input>&#160;<xsl:value-of select="$coverage.module.name"/>
				</td>
			</tr>
		</table>
		<div style="display:none">
			<xsl:attribute name="id">divDetails_<xsl:value-of select="$coverage.module.id"/></xsl:attribute>
			<blockquote>
				<xsl:apply-templates select=".//Messages" />
			</blockquote>
		</div>		
	</xsl:template>
	
	<xsl:template match="Type">
	
	
	
	</xsl:template>
	
	<xsl:template match="Messages">
	
		<xsl:if test="count(./Message) > 0">
		
		<xsl:call-template name="ClassOrAssemblyParent">
			<xsl:with-param name="messages" select="." />
		</xsl:call-template>
			
		<table border="0" cell-padding="6" cell-spacing="0" width="90%">
			<xsl:apply-templates select="./Message" />
		</table>
		</xsl:if>
		
	</xsl:template>
	
	<xsl:template match="Message">
	
		<xsl:variable name="module">
			<xsl:call-template name="replace">
				<xsl:with-param name="string" select="../../@Name" />
				<xsl:with-param name="search" select="'.'" />
				<xsl:with-param name="replace" select="'_'" />
			</xsl:call-template>
		</xsl:variable>
		<xsl:variable name="typename" select="@TypeName" />
		<xsl:variable name="fullname" select="concat($module,'_',$typename)" />
		<xsl:variable name="issues" select="count(Issue)" />
		
		<tr>
			<xsl:if test="position() mod 2 = 0">
				<xsl:attribute name="class">section-oddrow</xsl:attribute>
			</xsl:if>
			<td valign="top" width="10%">
				<xsl:attribute name="rowspan"><xsl:value-of select="$issues" /></xsl:attribute>
				<xsl:call-template name="ErrorsAndWarningsIcon">
					<xsl:with-param name="messages" select="." />
				</xsl:call-template>
				<input type="image" src="/ccnet/images/arrow_plus_small.gif">
					<xsl:attribute name="id">imgMsgDetails_<xsl:value-of select="$fullname"/></xsl:attribute>
					<xsl:attribute name="onClick">javascript:toggleDiv('imgMsgDetails_<xsl:value-of select="$fullname"/>', 'divMsgDetails_<xsl:value-of select="$fullname"/>');</xsl:attribute>
				</input>
			</td>
			<td valign="top" width="90%">
				<xsl:value-of select="Issue"/>
			</td>
		</tr>
		<xsl:for-each select="Issue[position() > 1]">
			<tr>
				<td valign="top" width="90%">
					<xsl:value-of select="."/>
				</td>
			</tr>
		</xsl:for-each>
		<tr>
			<td></td>
			<td colspan="2">
				<div style="display:none">
					<xsl:attribute name="id">divMsgDetails_<xsl:value-of select="$fullname"/></xsl:attribute>
					<xsl:apply-templates select="//cruisecontrol//FxCopReport//Rule[@TypeName=$typename]" />
				</div>
			</td>
		</tr>
		
	</xsl:template>
	
	<xsl:template match="Rule">
		
		<table border="0" cell-padding="6" cell-spacing="0" width="100%" bgcolor="white">
			<tr><td valign="top">Rule:</td><td><a target="_new"><xsl:attribute name="href"><xsl:value-of select="Url" /></xsl:attribute></a><xsl:value-of select="Name" /></td></tr>
			<tr><td valign="top">Category:</td><td><xsl:value-of select="@Category" /></td></tr>
			<tr><td valign="top">Check ID:</td><td><xsl:value-of select="@CheckId" /></td></tr>
			<tr><td valign="top">Description:</td><td><xsl:value-of select="Description" /></td></tr>
			<tr><td valign="top">File:</td><td><xsl:value-of select="File/@Name" /> [<xsl:value-of select="File/@Version" />]</td></tr>
		</table>
	
	</xsl:template>
	
	<xsl:template name="ErrorsAndWarningsIcon">
		<xsl:param name="messages"/>
		
		<xsl:variable name="warnings" select="count($messages[Issue/@Level='Warning'])"/>
		<xsl:variable name="cWarnings" select="count($messages[Issue/@Level='CriticalWarning'])"/>
		<xsl:variable name="errors" select="count($messages[Issue/@Level='Error'])"/>
		<xsl:variable name="cErrors" select="count($messages[Issue/@Level='CriticalError'])"/>
		
		<xsl:choose>
		<xsl:when test="$cErrors > 0">
			<img src="/ccnet/images/fxcop-critical-error.gif">
				<xsl:attribute name="title">Critical Errors: <xsl:value-of select="$cErrors"/></xsl:attribute>
			</img>
		</xsl:when>
		<xsl:when test="$errors > 0">
			<img src="/ccnet/images/fxcop-error.gif">
				<xsl:attribute name="title">Errors: <xsl:value-of select="$errors"/></xsl:attribute>
			</img>
		</xsl:when>
		<xsl:when test="$cWarnings > 0">
			<img src="/ccnet/images/fxcop-critical-warning.gif">
				<xsl:attribute name="title">Critical Warnings: <xsl:value-of select="$cWarnings"/></xsl:attribute>
			</img>
		</xsl:when>
		<xsl:when test="$warnings > 0">
			<img src="/ccnet/images/fxcop-warning.gif">
				<xsl:attribute name="title">Warnings: <xsl:value-of select="$warnings"/></xsl:attribute>
			</img>
		</xsl:when>
		<xsl:otherwise>
			<img src="/ccnet/images/check.jpg" width="16" height="16" title="No errors or warnings"/>
		</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xsl:template name="replace">
		<xsl:param name="string" />
		<xsl:param name="search" />
		<xsl:param name="replace" />
		<xsl:if test="contains($string, $search)">
			<xsl:value-of select="concat(substring-before($string, $search),$replace)" />
			<xsl:call-template name="replace">
				<xsl:with-param name="string">
					<xsl:value-of select="substring-after($string, $search)" />
				</xsl:with-param>
				<xsl:with-param name="search" select="$search" />
				<xsl:with-param name="replace" select="$replace" />
			</xsl:call-template>
		</xsl:if>
		<xsl:if test="not(contains($string, $search))">
			<xsl:value-of select="$string" />
		</xsl:if>
	</xsl:template>
	
	<xsl:template name="ClassOrAssemblyParent">
		<xsl:param name="messages" />
		
		<xsl:variable name="parent" select="$messages/../@Name" />
		<xsl:variable name="path" select="concat($messages/Message[1]/Issue[1]/@Path,'\',$messages/Message[1]/Issue[1]/@File)" />
		
		<xsl:choose>
			<xsl:when test="not($path='\')">
				<a target="_new"><xsl:attribute name="href"><xsl:value-of select="$path" /></xsl:attribute><xsl:value-of select="$parent" /></a>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$parent" />
			</xsl:otherwise>
		</xsl:choose>
		
	</xsl:template>
	
</xsl:stylesheet>
