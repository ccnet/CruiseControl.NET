namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    using System.IO;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    [ReflectorType("pathFilter")]
    public class PathFilter : IModificationFilter
    {
        private string pathPattern;

        [ReflectorProperty("pattern", Required=true)]
        public string Pattern
        {
            get
            {
                return pathPattern;
            }
            set
            {
                pathPattern = value;
            }
        }

        public bool Accept(Modification modification)
        {
            if (modification.FolderName == null || modification.FileName == null)
            {
                return false;
            }
            string path = Path.Combine(modification.FolderName, modification.FileName);
            return PathUtils.MatchPath(Pattern, path, true);
        }

        public override string ToString()
        {
            return "PathFilter " + Pattern;
        }
    }
}
