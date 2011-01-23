namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Mercurial
{
	using NUnit.Framework;
	using Exortech.NetReflector;
	using System;
	using ThoughtWorks.CruiseControl.Core;
	using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial;

	/// <summary>
	/// Test fixture for the <see cref="MercurialModification"/> class.
	/// </summary>
	[TestFixture]
	public class MercurialModificationTest
	{
		#region Tests

		[Test]
		public void ShouldConvertToModification()
		{
			var hgMod = new MercurialModification
			{
				ChangeNumber = 100,
				Comment = Guid.NewGuid().ToString(),
				EmailAddress = Guid.NewGuid().ToString(),
				FileName = Guid.NewGuid().ToString(),
				FolderName = Guid.NewGuid().ToString(),
				ModifiedTime = DateTime.UtcNow,
				UserName = Guid.NewGuid().ToString(),
				Version = Guid.NewGuid().ToString()
			};

			var ccnetMod = (Modification) hgMod;

			Assert.That(ccnetMod.ChangeNumber, Is.EqualTo(hgMod.ChangeNumber.ToString()));
			Assert.That(ccnetMod.Comment, Is.EqualTo(hgMod.Comment));
			Assert.That(ccnetMod.EmailAddress, Is.EqualTo(hgMod.EmailAddress));
			Assert.That(ccnetMod.FileName, Is.EqualTo(hgMod.FileName));
			Assert.That(ccnetMod.FolderName, Is.EqualTo(hgMod.FolderName));
			Assert.That(ccnetMod.ModifiedTime, Is.EqualTo(hgMod.ModifiedTime));
			Assert.That(ccnetMod.UserName, Is.EqualTo(hgMod.UserName));
			Assert.That(ccnetMod.Version, Is.EqualTo(hgMod.Version));
		}

		#endregion
	}
}
