<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://www.w3.org/TR/xhtml1/strict">

<xsl:template match="/FxCopReport">
	<html>
	<head><title>Analysis Report by Rule</title></head>
	<style>
		#Title {font-family: Verdana; font-size: 14pt; color: black; font-weight: bold}
		.RowHeader {font-family: Verdana; font-size: 8pt; color: black}
		.Severity1 {font-family: Verdana; font-size: 8pt; color: darkred; font-weight: bold; }
		.Severity2 {font-family: Verdana; font-size: 8pt; color: royalblue; font-weight: bold; }
		.Severity3 {font-family: Verdana; font-size: 8pt; color: green; font-weight: bold; }
		.Severity4 {font-family: Verdana; font-size: 8pt; color: darkgray; font-weight: bold; }
		.PropertyName {font-family: Verdana; font-size: 8pt; color: black; font-weight: bold}
		.PropertyContent {font-family: Verdana; font-size: 8pt; color: black}
		.NodeIcon { font-family: WebDings; font-size: 12pt; color: navy; padding-right: 5;}
		.MessagesIcon { font-family: WebDings; font-size: 12pt; color: red;}
		.RuleDetails { padding-top: 10;}
		.SourceCode { background-color:#DDDDFF; }
		.RuleBlock { background-color:#EEEEFF; }
		.MessageNumber { font-family: Verdana; font-size: 10pt; color: darkred; }
		.MessageBlock { font-family: Verdana; font-size: 10pt; color: darkred; }
		.Resolution {font-family: Verdana; font-size: 8pt; color: black; }		
		.NodeLine { font-family: Verdana; font-size: 9pt;}
		.Note { font-family: Verdana; font-size: 9pt; color:black; background-color: #DDDDFF; }
		.NoteUser { font-family: Verdana; font-size: 9pt; font-weight: bold; }
		.NoteTime { font-family: Verdana; font-size: 8pt; font-style: italic; }
		.Button { font-family: Verdana; font-size: 9pt; color: blue; background-color: #EEEEEE; }
		.RuleNode { font-family: Verdana; font-size: 9pt;}
		TH { text-align: left; font-family: Verdana; font-size: 9pt;} 
	</style>
	<script>
		function ViewState(blockId)
		{
			var block = document.all.item(blockId);
			if (block.style.display=='none')
			{
				block.style.display='block';
			}
			else
			{ 
				block.style.display='none';
			}
		}
		
		function SwitchAll(how)
		{
			var len = document.all.length-1;
			for(i=0;i!=len;i++)
			{	
				var block = document.all[i];
				if (block != null)
				{
				    if(block.className == 'NodeDiv' || 
						block.className == 'RuleDiv' )					
				    {
					    block.style.display=how;
					}					
				}
			}
		}

		function ExpandAll()
		{
			SwitchAll('block');
		}
		
		function CollapseAll()
		{
			SwitchAll('none');
		}

	</script>
	<body bgcolor="white" alink="Black" vlink="Black" link="Black">

	<!-- Report Title -->
	<div id="Title">
		FxCop <xsl:value-of select="@Version"/> Analysis Report by Rule
	</div>
	<br/>
	<table>
		<tr>
			<td class="Button">
				<a onClick="ExpandAll();">Expand All</a>
			</td>
			<td class="Button">
				<a onClick="CollapseAll();">Collapse All</a>
			</td>
		</tr>
	</table>	
	<br/>
	<xsl:apply-templates select="Rules"/>
	</body>
	</html>
</xsl:template>

<xsl:template match="Message">
	<xsl:variable name="rulename" select="Rule/@TypeName"/>
	<div class="MessageBlock" style="position: relative; padding-left: 22;">
		<table width="100%">
			<tr>
				<th>
					Message
				</th>
			</tr>
			<xsl:apply-templates select="Notes" mode="notes"/>
			<tr bgcolor="#EEEEEE">
				<td class="RowHeader" width="20%">Severity:</td>
				<td>
					<xsl:variable name="severity" select="/FxCopReport/Rules/Rule[@TypeName=$rulename]/Severity" />
					<xsl:attribute name="class">Severity<xsl:value-of select="$severity" /></xsl:attribute>
					<xsl:value-of select="$severity" />
				</td>
			</tr>
			<tr>
				<td class="RowHeader">
					Certainty:
				</td>
				<td>
					<xsl:variable name="certainty" select="/FxCopReport/Rules/Rule[@TypeName=$rulename]/Certainty" />
					<xsl:attribute name="class">Severity<xsl:value-of select="$certainty" /></xsl:attribute>
					<xsl:value-of select="$certainty" />
				</td>
			</tr>
			<tr bgcolor="#EEEEEE">
				<td class="RowHeader">
					Resolution:
				</td>
				<td class="Resolution">
					<xsl:value-of select="Resolution/Text/text()" />
				</td>
			</tr>
			<xsl:choose>
				<xsl:when test="SourceCode">
					<tr>
				        <td class="RowHeader">
							Source:
						</td>
						<td class="PropertyContent">			        
							<a>
								<xsl:attribute name="href">
									<xsl:value-of select="SourceCode/@Path"/>/<xsl:value-of select="SourceCode/@File"/>
								</xsl:attribute>
								<xsl:value-of select="SourceCode/@Path"/>/<xsl:value-of select="SourceCode/@File"/>
							</a>
							(Line <xsl:value-of select="SourceCode/@Line"/>)
						</td>
					</tr>
				</xsl:when>
			</xsl:choose>
		</table>
	</div>
</xsl:template>

<xsl:template match="Notes" mode="notes">
	<xsl:for-each select="Note">
		<xsl:variable name="id" select="@ID"/>
		<xsl:apply-templates select="/FxCopReport/Notes/Note[@ID=$id]" mode="notes"/>
	</xsl:for-each>
</xsl:template>

<xsl:template match="Note" mode="notes">
	<tr class="Note">
		<td colspan="3" class="Note">
		<nobr class="NoteUser"><xsl:value-of select="@UserName"/></nobr>
		&#160;		
		<nobr class="NoteTime">[<xsl:value-of select="@Modified"/>]</nobr>:
		<xsl:value-of select="."/>
		</td>		
	</tr>
</xsl:template>

<xsl:template match="Rule">
	<xsl:variable name="nodeId" select="generate-id()"/>
	<xsl:variable name="rulename" select="@TypeName"/>
	<div class="RuleNode">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x009D;</nobr>	
        <xsl:value-of select="Name" />
        <nobr class="MessageNumber">
			(<xsl:value-of select="count(/FxCopReport/Targets/Target/descendant::Rule[@TypeName=$rulename])+count(/FxCopReport/Namespaces/Namespace/descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
		<br/>
    </div>
	<div class="RuleDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<table width="100%" class="RuleBlock">	
			<xsl:apply-templates select="." mode="ruledetails" />
		</table>
		<xsl:apply-templates select="/FxCopReport/Namespaces/Namespace[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="/FxCopReport/Targets/Target[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
	</div>
</xsl:template>

<xsl:template match="Target">
<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x0032;</nobr><xsl:value-of select="@Name"></xsl:value-of>
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>

		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Modules/Module[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Resources[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>			
	</div>
</xsl:template>


<xsl:template match="Module">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x0031;</nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>	
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
		<xsl:apply-templates select="Namespaces/Namespace[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Methods/Method[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
	</div>
</xsl:template>

<xsl:template match="Namespace">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr style="color: navy;">{} </nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
		<xsl:apply-templates select="Classes/Class[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="ValueTypes/ValueType[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Interfaces/Interface[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Delegates/Delegate[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Enums/Enum[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
	</div>
</xsl:template>

<xsl:template match="Class">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x003C;</nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>	
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
		<xsl:apply-templates select="Classes/Class[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="ValueTypes/ValueType[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Interfaces/Interface[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Delegates/Delegate[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Enums/Enum[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Constructors/Contructor[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Properties/Property[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Events/Event[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Fields/Field[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Methods/Method[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
	</div>
</xsl:template>


<xsl:template match="ValueType">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x003C;</nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>	
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
		<xsl:apply-templates select="Classes/Class[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="ValueTypes/ValueType[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Interfaces/Interface[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Delegates/Delegate[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Enums/Enum[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Constructors/Contructor[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Properties/Property[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Events/Event[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Fields/Field[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Methods/Method[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
	</div>
</xsl:template>

<xsl:template match="Interface">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x003C;</nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>	
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
		<xsl:apply-templates select="Properties/Property[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Events/Event[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Methods/Method[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
	</div>
</xsl:template>


<xsl:template match="Delegate">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x003C;</nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>	
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
		<xsl:apply-templates select="Constructors/Contructor[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Methods/Method[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
	</div>
</xsl:template>


<xsl:template match="Enum">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x003C;</nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>	
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
		<xsl:apply-templates select="Fields/Field[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
	</div>
</xsl:template>

<xsl:template match="Event">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x007E;</nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>	
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
		<xsl:apply-templates select="Methods/Method[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
	</div>
</xsl:template>

<xsl:template match="Constructor">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x003D;</nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>	
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
		<xsl:apply-templates select="Parameters/Parameter[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
	</div>
</xsl:template>


<xsl:template match="Property">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x0098;</nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>	
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
		<xsl:apply-templates select="Parameters/Parameter[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
		<xsl:apply-templates select="Methods/Method[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
	</div>
</xsl:template>

<xsl:template match="Field">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x00EB;</nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>	
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
	</div>
</xsl:template>

<xsl:template match="Method">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x0061;</nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>	
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
		<xsl:apply-templates select="Parameters/Parameter[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
	</div>
</xsl:template>

<xsl:template match="Parameter">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x00EB;</nobr>	
		Parameter <xsl:value-of select="@Name"></xsl:value-of>	
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
	</div>
</xsl:template>

<xsl:template match="Resources">
	<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x00CC;</nobr>	
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Resource[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>
	</div>
</xsl:template>

<xsl:template match="Resource">
<xsl:param name="rulename"></xsl:param>
	<xsl:variable name="nodeId" select="generate-id()"/>
	<div class="NodeLine">
		<xsl:attribute name="onClick">
			javascript:ViewState('<xsl:value-of select="$rulename"/>
			<xsl:value-of select="$nodeId"/>');
		</xsl:attribute>
		<nobr class="NodeIcon">&#x00CC;</nobr>	
		<xsl:value-of select="@Name"></xsl:value-of>			
		<nobr class="MessageNumber">
			(<xsl:value-of select="count(descendant::Rule[@TypeName=$rulename])"></xsl:value-of>)
		</nobr>
	</div>
	<div class="NodeDiv" style="display: none; position: relative; padding-left: 11;">
		<xsl:attribute name="id">
			<xsl:value-of select="$rulename"/><xsl:value-of select="$nodeId"/>
		</xsl:attribute>
		<xsl:apply-templates select="Messages/Message[descendant::Rule[@TypeName=$rulename]]">
			<xsl:with-param name="rulename"><xsl:value-of select="$rulename"/></xsl:with-param>
		</xsl:apply-templates>		
	</div>
</xsl:template>

<xsl:template match="Description" mode="ruledetails">
	<tr>
		<td class="PropertyName">Rule Description:</td>
		<td class="PropertyContent"><xsl:value-of select="text()" /></td>
	</tr>	
</xsl:template>

<xsl:template match="LongDescription" mode="ruledetails">
	<!-- Test, don't display line if no data present -->
	<xsl:choose>
		<xsl:when test="text()">
			<tr>
				<td class="PropertyName">Long Description:</td>
				<td class="PropertyContent"><xsl:value-of select="text()" /></td>
			</tr>	
		</xsl:when>
	</xsl:choose>
</xsl:template>

<xsl:template match="File" mode="ruledetails">
	<tr>
		<td class="PropertyName">Rule File:</td>
		<td class="PropertyContent"><xsl:value-of select="@Name"/> [<xsl:value-of select="@Version"/>]</td>
	</tr>	
</xsl:template>

<xsl:template match="Rule" mode="ruledetails">
	<xsl:apply-templates select="Description" mode="ruledetails" />
	<xsl:apply-templates select="LongDescription" mode="ruledetails" />
	<xsl:apply-templates select="File" mode="ruledetails" />
</xsl:template>

<!-- End Rule Details -->

</xsl:stylesheet>