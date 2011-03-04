namespace CruiseControl.Common.Messages
{
    using System;

    public class ProjectStatus
    {
        #region Public properties
        #region Status
        public string Status { get; set; }
        #endregion

        #region LastBuildDate
        public DateTime LastBuildDate { get; set; }
        #endregion
        #endregion
    }
}
