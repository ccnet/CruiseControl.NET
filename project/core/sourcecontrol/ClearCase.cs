using System;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// Provides for using ClearCase.
	/// </summary>
	/// <remarks>
	/// Written by Garrett M. Smith (gsmith@thoughtworks.com).
	/// </remarks>
	[ReflectorType("clearCase")]
	public class ClearCase : ProcessSourceControl, ITemporaryLabeller
	{
		private string		_executable		= "cleartool.exe";
		private string		_viewPath;
		private string		_viewName;
		private bool		_useBaseline	= false;
		private bool		_useLabel		= true;
		private string		_tempBaseline;
		private string		_projectVobName;

		private const string _TEMPORARY_BASELINE_PREFIX = "CruiseControl.NETTemporaryBaseline_";
		public const string DATETIME_FORMAT = "dd-MMM-yyyy.HH:mm:ss";

		public ClearCase() : base(new ClearCaseHistoryParser())
		{
		}

		[ReflectorProperty("executable", Required=false)]
		public string Executable 
		{
			get { return _executable; }
			set { _executable = value; }
		}

		[ReflectorProperty("projectVobName", Required=false)]
		public string ProjectVobName
		{
			get { return _projectVobName; }
			set { _projectVobName = value; }
		}

		[ReflectorProperty("useBaseline", Required=false)]
		public bool UseBaseline
		{
			get { return _useBaseline; }
			set { _useBaseline = value; }
		}

		[ReflectorProperty("useLabel", Required=false)]
		public bool UseLabel
		{
			get { return _useLabel; }
			set { _useLabel = value; }
		}

		[ReflectorProperty("viewName", Required=false)]
		public string ViewName 
		{
			get { return _viewName; }
			set { _viewName = value; }
		}

		[ReflectorProperty("viewPath", Required=false)]
		public string ViewPath 
		{
			get { return _viewPath; }
			set { _viewPath = value; }
		}

		internal string TempBaseline
		{
			get { return _tempBaseline; }
			set { _tempBaseline = value; }
		}

		public override Modification[] GetModifications( DateTime from, DateTime to )
		{
			return base.GetModifications( CreateHistoryProcessInfo( from, to ), from, to );
		}

		/// <summary>
		/// Executes the two processes needed to label the source tree in ClearCase.
		/// </summary>
		/// <remarks>
		///	ClearCase needs to execute two processes to label a source tree; most source control systems
		///	take only one.
		/// </remarks>
		/// <param name="label">the label to apply</param>
		/// <param name="result">the timestamp of the label; ignored for this implementation</param>
		public override void LabelSourceControl( string label, IIntegrationResult result ) 
		{
			if ( UseBaseline )
			{
				RenameBaseline( label );
			}
			if ( UseLabel )
			{
				ProcessResult processResult = base.Execute( CreateLabelTypeProcessInfo( label ) );
				Log.Debug( "standard output from label: " + processResult.StandardOutput );
				ExecuteIgnoreNonVobObjects( CreateMakeLabelProcessInfo( label ) );
			}
		}

		public void CreateTemporaryLabel()
		{
			if ( UseBaseline )
			{
				TempBaseline = CreateTemporaryBaselineName();
				ValidateBaselineConfiguration();
				base.Execute( CreateTempBaselineProcessInfo( TempBaseline ) );
			}
		}

		public void DeleteTemporaryLabel()
		{
			if ( UseBaseline )
			{
				ValidateBaselineConfiguration();
				RemoveBaseline();
			}
		}

		internal ProcessInfo CreateTempBaselineProcessInfo( string name )
		{
			string args = string.Format( "mkbl -view {0} -identical {1}", ViewName, name );
			Log.Debug( string.Format( "command line is: {0} {1}", Executable, args) );
			return new ProcessInfo( Executable, args );
		}

		internal string CreateTemporaryBaselineName()
		{
			return _TEMPORARY_BASELINE_PREFIX + DateTime.Now.ToString( "MM-dd-yyyy-HH-mm-ss" );
		}

		// This is a HACK.  ProcessSourceControl.Execute doesn't allow the flexibility ClearCase needs
		// to allow nonzero exit codes and to selectively ignore certian error messages.
		internal void ExecuteIgnoreNonVobObjects( ProcessInfo info )
		{
			info.TimeOut = Timeout;
			ProcessResult result = _executor.Execute( info );

			if (result.TimedOut)
			{
				throw new CruiseControlException("Source control operation has timed out.");
			}
			else if (result.Failed && HasFatalError( result.StandardError ) )
			{
				throw new CruiseControlException(string.Format("Source control operation failed: {0}. Process command: {1} {2}", 
					result.StandardError, info.FileName, info.Arguments));
			}
			else if (result.HasErrorOutput)
			{
				Log.Warning(string.Format("Source control wrote output to stderr: {0}", result.StandardError));
			}
		}

		/// <summary>
		/// Returns true if there is an error indicating the operation did not complete successfully.
		/// </summary>
		/// <remarks>
		/// Currently, a fatal error is any error output line that is not <c>Error: Not a vob object:</c>.
		/// We ignore this error because it occurs any time there is a non-versioned (i.e. compiled .DLL) file
		/// in the viewpath.  But the make label operation completed successfully.
		/// </remarks>
		/// <param name="standardError">the standard error from the process</param>
		/// <returns><c>true</c> if there is a fatal error</returns>
		internal bool HasFatalError( string standardError )
		{
			if ( standardError == null )
			{
				return false;
			}
			StringReader reader = new StringReader( standardError );
			try 
			{
				String line = null;
				while ( ( line = reader.ReadLine() ) != null )
				{
					if ( line.IndexOf( "Error: Not a vob object:" ) == -1 )
					{
						return true;
					}
				}
				return false;
			}
			finally
			{
				reader.Close();
			}
		}

		internal ProcessInfo CreateHistoryProcessInfo( DateTime from, DateTime to )
		{
			string fromDate = from.ToString( DATETIME_FORMAT );
			string args = CreateHistoryArguments( fromDate );
			Log.Debug( string.Format( "cleartool commandline: {0} {1}", Executable, args ) );
			ProcessInfo processInfo = new ProcessInfo( Executable, args );
			return processInfo;
		}

		/// <summary>
		/// Creates a process info object for the process that creates a new label type.
		/// </summary>
		/// <param name="label">the label to apply</param>
		/// <returns>the process execution info</returns>
		internal ProcessInfo CreateLabelTypeProcessInfo( string label )
		{
			string args = " mklbtype -c \"CRUISECONTROL Comment\" " + label;
			Log.Debug( string.Format( "mklbtype: {0} {1}; [working dir: {2}]", Executable, args, ViewPath ) );
			return new ProcessInfo( Executable, args, ViewPath );
		}

		/// <summary>
		/// Creates a process info object for the process that applies a label.
		/// </summary>
		/// <param name="label">the label to apply</param>
		/// <returns>the process execution info</returns>
		internal ProcessInfo CreateMakeLabelProcessInfo( string label )
		{
			string args = String.Format( @" mklabel -recurse {0} {1}", label, ViewPath );
			Log.Debug( string.Format( "mklabel: {0} {1}", Executable, args ) );
			return new ProcessInfo( Executable, args );
		}

		internal ProcessInfo CreateRemoveBaselineProcessInfo()
		{
			string args = string.Format( "rmbl -force {0}@\\{1}", TempBaseline, ProjectVobName );
			Log.Debug( string.Format( "remove baseline: {0} {1}", Executable, args ) );
			return new ProcessInfo( Executable, args );
		}

		internal ProcessInfo CreateRenameBaselineProcessInfo( string name )
		{
			string args = string.Format( "rename baseline:{0}@\\{1} {2}", TempBaseline, ProjectVobName, name  );
			Log.Debug( string.Format( "rename baseline: {0} {1}", Executable, args ) );
			return new ProcessInfo( Executable, args );
		}

		internal void ValidateBaselineName( string name )
		{
			if ( name == null
				|| name.Length == 0
				|| name.IndexOf( " " ) > -1 )
			{
				throw new CruiseControlException( string.Format( "invalid baseline name: \"{0}\" (Does your prefix have a space in it?)", name ) );
			}
		}

		private string CreateHistoryArguments( string fromDate )
		{
			return "lshist  -r  -nco -since " + fromDate + " -fmt \"%u" + ClearCaseHistoryParser.DELIMITER
				+ "%Vd"	+ ClearCaseHistoryParser.DELIMITER + "%En" + ClearCaseHistoryParser.DELIMITER
				+ "%Vn" + ClearCaseHistoryParser.DELIMITER + "%o" + ClearCaseHistoryParser.DELIMITER
				+ "!%l" + ClearCaseHistoryParser.DELIMITER + "!%a" + ClearCaseHistoryParser.DELIMITER
				+ "%Nc" + ClearCaseHistoryParser.END_OF_STRING_DELIMITER + "\\n\" " + _viewPath;
		}

		private void RemoveBaseline()
		{
			base.Execute( CreateRemoveBaselineProcessInfo() );
		}

		private void RenameBaseline( string name )
		{
			ValidateBaselineConfiguration();
			ValidateBaselineName( name );
			base.Execute( CreateRenameBaselineProcessInfo( name ) );
		}

		private void ValidateBaselineConfiguration()
		{
			if ( UseBaseline
				&& ( ProjectVobName == null 
				|| ViewName == null ) )
			{
				throw new CruiseControlException( "you must specify the project VOB and view name if UseBaseLine is true" );
			}
		}	
	}
}
