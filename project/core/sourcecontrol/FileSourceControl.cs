using System;
using System.Collections;
using System.IO;
using Exortech.NetReflector;

namespace tw.ccnet.core.sourcecontrol
{
	[ReflectorType("filesystem")]
	public class FileSourceControl : ISourceControl
	{
		private string _repositoryRoot;

		[ReflectorProperty("repositoryRoot")]
		public string RepositoryRoot
		{
			get { return _repositoryRoot; }
			set { _repositoryRoot = value; }
		}

		public Modification[] GetModifications(DateTime from, DateTime to)
		{
			ArrayList modifications;
			DirectoryInfo root = new DirectoryInfo(RepositoryRoot);

			modifications = getMods(root, from , to);

			return (Modification[])modifications.ToArray(typeof(Modification));
		}

		private ArrayList getMods(DirectoryInfo dir, DateTime from, DateTime to) 
		{
			ArrayList mods = new ArrayList();

			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files) 
			{
				if (IsLocalFileChanged(file, from)) 
				{
					mods.Add(CreateModification(file));
				}
			}

			DirectoryInfo[] subs = dir.GetDirectories();
			foreach (DirectoryInfo sub in subs) 
			{
				mods.AddRange(getMods(sub, from, to));
			}

			return mods;
		}

		private Modification CreateModification(FileInfo info)
		{
			Modification modification = new Modification();
			modification.FileName = info.Name;
			modification.ModifiedTime = info.LastWriteTime;
			modification.FolderName = info.DirectoryName;
			return modification;
		}

		private bool IsLocalFileChanged(FileInfo reposFile, DateTime date)
		{
			return reposFile.LastWriteTime > date;
		}
	}
}
