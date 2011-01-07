namespace CruiseControl.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a set of grouped modifications.
    /// </summary>
    public class ModificationSet
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ModificationSet"/> class.
        /// </summary>
        public ModificationSet(params Modification[] modifications)
        {
            this.Modifications = new List<Modification>(modifications);
        }
        #endregion

        #region Public properties
        #region Modifications
        /// <summary>
        /// Gets the modifications.
        /// </summary>
        public IList<Modification> Modifications { get; private set; }
        #endregion
        #endregion
    }
}
