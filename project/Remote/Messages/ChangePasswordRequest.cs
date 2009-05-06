using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// A change password request message.
    /// </summary>
    [XmlRoot("changePasswordMessage")]
    [Serializable]
    public class ChangePasswordRequest
        : ServerRequest
    {
        #region Private fields
        private string oldPassword;
        private string newPassword;
        private string userName;
        #endregion

        #region Public properties
        #region OldPassword
        /// <summary>
        /// The old password
        /// </summary>
        [XmlAttribute("oldPassword")]
        public string OldPassword
        {
            get { return oldPassword; }
            set { oldPassword = value; }
        }
        #endregion

        #region NewPassword
        /// <summary>
        /// The new password
        /// </summary>
        [XmlAttribute("newPassword")]
        public string NewPassword
        {
            get { return newPassword; }
            set { newPassword = value; }
        }
        #endregion

        #region UserName
        /// <summary>
        /// The user name.
        /// </summary>
        [XmlAttribute("userName")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        #endregion
        #endregion
    }
}
