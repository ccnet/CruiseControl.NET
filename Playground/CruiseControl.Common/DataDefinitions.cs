namespace CruiseControl.Common
{
    /// <summary>
    /// The data definitions to include in a query.
    /// </summary>
    public enum DataDefinitions
    {
        /// <summary>
        /// Include both definitions.
        /// </summary>
        Both,

        /// <summary>
        /// Include only the input definitions.
        /// </summary>
        InputOnly,

        /// <summary>
        /// Include only the output definitions.
        /// </summary>
        OutputOnly,

        /// <summary>
        /// Do not include any data definitions.
        /// </summary>
        None,
    }
}