using System;

namespace tw.ccnet.core.builder
{
	public class BuilderException : CruiseControlException
	{
		private IBuilder _runner;

		public BuilderException(IBuilder runner, string message) 
			: base(message) 
		{
			_runner = runner;
		}

		public BuilderException(IBuilder runner, string message, Exception innerException) 
			: base(message, innerException)
		{
			_runner = runner;
		}

		public IBuilder Builder
		{
			get { return _runner; }
		}

		public override string ToString()
		{
			return base.ToString() + _runner.ToString();
		}
	}
}
