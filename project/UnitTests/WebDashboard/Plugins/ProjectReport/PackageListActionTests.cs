namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
    using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
    using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

    public class PackageListActionTests
    {
        #region Private fields
        private MockRepository mocks;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }
        #endregion

        #region Tests
        [Test]
        public void ExecuteGeneratesList()
        {
            var package1 = new PackageDetails
            {
                BuildLabel = "label1",
                DateTime = new DateTime(2010, 1, 2, 3, 4, 5),
                FileName = "somewhere\\thefile.type",
                Name = "theName",
                NumberOfFiles = 2,
                Size = 1234
            };
            var package2 = new PackageDetails
            {
                BuildLabel = "label2",
                DateTime = new DateTime(2010, 10, 9, 8, 7, 6),
                FileName = "anotherfile.txt",
                Name = "secondName",
                NumberOfFiles = 5,
                Size = 9876
            };
            var packages = new PackageDetails[] { package1, package2 };
            var projectName = "Test Project";
            var farmService = this.mocks.StrictMock<IFarmService>();
            var viewGenerator = this.mocks.StrictMock<IVelocityViewGenerator>();
            var cruiseRequest = this.mocks.StrictMock<ICruiseRequest>();
            var projectSpec = this.mocks.StrictMock<IProjectSpecifier>();
            SetupResult.For(cruiseRequest.ProjectName).Return(projectName);
            SetupResult.For(cruiseRequest.ProjectSpecifier).Return(projectSpec);
            SetupResult.For(cruiseRequest.RetrieveSessionToken()).Return(null);
            SetupResult.For(farmService.RetrievePackageList(projectSpec, null)).Return(packages);
            Expect.Call(viewGenerator.GenerateView(null, null))
                .Callback<string, Hashtable>((n, ht) =>
                {
                    Assert.AreEqual("PackageList.vm", n);
                    Assert.IsNotNull(ht);
                    Assert.IsTrue(ht.ContainsKey("projectName"));
                    Assert.AreEqual(projectName, ht["projectName"]);
                    Assert.IsTrue(ht.ContainsKey("packages"));
                    Assert.IsInstanceOf<List<PackageListAction.PackageDisplay>>(ht["packages"]);
                    return true;
                })
                .Return(new HtmlFragmentResponse("from nVelocity"));

            this.mocks.ReplayAll();
            var plugin = new PackageListAction(viewGenerator, farmService);
            var response = plugin.Execute(cruiseRequest);

            this.mocks.VerifyAll();
            Assert.IsInstanceOf<HtmlFragmentResponse>(response);
            var actual = response as HtmlFragmentResponse;
            var expected = "from nVelocity";
            Assert.AreEqual(expected, actual.ResponseFragment);
        }
        #endregion
    }
}
