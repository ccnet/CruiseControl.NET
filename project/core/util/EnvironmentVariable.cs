using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// A class to represent an environment variable.
    /// </summary>
    /// <title>Environment Variable</title>
    [ReflectorType("variable")]
    public class EnvironmentVariable
    {
        /// <summary>
        /// The name of the environment variable.
        /// </summary>
        [ReflectorProperty("name", Required = true)]
        public string name;

        /// <summary>
        /// The value of the environment variable.
        /// </summary>
        /// <remarks>
        /// If not set or if set to null, the value is "".
        /// </remarks>
        private string my_value = null;
        [ReflectorProperty("value", Required = false)]
        public string value
        {
            get { return (my_value == null) ?string.Empty : my_value; }
            set { my_value = value; }
        }
    }
}
