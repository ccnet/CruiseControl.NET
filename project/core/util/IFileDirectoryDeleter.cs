namespace ThoughtWorks.CruiseControl.Core.Util
{
	public interface IFileDirectoryDeleter
	{
		/// <summary>
		/// Deletes any file or directory called filename.
		/// If filename is a directory, deletes recursively
		/// All readonly objects encountered are also deleted
		/// </summary>
		/// <param name="filename"></param>
		void DeleteIncludingReadOnlyObjects(string filename);
	}
}
