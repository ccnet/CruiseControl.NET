using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// The PathFilter can be used to filter modifications on the basis of their file path.
    /// </summary>
    /// <title>PathFilter</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;pathFilter&gt;
    /// &lt;pattern&gt;$/Kunigunda/ServiceLocator/Sources/Kunigunda.ServiceLocator/AssemblyInfo.cs&lt;/pattern&gt;
    /// &lt;/pathFilter&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <include>PathFilter Wildcards</include>
    /// </remarks>
	[ReflectorType("pathFilter")]
	public class PathFilter : IModificationFilter
	{
		private string pathPattern;
        private bool caseSensitive = true;

        /// <summary>
        /// This is the pattern used to compare the modification path against. The pattern should match the path of the files in the
        /// repository (not the path of the files in the working directory). See below for examples of the syntax for this element. Each
        /// PathFilter contains a single pattern element.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
		[ReflectorProperty("pattern", Required=true)]
		public string Pattern
		{
			get { return pathPattern; }
			set { pathPattern = value; }
		}

        /// <summary>
        /// Sets casesensitive searching on or off. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
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
