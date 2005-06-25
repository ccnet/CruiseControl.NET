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
			serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

			serviceInstaller = new ServiceInstaller();
			serviceInstaller.StartType = ServiceStartMode.Automatic;
			SetServiceName(CCService.DefaultServiceName);

			Installers.AddRange(new Installer[]
				{
					serviceProcessInstaller,
					serviceInstaller
				});
		}

		public override string HelpText
		{
			get { return string.Format("Usage: installutil [/u] [/{0}=MyCCService] ccnet.service.exe", ServiceNameSwitch); }
		}

		protected override void OnBeforeInstall(IDictionary stateSaver)
		{
			string serviceName = ServiceName(stateSaver);
			stateSaver[ServiceNameSwitch] = serviceName;
			SetServiceName(serviceName);
			base.OnBeforeInstall(stateSaver);
		}

		protected override void OnBeforeUninstall(IDictionary savedState)
		{
			SetServiceName(ServiceName(savedState));
			base.OnBeforeUninstall(savedState);
		}

		private string ServiceName(IDictionary savedState)
		{
			if (Context.Parameters.ContainsKey(ServiceNameSwitch))
			{
				return Context.Parameters[ServiceNameSwitch];
			}
			else if(savedState.Contains(ServiceNameSwitch))
			{
				return savedState[ServiceNameSwitch].ToString();
			}
			return CCService.DefaultServiceName;
		}

		private void SetServiceName(string serviceName)
		{
			serviceInstaller.DisplayName = serviceName;
			serviceInstaller.ServiceName = serviceName;
		}
	}
}