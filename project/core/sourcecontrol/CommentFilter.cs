using System.Text.RegularExpressions;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// A FilteredSourceControl filter that compares modification comments to a specified regular expression.
    /// </summary>
    [ReflectorType("commentFilter")]
    public class CommentFilter : IModificationFilter
    {
        /// <summary>
        /// The regular expression (<see cref="System.Text.RegularExpressions.Regex"/>) to compare against.
        /// </summary>
        [ReflectorProperty("pattern", Required = true)]
        public string Pattern;

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