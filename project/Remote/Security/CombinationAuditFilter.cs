using System;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Combines two or more filters together.
    /// </summary>
    [Serializable]
    public class CombinationAuditFilter
        : AuditFilterBase
    {
        private IAuditFilter[] combinedFilters;

        /// <summary>
        /// Starts a new filter with the security right.
        /// </summary>
        /// <param name="filters"></param>
        public CombinationAuditFilter(params IAuditFilter[] filters)
            : this(filters, null) { }

        /// <summary>
        /// Starts a new filter with the security right and inner filter.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="innerFilter"></param>
        public CombinationAuditFilter(IAuditFilter[] filters, IAuditFilter innerFilter)
            : base(innerFilter)
        {
            this.combinedFilters = filters;
        }

        /// <summary>
        /// Checks if the security right matches.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override bool DoCheckFilter(AuditRecord record)
        {
            bool include = false;
            foreach (IAuditFilter filter in combinedFilters)
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
