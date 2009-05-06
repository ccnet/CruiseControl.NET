using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Remote.Security;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// A request message for reading the audit log.
    /// </summary>
    [XmlRoot("readAuditMessage")]
    [Serializable]
    public class ReadAuditRequest
        : ServerRequest
    {
        #region Private fields
        private int startRecord = 0;
        private int numberOfRecords = int.MaxValue;
        private AuditFilterBase filter;
        #endregion

        #region Public properties
        #region StartRecord
        /// <summary>
        /// The starting record number.
        /// </summary>
        [XmlAttribute("start")]
        [DefaultValue(0)]
        public int StartRecord
        {
            get { return startRecord; }
            set { startRecord = value; }
        }
        #endregion

        #region NumberOfRecords
        /// <summary>
        /// The number of records to read.
        /// </summary>
        [XmlAttribute("number")]
        [DefaultValue(int.MaxValue)]
        public int NumberOfRecords
        {
            get { return numberOfRecords; }
            set { numberOfRecords = value; }
        }
        #endregion

        #region Filter
        /// <summary>
        /// The filter to apply.
        /// </summary>
        [XmlElement("filter")]
        public AuditFilterBase Filter
        {
            get { return filter; }
            set { filter = value; }
        }
        #endregion
        #endregion
    }
}
