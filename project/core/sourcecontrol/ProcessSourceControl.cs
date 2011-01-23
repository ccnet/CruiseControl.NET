using System;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Collections.Generic;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// 	
    /// </summary>
	public abstract class ProcessSourceControl 
        : SourceControlBase
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		protected ProcessExecutor executor;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		protected IHistoryParser historyParser;
		private Timeout timeout = Timeout.DefaultTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessSourceControl" /> class.	
        /// </summary>
        /// <param name="historyParser">The history parser.</param>
        /// <remarks></remarks>
	    protected ProcessSourceControl(IHistoryParser historyParser) : this(historyParser, new ProcessExecutor())
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessSourceControl" /> class.	
        /// </summary>
        /// <param name="historyParser">The history parser.</param>
        /// <param name="executor">The executor.</param>
        /// <remarks></remarks>
	    protected ProcessSourceControl(IHistoryParser historyParser, ProcessExecutor executor)
		{
			this.executor = executor;
			this.historyParser = historyParser;
		}

        /// <summary>
        /// Gets the process executor.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		protected ProcessExecutor ProcessExecutor
		{
			get { return executor; }
		}

        /// <summary>
        /// Sets the timeout period for the source control operation. See <link>Timeout Configuration</link> for details. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>10 minutes</default>
		[ReflectorProperty("timeout", typeof (TimeoutSerializerFactory), Required = false)]
		public Timeout Timeout
		{
			get { return timeout; }
			set { timeout = (value == null) ? Timeout.DefaultTimeout : value; }
		}

        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		protected Modification[] GetModifications(ProcessInfo info, DateTime from, DateTime to)
		{
			ProcessResult result = Execute(info);
			return ParseModifications(result, from, to);
		}

        /// <summary>
        /// Executes the specified process info.	
        /// </summary>
        /// <param name="processInfo">The process info.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		protected ProcessResult Execute(ProcessInfo processInfo)
		{
			processInfo.TimeOut = Timeout.Millis;
			ProcessResult result = executor.Execute(processInfo);

			if (result.TimedOut)
			{
				throw new CruiseControlException("Source control operation has timed out.");
			}
			else if (result.Failed)
			{
                var message = string.Format(
                    CultureInfo.CurrentCulture, "Source control operation failed: {0}. Process command: {1} {2}",
                    result.StandardError,
                    processInfo.FileName,
                    processInfo.PublicArguments);
				throw new CruiseControlException(message);
			}
			else if (result.HasErrorOutput)
			{
				Log.Warning(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Source control wrote output to stderr: {0}", result.StandardError));
			}
			return result;
		}

        /// <summary>
        /// Parses the modifications.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		protected Modification[] ParseModifications(ProcessResult result, DateTime from, DateTime to)
		{
			return ParseModifications(new StringReader(result.StandardOutput), from, to);
		}

        /// <summary>
        /// Parses the modifications.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="lastRevision">The last revision.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected Modification[] ParseModifications(ProcessResult result, string lastRevision)
        {
            var mods = ParseModifications(new StringReader(result.StandardOutput), DateTime.MinValue, DateTime.MaxValue);
            if (!string.IsNullOrEmpty(lastRevision))
            {
                var actualModes = new List<Modification>();
                foreach (var mod in mods)
                {
                    if (mod.ChangeNumber != lastRevision) actualModes.Add(mod);
                }
                mods = actualModes.ToArray();
            }
            return mods;
        }

        /// <summary>
        /// Parses the modifications.	
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		protected Modification[] ParseModifications(TextReader reader, DateTime from, DateTime to)
		{
			return historyParser.Parse(reader, from, to);
		}

        /// <summary>
        /// Gets the source.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
		public override void GetSource(IIntegrationResult result)
		{
		}

        /// <summary>
        /// Initializes the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <remarks></remarks>
        public override void Initialize(IProject project)
		{
		}

        /// <summary>
        /// Purges the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <remarks></remarks>
        public override void Purge(IProject project)
		{
		}

        // rw issue
        /// <summary>
        /// Converts the comment (or parts from it) into an url pointing to the issue for this build. See <link>IssueUrlBuilder</link> for 
        /// more details.
        /// </summary>
        /// <version>1.4</version>
        /// <default>None</default>
        [ReflectorProperty("issueUrlBuilder", InstanceTypeKey = "type", Required = false)]
        public IModificationUrlBuilder IssueUrlBuilder { get; set; }

        /// <summary>
        /// Fills the issue URL.	
        /// </summary>
        /// <param name="modifications">The modifications.</param>
        /// <remarks></remarks>
        protected void FillIssueUrl(Modification[] modifications)
        {
            if ((IssueUrlBuilder != null) && (modifications != null))
            {
                IssueUrlBuilder.SetupModification(modifications);
            }
        }
	}
}