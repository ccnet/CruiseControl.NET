<%@ Page language="c#" Codebehind="Default.aspx.cs" Inherits="ThoughtWorks.CruiseControl.WebDashboard.Default" AutoEventWireup="false" %>
<HTML>
	<HEAD>
		<title>CruiseControl.NET</title>
		<meta name="vs_showGrid" content="True">
		<link type="text/css" rel="stylesheet" href="cruisecontrol.css">
	</HEAD>
	<body class="wholepage" topmargin="0" leftmargin="0" marginheight="0" marginwidth="0">
		<!-- head: logo, controls -->
		<table class="main-panel" border="0" align="center" cellpadding="0" cellspacing="0" width="100%">
			<tr>
				<td valign="middle" align="left">
					<img src="images/shim.gif" width="6" border="0"> <a href="http://ccnet.thoughtworks.com">
						<img src="images/ccnet_logo.gif" border="0"></a>
				</td>
			</tr>
		</table>
		<!-- body: main content panels -->
		<table class="TopControls" border="0" width="100%" cellpadding="3" cellspacing="0" height="25">
			<tr>
				<td valign="middle" align="left">
					<div runat="server" id="TopControlsLocation" />
				</td>
			</tr>
		</table>
		<table border="0" align="center" cellpadding="0" cellspacing="0" width="100%" bgcolor="#333399">
			<tr>
				<td valign="top" id="LeftHandSide" bgcolor="#eeeedd" width="180">
					<table id="Plugins" cellpadding="3" cellspacing="0" border="0">
						<tr>
							<td><img src="images/shim.gif" width="180" height="1"></td>
						</tr>
						<tr>
							<td>
								<div runat="server" class="SideBar" id="SideBarLocation" />
							</td>
						</tr>
					</table>
				</td>
				<td width="2">
				<td valign="top" bgcolor="#ffffff">
					<table border="0" align="center" cellpadding="3" cellspacing="0" width="100%">
						<tr>
							<td>
								<form runat="server" ID="Form1">
									<div id="ParentControl" runat="server" />
								</form>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td height="2" colspan="3"></td>
			</tr>
		</table>
		<!-- footer: twlogo -->
		<table width="100%">
			<tr>
				<td align="right">
					<a href="http://www.thoughtworks.com/" border="0"><img src="images/tw_dev_logo.gif" border="0"><img src="images/shim.gif" width="6" border="0">
					</a>
				</td>
			</tr>
		</table>
	</body>
</HTML>
