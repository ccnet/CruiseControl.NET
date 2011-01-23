namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Mercurial
{
	using System;
	using ThoughtWorks.CruiseControl.Core.Util;

	/// <summary>
	/// A stub for IFileDirectoryDeleter to allow for testing the Mercurial source control.
	/// </summary>
	public class StubFileDirectoryDeleter : IFileDirectoryDeleter
	{
		public void DeleteIncludingReadOnlyObjects(string path)
		{
		}
	}
}
