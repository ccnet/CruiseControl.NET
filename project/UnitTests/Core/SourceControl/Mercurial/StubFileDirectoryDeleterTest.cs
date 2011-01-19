namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Mercurial
{
	using NUnit.Framework;
	using System;

	/// <summary>
	/// Coverage test for <see cref="StubFileDirectoryDeleter"/>.
	/// </summary>
	[TestFixture]
	public class StubFileDirectoryDeleterTest
	{
		#region Private Members

		private StubFileDirectoryDeleter sd;

		#endregion

		#region SetUp Method

		[SetUp]
		public void SetUp()
		{
			sd = new StubFileDirectoryDeleter();
		}

		#endregion

		#region Tests

		[Test]
		public void StubFileDirectoryDeleterCoverage()
		{
			sd.DeleteIncludingReadOnlyObjects("asdf");
		}

		#endregion
	}
}
