<%@ Page language="c#" Codebehind="Default.aspx.cs" AutoEventWireup="false" Inherits="ThoughtWorks.CruiseControl.WebDashboard.Default" %>
<HTML>
	<HEAD>
		<TITLE>Project Dashboard</TITLE>
	</HEAD>
	<h1>Project Dashboard</h1>
	<P>
		<asp:Label id="StatusLabel" runat="server" Visible="False"></asp:Label></P>
	<asp:datagrid id="StatusGrid" Width="784px" AutoGenerateColumns="False" GridLines="None" CellPadding="3"
		BackColor="White" BorderWidth="2px" CellSpacing="1" BorderStyle="Ridge" BorderColor="White"
		runat="server">
		<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#9471DE"></SelectedItemStyle>
		<ItemStyle ForeColor="Black" BackColor="#DEDFDE"></ItemStyle>
		<HeaderStyle Font-Size="Larger" Font-Bold="True" ForeColor="#E7E7FF" BackColor="#4A3C8C"></HeaderStyle>
		<FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
		<Columns>
			<asp:HyperLinkColumn DataNavigateUrlField="webURL" DataTextField="Name" HeaderText="Project Name"></asp:HyperLinkColumn>
			<asp:BoundColumn DataField="BuildStatus" HeaderText="Last Build Status">
				<ItemStyle Font-Bold="True"></ItemStyle>
			</asp:BoundColumn>
			<asp:BoundColumn DataField="LastBuildDate" HeaderText="Last Build Time" />
			<asp:BoundColumn DataField="LastBuildLabel" HeaderText="Last Build Label" />
			<asp:BoundColumn DataField="Status" HeaderText="CCNet Status" />
			<asp:BoundColumn DataField="Activity" HeaderText="Activity" />
			<asp:HyperLinkColumn Text="Force" DataNavigateUrlField="Name" DataNavigateUrlFormatString="?project={0}" HeaderText="Force Build" />
		</Columns>
		<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"></PagerStyle>
	</asp:datagrid>
	<P><asp:label id="ExceptionTitleLabel" runat="server">There were exceptions connecting to the following projects:</asp:label></P>
	<P><asp:datagrid id="ExceptionGrid" CellPadding="4" BackColor="White" BorderWidth="1px" BorderStyle="None"
			BorderColor="#CC9966" runat="server">
			<SelectedItemStyle Font-Bold="True" ForeColor="#663399" BackColor="#FFCC66"></SelectedItemStyle>
			<ItemStyle ForeColor="#330099" BackColor="White"></ItemStyle>
			<HeaderStyle Font-Bold="True" ForeColor="#FFFFCC" BackColor="#990000"></HeaderStyle>
			<FooterStyle ForeColor="#330099" BackColor="#FFFFCC"></FooterStyle>
			<PagerStyle HorizontalAlign="Center" ForeColor="#330099" BackColor="#FFFFCC"></PagerStyle>
		</asp:datagrid></P>
</HTML>
