<%@ Page language="c#" Codebehind="AddProject.aspx.cs" Inherits="ThoughtWorks.CruiseControl.WebDashboard.AddProject" AutoEventWireup="false" %>
<HTML>
	<HEAD>
		<TITLE>Add Project</TITLE>
	</HEAD>
	<body>
		<form runat="server">
			<P><STRONG><FONT color="#ff3333">This page is in development and not yet complete!</FONT></STRONG></P>
			<P>
				<asp:Label id="StatusMessageLabel" runat="server" Visible="False"></asp:Label></P>
			<TABLE id="Table1" cellPadding="1" border="0">
				<TR>
					<TD height="16">Server</TD>
					<TD height="16">
						<asp:DropDownList id="ServerDropDown" runat="server" />
					</TD>
				</TR>
				<tr>
					<td>Project Name</td>
					<td><asp:TextBox id="ProjectName" runat="server"></asp:TextBox></td>
				</tr>
				<tr>
					<TD>Source Control</TD>
					<TD>File System</TD>
				</tr>
				<TR>
					<TD></TD>
					<TD>
						<table id="Table2">
							<TR>
								<TD>Repository Root</TD>
								<TD><asp:TextBox id="RepositoryRoot" runat="server"></asp:TextBox></TD>
							</TR>
						</table>
					</TD>
				</TR>
				<TR>
					<TD>Builder
					</TD>
					<TD>Command Line</TD>
				</TR>
				<TR>
					<TD></TD>
					<TD>
						<TABLE id="Table3">
							<TR>
								<TD>Executable</TD>
								<TD>
									<asp:TextBox id="BuilderExecutable" runat="server"></asp:TextBox>
								</TD>
							</TR>
							<TR>
								<TD>Base Directory</TD>
								<TD>
									<asp:TextBox id="BuilderBaseDirectory" runat="server"></asp:TextBox>
								</TD>
							</TR>
							<TR>
								<TD>Build Args</TD>
								<TD>
									<asp:TextBox id="BuilderBuildArgs" runat="server"></asp:TextBox>
								</TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
			<P>
				<asp:Button id="SaveButton" runat="server" Text="Save"></asp:Button>&nbsp;<a href="Default.aspx">Return 
					to Dashboard</a>
			</P>
		</form>
	</body>
</HTML>
