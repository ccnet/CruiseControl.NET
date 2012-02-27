using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Label
{
    /// <summary>
    /// <para>
    /// By default, CCNet uses a plain incrementing build number as a build label. Some source controls (e.g. <link>Perforce Source Control
    /// Block</link>) require you to use a different naming scheme if you want CCNet to apply labels to source control on successful builds.
    /// </para>
    /// <para>
    /// You can do this by specifying your own configuration of the default labeller in your project.
    /// </para>
    /// </summary>
    /// <title>Default Labeller</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;labeller type="defaultlabeller"&gt;
    /// &lt;initialBuildLabel&gt;1&lt;/initialBuildLabel&gt;
    /// &lt;prefix&gt;Foo-1-&lt;/prefix&gt;
    /// &lt;incrementOnFailure&gt;true&lt;/incrementOnFailure&gt;
    /// &lt;labelFormat&gt;00000&lt;/labelFormat&gt;
    /// &lt;/labeller&gt;
    /// </code>
    /// </example>
    [ReflectorType("defaultlabeller")]
    public class DefaultLabeller
        : LabellerBase
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const int INITIAL_LABEL = 1;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLabeller"/> class.
        /// </summary>
        public DefaultLabeller()
        {
            this.LabelPrefix = string.Empty;
            this.LabelPostfix = string.Empty;
            this.InitialBuildLabel = INITIAL_LABEL;
            this.IncrementOnFailed = false;
            this.LabelFormat = "0";
        }
        #endregion

        /// <summary>
        /// Any string to be put in front of all labels.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("prefix", Required = false)]
        public string LabelPrefix { get; set; }

        /// <summary>
        /// Any string to be put at the end of all labels.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("postfix", Required = false)]
        public string LabelPostfix { get; set; }

        /// <summary>
        /// Allows you to set the initial build number.
        /// This will only be used when on the first build of a project, meaning that when you change this value,
        /// you'll have to stop the CCNet service and delete the state file.
        /// </summary>
        /// <version>1.5</version>
        /// <default>1</default>
        [ReflectorProperty("initialBuildLabel", Required = false)]
        public int InitialBuildLabel { get; set; }

        /// <summary>
        /// If true, the label will be incremented even if the build fails. Otherwise it will only be incremented if the build succeeds. 
        /// </summary>
        /// <version>1.1</version>
        /// <default>false</default>
        [ReflectorProperty("incrementOnFailure", Required = false)]
        public bool IncrementOnFailed { get; set; }

        /// <summary>
        /// A format applied to the buildnumber. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>0</default>
        [ReflectorProperty("labelFormat", Required = false)]
        public string LabelFormat { get; set; }

        /// <summary>
        /// Generates the specified integration result.	
        /// </summary>
        /// <param name="integrationResult">The integration result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string Generate(IIntegrationResult integrationResult)
        {
            IntegrationSummary lastIntegration = integrationResult.LastIntegration;
            if (integrationResult == null || lastIntegration.IsInitial())
            {
				return LabelPrefix + InitialBuildLabel.ToString(LabelFormat, CultureInfo.CurrentCulture) + LabelPostfix;
            }
            else if (ShouldIncrementLabel(lastIntegration))
            {
                return LabelPrefix + IncrementLabel(lastIntegration.Label) + LabelPostfix;
            }
            else
            {
                return integrationResult.LastIntegration.Label;
            }
        }

        private bool ShouldIncrementLabel(IntegrationSummary previousResult)
        {
            return previousResult.Status == IntegrationStatus.Success || IncrementOnFailed;
        }

        private string IncrementLabel(string label)
        {
            if (LabelPostfix.Length == 0 && LabelPrefix.Length > 0)
            {
                string numericLabel = Regex.Replace(label, @".*?(\d+$)", "$1");
                int newLabel = int.Parse(numericLabel, CultureInfo.CurrentCulture);
                newLabel++;
                return newLabel.ToString(LabelFormat, CultureInfo.CurrentCulture);

            }

            if (LabelPrefix.Length == 0 && LabelPostfix.Length >= 0)
            {
                string numericLabel = Regex.Replace(label, @"\D*?(\d{1,9}).*", "$1");
                int newLabel = int.Parse(numericLabel, CultureInfo.CurrentCulture);
                newLabel++;
                return newLabel.ToString(LabelFormat, CultureInfo.CurrentCulture);

            }

            // prefix and postfix are present, do some extra checking
            MatchCollection numericParts = Regex.Matches(label, @"\D*?(\d{1,9})\D*");

            if (numericParts.Count == 1)
            {
                int newLabel = int.Parse(numericParts[0].ToString(), CultureInfo.CurrentCulture);
                newLabel++;
                return newLabel.ToString(LabelFormat, CultureInfo.CurrentCulture);
            }


            // multiple numeric parts found, remove pre and postfix from the label
            // and scan for numeric parts
            // if only 1 part found, that should be the label
            // otherwise it is impossible to identify the label

            label = label.Replace(LabelPrefix, string.Empty);
            label = label.Replace(LabelPostfix, string.Empty);

            numericParts = Regex.Matches(label, @"\D*?(\d{1,9})\D*");

            if (numericParts.Count == 1)
            {
                int newLabel = int.Parse(numericParts[0].ToString(), CultureInfo.CurrentCulture);
                newLabel++;
                return newLabel.ToString(LabelFormat, CultureInfo.CurrentCulture);
            }

            throw new CruiseControlException("Unable to determine numeric part in label, pre and postfix may not contain multiple numeric parts");
        }
    }
}
