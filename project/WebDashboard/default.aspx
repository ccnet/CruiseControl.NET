<!-- 
Note to people reading source code - CruiseControl.NET includes an HttpHandler which handles all .aspx requests. 
This file, default.aspx, should never be processed by 'normal' ASP.NET and is here just as a page to explain configuration problems 
-->
<html>
<head>
<title>CruiseControl.NET</title>
<link type="text/css" rel="stylesheet" href="cruisecontrol.css">
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
</p>

</body>
</html>
