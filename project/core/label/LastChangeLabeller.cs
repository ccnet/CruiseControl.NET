using System;
using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Label
{
    /// <summary>
    /// Labeller for use with source code control systems that have numbered changes.
    /// This labeller uses the last change number (IIntegrationResult.LastChangeNumber) 
    /// as the build number, with an optional prefix.
    /// </summary>
    /// <remarks>
    /// This code is based on code\label\DefaultLabeller.cs.
    /// </remarks> 
    [ReflectorType("lastChangeLabeller")]
    public class LastChangeLabeller : ILabeller
    {
        /// <summary>
        /// The string to be prepended onto the last change number.
        /// </summary>
        [ReflectorProperty("prefix", Required = false)]
        public string LabelPrefix = string.Empty;

        /// <summary>
        /// Controls whether duplicate subsequent labels are permitted or not. If true, duplicate labels are left
        /// intact. If false, the label will always be suffixed with ".n", where "n" is incremented for each
        /// successive duplication. Defaults to true.
        /// </summary>
        [ReflectorProperty("allowDuplicateSubsequentLabels", Required = false)]
        public bool AllowDuplicateSubsequentLabels = true;



        /// <summary>
        /// Generate a label string from the last change number.
        /// If there is no valid change number (e.g. for a forced build without modifications),
        /// then the last integration label is used.
        /// </summary>
        /// <param name="resultFromThisBuild">IntegrationResult object for the current build</param>
        /// <returns>the new label</returns>
        public virtual string Generate(IIntegrationResult resultFromThisBuild)
        {
            int changeNumber = resultFromThisBuild.LastChangeNumber;
            IntegrationSummary lastIntegration = resultFromThisBuild.LastIntegration;
            string changeLabel;

            Log.Debug(string.Format("Last change number is \"{0}\"", changeNumber));
            if (changeNumber != 0)
                changeLabel = LabelPrefix + changeNumber;
            else if (!lastIntegration.IsInitial() && lastIntegration.Label != null)
                changeLabel = lastIntegration.Label;
            else
                changeLabel = LabelPrefix + "unknown";

            if (!AllowDuplicateSubsequentLabels)
            {
                return IncrementLabel(changeLabel);
            }
            return changeLabel;
        }

        // Nothing seems to use this, but ILabellers are ITasks, and ITasks need a Run().  All the other
        // labellers use this implementation, so we will too.
        public void Run(IIntegrationResult result)
        {
            result.Label = Generate(result);
        }

        private string IncrementLabel(string label)
        {
            int current = 0;
            Match match = Regex.Match(label, @"(.*\d+)\.(\d+)$");
            if (match.Success && match.Groups.Count >= 3)
            {
                current = Int32.Parse(match.Groups[2].Value);
                label = match.Groups[1].Value;
            }
            return String.Format("{0}.{1}", label, current + 1);
        }


    }
}