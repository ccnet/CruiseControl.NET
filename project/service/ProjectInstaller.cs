using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ThoughtWorks.CruiseControl.Service
{
	/// <summary>
	/// Installs CCService as a Windows Service.
	/// </summary>
	[RunInstaller(true)]
	public class ProjectInstaller : Installer
	{
		private ServiceProcessInstaller serviceProcessInstaller;
		private ServiceInstaller serviceInstaller;

		public ProjectInstaller()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.serviceProcessInstaller = new ServiceProcessInstaller();
			this.serviceInstaller = new ServiceInstaller();
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
			this.Installers.AddRange(new Installer[]
				{
					this.serviceProcessInstaller,
					this.serviceInstaller
				});
		}
	}
}