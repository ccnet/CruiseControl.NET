<%@ Page language="c#" Codebehind="Default.aspx.cs" Inherits="ThoughtWorks.CruiseControl.Web.Default" AutoEventWireup="false" %>
<%@ Register TagPrefix="CCNet" Namespace="ThoughtWorks.CruiseControl.Web" Assembly="ThoughtWorks.CruiseControl.Web" %>
<HTML>
	<HEAD>
		<TITLE>CruiseControl .NET Build Results</TITLE>
	</HEAD>
	<body>
		<CCNet:PluginLinks id="PluginLinks" runat="server" />
		<div id="Contents" runat="server"></div>
	</body>
</HTML>
