namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	public class DataTaskResult : ITaskResult
	{
		private readonly string data;

		public DataTaskResult(string data)
		{
			this.data = data;
		}

		public string Data
		{
			get { return data; }
		}

        #region Public methods
        #region CheckIfSuccess()
        /// <summary>
        /// Checks whether the result was successful.
        /// </summary>
        /// <returns><c>true</c> if the result was successful, <c>false</c> otherwise.</returns>
        public bool CheckIfSuccess()
        {
            return true;
        }
        #endregion
        #endregion
    }
}
