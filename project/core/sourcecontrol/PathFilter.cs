using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// The PathFilter can be used to filter modifications on the basis of their file path.
    /// <code>
    /// ** for any directory matching
    /// *  means zero or more of any characters
    /// ?  means one and only one of any character
    /// </code>
    /// </summary>
    /// <title>PathFilter</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;pathFilter&gt;
    /// &lt;pattern&gt;/Kunigunda/ServiceLocator/Sources/Kunigunda.ServiceLocator/AssemblyInfo.cs&lt;/pattern&gt;
    /// &lt;/pathFilter&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <code>
    /// example             |    Matches     
    /// --------------------+-----------------------------------------------------------------------------------------------
    /// **/sources/**/*.*   | Matches any file in   /working/sources   /working/sources/CVS    /working/build/target/sources
    /// *.*                 | any file that is in the root of the repository.
    /// /theFolder/*.*      | any file that is in theFolder
    /// **/theName.dat      | all file named 'theName.dat' in any folder of the repository
    /// **/*.*              | matches all files in any folder 
    /// **/the*.dat         | matches all files that start with 'the' and have a dat extention in any folder
    /// **/the*Folder/*.*   | matches all files in folders that start with 'the' and end with 'Folder'
    /// </code>
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
        /// <summary>
        /// Accepts the specified modification.	
        /// </summary>
        /// <param name="modification">The modification.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public bool Accept(Modification modification)
		{
			if (modification.FolderName == null || modification.FileName == null)
			{
				return false;
			}
			string path = Path.Combine(modification.FolderName, modification.FileName);
			return PathUtils.MatchPath(Pattern, path, caseSensitive);
		}

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override string ToString()
		{
			return "PathFilter " + Pattern;
		}
	}
}
