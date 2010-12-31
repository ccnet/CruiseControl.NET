namespace CruiseControl.Core
{
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// A block that provides access to a source control repository.
    /// </summary>
    public abstract class SourceControlBlock
        : ProjectItem
    {
        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this block.
        /// </summary>
        public virtual void Validate()
        { }
        #endregion

        #region Initialise()
        /// <summary>
        /// Initialises this block at the start of an integration.
        /// </summary>
        public virtual void Initialise()
        { }
        #endregion

        #region GetModifications()
        /// <summary>
        /// Gets any modifications.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public abstract void GetModifications(GetModificationsParameters parameters);
        #endregion

        #region Label()
        /// <summary>
        /// Adds a label to the repository.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public abstract void Label(LabelParameters parameters);
        #endregion

        #region GetSource()
        /// <summary>
        /// Gets the latest source from the repository.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public abstract void GetSource(GetSourceParameters parameters);
        #endregion

        #region CleanUp()
        /// <summary>
        /// Cleans up after an integration.
        /// </summary>
        public virtual void CleanUp()
        { }
        #endregion
        #endregion
    }
}
