<%@ Page language="c#" Codebehind="Tests.aspx.cs" Inherits="ThoughtWorks.CruiseControl.Web.Tests" AutoEventWireup="false" %>
<%@ Register TagPrefix="CCNet" Namespace="ThoughtWorks.CruiseControl.Web" Assembly="ThoughtWorks.CruiseControl.Web" %>
<HTML>
	<HEAD>
		<TITLE>CruiseControl .NET Test Results</TITLE>
	</HEAD>
	<body>
		<CCNet:PluginLinks id="PluginLinks" runat="server" />
		<div id="BodyArea" runat="server" />
	</body>
</HTML>
