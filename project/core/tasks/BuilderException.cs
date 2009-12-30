#pragma warning disable 1591
using System;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	public class BuilderException : CruiseControlException
	{
		private readonly ITask _runner;

		public BuilderException(ITask runner, string message) 
			: base(message) 
		{
			_runner = runner;
		}

		public BuilderException(ITask runner, string message, Exception innerException) 
			: base(message, innerException)
		{
			_runner = runner;
		}

		public ITask Builder
		{
			get { return _runner; }
		}

		public override string ToString()
		{
			return base.ToString() + _runner;
		}
	}
}
