using System;
using System.Xml.Serialization;

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
        /// Initialises a new <see cref="UserAuditFilter"/>.
        /// </summary>
        public UserAuditFilter()
        {
        }

        /// <summary>
        /// Starts a new filter with the user name.
        /// </summary>
        /// <param name="userName"></param>
        public UserAuditFilter(string userName)
            : this(userName, null) { }

        #region Public properties
        #region UserName
        /// <summary>
        /// The name of the user.
        /// </summary>
        [XmlAttribute("user")]
        public string UserName
        {
            get { return user; }
            set { user = value; }
        }
        #endregion
        #endregion

        /// <summary>
        /// Starts a new filter with the user name and inner filter..
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="innerFilter"></param>
        public UserAuditFilter(string userName, AuditFilterBase innerFilter)
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
            bool include = string.Equals(this.user, record.UserName, StringComparison.CurrentCulture);
            return include;
        }
    }
}
