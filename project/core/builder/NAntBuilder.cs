using System;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using tw.ccnet.core.util;
using tw.ccnet.core.builder;
using tw.ccnet.remote;
using Exortech.NetReflector;

namespace tw.ccnet.core.builder
{
	[ReflectorType("nant")]
	public class NAntBuilder : IBuilder
	{
		private readonly string DEFAULT_BUILDARGS = "-logger:" + ConfigurationSettings.AppSettings["NAnt.Logger"];

		private string _executable;
		private string _baseDirectory;
		private string _buildArgs;
		private string _buildfile;
		private int _buildTimeout = 0;
		private string[] _targets;

		public NAntBuilder() 
		{
			_buildArgs = DEFAULT_BUILDARGS;
		}

		[ReflectorProperty("executable")]
		public string Executable
		{
			get { return _executable; }
			set { _executable = value; }
		}		

		[ReflectorProperty("baseDirectory")]
		public string BaseDirectory
		{
			get { return _baseDirectory; }
			set { _baseDirectory = value; }
		}

		//TODO: can this be optional?
		[ReflectorProperty("buildFile")]
		public string BuildFile
		{
			get { return _buildfile; }
			set { _buildfile = value; }
		}

		[ReflectorProperty("buildArgs", Required=false)]
		public string BuildArgs
		{
			get { return _buildArgs; }
			set { _buildArgs = value; }
		}

		[ReflectorArray("targetList", Required=false)]
		public string[] Targets
		{
			get 
			{ 
				if (_targets == null)
				{
					_targets = new string[0];
				}
				return _targets; 
			}
			set { _targets = value; }
		}

		[ReflectorProperty("buildTimeout", Required=false)]
		public int BuildTimeout 
		{
			get { return _buildTimeout; }
			set { _buildTimeout = value; }
		}

		public string LabelToApply = "NO-LABEL";
		
		public bool ShouldRun(IntegrationResult result)
		{
			return result.Working && result.Modifications.Length > 0;
		}

		public void Run(IntegrationResult result)
		{
			if (result.Label != null && result.Label != "")
			{
				LabelToApply = result.Label;
			}
			try
			{			
				int exitCode = AttemptExecute(result);
				result.Status = (exitCode == 0) ? IntegrationStatus.Success : IntegrationStatus.Failure;
			}
			catch (Exception e) 
			{
				throw new BuilderException(this, string.Format("Unable to execute: {0}\n{1}", BuildCommand, e), e);
			}
		}

		private string BuildCommand
		{
			get { return string.Format("{0} {1}", Executable, BuildArgs); }
		}
		
		internal string CreateArgs()
		{
			return string.Format("-buildfile:{0} {1} -D:label-to-apply={2} {3}",
				BuildFile, BuildArgs, LabelToApply, String.Join(" ", Targets));
		}

		protected virtual int AttemptExecute(IntegrationResult result)
		{
			Process process = ProcessUtil.CreateProcess(Executable, CreateArgs(), BaseDirectory);
			try
			{
				TextReader stdOut = ProcessUtil.ExecuteRedirected(process);
				result.Output = stdOut.ReadToEnd();
				if (BuildTimeout > 0)
				{
					process.WaitForExit(BuildTimeout);
				}
				else
				{
					process.WaitForExit();
				}
				return process.ExitCode;
			}
			finally
			{
				process.Close();
			}
		}

		public override string ToString()
		{
			return string.Format(@" BaseDirectory: {0}, Targets: {1}, Executable: {2}, BuildFile: {3}", 
				BaseDirectory, String.Join(", ", Targets), Executable, BuildFile);
		}
	}	
}