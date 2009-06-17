using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("pathFilter")]
	public class PathFilter : IModificationFilter
	{
		private string pathPattern;
        private bool caseSensitive = true;

		[ReflectorProperty("pattern", Required=true)]
		public string Pattern
		{
			get { return pathPattern; }
			set { pathPattern = value; }
		}

        [ReflectorProperty("caseSensitive", Required = false)]
        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set { caseSensitive = value; } 
        }
		public bool Accept(Modification modification)
		{
			if (modification.FolderName == null || modification.FileName == null)
			{
				return false;
			}
			string path = Path.Combine(modification.FolderName, modification.FileName);
			return PathUtils.MatchPath(Pattern, path, caseSensitive);
		}

		public override string ToString()
		{
			return "PathFilter " + Pattern;
		}
	}
}
