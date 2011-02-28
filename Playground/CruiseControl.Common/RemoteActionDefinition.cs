namespace CruiseControl.Common
{
    using System.Runtime.Serialization;
    using System.Xml.Linq;

    /// <summary>
    /// The definition of an action.
    /// </summary>
    [DataContract]
    public class RemoteActionDefinition
    {
        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        /// <value>
        /// The name of the action.
        /// </value>
        [DataMember]
        public string Name { get; set; }
        #endregion

        #region Description
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description of the action.
        /// </value>
        [DataMember]
        public string Description { get; set; }
        #endregion

        #region InputData
        /// <summary>
        /// Gets or sets the input data definition.
        /// </summary>
        /// <value>
        /// The input data definition.
        /// </value>
        [DataMember]
        public string InputData { get; set; }
        #endregion

        #region OutputData
        /// <summary>
        /// Gets or sets the output data definition.
        /// </summary>
        /// <value>
        /// The output data definition.
        /// </value>
        [DataMember]
        public string OutputData { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region ToString()
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var input = ExtractDefinitionName(this.InputData);
            var output = ExtractDefinitionName(this.OutputData);
            var message = "[" + this.Name + "(" + input + ")=>(" + output + ")]";
            return message;
        }
        #endregion
        #endregion

        #region Private methods
        #region ExtractDefinitionName()
        /// <summary>
        /// Extracts the name of the definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>
        /// The name of the definition if found; "?" otherwise.
        /// </returns>
        private static string ExtractDefinitionName(string definition)
        {
            if (string.IsNullOrEmpty(definition))
            {
                return "?";
            }

            var xml = XDocument.Parse(definition);
            return xml.Root.Attribute("name").Value;
        }
        #endregion
        #endregion
    }
}