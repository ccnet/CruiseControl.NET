namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	public class DataTaskResult : ITaskResult
	{
		private string _data;

		public DataTaskResult(string data)
		{
			_data = data;
		}

		public string Data
		{
			get { return _data; }
		}
	}
}