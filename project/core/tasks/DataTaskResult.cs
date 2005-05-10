using System;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	public class DataTaskResult : ITaskResult
	{
		private string data;

		public DataTaskResult(string data)
		{
			this.data = data;
		}

		public string Data
		{
			get { return data; }
		}

		public bool Succeeded()
		{
			return true;
		}

		public bool Failed()
		{
			return false;
		}

	}
}