using System;
using System.Collections.Generic;
using Moq;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
    public class ExtensionProtocolStub
        : ITransportExtension
    {
        #region ITransportExtension Members

        private BuildServer configuration;
        private string settings;

        public string DisplayName
        {
            get { throw new NotImplementedException(); }
        }

        public string Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public BuildServer Configuration
        {
            get { return configuration; }
            set { configuration = value; }
        }

        public CCTrayProject[] GetProjectList(ThoughtWorks.CruiseControl.CCTrayLib.Configuration.BuildServer server)
        {
            List<CCTrayProject> projectsList = new List<CCTrayProject>();
            projectsList.Add(new CCTrayProject(server, "Test project #1"));
            return projectsList.ToArray();
        }

        public ICruiseProjectManager RetrieveProjectManager(string projectName)
        {
            var projectManagerMock = new Mock<ICruiseProjectManager>();
            projectManagerMock.SetupGet(manager => manager.ProjectName).Returns(projectName);
            return projectManagerMock.Object as ICruiseProjectManager;
        }

        public ICruiseServerManager RetrieveServerManager()
        {
            var serverMock = new Mock<ICruiseServerManager>();
            serverMock.SetupGet(manager => manager.Configuration).Returns(configuration);
            return serverMock.Object as ICruiseServerManager;
        }

        public bool Configure(System.Windows.Forms.IWin32Window owner)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
