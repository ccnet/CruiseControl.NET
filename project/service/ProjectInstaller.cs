using System.Collections;
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
		private const string ServiceNameSwitch = "ServiceName";
		private ServiceProcessInstaller serviceProcessInstaller;
		private ServiceInstaller serviceInstaller;

		public ProjectInstaller()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			serviceProcessInstaller = new ServiceProcessInstaller();
			serviceInstaller = new ServiceInstaller();
			SetServiceName(CCService.DefaultServiceName);

			Installers.AddRange(new Installer[]
				{
					serviceProcessInstaller,
					serviceInstaller
				});
		}

		private void SetServiceName(string serviceName)
		{
			serviceInstaller.DisplayName = serviceName;
			serviceInstaller.ServiceName = serviceName;
		}

		public override string HelpText
		{
			get { return string.Format("Usage: installutil [/u] [/{0}=MyCCService] ccnet.service.exe", ServiceNameSwitch); }
		}

		public override void Install(IDictionary stateSaver)
		{
			if (stateSaver.Contains(ServiceNameSwitch))
			{
				SetServiceName(stateSaver[ServiceNameSwitch].ToString());
			}
			base.Install(stateSaver);
		}
	}
}