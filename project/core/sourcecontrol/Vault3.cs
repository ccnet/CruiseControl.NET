using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// Integrates with Vault 3.0.0 - 3.1.6
	/// </summary>
	public class Vault3 : ProcessSourceControl
	{
		private static readonly Regex MatchVaultElements = new Regex("<vault>(?:.|\n)*</vault>", RegexOptions.IgnoreCase);

		// This is used to determine if CC.NET actually applied a label.  In the event of a failed integration 
		// (when labels are turned on) the label is removed.  We want to ensure that we only remove a label
		// we actually applied.
		private bool _labelApplied = false;

		protected VaultVersionChecker _shim;

		public Vault3(VaultVersionChecker versionCheckerShim) : base(new VaultHistoryParser())
		{
			_shim = versionCheckerShim;
			this.Timeout = _shim.Timeout;
		}

		public Vault3(VaultVersionChecker versionCheckerShim, IHistoryParser historyParser, ProcessExecutor executor) : base(historyParser, executor)
		{
			this._shim = versionCheckerShim;
			this.Timeout = _shim.Timeout;
		}

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			_labelApplied = false;
			Log.Info(string.Format("Checking for modifications to {0} in Vault Repository \"{1}\" between {2} and {3}", _shim.Folder, _shim.Repository, from.StartTime, to.StartTime));
			ProcessResult result = ExecuteWithRetries(ForHistoryProcessInfo(from, to));
			return ParseModifications(result, from.StartTime, to.StartTime);
		}

		/// <summary>
		/// This is called by IntegrationRunner when the build is complete.  To ensure we're building with the labelled code,
		/// we labelled just before we retrieved the sourece.  So here, we remove that label if the build failed.
		/// </summary>
		/// <param name="result"></param>
		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (! _shim.ApplyLabel) return;

			if (_shim.AutoGetSource)
			{
				if (result.Status != IntegrationStatus.Success)
				{
					// Make sure we only remove the label if we actually applied it.  It's possible that the integration 
					// failed because the label already exists.  In this case, we certainly don't want to remove it.
					if (_labelApplied)
					{
						Log.Info(string.Format(
							"Integration failed.  Removing label \"{0}\" from {1} in repository {2}.", result.Label, _shim.Folder, _shim.Repository));
						Execute(RemoveLabelProcessInfo(result));
					}
					else
						Log.Debug(string.Format(
							"Integration failed, but a label was never successfully applied to {0} in repository {1}, so skipping removal.",
							_shim.Folder, _shim.Repository));
				}
			}
			else
			{
				Log.Info(string.Format("Applying label \"{0}\" to {1} in repository {2}.", result.Label, _shim.Folder, _shim.Repository));
				Execute(LabelProcessInfo(result));
			}
		}

		public override void GetSource(IIntegrationResult result)
		{
			if (!_shim.AutoGetSource) return;

			_labelApplied = false;

			if (StringUtil.IsBlank(_shim.WorkingDirectory) && !(!_shim.ApplyLabel && _shim.UseVaultWorkingDirectory && !_shim.CleanCopy))
			{
				_shim.WorkingDirectory = GetVaultWorkingFolder(result);
				if (StringUtil.IsBlank(_shim.WorkingDirectory))
					throw new VaultException(
						string.Format("Vault user {0} has no working folder set for {1} in repository {2} and no working directory has been specified.",
						              _shim.Username, _shim.Folder, _shim.Repository));
			}

			if (_shim.ApplyLabel)
			{
				Log.Info(string.Format("Applying label \"{0}\" to {1} in repository {2}.", result.Label, _shim.Folder, _shim.Repository));
				Execute(LabelProcessInfo(result));
				_labelApplied = true;
			}

			if (_shim.CleanCopy && !StringUtil.IsBlank(this._shim.WorkingDirectory))
			{
				Log.Debug("Cleaning out source folder: " + result.BaseFromWorkingDirectory(_shim.WorkingDirectory));
				new IoService().EmptyDirectoryIncludingReadOnlyObjects(result.BaseFromWorkingDirectory(_shim.WorkingDirectory));
			}

			Log.Info("Getting source from Vault");
			Execute(GetSourceProcessInfo(result, _shim.ApplyLabel));
		}

		/// <summary
		/// The Vault command line client (vault.exe), at least for
		/// version 2.0.4, is not guaranteed to output valid XML in
		/// that there may be some not XML output surrounding the XML.
		/// This method strips away any non-XML	output surrounding
		/// the <vault>...</vault> elements.
		/// </summary
		/// <param name="output">String containing all vault command-line client output.</param>
		/// <returns>string containing only the XML output from the Vault client.</returns>
		/// <exception cref="CruiseControlException">The <vault> start element or </vault> end element cannot be found.</exception>
		public static string ExtractXmlFromOutput(string output)
		{
			string value = MatchVaultElements.Match(output).Value;
			if (value.Length == 0)
			{
				throw new VaultException(string.Format("The output does not contain the expected <vault> element: {0}", output));
			}
			return value;
		}

		private ProcessInfo GetSourceProcessInfo(IIntegrationResult result, bool getByLabel)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			if (getByLabel)
			{
				builder.AddArgument("getlabel", _shim.Folder);
				builder.AddArgument(result.Label);
				if (_shim.UseVaultWorkingDirectory)
					builder.AddArgument("-labelworkingfolder", result.BaseFromWorkingDirectory(_shim.WorkingDirectory));
				else
					builder.AddArgument("-destpath", result.BaseFromWorkingDirectory(_shim.WorkingDirectory));
			}
			else
			{
				builder.AddArgument("get", _shim.Folder);
				if (_shim.UseVaultWorkingDirectory)
					builder.AppendArgument("-performdeletions removeworkingcopy");
				else
					builder.AddArgument("-destpath", result.BaseFromWorkingDirectory(_shim.WorkingDirectory));
			}

			builder.AddArgument("-merge", "overwrite");
			builder.AppendArgument("-makewritable");
			builder.AddArgument("-setfiletime", _shim.setFileTime);
			AddCommonOptionalArguments(builder);
			return ProcessInfoFor(builder.ToString(), result);
		}

		private ProcessInfo LabelProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddArgument("label", _shim.Folder);
			builder.AddArgument(result.Label);
			AddCommonOptionalArguments(builder);
			return ProcessInfoFor(builder.ToString(), result);
		}

		private ProcessInfo RemoveLabelProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddArgument("deletelabel", _shim.Folder);
			builder.AddArgument(result.Label);
			AddCommonOptionalArguments(builder);
			return ProcessInfoFor(builder.ToString(), result);
		}

		protected ProcessInfo ForHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessInfo info = ProcessInfoFor(BuildHistoryProcessArgs(from.StartTime, to.StartTime), from);
			Log.Debug("Vault History command: " + info.ToString());
			return info;
		}

		protected ProcessInfo ProcessInfoFor(string args, IIntegrationResult result)
		{
			return new ProcessInfo(_shim.Executable, args, result.BaseFromWorkingDirectory(_shim.WorkingDirectory));
		}

		// "history ""{0}"" -excludeactions label -rowlimit 0 -begindate {1:s} -enddate {2:s}
		// rowlimit 0 or -1 means unlimited (default is 1000 if not specified)
		// TODO: might want to make rowlimit configurable?
		private string BuildHistoryProcessArgs(DateTime from, DateTime to)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddArgument("history", _shim.Folder);
			builder.AppendArgument(_shim.HistoryArgs);
			builder.AddArgument("-begindate", from.ToString("s"));
			builder.AddArgument("-enddate", to.ToString("s"));
			AddCommonOptionalArguments(builder);
			return builder.ToString();
		}

		protected void AddCommonOptionalArguments(ProcessArgumentBuilder builder)
		{
			builder.AddArgument("-host", _shim.Host);
			builder.AddArgument("-user", _shim.Username);
			builder.AddArgument("-password", _shim.Password);
			builder.AddArgument("-repository", _shim.Repository);
			builder.AppendIf(_shim.Ssl, "-ssl");

			builder.AddArgument("-proxyserver", _shim.proxyServer);
			builder.AddArgument("-proxyport", _shim.proxyPort);
			builder.AddArgument("-proxyuser", _shim.proxyUser);
			builder.AddArgument("-proxypassword", _shim.proxyPassword);
			builder.AddArgument("-proxydomain", _shim.proxyDomain);

			builder.AppendArgument(_shim.otherVaultArguments);
		}

		/// <summary>
		/// When getting by label, the vault command-line client requires a disk path even if you're retrieving
		/// into a working folder.  This retrieves that working path so we can specify it in the get command.  We also
		/// need to know the working directory before we retrieve source if we're going to clean it out, when cleanCopy is true.
		/// Returns true if a working folder was found and WorkingDirectory was set, false if not.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		protected string GetVaultWorkingFolder(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddArgument("listworkingfolders");
			AddCommonOptionalArguments(builder);

			ProcessInfo processInfo = ProcessInfoFor(builder.ToString(), result);
			ProcessResult processResult = Execute(processInfo);

			// parse list of working folders
			XmlDocument xml = GetVaultResponse(processResult, processInfo);
			XmlNodeList workingFolderNodes = xml.SelectNodes("/vault/listworkingfolders/workingfolder");
			XmlAttribute repositoryFolderAtt;
			XmlAttribute localFolderAtt;
			foreach (XmlNode workingFolderNode in workingFolderNodes)
			{
				repositoryFolderAtt = workingFolderNode.Attributes["reposfolder"];
				localFolderAtt = workingFolderNode.Attributes["localfolder"];
				if (repositoryFolderAtt != null && localFolderAtt != null)
					if (repositoryFolderAtt.InnerText == _shim.Folder)
					{
						return localFolderAtt.InnerText;
					}
			}

			return null;
		}

		private XmlDocument GetVaultResponse(ProcessResult result, ProcessInfo info)
		{
			XmlDocument xml = new XmlDocument();
			try
			{
				xml.LoadXml(ExtractXmlFromOutput(result.StandardOutput));
			}
			catch (XmlException)
			{
				throw new VaultException(string.Format(
					"Unable to parse vault XML output for vault command: [{0}].  Vault Output: [{1}]", info.Arguments, result.StandardOutput));
			}
			return xml;
		}

		/// <summary>
		/// Will execute the provided process and retry according to the configured pollRetryAttempts and pollRetryWait settings.  Use
		/// with caution because we can't precisely catch only certain failures because we're using the command-line client.  Intended
		/// to be used when polling for changes to better handle intermittent network issues or Vault server contention.
		/// </summary>
		/// <param name="processInfo"></param>
		/// <returns></returns>
		protected ProcessResult ExecuteWithRetries(ProcessInfo processInfo)
		{
			ProcessResult result = null;
			for(int i=0; i < _shim.pollRetryAttempts; i++)
			{
				try
				{
					result = Execute(processInfo);
					return result;
				}
				catch(CruiseControlException e)
				{
					if (i+1 == _shim.pollRetryAttempts)
						throw;
					else
					{
						Log.Warning(string.Format("Attempt {0} of {1}: {2}", i+1, _shim.pollRetryAttempts, e.ToString()));
						Log.Debug(string.Format("Sleeping {0} seconds", _shim.pollRetryWait));
						Thread.Sleep(_shim.pollRetryWait * 1000);
					}
				}
			}
			throw new CruiseControlException("This should never happen.  Failed to execute within the loop, there's probably an off-by-one error above.");
		}

		public class VaultException : CruiseControlException
		{
			public VaultException(string message) : base(message)
			{}
		}

	}
}