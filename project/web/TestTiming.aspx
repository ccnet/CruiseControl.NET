<%@ Page language="c#" Codebehind="TestTiming.aspx.cs" AutoEventWireup="false" Inherits="ThoughtWorks.CruiseControl.Web.TestTiming" %>
<%@ Register TagPrefix="CCNet" Namespace="ThoughtWorks.CruiseControl.Web" Assembly="ThoughtWorks.CruiseControl.Web" %>
<HTML>
	<HEAD>
		<TITLE>CruiseControl .NET Test Timings</TITLE>
	</HEAD>
	<body>
		<CCNet:PluginLinks id="PluginLinks" runat="server" />
		<div id="BodyArea" runat="server" />
	</body>
</HTML>
