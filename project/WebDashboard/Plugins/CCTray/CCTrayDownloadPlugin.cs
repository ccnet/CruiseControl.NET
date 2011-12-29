using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.CCTray
{
    /// <summary>
    /// This plugins foresees to download CCtray. 
    /// CCtray installation files must be located in the <b>cctray</b> subfolder where the web.config resides.
    /// <para>
    /// LinkDescription : Download CCTray.
    /// </para>
    /// </summary>
    /// <title>CCTray Download Plugin</title>
    /// <version>1.4.3</version>
	[ReflectorType("cctrayDownloadPlugin")]
	public class CCTrayDownloadPlugin : IPlugin
	{
		private readonly IActionInstantiator actionInstantiator;

		public CCTrayDownloadPlugin(IActionInstantiator actionInstantiator)
		{
			this.actionInstantiator = actionInstantiator;
		}

		public string LinkDescription
		{
			get { return "Download CCTray"; }
		}

		public INamedAction[] NamedActions
		{
			get
			{
				return new INamedAction[]
					{
						new ImmutableNamedAction(CCTrayDownloadAction.ActionName, actionInstantiator.InstantiateAction(typeof (CCTrayDownloadAction)))
					};
			}
		}
	}
}