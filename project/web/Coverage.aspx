<%@ Page language="c#" Codebehind="Coverage.aspx.cs" Inherits="ThoughtWorks.CruiseControl.Web.Coverage" AutoEventWireup="false" %>
<%@ Register TagPrefix="CCNet" Namespace="ThoughtWorks.CruiseControl.Web" Assembly="ThoughtWorks.CruiseControl.Web" %>
<HTML>
	<HEAD>
		<TITLE>Coverage Results</TITLE>
	</HEAD>
	<body>
		<CCNet:PluginLinks id="PluginLinks" runat="server" />
		<div id="BodyArea" runat="server" />
	</body>
</HTML>
