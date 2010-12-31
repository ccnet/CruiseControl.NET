namespace CruiseControl.Core.SourceControl
{
    using System;
    using CruiseControl.Core.Interfaces;

    public class Subversion
        : SourceControlBlock
    {
        #region Public properties
        #region TrunkUrl
        /// <summary>
        /// Gets or sets the trunk URL.
        /// </summary>
        /// <value>
        /// The trunk URL.
        /// </value>
        public string TrunkUrl { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this block.
        /// </summary>
        public override void Validate()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Initialise()
        /// <summary>
        /// Initialises this block at the start of an integration.
        /// </summary>
        public override void Initialise()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetModifications()
        /// <summary>
        /// Gets any modifications.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public override void GetModifications(GetModificationsParameters parameters)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Label()
        /// <summary>
        /// Adds a label to the repository.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public override void Label(LabelParameters parameters)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetSource()
        /// <summary>
        /// Gets the latest source from the repository.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public override void GetSource(GetSourceParameters parameters)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region CleanUp()
        /// <summary>
        /// Cleans up after an integration.
        /// </summary>
        public override void CleanUp()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
