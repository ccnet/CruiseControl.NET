using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Core.Security.Auditing;
using ThoughtWorks.CruiseControl.Core.Config;
using System.Globalization;
using System.IO;
using System.Xml;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Defines a security manager implementation that implements security with configuration
    /// in external files.
    /// </summary>
    /// <title>External File Server Security</title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;externalFileSecurity&gt;
    /// &lt;cache type="inMemoryCache" duration="10" mode="sliding"/&gt;
    /// &lt;files&gt;
    /// &lt;file&gt;users.xml&lt;/file&gt;
    /// &lt;file&gt;permissions.xml&lt;/file&gt;
    /// &lt;/files&gt;
    /// &lt;/externalFileSecurity&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>External File Format</heading>
    /// <para>
    /// The elementsin the external file uses the standard user (<link>Security Users</link>) and permission definitions 
    /// (<link>Security Permissions</link>).
    /// </para>
    /// <para>
    /// It is possible to define multiple external security files. Each file can define the users and/or permissions for different areas (e.g.
    /// different departments).
    /// </para>
    /// <includePage>General Security Permissions</includePage>
    /// </remarks>
    [ReflectorType("externalFileSecurity")]
    public class ExternalFileSecurityManager
        : SecurityManagerBase, IConfigurationValidation
    {
        #region Private consts
        private const string CONFIG_ASSEMBLY_PATTERN = "ccnet.*.plugin.dll";
        #endregion

        #region Private fields
        private string[] files;
        private Dictionary<string, IAuthentication> loadedUsers;
        private List<IAuthentication> wildCardUsers;
        private Dictionary<string, IPermission> loadedPermissions;
        private bool isInitialised = false;
        private NetReflectorTypeTable typeTable;
        private NetReflectorReader reflectionReader;
        private Dictionary<string, string> settingFileMap;
		private readonly IExecutionEnvironment executionEnvironment;
        #endregion

		public ExternalFileSecurityManager() : this(new ExecutionEnvironment())
		{}

		public ExternalFileSecurityManager(IExecutionEnvironment executionEnvironment)
		{
			this.executionEnvironment = executionEnvironment;
		}

        #region Public properties
        #region Files
        /// <summary>
        /// The files to load.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("files")]
        public string[] Files
        {
            get { return files; }
            set { files = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialise the security manager.
        /// </summary>
        public override void Initialise()
        {
            if (!isInitialised)
            {
                // Initialise the reader
                typeTable = new NetReflectorTypeTable();
                typeTable.Add(AppDomain.CurrentDomain);
                typeTable.Add(Directory.GetCurrentDirectory(), CONFIG_ASSEMBLY_PATTERN);
                typeTable.InvalidNode += delegate(InvalidNodeEventArgs args)
                {
                    throw new Exception(args.Message);
                };
                reflectionReader = new NetReflectorReader(typeTable);

                // Initialise the local caches
                SessionCache.Initialise();
                loadedUsers = new Dictionary<string, IAuthentication>();
                wildCardUsers = new List<IAuthentication>();
                loadedPermissions = new Dictionary<string, IPermission>();

                // Load each file
                settingFileMap = new Dictionary<string, string>();
                foreach (string fileName in files)
                {
                    LoadFile(fileName);
                }
            }

            isInitialised = true;
        }
        #endregion

        #region RetrieveUser()
        /// <summary>
        /// Retrieves a user from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public override IAuthentication RetrieveUser(string identifier)
        {
            IAuthentication setting = null;
            if (!string.IsNullOrEmpty(identifier))
            {
                // If initialised then use the loaded dictionaries
                if (isInitialised)
                {
                    identifier = identifier.ToLower(CultureInfo.InvariantCulture);

                    if ((setting == null) && (loadedUsers.ContainsKey(identifier)))
                    {
                        setting = loadedUsers[identifier];
                    }

                    if (setting == null)
                    {
                        // Attempt to find a matching wild-card
                        foreach (IAuthentication wildCard in wildCardUsers)
                        {
                            if (SecurityHelpers.IsWildCardMatch(wildCard.Identifier, identifier))
                            {
                                setting = wildCard;
                                break;
                            }
                        }
                    }
                }
            }
            return setting;
        }
        #endregion

        #region RetrievePermission()
        /// <summary>
        /// Retrieves a permission from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public override IPermission RetrievePermission(string identifier)
        {
            IPermission setting = null;
            if (!string.IsNullOrEmpty(identifier))
            {
                // If initialised then use the loaded dictionaries
                if (isInitialised)
                {
                    identifier = identifier.ToLower(CultureInfo.InvariantCulture);

                    if (loadedPermissions.ContainsKey(identifier))
                    {
                        setting = loadedPermissions[identifier];
                    }
                }
            }
            return setting;
        }
        #endregion

        #region ListAllUsers()
        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <returns>
        /// A list of <see cref="UserNameCredentials"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public override List<UserDetails> ListAllUsers()
        {
            List<UserDetails> usersList = new List<UserDetails>();
            foreach (IAuthentication userDetails in loadedUsers.Values)
            {
                // Generate the details to return
                UserDetails user = new UserDetails();
                user.UserName = userDetails.UserName;
                user.DisplayName = userDetails.DisplayName;
                user.Type = userDetails.AuthenticationName;
                usersList.Add(user);

            }
            return usersList;
        }
        #endregion

        #region CheckServerPermission()
        /// <summary>
        /// Checks whether the user can perform the specified action at the server level.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns>True if the permission is valid, false otherwise.</returns>
        public override bool CheckServerPermission(string userName, SecurityPermission permission)
        {
            SecurityRight currentRight = SecurityRight.Inherit;

            // Iterate through the permissions stopping when we hit the first non-inherited permission
            foreach (IPermission permissionToCheck in loadedPermissions.Values)
            {
                if (permissionToCheck.CheckUser(this, userName)) currentRight = permissionToCheck.CheckPermission(this, permission);
                if (currentRight != SecurityRight.Inherit) break;
            }

            // If we don't have a result, then use the default right
            if (currentRight == SecurityRight.Inherit) currentRight = GetDefaultRight(permission);
            return (currentRight == SecurityRight.Allow);
        }
        #endregion

        #region Validate()
        /// <summary>
        /// Checks the internal validation of the item.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        public virtual void Validate(IConfiguration configuration, object parent, IConfigurationErrorProcesser errorProcesser)
        {
            List<string> settings = new List<string>();
            List<string> duplicates = new List<string>();

            Initialise();
        }
        #endregion

        #region ChangePassword()
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="oldPassword">The person's old password.</param>
        /// <param name="newPassword">The person's new password.</param>
        public override void ChangePassword(string sessionToken, string oldPassword, string newPassword)
        {
            // Retrieve the user
            string userName = GetUserName(sessionToken);
            if (string.IsNullOrEmpty(userName)) throw new SessionInvalidException();
            IAuthentication user = RetrieveUser(userName);
            if (user == null) throw new SessionInvalidException();

            // Validate the old password
            LoginRequest credientals = new LoginRequest(userName);
            credientals.AddCredential(LoginRequest.PasswordCredential, oldPassword);
            if (!user.Authenticate(credientals))
            {
                LogEvent(null, userName, SecurityEvent.ChangePassword, SecurityRight.Deny, "Old password is incorrect");
                throw new SecurityException("Old password is incorrect");
            }

            // Change the password
            LogEvent(null, userName, SecurityEvent.ChangePassword, SecurityRight.Allow, null);
            user.ChangePassword(newPassword);

            // Update the file
            UpdateSetting(user);
        }
        #endregion

        #region ResetPassword()
        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="userName">The user name to reset the password for.</param>
        /// <param name="newPassword">The person's new password.</param>
        public override void ResetPassword(string sessionToken, string userName, string newPassword)
        {
            // Retrieve the user and make sure they have the right permission
            string currentUser = GetUserName(sessionToken);
            if (string.IsNullOrEmpty(currentUser)) throw new SessionInvalidException();
            if (!CheckServerPermission(currentUser, SecurityPermission.ModifySecurity))
            {
                LogEvent(null, currentUser, SecurityEvent.ResetPassword, SecurityRight.Deny, null);
                throw new PermissionDeniedException("Reset password");
            }

            // Change the password
            LogEvent(null, currentUser, SecurityEvent.ResetPassword, SecurityRight.Allow,
                string.Format("Reset password for '{0}'", userName));
            IAuthentication user = RetrieveUser(userName);
            if (user == null) throw new SessionInvalidException();
            user.ChangePassword(newPassword);

            // Update the file
            UpdateSetting(user);
        }
        #endregion
        #endregion

        #region Private methods
        #region LoadFile()
        /// <summary>
        /// Loads all the settings from a file.
        /// </summary>
        /// <param name="fileName"></param>
        private void LoadFile(string fileName)
        {
            XmlDocument sourceDocument = new XmlDocument();
			sourceDocument.Load(executionEnvironment.EnsurePathIsRooted(fileName));

            foreach (XmlElement setting in sourceDocument.DocumentElement.SelectNodes("*"))
            {
                object loadedItem = reflectionReader.Read(setting);
                if (loadedItem is IPermission)
                {
                    IPermission permission = loadedItem as IPermission;
                    permission.Manager = this;
                    string identifier = permission.Identifier.ToLower(CultureInfo.InvariantCulture);
                    if (loadedPermissions.ContainsKey(identifier)) loadedPermissions.Remove(identifier);
                    loadedPermissions.Add(identifier, permission);
                    LinkIdentifierWithFile(fileName, identifier);
                }
                else if (loadedItem is IAuthentication)
                {
                    IAuthentication authentication = loadedItem as IAuthentication;
                    authentication.Manager = this;
                    string identifier = authentication.Identifier.ToLower(CultureInfo.InvariantCulture);
                    if (loadedUsers.ContainsKey(identifier)) loadedUsers.Remove(identifier);
                    if (authentication.Identifier.Contains("*"))
                    {
                        wildCardUsers.Add(authentication);
                    }
                    else
                    {
                        loadedUsers.Add(identifier, authentication);
                    }
                    LinkIdentifierWithFile(fileName, identifier);
                }
                else
                {
                    throw new Exception("Unknown security item: " + setting.OuterXml);
                }
            }
        }
        #endregion

        #region LinkIdentifierWithFile()
        /// <summary>
        /// Links an identifier with the file it came from.
        /// </summary>
        /// <param name="fileName">The source file.</param>
        /// <param name="identifier">The identifier.</param>
        private void LinkIdentifierWithFile(string fileName, string identifier)
        {
            if (settingFileMap.ContainsKey(identifier))
            {
                settingFileMap[identifier] = fileName;
            }
            else
            {
                settingFileMap.Add(identifier, fileName);
            }
        }
        #endregion

        #region UpdateSetting()
        /// <summary>
        /// Updates the file that a setting is in.
        /// </summary>
        /// <param name="setting">The setting that has been changed.</param>
        private void UpdateSetting(ISecuritySetting setting)
        {
            // Load the file that the setting is in
            string fileName = settingFileMap[setting.Identifier];
            XmlDocument sourceDocument = new XmlDocument();
			sourceDocument.Load(executionEnvironment.EnsurePathIsRooted(fileName));

            // Find the item that is being updated
            foreach (XmlElement settingEl in sourceDocument.DocumentElement.SelectNodes("*"))
            {
                object loadedItem = reflectionReader.Read(settingEl);

                if (loadedItem is ISecuritySetting)
                {
                    string identifier = (loadedItem as ISecuritySetting).Identifier;
                    if (identifier == setting.Identifier)
                    {
                        // Update the item with the new setting
                        StringWriter buffer = new StringWriter();
                        new ReflectorTypeAttribute(settingEl.Name).Write(new XmlTextWriter(buffer), setting);
                        XmlElement element = sourceDocument.CreateElement("changed");
                        element.InnerXml = buffer.ToString();
                        settingEl.ParentNode.ReplaceChild(element.FirstChild, settingEl);
                        break;
                    }
                }
            }

            // Save the updated document
            sourceDocument.Save(fileName);
        }
        #endregion
        #endregion
    }
}
