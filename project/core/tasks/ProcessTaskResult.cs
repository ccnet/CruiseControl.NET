using System;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// 	
    /// </summary>
	public class ProcessTaskResult : ITaskResult
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		protected readonly ProcessResult result;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
	    protected bool ignoreStandardOutputOnSuccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessTaskResult" /> class.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public ProcessTaskResult(ProcessResult result)
            : this(result, false)
        {
        }

        /// <summary>
        /// Constructor of ProcessTaskResult
        /// </summary>
        /// <param name="result">Process result data.</param>
        /// <param name="ignoreStandardOutputOnSuccess">Set this to true if you do not want the standard output (stdout) of the process to be merged in the build log; otherwise false.</param>
	    public ProcessTaskResult(ProcessResult result, bool ignoreStandardOutputOnSuccess)
		{
			this.result = result;
	        this.ignoreStandardOutputOnSuccess = ignoreStandardOutputOnSuccess;

			if (this.CheckIfSuccess())
			{
				Log.Info("Task output: " + result.StandardOutput);
				string input = result.StandardError;
				if (!string.IsNullOrEmpty(input)) 
					Log.Info("Task error: " + result.StandardError);
			}
		}

        /// <summary>
        /// Gets the data.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public virtual string Data
		{
			get 
            {
                if (!ignoreStandardOutputOnSuccess || result.Failed)
                {
                    return StringUtil.Join(Environment.NewLine, result.StandardOutput, result.StandardError);
                }
                else
                {
                    return result.StandardError;
                }
            }
		}

        /// <summary>
        /// Writes to.	
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <remarks></remarks>
		public virtual void WriteTo(XmlWriter writer)
		{
			writer.WriteStartElement("task");
			if (result.Failed) 
				writer.WriteAttributeString("failed", true.ToString());

			if (result.TimedOut) 
				writer.WriteAttributeString("timedout", true.ToString());

            if (!ignoreStandardOutputOnSuccess || result.Failed)
                writer.WriteElementString("standardOutput", result.StandardOutput);

			writer.WriteElementString("standardError", result.StandardError);
			writer.WriteEndElement();
		}

        #region Public methods
        #region CheckIfSuccess()
        /// <summary>
        /// Checks whether the result was successful.
        /// </summary>
        /// <returns><c>true</c> if the result was successful, <c>false</c> otherwise.</returns>
        public bool CheckIfSuccess()
        {
            return !result.Failed;
        }
        #endregion
        #endregion
	}
}
