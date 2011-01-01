namespace CruiseControl.Core.Utilities
{
    using System;
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// Provides access to the clock on the machine.
    /// </summary>
    public class SystemClock
        : IClock
    {
        #region Public properties
        #region Now
        /// <summary>
        /// Gets the current date and time.
        /// </summary>
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
        #endregion

        #region Today
        /// <summary>
        /// Gets the current date.
        /// </summary>
        public DateTime Today
        {
            get { return DateTime.Today; }
        }
        #endregion
        #endregion
    }
}
