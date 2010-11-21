
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public class ProcessOutputEventArgs : EventArgs
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessOutputEventArgs" /> class.	
        /// </summary>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="data">The data.</param>
        /// <remarks></remarks>
		public ProcessOutputEventArgs(ProcessOutputType outputType, string data)
		{
			this.OutputType = outputType;
			this.Data = data;
		}

        /// <summary>
        /// Gets or sets the type of the output.	
        /// </summary>
        /// <value>The type of the output.</value>
        /// <remarks></remarks>
		public ProcessOutputType OutputType { get; set; }
        /// <summary>
        /// Gets or sets the data.	
        /// </summary>
        /// <value>The data.</value>
        /// <remarks></remarks>
		public string Data { get; set; }
	}
}
