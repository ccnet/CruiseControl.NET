<%@ Page language="c#" Codebehind="Decorator.aspx.cs" AutoEventWireup="false" Inherits="ThoughtWorks.CruiseControl.WebDashboard.Decorator" %>
<%@ Register Namespace="SiteMesh.DecoratorControls" TagPrefix="decorator" Assembly="Sitemesh" %>
<!DOCTYPE html PUBLIC "-//W3C//Dtd XHTML 1.0 Transitional//EN" "http://localhost/NUnitAsp/dtd/xhtml1-transitional.dtd">
<HTML>
	<HEAD>
		<title>
			<decorator:title runat="server" defaulttitle="CruiseControl.Net Build Results" ID="Title1" /></title>
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
			<table class="TopControls" border="0" width="100%" cellpadding="3" cellspacing="0"
				height="25">
				<tr>
					<td valign="middle" align="left">
						<div runat="server" id="TopControlsLocation" />
					</td>
				</tr>
			</table>
			<table border="0" align="center" cellpadding="0" cellspacing="0" width="100%" bgcolor="#333399">
				<tr>
					<td valign="top" id="LeftHandSide" bgcolor="#eeeedd" width="180px">
						<table id="Plugins" cellpadding="3" cellspacing="0" border="0">
							<tr>
								<td><img src="images/shim.gif" width="180px" height="1"></td>
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
								<div id="decoratorLocation">
									<decorator:body id="Body1" runat="server"></decorator:body>
								</div>
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
