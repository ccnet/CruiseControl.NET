namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
	using System;

	/// <summary>
	/// Represents a modification object of the <see cref="Mercurial"/> soiurce control.
	/// </summary>
	public class MercurialModification
	{
		public string Version;

		public string Type
		{
			get { return "Changeset"; }
		}

		public string FileName;
		public string FolderName;

		public DateTime ModifiedTime;

		public string UserName;
		public int ChangeNumber;
		public string Comment;
		public string EmailAddress;

		public static implicit operator Modification(MercurialModification hg)
		{
			return new Modification
			{
				Version = hg.Version,
				ModifiedTime = hg.ModifiedTime,
				Type = hg.Type,
				FileName = hg.FileName,
				FolderName = hg.FolderName,
				UserName = hg.UserName,
				ChangeNumber = hg.ChangeNumber.ToString(),
				Comment = hg.Comment,
				EmailAddress = hg.EmailAddress
			};
		}
	}
}
