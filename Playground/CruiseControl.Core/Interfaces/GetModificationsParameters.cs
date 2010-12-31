namespace CruiseControl.Core.Interfaces
{
    using System;

    public class GetModificationsParameters
    {
        #region Public properties
        #region StartDate
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public DateTime? StartDate { get; set; }
        #endregion

        #region EndDate
        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        public DateTime? EndDate { get; set; }
        #endregion
        #endregion
    }
}
