<%@ Page language="c#" Codebehind="ProjectReport.aspx.cs" Inherits="ThoughtWorks.CruiseControl.WebDashboard.ProjectReport" AutoEventWireup="false" %>
<HTML>
	<HEAD>
		<TITLE>CruiseControl .NET Build Results</TITLE>
	</HEAD>
	<body>
		<table cellpadding="0" cellspacing="0" width="100%">
			<tr>
				<td id="HeaderCell" rowspan="2" bgcolor="#ffffff" runat="server"></td>
				<!-- Context Menu top right -->
				<td align="right" valign="top">
					<table class="main-panel" bgcolor="#000066" cellpadding="0" cellspacing="0">
						<tr>
							<td height="18" width="18" bgcolor="#ffffff"><img src="images/shim.gif" border="0"></td>
							<td height="18" width="18"><img src="images/shim.gif" border="0"></td>
							<td colspan="2"><img src="images/shim.gif" border="0"></td>
						</tr>
						<tr>
							<td height="18" width="18" bgcolor="#ffffff"><img src="images/shim.gif" border="0"></td>
							<td height="18" width="18"><img src="images/shim.gif" border="0"></td>
							<td valign="center">
								<span id="PluginLinks" runat="server"></span>
							</td>
							<td><img src="images/shim.gif" width="6" border="0"></td>
						</tr>
						<tr>
							<td height="18" width="18" bgcolor="#ffffff"><img src="images/shim.gif" border="0"></td>
							<td height="18" width="18"><img src="images/shim.gif" border="0"></td>
							<td colspan="2"><img src="images/shim.gif" border="0"></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<!-- goes underneath panel -->
				<td><img src="images/shim.gif" border="0"></td>
			</tr>
			<tr>
				<td id="DetailsCell" runat="server" colspan="2"></td>
			</tr>
		</table>
	</body>
</HTML>
