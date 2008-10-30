using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("pathFilter")]
	public class PathFilter : IModificationFilter
	{
		private string pathPattern;
        private bool isCaseSensitive = true;


		[ReflectorProperty("pattern", Required=true)]
		public string Pattern
		{
			get { return pathPattern; }
			set { pathPattern = value; }
		}

        [ReflectorProperty("isCaseSensitive", Required = false)]
        public bool IsCaseSensitive
        {
            get { return isCaseSensitive; }
            set { isCaseSensitive = value; } 
        }
		public bool Accept(Modification modification)
		{
			if (modification.FolderName == null || modification.FileName == null)
			{
				return false;
			}
			string path = Path.Combine(modification.FolderName, modification.FileName);
			return PathUtils.MatchPath(Pattern, path, IsCaseSensitive);
		}

		public override string ToString()
		{
			return "PathFilter " + Pattern;
		}
	}
}
