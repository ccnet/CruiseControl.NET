using System;
using System.Collections;
using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("filesystem")]
	public class FileSourceControl : ISourceControl
	{
		private string _repositoryRoot;
		private bool _ignoreMissingRoot;

		[ReflectorProperty("repositoryRoot")]
		public string RepositoryRoot
		{
			get { return _repositoryRoot; }
			set { _repositoryRoot = value; }
		}

		[ReflectorProperty("ignoreMissingRoot", Required=false)]
		public bool IgnoreMissingRoot
		{
			get { return _ignoreMissingRoot; }
			set { _ignoreMissingRoot = value; }
		}

		public Modification[] GetModifications(DateTime from, DateTime to)
		{
			ArrayList modifications;
			DirectoryInfo root = new DirectoryInfo(RepositoryRoot);

			modifications = GetMods(root, from , to);

			return (Modification[])modifications.ToArray(typeof(Modification));
		}

		public bool ShouldRun(IIntegrationResult result)
		{
			return result.Working;
		}

		public void Run(IIntegrationResult result)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
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

		public void LabelSourceControl(string label, DateTime timeStamp) 
		{
		}

		public void GetSource(IIntegrationResult result)
		{
			
		}

		public void Initialize(IProject project)
		{
		}

		public void Purge(IProject project)
		{
		}
	}
}
