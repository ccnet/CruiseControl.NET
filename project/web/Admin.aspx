<%@ Page language="c#" Codebehind="Admin.aspx.cs" Inherits="ThoughtWorks.CruiseControl.Web.Admin" AutoEventWireup="false" %>
<!DOCTYPE html PUBLIC "-//W3C//Dtd XHTML 1.0 Transitional//EN" "http://localhost/NUnitAsp/dtd/xhtml1-transitional.dtd">
<HTML>
	<HEAD>
		<title>CruiseControl.Net Home page</title>
		<LINK href="cruisecontrol.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body leftMargin="0" background="images/bg_blue_stripe.gif" topMargin="0" marginwidth="0" marginheight="0">
		<div><IMG height="6" src="images/shim.gif" border="0"></div>
		<!-- head: logo, controls -->
		<table class="main-panel" cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
			<tr>
				<td vAlign="center" align="left"><IMG src="images/shim.gif" width="6" border="0"> <a href="http://ccnet.opensource.thoughtworks.net">
						<IMG src="images/ccnet_logo.gif" border="0"></a>
				</td>
				<td><IMG src="images/shim.gif" width="6" border="0"></td>
			</tr>
		</table>
		<!-- body: main content panels -->
		<table cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
			<tr>
				<td vAlign="top" width="100%">
					<table cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
						<tr>
							<td><IMG src="images/corner_white_ul.gif" border="0"></td>
							<td bgColor="#ffffff"><IMG src="images/shim.gif" border="0"></td>
						</tr>
						<tr>
							<td bgColor="#ffffff"><IMG src="images/shim.gif" border="0"></td>
							<td id="contentCell" vAlign="top" align="left" width="100%" bgColor="#ffffff" runat="server">
								<form id="Form1" action="test.aspx" runat="server">
									<table>
										<tr>
											<td>
												<table>
													<tr>
														<td>
															<asp:Literal id="statusLiteral" runat="server"></asp:Literal></td>
														<td><asp:button id="startServer" runat="server" Text="Start Server"></asp:button></td>
														<td><asp:button id="stopServer" runat="server" Text="Stop Server"></asp:button></td>
														<td><asp:button id="stopNow" runat="server" Text="Stop Now"></asp:button></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td></td>
										</tr>
										<tr>
											<td></td>
										</tr>
									</table>
								</form>
							</td>
						</tr>
						<tr>
							<td><IMG src="images/corner_white_ll.gif" border="0"></td>
							<td bgColor="#ffffff"><IMG src="images/shim.gif" border="0"></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td vAlign="top" width="100%">
					<!-- log details -->
					<!-- footer: twlogo -->
					<table width="100%">
						<tr>
							<td align="right"><a href="http://www.thoughtworks.com/" border="0"><IMG src="images/tw_dev_logo.gif" border="0"><IMG src="images/shim.gif" width="6" border="0">
								</a>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</body>
</HTML>
