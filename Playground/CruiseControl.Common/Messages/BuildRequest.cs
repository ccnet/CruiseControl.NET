namespace CruiseControl.Common.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// A request for a build.
    /// </summary>
    public class BuildRequest
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildRequest"/> class.
        /// </summary>
        public BuildRequest()
        {
            this.Values = new List<BuildRequestValue>();
        }
        #endregion

        #region Public properties
        #region Values
        /// <summary>
        /// Gets the values.
        /// </summary>
        public IList<BuildRequestValue> Values { get; private set; }
        #endregion
        #endregion
    }
}
