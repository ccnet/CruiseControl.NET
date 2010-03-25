using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// An environment variable.
    /// </summary>
    /// <title>Environment Variable</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;variable name="TestVar" value="TextValue" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("variable")]
    public class EnvironmentVariable
    {
        private string my_value = null;

        /// <summary>
        /// The name of the environment variable.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("name", Required = true)]
        public string name { get; set; }

        /// <summary>
        /// The value of the environment variable.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("value", Required = false)]
        public string value
        {
            get { return (my_value == null) ?string.Empty : my_value; }
            set { my_value = value; }
        }
    }
}
