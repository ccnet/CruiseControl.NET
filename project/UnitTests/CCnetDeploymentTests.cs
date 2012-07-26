using System;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests
{
    /// <summary>
    /// Class containing tests for CCNet deployment issues.
    /// Just to make sure that certaing settings are ok before a package is made
    /// ex.: 
    /// ° check that there are xsl files in the xsl files folder
    /// ° check that there are no default passwords
    /// </summary>
    [TestFixture]
    public class CCnetDeploymentTests
    {

        [Test]
        public void TestForAdminPackageOfWebDashboardIsEmpty()
        {
#if DEBUG
            string configFile = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\..\Webdashboard\dashboard.config");            
#else
            string configFile = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\Project\Webdashboard\dashboard.config");
#endif
            Assert.IsTrue(System.IO.File.Exists(configFile), "Dashboard.config not found at {0}", configFile);

            System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.Load(configFile);

            var adminPluginNode = xdoc.SelectSingleNode("/dashboard/plugins/farmPlugins/administrationPlugin");

            Assert.IsNotNull(adminPluginNode, "Admin package configuration not found in dashboard.config at {0}", configFile);

            var pwd = adminPluginNode.Attributes["password"];

            Assert.IsNotNull(pwd, "password attribute not defined in admin packackage in dashboard.config at {0}", configFile);

            Assert.AreEqual("", pwd.Value, "Password must be empty string, to force users to enter one. No default passwords allowed in distribution");


        }

    }
}
