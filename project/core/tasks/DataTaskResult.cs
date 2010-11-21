namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// 	
    /// </summary>
	public class DataTaskResult : ITaskResult
	{
		private readonly string data;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTaskResult" /> class.	
        /// </summary>
        /// <param name="data">The data.</param>
        /// <remarks></remarks>
		public DataTaskResult(string data)
		{
			this.data = data;
		}

        /// <summary>
        /// Gets the data.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
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
