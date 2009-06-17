using System.Text.RegularExpressions;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    [ReflectorType("regexIssueTracker")]
    public class RegExIssueTrackerUrlBuilder : IModificationUrlBuilder
    {
        private string _find;
        private string _replace;

        [ReflectorProperty("find")]
        public string Find
        {
            get { return _find; }
            set { _find = value; }
        }


        [ReflectorProperty("replace")]
        public string Replace
        {
            get { return _replace; }
            set { _replace = value; }
        }

        public void SetupModification(Modification[] modifications)
        {
            foreach (Modification mod in modifications)
            {
                if ((mod != null) && !string.IsNullOrEmpty(mod.Comment) && mod.Comment.Length > 0)
                {
                    if (Regex.IsMatch(mod.Comment, _find))
                    {
                        mod.IssueUrl = Regex.Replace(mod.Comment, _find, _replace);
                    }
                }

            }
        }
    }
}
