<%@ Page language="c#" Codebehind="Default.aspx.cs" AutoEventWireup="false" Inherits="ThoughtWorks.CruiseControl.WebDashboard.Default" %>
<HTML>
	<HEAD>
		<TITLE>Project Dashboard</TITLE>
	</HEAD>
	<body>
		<h1>Project Dashboard</h1>
		<P>
			<asp:Label id="lblBuilding" runat="server" ForeColor="Red" Font-Bold="True" Font-Size="Small"
				Visible="False">Label</asp:Label></P>
		<asp:datagrid id="StatusGrid" runat="server" BorderColor="White" BorderStyle="Ridge" CellSpacing="1"
			BorderWidth="2px" BackColor="White" CellPadding="3" GridLines="None" AutoGenerateColumns="False"
			Width="784px">
			<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#9471DE"></SelectedItemStyle>
			<ItemStyle ForeColor="Black" BackColor="#DEDFDE"></ItemStyle>
			<HeaderStyle Font-Size="Larger" Font-Bold="True" ForeColor="#E7E7FF" BackColor="#4A3C8C"></HeaderStyle>
			<FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
			<Columns>
				<asp:HyperLinkColumn DataNavigateUrlField="webURL" DataTextField="Name" HeaderText="Project Name"></asp:HyperLinkColumn>
				<asp:BoundColumn DataField="BuildStatus" HeaderText="Last Build Status">
					<ItemStyle Font-Bold="True"></ItemStyle>
				</asp:BoundColumn>
				<asp:BoundColumn DataField="LastBuildDate" HeaderText="Last Build Time"></asp:BoundColumn>
				<asp:BoundColumn DataField="LastBuildLabel" HeaderText="Last Build Label"></asp:BoundColumn>
				<asp:BoundColumn DataField="Activity" HeaderText="Activity"></asp:BoundColumn>
			</Columns>
			<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"></PagerStyle>
		</asp:datagrid>
		<P>
			<asp:Label id="ExceptionTitleLabel" runat="server">There were exceptions connecting to the following projects:</asp:Label></P>
		<P>
			<asp:DataGrid id="ExceptionGrid" runat="server" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px"
				BackColor="White" CellPadding="4">
				<SelectedItemStyle Font-Bold="True" ForeColor="#663399" BackColor="#FFCC66"></SelectedItemStyle>
				<ItemStyle ForeColor="#330099" BackColor="White"></ItemStyle>
				<HeaderStyle Font-Bold="True" ForeColor="#FFFFCC" BackColor="#990000"></HeaderStyle>
				<FooterStyle ForeColor="#330099" BackColor="#FFFFCC"></FooterStyle>
				<PagerStyle HorizontalAlign="Center" ForeColor="#330099" BackColor="#FFFFCC"></PagerStyle>
			</asp:DataGrid></P>
		<P><STRONG><FONT size="4">Force build:</FONT></STRONG></P>
		<form runat="server">
			<P>
				<asp:DropDownList id="lstProject" Width="144px" runat="server"></asp:DropDownList>
				<asp:Button id="cmdBuild" runat="server" Text="Build"></asp:Button></P>
		</form>
	</body>
</HTML>
