<%@ Page language="c#" Codebehind="FxCop.aspx.cs" Inherits="ThoughtWorks.CruiseControl.Web.FxCop" AutoEventWireup="false" %>
<%@ Register TagPrefix="CCNet" Namespace="ThoughtWorks.CruiseControl.Web" Assembly="ThoughtWorks.CruiseControl.Web" %>
<HTML>
	<HEAD>
		<TITLE>FxCop Results</TITLE>
	</HEAD>
	<body>
		<CCNet:PluginLinks id="PluginLinks" runat="server" />
		<div id="BodyArea" runat="server" />
	</body>
</HTML>
