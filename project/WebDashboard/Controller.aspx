<%@ Page language="c#" Codebehind="Controller.aspx.cs" Inherits="ThoughtWorks.CruiseControl.WebDashboard.Controller" AutoEventWireup="false" %>
<HTML>
	<HEAD>
		<TITLE>Add Project</TITLE>
	</HEAD>
	<body>
		<form runat="server">
			<P><STRONG><FONT color="#ff3333">This page is in development and not yet complete!</FONT></STRONG></P>
			<div id="ParentControl" runat="server" />
			<asp:DataGrid id="DataGrid1" runat="server" AutoGenerateColumns="False">
				<Columns>
					<asp:BoundColumn DataField="Key"></asp:BoundColumn>
					<asp:BoundColumn DataField="Value"></asp:BoundColumn>
				</Columns>
			</asp:DataGrid>
		</form>
	</body>
</HTML>
