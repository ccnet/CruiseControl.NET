<%@ Page language="c#" Codebehind="BasicTest.aspx.cs" AutoEventWireup="false" Inherits="ThoughtWorks.CruiseControl.XMLRPCWebService.XMLRPCWebServiceBasicTest" %>
<HTML>
	<HEAD>
		<TITLE>Project Dashboard</TITLE>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<P>
				<asp:Label id="Label1" runat="server"></asp:Label></P>
			<asp:DataGrid id="DataGrid1" runat="server" BorderColor="White" BorderStyle="Ridge" CellSpacing="1"
				BorderWidth="2px" BackColor="White" CellPadding="3" GridLines="None" ShowHeader="False">
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#9471DE"></SelectedItemStyle>
				<ItemStyle ForeColor="Black" BackColor="#DEDFDE"></ItemStyle>
				<HeaderStyle Font-Bold="True" ForeColor="#E7E7FF" BackColor="#4A3C8C"></HeaderStyle>
				<FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
				<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"></PagerStyle>
			</asp:DataGrid></form>
	</body>
</HTML>
