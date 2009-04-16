using System;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Filters by a security right.
    /// </summary>
    [Serializable]
    public class SecurityRightAuditFilter
        : AuditFilterBase
    {
        private SecurityRight right;

        /// <summary>
        /// Starts a new filter with the security right.
        /// </summary>
        /// <param name="securityRight"></param>
        public SecurityRightAuditFilter(SecurityRight securityRight)
            : this(securityRight, null) { }

        /// <summary>
        /// Starts a new filter with the security right and inner filter.
        /// </summary>
        /// <param name="securityRight"></param>
        /// <param name="innerFilter"></param>
        public SecurityRightAuditFilter(SecurityRight securityRight, IAuditFilter innerFilter)
            : base(innerFilter)
        {
            this.right = securityRight;
        }

        /// <summary>
        /// Checks if the security right matches.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override bool DoCheckFilter(AuditRecord record)
        {
            bool include = (this.right == record.SecurityRight);
            return include;
        }
    }
}
