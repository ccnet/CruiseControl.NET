<%@ Page language="c#" Codebehind="Default.aspx.cs" AutoEventWireup="false" Inherits="ThoughtWorks.CruiseControl.WebDashboard.Default" %>

<HTML>
  <HEAD>
		<TITLE>Project Dashboard</TITLE>
</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<table width="100%">
				<tr>
					<td>
						<P><asp:label id="StatusLabel" Visible="False" runat="server" ForeColor="#4A3C8C" Font-Bold="True"
								Font-Size="Larger"></asp:label></P>
						<asp:datagrid id="StatusGrid" runat="server" BorderColor="#333399" BorderStyle="Ridge" BorderWidth="1px" BackColor="White" CellPadding="3" GridLines="Horizontal" AutoGenerateColumns="False" Width="100%">
<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#9471DE">
</SelectedItemStyle>

<ItemStyle ForeColor="Black" BackColor="Ivory">
</ItemStyle>

<HeaderStyle Font-Size="Larger" Font-Bold="True" ForeColor="Ivory" BackColor="#333399">
</HeaderStyle>

<FooterStyle ForeColor="Black" BackColor="#C6C3C6">
</FooterStyle>

<Columns>
<asp:HyperLinkColumn DataNavigateUrlField="webURL" DataTextField="Name" HeaderText="Project Name">
<ItemStyle HorizontalAlign="Left">
</ItemStyle>
</asp:HyperLinkColumn>
<asp:BoundColumn DataField="BuildStatus" HeaderText="Last Build Status">
<HeaderStyle HorizontalAlign="Center">
</HeaderStyle>

<ItemStyle Font-Bold="True" HorizontalAlign="Center">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="LastBuildDate" HeaderText="Last Build Time">
<HeaderStyle HorizontalAlign="Center">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="LastBuildLabel" HeaderText="Last Build Label">
<HeaderStyle HorizontalAlign="Center">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="Status" HeaderText="CCNet Status">
<HeaderStyle HorizontalAlign="Center">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="Activity" HeaderText="Activity">
<HeaderStyle HorizontalAlign="Center">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center">
</ItemStyle>
</asp:BoundColumn>
<asp:TemplateColumn HeaderText="Force Build">
<HeaderStyle HorizontalAlign="Center">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center">
</ItemStyle>

<ItemTemplate>
<!-- We set the command argument of the button to be the project name so that we know which project to force -->
<asp:Button id=ProjectSpecificForceBuildButton runat="server" Text="Force" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Name") %>' CausesValidation="False" CommandName="forcebuild"></asp:Button>
</ItemTemplate>
</asp:TemplateColumn>
</Columns>

<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6">
</PagerStyle>
						</asp:datagrid>
						<P><asp:label id="ExceptionTitleLabel" runat="server">There were exceptions connecting to the following projects:</asp:label></P>
						<P><asp:datagrid id="ExceptionGrid" runat="server" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px"
								BackColor="White" CellPadding="4">
								<SelectedItemStyle Font-Bold="True" ForeColor="#663399" BackColor="#FFCC66"></SelectedItemStyle>
								<ItemStyle ForeColor="#330099" BackColor="White"></ItemStyle>
								<HeaderStyle Font-Bold="True" ForeColor="#FFFFCC" BackColor="#990000"></HeaderStyle>
								<FooterStyle ForeColor="#330099" BackColor="#FFFFCC"></FooterStyle>
								<PagerStyle HorizontalAlign="Center" ForeColor="#330099" BackColor="#FFFFCC"></PagerStyle>
							</asp:datagrid></P>
						<br>
						<asp:button id="RefreshButton" runat="server" Text="Refresh status"></asp:button>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
