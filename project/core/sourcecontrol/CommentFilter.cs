using System.Text.RegularExpressions;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// A FilteredSourceControl filter that compares modification comments to a specified regular expression.
    /// </summary>
    /// <title>CommentFilter</title>
    /// <version>1.3</version>
    /// <example>
    /// <code>
    /// &lt;commentFilter&gt;
    /// &lt;pattern&gt;Ignore: .*&lt;/pattern&gt;
    /// &lt;/commentFilter&gt;
    /// </code>
    /// </example>
    [ReflectorType("commentFilter")]
    public class CommentFilter : IModificationFilter
    {
        /// <summary>
        /// This is the pattern used to compare the modification comment against. The pattern is specified according to the rules of the .net
        /// System.Text.RegularExpressions.Regex class. Each CommentFilter contains a single pattern element. 
        /// </summary>
        /// <version>1.3</version>
        /// <default>n/a</default>
        [ReflectorProperty("pattern", Required = true)]
        public string Pattern { get; set; }

        /// <summary>
        /// Does the modification pass the filter?
        /// </summary>
        /// <param name="modification">The modification to check.</param>
        /// <returns>True if the modification's comment matches the pattern, false otherwise.</returns>
        public bool Accept(Modification modification)
        {
            if (modification.Comment == null)
                return false;
            return Regex.IsMatch(modification.Comment, Pattern);
        }

        public override string ToString()
        {
            return "commentFilter " + Pattern;
        }
    }
}