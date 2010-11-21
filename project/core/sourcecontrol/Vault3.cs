using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// Integrates with Vault 3.0.0 - 3.1.6
	/// </summary>
	public class Vault3 : ProcessSourceControl
	{
		private static readonly Regex MatchVaultElements = new Regex("<vault>(?:.|\n)*</vault>", RegexOptions.IgnoreCase);
	    private readonly IFileDirectoryDeleter fileDirectoryDeleter = new IoService();

		// This is used to determine if CC.NET actually applied a label.  In the event of a failed integration 
		// (when labels are turned on) the label is removed.  We want to ensure that we only remove a label
		// we actually applied.
		private bool _labelApplied = false;

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		protected VaultVersionChecker _shim;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vault3" /> class.	
        /// </summary>
        /// <param name="versionCheckerShim">The version checker shim.</param>
        /// <remarks></remarks>
		public Vault3(VaultVersionChecker versionCheckerShim) : base(new VaultHistoryParser())
		{
			_shim = versionCheckerShim;
			this.Timeout = _shim.Timeout;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Vault3" /> class.	
        /// </summary>
        /// <param name="versionCheckerShim">The version checker shim.</param>
        /// <param name="historyParser">The history parser.</param>
        /// <param name="executor">The executor.</param>
        /// <remarks></remarks>
		public Vault3(VaultVersionChecker versionCheckerShim, IHistoryParser historyParser, ProcessExecutor executor) : base(historyParser, executor)
		{
			this._shim = versionCheckerShim;
			this.Timeout = _shim.Timeout;
		}

        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			_labelApplied = false;
			Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Checking for modifications to {0} in Vault Repository \"{1}\" between {2} and {3}", _shim.Folder, _shim.Repository, from.StartTime, to.StartTime));
			ProcessResult result = ExecuteWithRetries(ForHistoryProcessInfo(from, to));

            Modification[] modifications = ParseModifications(result, from.StartTime, to.StartTime);
            base.FillIssueUrl(modifications);
            return modifications;
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
							CultureInfo.CurrentCulture, "Integration failed.  Removing label \"{0}\" from {1} in repository {2}.", result.Label, _shim.Folder, _shim.Repository));
						Execute(RemoveLabelProcessInfo(result));
					}
					else
						Log.Debug(string.Format(
							CultureInfo.CurrentCulture, "Integration failed, but a label was never successfully applied to {0} in repository {1}, so skipping removal.",
							_shim.Folder, _shim.Repository));
				}
			}
			else
			{
				Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Applying label \"{0}\" to {1} in repository {2}.", result.Label, _shim.Folder, _shim.Repository));
				Execute(LabelProcessInfo(result));
			}
		}

        /// <summary>
        /// Gets the source.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
		public override void GetSource(IIntegrationResult result)
		{
			if (!_shim.AutoGetSource) return;

			_labelApplied = false;

            if (string.IsNullOrEmpty(_shim.WorkingDirectory) && !(!_shim.ApplyLabel && _shim.UseVaultWorkingDirectory && !_shim.CleanCopy))
			{
				_shim.WorkingDirectory = GetVaultWorkingFolder(result);
                if (string.IsNullOrEmpty(_shim.WorkingDirectory))
					throw new VaultException(
						string.Format(System.Globalization.CultureInfo.CurrentCulture,"Vault user {0} has no working folder set for {1} in repository {2} and no working directory has been specified.",
						              _shim.Username, _shim.Folder, _shim.Repository));
			}

			if (_shim.ApplyLabel)
			{
				Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Applying label \"{0}\" to {1} in repository {2}.", result.Label, _shim.Folder, _shim.Repository));
				Execute(LabelProcessInfo(result));
				_labelApplied = true;
			}

            if (_shim.CleanCopy && !string.IsNullOrEmpty(this._shim.WorkingDirectory))
			{
				Log.Debug("Cleaning out source folder: " + result.BaseFromWorkingDirectory(_shim.WorkingDirectory));
                fileDirectoryDeleter.DeleteIncludingReadOnlyObjects(result.BaseFromWorkingDirectory(_shim.WorkingDirectory));
			}

			Log.Info("Getting source from Vault");
			Execute(GetSourceProcessInfo(result, _shim.ApplyLabel));
		}

		/// <summary>
		/// The Vault command line client (vault.exe), at least for
		/// version 2.0.4, is not guaranteed to output valid XML in
		/// that there may be some not XML output surrounding the XML.
		/// This method strips away any non-XML	output surrounding
		/// the <vault>...</vault> elements.
		/// </summary>
		/// <param name="output">String containing all vault command-line client output.</param>
		/// <returns>string containing only the XML output from the Vault client.</returns>
		/// <exception cref="CruiseControlException">The <vault> start element or </vault> end element cannot be found.</exception>
		public static string ExtractXmlFromOutput(string output)
		{
			string value = MatchVaultElements.Match(output).Value;
			if (value.Length == 0)
			{
				throw new VaultException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"The output does not contain the expected <vault> element: {0}", output));
			}
			return value;
		}

		private ProcessInfo GetSourceProcessInfo(IIntegrationResult result, bool getByLabel)
		{
            var builder = new PrivateArguments();
			if (getByLabel)
			{
				builder.Add("getlabel ", _shim.Folder, true);
				builder.Add(result.Label);
				if (_shim.UseVaultWorkingDirectory)
					builder.Add("-labelworkingfolder ", result.BaseFromWorkingDirectory(_shim.WorkingDirectory), true);
				else
					builder.Add("-destpath ", result.BaseFromWorkingDirectory(_shim.WorkingDirectory), true);
			}
			else
			{
				builder.Add("get ", _shim.Folder, true);
				if (_shim.UseVaultWorkingDirectory)
					builder.Add("-performdeletions removeworkingcopy");
				else
					builder.Add("-destpath ", result.BaseFromWorkingDirectory(_shim.WorkingDirectory), true);
			}

			builder.Add("-merge ", "overwrite");
			builder.Add("-makewritable");
			builder.Add("-setfiletime ", _shim.setFileTime);
			AddCommonOptionalArguments(builder);
			return ProcessInfoFor(builder, result);
		}

		private ProcessInfo LabelProcessInfo(IIntegrationResult result)
		{
            var builder = new PrivateArguments();
			builder.Add("label ", _shim.Folder);
			builder.Add(result.Label);
			AddCommonOptionalArguments(builder);
			return ProcessInfoFor(builder, result);
		}

		private ProcessInfo RemoveLabelProcessInfo(IIntegrationResult result)
		{
            var builder = new PrivateArguments();
			builder.Add("deletelabel ", _shim.Folder);
			builder.Add(result.Label);
			AddCommonOptionalArguments(builder);
			return ProcessInfoFor(builder, result);
		}

        /// <summary>
        /// Fors the history process info.	
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		protected ProcessInfo ForHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessInfo info = ProcessInfoFor(BuildHistoryProcessArgs(from.StartTime, to.StartTime), from);
			Log.Debug("Vault History command: " + info.ToString());
			return info;
		}

        /// <summary>
        /// Processes the info for.	
        /// </summary>
        /// <param name="args">The args.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		protected ProcessInfo ProcessInfoFor(PrivateArguments args, IIntegrationResult result)
		{
			return new ProcessInfo(_shim.Executable, args, result.BaseFromWorkingDirectory(_shim.WorkingDirectory));
		}

		// "history ""{0}"" -excludeactions label,obliterate -rowlimit 0 -begindate {1:s} -enddate {2:s}
		// rowlimit 0 or -1 means unlimited (default is 1000 if not specified)
		// TODO: might want to make rowlimit configurable?
		private PrivateArguments BuildHistoryProcessArgs(DateTime from, DateTime to)
		{
            var builder = new PrivateArguments();
			builder.Add("history ", _shim.Folder);
			builder.Add(_shim.HistoryArgs);
			builder.Add("-begindate ", from.ToString("s", CultureInfo.CurrentCulture));
			builder.Add("-enddate ", to.ToString("s", CultureInfo.CurrentCulture));
			AddCommonOptionalArguments(builder);
            return builder;
		}

        /// <summary>
        /// Adds the common optional arguments.	
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <remarks></remarks>
        protected void AddCommonOptionalArguments(PrivateArguments builder)
		{
			builder.AddIf(!string.IsNullOrEmpty(_shim.Host), "-host ", _shim.Host);
            builder.AddIf(!string.IsNullOrEmpty(_shim.Username), "-user ", _shim.Username);
            builder.AddIf(_shim.Password != null, "-password ", _shim.Password);
            builder.AddIf(!string.IsNullOrEmpty(_shim.Repository), "-repository ", _shim.Repository, true);
			builder.AddIf(_shim.Ssl, "-ssl");

			builder.AddIf(!string.IsNullOrEmpty(_shim.proxyServer), "-proxyserver ", _shim.proxyServer);
            builder.AddIf(!string.IsNullOrEmpty(_shim.proxyPort), "-proxyport ", _shim.proxyPort);
            builder.AddIf(!string.IsNullOrEmpty(_shim.proxyUser), "-proxyuser ", _shim.proxyUser);
            builder.AddIf(!string.IsNullOrEmpty(_shim.proxyPassword), "-proxypassword ", _shim.proxyPassword);
            builder.AddIf(!string.IsNullOrEmpty(_shim.proxyDomain), "-proxydomain ", _shim.proxyDomain);

			builder.Add(_shim.otherVaultArguments);
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
			var builder = new PrivateArguments("listworkingfolders");
			AddCommonOptionalArguments(builder);

			ProcessInfo processInfo = ProcessInfoFor(builder, result);
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
				var message = string.Format(
					CultureInfo.CurrentCulture, "Unable to parse vault XML output for vault command: [{0}].  Vault Output: [{1}]", 
                    info.PublicArguments, 
                    result.StandardOutput);
                throw new VaultException(message);
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
						Log.Warning(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Attempt {0} of {1}: {2}", i+1, _shim.pollRetryAttempts, e.ToString()));
						Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Sleeping {0} seconds", _shim.pollRetryWait));
						Thread.Sleep(_shim.pollRetryWait * 1000);
					}
				}
			}
			throw new CruiseControlException("This should never happen.  Failed to execute within the loop, there's probably an off-by-one error above.");
		}

        /// <summary>
        /// 	
        /// </summary>
		public class VaultException : CruiseControlException
		{
            /// <summary>
            /// Initializes a new instance of the <see cref="VaultException" /> class.	
            /// </summary>
            /// <param name="message">The message.</param>
            /// <remarks></remarks>
			public VaultException(string message) : base(message)
			{}
		}

	}
}