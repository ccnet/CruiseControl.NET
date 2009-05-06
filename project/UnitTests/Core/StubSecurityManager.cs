using Exortech.NetReflector;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Core.Security.Auditing;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
    /// <summary>
    /// Since this version of NMock doesn't support generics this is a stub for the security manager
    /// </summary>
    [ReflectorType("securityStub")]
    public class StubSecurityManager
        : ISecurityManager
    {
        private string[] methods = new string[] {
                "Initialise",
                "Login",
                "Logout",
                "ValidateSession",
                "GetUserName",
                "RetrieveSetting",
                "GetDisplayName"
            };
        private int[] expectedCounts = new int[7];
        private int[] actualCounts = new int[7];
        private Queue<string> loginResults = new Queue<string>();
        private Queue<bool> validateSessionResults = new Queue<bool>();
        private Queue<string> getUserNameResults = new Queue<string>();
        private Queue<string> getDisplayNameResults = new Queue<string>();
        private Queue<object> retrieveSettingsResults = new Queue<object>();

        public void Initialise()
        {
            actualCounts[0]++;
        }

        [ReflectorProperty("dummy")]
        public string Dummy
        {
            get { return "Dummy Value"; }
            set { }
        }

        [ReflectorProperty("password")]
        public string Password
        {
            get { return "Dummy Value"; }
            set { }
        }

        public string Login(LoginRequest credentials)
        {
            actualCounts[1]++;
            return loginResults.Dequeue();
        }

        public void Logout(string sessionToken)
        {
            actualCounts[2]++;
        }

        public bool ValidateSession(string sessionToken)
        {
            actualCounts[3]++;
            return validateSessionResults.Dequeue();
        }

        public string GetUserName(string sessionToken)
        {
            actualCounts[4]++;
            if (getUserNameResults.Count == 0)
            {
                throw new Exception("Unexpected call to GetUserName");
            }
            else
            {
                return getUserNameResults.Dequeue();
            }
        }

        public string GetDisplayName(string sessionToken)
        {
            actualCounts[6]++;
            if (getDisplayNameResults.Count == 0)
            {
                throw new Exception("Unexpected call to GetDisplayName");
            }
            else
            {
                return getDisplayNameResults.Dequeue();
            }
        }

        /// <summary>
        /// Retrieves a user from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public IAuthentication RetrieveUser(string identifier)
        {
            actualCounts[5]++;
            return retrieveSettingsResults.Dequeue() as IAuthentication;
        }

        /// <summary>
        /// Retrieves a permission from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public IPermission RetrievePermission(string identifier)
        {
            actualCounts[5]++;
            return retrieveSettingsResults.Dequeue() as IPermission;
        }

        public void Verify()
        {
            for (int iLoop = 0; iLoop < methods.Length; iLoop++)
            {
                Assert.AreEqual(expectedCounts[iLoop], actualCounts[iLoop], methods[iLoop] + " call count does not match expected");
            }
        }

        public void Expect(string method, params object[] parameters)
        {
            switch (method)
            {
                case "Initialise":
                    expectedCounts[0]++;
                    break;
                case "Login":
                    expectedCounts[1]++;
                    break;
                case "Logout":
                    expectedCounts[2]++;
                    break;
                case "ValidateSession":
                    expectedCounts[3]++;
                    break;
                case "GetUserName":
                    expectedCounts[4]++;
                    break;
                case "RetrieveSetting":
                    expectedCounts[5]++;
                    break;
                case "GetDisplayName":
                    expectedCounts[6]++;
                    break;
                default:
                    Assert.Fail("Unknown method: " + method);
                    break;
            }
        }

        public void ExpectAndReturn(string method, object result, params object[] parameters)
        {
            Expect(method, parameters);
            switch (method)
            {
                case "Login":
                    loginResults.Enqueue((string)result);
                    break;
                case "ValidateSession":
                    validateSessionResults.Enqueue((bool)result);
                    break;
                case "GetUserName":
                    getUserNameResults.Enqueue((string)result);
                    break;
                case "RetrieveSetting":
                    retrieveSettingsResults.Enqueue(result);
                    break;
                case "GetDisplayName":
                    getDisplayNameResults.Enqueue((string)result);
                    break;
                default:
                    Assert.Fail("Unknown method: " + method);
                    break;
            }
        }

        /// <summary>
        /// Sends a security event to the audit loggers.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="userName">The name of the user.</param>
        /// <param name="eventType">The type of event.</param>
        /// <param name="eventRight">The right of the event.</param>
        /// <param name="message">Any security message.</param>
        public void LogEvent(string projectName, string userName, SecurityEvent eventType, SecurityRight eventRight, string message)
        {
        }

        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <returns>
        /// A list of <see cref="UserNameCredentials"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public virtual List<UserDetails> ListAllUsers()
        {
            return new List<UserDetails>();
        }

        /// <summary>
        /// Checks the security permissions for a user against one or more projects.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="projectNames">The names of the projects to check.</param>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(string userName, params string[] projectNames)
        {
            return new List<SecurityCheckDiagnostics>();
        }

        public virtual bool CheckServerPermission(string userName, SecurityPermission permission)
        {
            return true;
        }

        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(int startPosition, int numberOfRecords)
        {
            List<AuditRecord> records = new List<AuditRecord>();
            return records;
        }

        /// <summary>
        /// Reads all the specified number of filtered audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <param name="filter">The filter to use.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(int startPosition, int numberOfRecords, AuditFilterBase filter)
        {
            List<AuditRecord> records = new List<AuditRecord>();
            return records;
        }

        #region ChangePassword()
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="oldPassword">The person's old password.</param>
        /// <param name="newPassword">The person's new password.</param>
        public virtual void ChangePassword(string sessionToken, string oldPassword, string newPassword)
        {
            throw new NotImplementedException("Password management is not allowed for this security manager");
        }
        #endregion

        #region ResetPassword()
        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="userName">The user name to reset the password for.</param>
        /// <param name="newPassword">The person's new password.</param>
        public virtual void ResetPassword(string sessionToken, string userName, string newPassword)
        {
            throw new NotImplementedException("Password management is not allowed for this security manager");
        }
        #endregion

        #region RetrieveComponent()
        /// <summary>
        /// Retrieves a component from the security manager.
        /// </summary>
        /// <typeparam name="TComponent">The type of component to retrieve.</typeparam>
        /// <returns>The component of the specified type, if available, null otherwise.</returns>
        public TComponent RetrieveComponent<TComponent>()
            where TComponent : class
        {
            return null;
        }
        #endregion

        #region GetDefaultRight()
        /// <summary>
        /// Gets the default right for a permission.
        /// </summary>
        /// <param name="permission">The permission to retrieve the default for.</param>
        /// <returns>The default right.</returns>
        public virtual SecurityRight GetDefaultRight(SecurityPermission permission)
        {
            return SecurityRight.Allow;
        }
        #endregion
    }
}
