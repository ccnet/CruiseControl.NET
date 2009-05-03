using System;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Filters by a user name.
    /// </summary>
    [Serializable]
    public class UserAuditFilter
        : AuditFilterBase
    {
        private string user;

        /// <summary>
        /// Starts a new filter with the user name.
        /// </summary>
        /// <param name="userName"></param>
        public UserAuditFilter(string userName)
            : this(userName, null) { }

        /// <summary>
        /// Starts a new filter with the user name and inner filter..
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="innerFilter"></param>
        public UserAuditFilter(string userName, IAuditFilter innerFilter)
            : base(innerFilter)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException("userName");
            this.user = userName;
        }

        /// <summary>
        /// Checks if the user name matches.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override bool DoCheckFilter(AuditRecord record)
        {
            bool include = string.Equals(this.user, record.UserName);
            return include;
        }
    }
}
