namespace CruiseControl.Core.Interfaces
{
    using System;

    /// <summary>
    /// Provides access to the clock.
    /// </summary>
    public interface IClock
    {
        #region Public properties
        #region Now
        /// <summary>
        /// Gets the current date and time.
        /// </summary>
        DateTime Now { get; }
        #endregion

        #region Today
        /// <summary>
        /// Gets the current date.
        /// </summary>
        DateTime Today { get; }
        #endregion
        #endregion
    }
}
