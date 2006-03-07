using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Reflection;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol {

	[ReflectorType("vault")]
	public class VaultVersionChecker : ISourceControl
	{
		public const string DefaultExecutable = @"C:\Program Files\SourceGear\Vault Client\vault.exe";
		public const string DefaultHistoryArgs = "-excludeactions label -rowlimit 0";

		public enum EForcedVaultVersion
		{
			None,
			Vault3,
			Vault317
		}

		private Vault3 _vaultSourceControl = null;
		private EForcedVaultVersion _forcedVaultVersion = EForcedVaultVersion.None;

		[ReflectorProperty("username", Required=false)]
		public string Username;

		[ReflectorProperty("password", Required=false)]
		public string Password;

		[ReflectorProperty("host", Required=false)]
		public string Host;

		[ReflectorProperty("repository", Required=false)]
		public string Repository;

		[ReflectorProperty("folder", Required=false)]
		public string Folder = "$";

		[ReflectorProperty("executable", Required=false)]
		public string Executable = DefaultExecutable;

		[ReflectorProperty("ssl", Required=false)]
		public bool Ssl = false;

		[ReflectorProperty("autoGetSource", Required=false)]
		public bool AutoGetSource = false;

		[ReflectorProperty("applyLabel", Required=false)]
		public bool ApplyLabel = false;

		[ReflectorProperty("historyArgs", Required=false)]
		public string HistoryArgs = DefaultHistoryArgs;

		[ReflectorProperty("useWorkingDirectory", Required=false)]
		public bool UseVaultWorkingDirectory = true;

		[ReflectorProperty("workingDirectory", Required=false)]
		public string WorkingDirectory;

		// A more sensible default here would be "current" IMHO, but "checkin" was hard-coded before the property was added, 
		// so I'm leaving that as the default so the behavior doesn't change on people when they upgrade.
		[ReflectorProperty("setFileTime", Required=false)]
		public string setFileTime = "checkin"; 

		[ReflectorProperty("cleanCopy", Required = false)]
		public bool CleanCopy = false;

		[ReflectorProperty("proxyServer", Required=false)]
		public string proxyServer;

		[ReflectorProperty("proxyPort", Required=false)]
		public string proxyPort;

		[ReflectorProperty("proxyUser", Required=false)]
		public string proxyUser;

		[ReflectorProperty("proxyPassword", Required=false)]
		public string proxyPassword;

		[ReflectorProperty("proxyDomain", Required=false)]
		public string proxyDomain;

		// Anything provided here will be included in every vault command
		[ReflectorProperty("otherVaultArguments", Required=false)]
		public string otherVaultArguments;

		[ReflectorProperty("pollRetryWait", Required=false)]
		public int pollRetryWait = 5;

		[ReflectorProperty("pollRetryAttempts", Required=false)]
		public int pollRetryAttempts = 3;

		public VaultVersionChecker() {}

		public VaultVersionChecker(IHistoryParser historyParser, ProcessExecutor executor, EForcedVaultVersion forceVersion)
		{
			_forcedVaultVersion = forceVersion;
			switch ( forceVersion )
			{
				case EForcedVaultVersion.Vault3:
					_vaultSourceControl = new Vault3(this, historyParser, executor);
					break;
				case EForcedVaultVersion.Vault317:
					_vaultSourceControl = new Vault317(this, historyParser, executor);
					break;
				default:
					Debug.Fail("You have to force a version of Vault from the unit tests.");
					break;
			}
		}

		public void Initialize(IProject project)
		{
			GetCorrectVaultInstance();
			_vaultSourceControl.Initialize(project);
		}
		
		public Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			GetCorrectVaultInstance();
			return _vaultSourceControl.GetModifications(from, to);
		}

		public void LabelSourceControl(IIntegrationResult result)
		{
			GetCorrectVaultInstance();
			_vaultSourceControl.LabelSourceControl(result);
		}

		public void GetSource(IIntegrationResult result)
		{
			GetCorrectVaultInstance();
			_vaultSourceControl.GetSource(result);
		}

		public void Purge(IProject project)
		{
			GetCorrectVaultInstance();
			_vaultSourceControl.Purge(project);
		}

		private void GetCorrectVaultInstance()
		{
			if ( _vaultSourceControl != null )
				return;

			if ( VaultVersionIs317OrBetter() )
			{
				Log.Debug("Vault CLC is at least version 3.1.7.");
				_vaultSourceControl = new Vault317(this);
			}
			else
			{
				Log.Debug("Vault CLC is older than version 3.1.7.");
				_vaultSourceControl = new Vault3(this);
			}
		}

		private bool VaultVersionIs317OrBetter()
		{
			switch ( _forcedVaultVersion )
			{
				case EForcedVaultVersion.Vault3:
					Log.Debug("Vault version 3 forced.");
					return false;
				case EForcedVaultVersion.Vault317:
					Log.Debug("Vault version 3.1.7 forced");
					return true;
			}

			Assembly vaultExe = Assembly.LoadFile(Executable);
			AssemblyName vaultExeName = vaultExe.GetName();

			Log.Debug(Executable + " is version " + vaultExeName.Version.ToString());

			// Return true if Vault Command-Line Client is version 3.1.7 or higher, otherwise return false.
			if ( vaultExeName.Version.Major > 3 )
				return true;
			if ( vaultExeName.Version.Major == 3 )
			{
				if ( vaultExeName.Version.Minor > 1 )
					return true;
				if ( vaultExeName.Version.Minor == 1 && vaultExeName.Version.Build >= 7 )
					return true;
			}
			return false;
		}
		
	}
}
