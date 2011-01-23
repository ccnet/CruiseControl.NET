namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IFileDirectoryDeleter
	{
		/// <summary>
        /// Deletes any file or directory called path.
        /// If path is a directory, deletes recursively
		/// All readonly objects encountered are also deleted
		/// </summary>
        /// <param name="path"></param>
        void DeleteIncludingReadOnlyObjects(string path);
	}
}
