<!-- 
Note to people reading source code - CruiseControl.NET includes an HttpHandler which handles all .aspx requests. 
This file, default.aspx, should never be processed by 'normal' ASP.NET and is here just as a page to explain configuration problems 
-->
<html>
<head>
<title>CruiseControl.NET</title>
<link type="text/css" rel="stylesheet" href="cruisecontrol.css" />
</head>
<body>
<h1>
CruiseControl.NET Configuration error
</h1>
<p>
Hi. You're seeing this page because there is a configuration problem with your installation of the CruiseControl.NET Dashboard. Make sure your web.config contains the following in the &lt;system.web&gt; section:
<pre>
&lt;httpHandlers&gt;
	&lt;add verb="*" path="*.aspx" type="ThoughtWorks.CruiseControl.WebDashboard.MVC.ASPNET.HttpHandler,ThoughtWorks.CruiseControl.WebDashboard"/&gt;
	&lt;add verb="*" path="*.xml" type="ThoughtWorks.CruiseControl.WebDashboard.MVC.ASPNET.HttpHandler,ThoughtWorks.CruiseControl.WebDashboard"/&gt;
&lt;/httpHandlers&gt;
</pre>

Also make sure you have setup IIS to map .aspx files to aspnet_isapi.dll. This should be setup for you automatically when you install the .NET Framework and run the aspnet_regiis.exe program.
If IIS was installed after the .NET framework, you will need to either run aspnet_regiis.exe or 
open IIS manager, right-click the ccnet website, select properties and then the ASP.NET tab.
Make sure the ASP.NET version has something displayed in the dropdown (eg. .NET 2.0, 2.0.50727) and click OK.
</p>

</body>
</html>