using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

namespace ThoughtWorks.CruiseControl.Service
{
	/// <summary>
	/// Summary description for ProjectInstaller.
	/// </summary>
	[RunInstaller(true)]
	public class ProjectInstaller : System.Configuration.Install.Installer
	{
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;

		public ProjectInstaller()
		{
			InitializeComponent();
        }

		#region Component Designer generated code
		private void InitializeComponent()
		{
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // serviceInstaller
            // 
            this.serviceInstaller.DisplayName = "CCService";
            this.serviceInstaller.ServiceName = "CCService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
                                                                                      this.serviceProcessInstaller,
                                                                                      this.serviceInstaller});
        }
		#endregion
	}
}
