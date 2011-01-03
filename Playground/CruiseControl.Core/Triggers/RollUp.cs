namespace CruiseControl.Core.Triggers
{
    using System;

    public class RollUp
        : Trigger
    {
        #region Protected methods
        #region OnCheck()
        /// <summary>
        /// Called when the trigger needs to be checked.
        /// </summary>
        /// <returns>
        /// An <see cref="IntegrationRequest"/> if tripped; <c>null</c> otherwise.
        /// </returns>
        protected override IntegrationRequest OnCheck()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
