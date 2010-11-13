namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using Exortech.NetReflector;

    /// <summary>
    /// Defines a variable to pass to FinalBuilder.
    /// </summary>
    /// <title>FBVariable</title>
    /// <version>1.3</version>
    /// <example>
    /// <code>
    /// &lt;FBVariable name="variable" value="something" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("FBVariable")]
    public class FBVariable
    {
        private string _name;
        private string _value;

        /// <summary>
        /// The name of the variable.
        /// </summary>
        /// <version>1.3</version>
        /// <default>n/a</default>
        [ReflectorProperty("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The value for the variable.
        /// </summary>
        /// <version>1.3</version>
        /// <default>n/a</default>
        [ReflectorProperty("value")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture,"FB Variable: {0} = {1}", Name, Value);
        }

        public FBVariable(string name, string avalue)
        {
            _name = name;
            _value = avalue;
        }

        public FBVariable() { }

    }
}
