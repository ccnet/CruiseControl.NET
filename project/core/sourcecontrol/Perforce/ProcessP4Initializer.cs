using System.IO;
using System.Net;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
	/// <summary>
	/// Sets up a Perforce environment by creating a client spec. Uses the P4 command line client to do this.
	/// We require a client name to do this, so if the user hasn't specified one then we create an appropriate
	/// one (see tests)
	/// </summary>
	public class ProcessP4Initializer : IP4Initializer
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public static readonly string ClientPrefix = "CCNet";
		private readonly IP4ProcessInfoCreator processInfoCreator;
		private readonly ProcessExecutor executor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessP4Initializer" /> class.	
        /// </summary>
        /// <param name="executor">The executor.</param>
        /// <param name="processInfoCreator">The process info creator.</param>
        /// <remarks></remarks>
		public ProcessP4Initializer(ProcessExecutor executor, IP4ProcessInfoCreator processInfoCreator)
		{
			this.executor = executor;
			this.processInfoCreator = processInfoCreator;
		}

        /// <summary>
        /// Initializes the specified p4.	
        /// </summary>
        /// <param name="p4">The p4.</param>
        /// <param name="project">The project.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <remarks></remarks>
		public void Initialize(P4 p4, string project, string workingDirectory)
		{
			CheckWorkingDirectoryIsValid(workingDirectory);
			CheckViewIsValid(p4.ViewForSpecifications);
			CreateClientNameIfOneNotSet(p4, project);
			ProcessInfo processInfo = processInfoCreator.CreateProcessInfo(p4, "client -i");
			processInfo.StandardInputContent = CreateClientSpecification(p4, workingDirectory);
			ProcessResult result = executor.Execute(processInfo);
			if (result.ExitCode != ProcessResult.SUCCESSFUL_EXIT_CODE)
			{
				throw new CruiseControlException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Failed to Initialize client (exit code was {0}).\r\nStandard output was: {1}\r\nStandard error was {2}", result.ExitCode, result.StandardOutput, result.StandardError));
			}
		}

		private void CreateClientNameIfOneNotSet(P4 p4, string projectName)
		{
			if (p4.Client == null || p4.Client == string.Empty)
			{
				p4.Client = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}-{1}-{2}", ClientPrefix, Dns.GetHostName(), projectName);
			}
		}

		private void CheckViewIsValid(string[] viewLines)
		{
			foreach (string viewLine in viewLines)
			{
				CheckViewIsValid(viewLine);
			}
		}

		private void CheckViewIsValid(string view)
		{
			if (!  ( (view.StartsWith("//") || view.StartsWith(@"""//")) && (view.EndsWith("/...") || view.EndsWith(@"/...""")) ) )
			{
				throw new CruiseControlException(string.Format(@"[{0}] is not a valid view - it should start with '//' and end with '/...'", view));
			}
		}

		private void CheckWorkingDirectoryIsValid(string directory)
		{
			if (!Path.IsPathRooted(directory))
			{
				throw new CruiseControlException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Working directory [{0}] does not represent an absolute path", directory));
			}
		}

		private string CreateClientSpecification(P4 p4, string workingDirectory)
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture,"Client: {0}\n\nRoot:   {1}\n\nView:\n{2}", p4.Client, workingDirectory, GenerateClientView(p4.ViewForSpecifications, p4.Client));
		}

		private string GenerateClientView(string[] view, string client)
		{
			StringBuilder builder = new StringBuilder();
			foreach (string viewLine in view)
			{
				builder.Append(string.Format(System.Globalization.CultureInfo.CurrentCulture," {0} {1}", viewLine, viewLine.Insert(2, client + "/")));
				builder.Append("\n");
			}
			return builder.ToString();
		}
	}
}
