using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Combines two or more filters together.
    /// </summary>
    [Serializable]
    public class CombinationAuditFilter
        : AuditFilterBase
    {
        private List<AuditFilterBase> combinedFilters = new List<AuditFilterBase>();

        /// <summary>
        /// Initialises a new <see cref="CombinationAuditFilter"/>.
        /// </summary>
        public CombinationAuditFilter()
        {
        }

        /// <summary>
        /// Starts a new filter with the filters to combine.
        /// </summary>
        /// <param name="filters"></param>
        public CombinationAuditFilter(params AuditFilterBase[] filters)
            : this(filters, null) { }

        /// <summary>
        /// Starts a new filter with the security right and inner filter.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="innerFilter"></param>
        public CombinationAuditFilter(AuditFilterBase[] filters, AuditFilterBase innerFilter)
            : base(innerFilter)
        {
            this.combinedFilters = new List<AuditFilterBase>(filters);
        }

        #region Public properties
        #region Filters
        /// <summary>
        /// The filters to combine.
        /// </summary>
        [XmlElement("filter")]
        public List<AuditFilterBase> Filters
        {
            get { return combinedFilters; }
            set { combinedFilters = value; }
        }
        #endregion
        #endregion

        /// <summary>
        /// Checks if the security right matches.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override bool DoCheckFilter(AuditRecord record)
        {
            bool include = false;
            foreach (AuditFilterBase filter in combinedFilters)
            {
                if (filter.CheckFilter(record))
                {
                    include = true;
                    break;
                }
            }
            return include;
        }
    }
}
