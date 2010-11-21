using System;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// 	
    /// </summary>
	public class BuilderException : CruiseControlException
	{
		private readonly ITask _runner;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderException" /> class.	
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
		public BuilderException(ITask runner, string message) 
			: base(message) 
		{
			_runner = runner;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderException" /> class.	
        /// </summary>
        /// <param name="runner">The runner.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <remarks></remarks>
		public BuilderException(ITask runner, string message, Exception innerException) 
			: base(message, innerException)
		{
			_runner = runner;
		}

        /// <summary>
        /// Gets the builder.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public ITask Builder
		{
			get { return _runner; }
		}

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override string ToString()
		{
			return base.ToString() + _runner;
		}
	}
}
