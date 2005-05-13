using System;
using System.Collections;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("filesystem")]
	public class FileSourceControl : ISourceControl
	{
		private string _repositoryRoot = "";
		private bool _ignoreMissingRoot;
		private readonly IFileSystem fileSystem;

		[ReflectorProperty("repositoryRoot")]
		public string RepositoryRoot
		{
			get { return _repositoryRoot; }
			set { _repositoryRoot = value; }
		}

		public FileSourceControl() : this (new SystemIoFileSystem()) { }

		public FileSourceControl(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}

		[ReflectorProperty("ignoreMissingRoot", Required=false)]
		public bool IgnoreMissingRoot
		{
			get { return _ignoreMissingRoot; }
			set { _ignoreMissingRoot = value; }
		}

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = false;
		
		public Modification[] GetModifications(DateTime from, DateTime to)
		{
			ArrayList modifications;
			DirectoryInfo root = new DirectoryInfo(RepositoryRoot);

			modifications = GetMods(root, from , to);

			return (Modification[])modifications.ToArray(typeof(Modification));
		}

		private ArrayList GetMods(DirectoryInfo dir, DateTime from, DateTime to) 
		{
			ArrayList mods = new ArrayList();

			try 
			{
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
					mods.AddRange(GetMods(sub, from, to));
				}
			} 
			catch (DirectoryNotFoundException exc) 
			{
				if (!_ignoreMissingRoot) 
				{
					throw exc;
				}
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

		public void LabelSourceControl(IIntegrationResult result) 
		{
		}

		public void GetSource(IIntegrationResult result)
		{
			if (AutoGetSource)
				fileSystem.Copy(_repositoryRoot, result.WorkingDirectory);
		}

		public void Initialize(IProject project)
		{
		}

		public void Purge(IProject project)
		{
		}
	}
}
