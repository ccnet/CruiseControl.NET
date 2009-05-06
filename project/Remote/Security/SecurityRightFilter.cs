using System;
using System.Xml.Serialization;

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
        /// Initialises a new <see cref="SecurityRightAuditFilter"/>.
        /// </summary>
        public SecurityRightAuditFilter()
        {
        }

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
        public SecurityRightAuditFilter(SecurityRight securityRight, AuditFilterBase innerFilter)
            : base(innerFilter)
        {
            this.right = securityRight;
        }

        #region Public properties
        #region SecurityRight
        /// <summary>
        /// The type of event.
        /// </summary>
        [XmlAttribute("right")]
        public SecurityRight SecurityRight
        {
            get { return right; }
            set { right = value; }
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
            bool include = (this.right == record.SecurityRight);
            return include;
        }
    }
}
