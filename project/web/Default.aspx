<%@ Page language="c#" Codebehind="Default.aspx.cs" Inherits="tw.ccnet.web.Default" AutoEventWireup="false" %>
<%@ Register TagPrefix="CCNet" TagName="Template" Src="Template.ascx" %>
<CCNet:Template runAt="server">
	<content>
		<p id="headerXsl" Runat="server" class="stylesection" />
		<p id="compileXsl" Runat="server" class="stylesection" />
		<p id="javadocXsl" runat="server" class="stylesection" />
		<p id="unittestsXsl" runat="server" class="stylesection" />
		<p id="modificationsXsl" Runat="server" class="stylesection" />
		<p id="distributablesXsl" runat="server" class="stylesection" />
	</content>
</CCNet:Template>
