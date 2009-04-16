using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Core.Security.Auditing;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class ExternalFileSecurityManagerTests
    {
        [Test]
        public void ManagerLoadsUsers()
        {
            ExternalFileSecurityManager manager = new ExternalFileSecurityManager();
            manager.Files = new string[]
                {
                    GenerateUsersFile()
                };
            manager.Initialise();
            IAuthentication actualUser = manager.RetrieveUser("johndoe");
            Assert.IsNotNull(actualUser, "User not found");
        }

        [Test]
        public void CanLogin()
        {
            ExternalFileSecurityManager manager = new ExternalFileSecurityManager();
            InitialiseManagerAndLogin(manager);
        }

        [Test]
        public void ChangePasswordWithValidDetails()
        {
            ExternalFileSecurityManager manager = new ExternalFileSecurityManager();
            string session = InitialiseManagerAndLogin(manager);
            manager.ChangePassword(session, "whoareyou", "whoami");

            string actual = TrimWhitespace(File.ReadAllText(GenerateUsersFileName()));
            string expected = GenerateUserFileContents("whoami");
            Assert.AreEqual(expected, actual, "File contents do not match");
        }

        [Test]
        [ExpectedException(typeof(SessionInvalidException))]
        public void ChangePasswordForUnknownSession()
        {
            ExternalFileSecurityManager manager = new ExternalFileSecurityManager();
            string session = InitialiseManagerAndLogin(manager);
            manager.ChangePassword("unknown", "whoareyou", "whoami");
        }

        [Test]
        [ExpectedException(typeof(SecurityException))]
        public void ChangePasswordWithWrongPassword()
        {
            ExternalFileSecurityManager manager = new ExternalFileSecurityManager();
            string session = InitialiseManagerAndLogin(manager);
            manager.ChangePassword(session, "wrong", "whoami");
        }

        [Test]
        public void ResetPasswordWithValidDetails()
        {
            ExternalFileSecurityManager manager = new ExternalFileSecurityManager();
            string session = InitialiseManagerAndLogin(manager, "janedoe");
            manager.ResetPassword(session, "johndoe", "whoami");

            string actual = TrimWhitespace(File.ReadAllText(GenerateUsersFileName()));
            string expected = GenerateUserFileContents("whoami");
            Assert.AreEqual(expected, actual, "File contents do not match");
        }

        [Test]
        [ExpectedException(typeof(SessionInvalidException))]
        public void ResetPasswordForUnknownSession()
        {
            ExternalFileSecurityManager manager = new ExternalFileSecurityManager();
            string session = InitialiseManagerAndLogin(manager);
            manager.ResetPassword("unknown", "johndoe", "whoami");
        }

        [Test]
        [ExpectedException(typeof(PermissionDeniedException))]
        public void ResetPasswordWithoutPermission()
        {
            ExternalFileSecurityManager manager = new ExternalFileSecurityManager();
            string session = InitialiseManagerAndLogin(manager);
            manager.ResetPassword(session, "johndoe", "whoami");
        }

        private string TrimWhitespace(string xml)
        {
            StringBuilder output = new StringBuilder();
            bool inTag = false;
            foreach (char character in xml)
            {
                switch (character)
                {
                    case '<':
                        inTag = true;
                        output.Append(character);
                        break;
                    case '>':
                        inTag = false;
                        output.Append(character);
                        break;
                    case ' ':
                    case '\r':
                    case '\n':
                        if (inTag) output.Append(character);
                        break;
                    default:
                        output.Append(character);
                        break;
                }
            }
            return output.ToString();
        }

        private string InitialiseManagerAndLogin(ExternalFileSecurityManager manager)
        {
            return InitialiseManagerAndLogin(manager, "johndoe");
        }

        private string InitialiseManagerAndLogin(ExternalFileSecurityManager manager, string userName)
        {
            manager.Files = new string[]
                {
                    GenerateUsersFile()
                };
            manager.Initialise();
            UserNameCredentials credentials = new UserNameCredentials(userName);
            credentials["password"] = "whoareyou";
            string session = manager.Login(credentials);
            Assert.IsFalse(string.IsNullOrEmpty(session), "Session has not been allocated");
            return session;
        }

        private string GenerateUsersFileName()
        {
            string fileName = Path.Combine(Path.GetTempPath(), "Users.xml");
            return fileName;
        }

        private string GenerateUsersFile()
        {
            string fileName = GenerateUsersFileName();

            string output = GenerateUserFileContents("whoareyou");

            if (File.Exists(fileName)) File.Delete(fileName);
            File.WriteAllText(fileName, output);

            return fileName;
        }

        private string GenerateUserFileContents(string password)
        {
            StringBuilder output = new StringBuilder();
            output.Append("<security>");
            output.Append("<passwordUser><password>whoareyou</password><name>georgedoe</name></passwordUser>");
            output.AppendFormat("<passwordUser><password>{0}</password><name>johndoe</name></passwordUser>", password);
            output.Append("<passwordUser><password>whoareyou</password><name>janedoe</name></passwordUser>");
            output.Append("<userPermission name=\"janedoe\" viewSecurity=\"Allow\" />");
            output.Append("</security>");
            return output.ToString();
        }
    }
}
