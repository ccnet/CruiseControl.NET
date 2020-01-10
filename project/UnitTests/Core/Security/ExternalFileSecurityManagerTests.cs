using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Core.Security.Auditing;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote.Messages;

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
        public void ChangePasswordForUnknownSession()
        {
            ExternalFileSecurityManager manager = new ExternalFileSecurityManager();
            string session = InitialiseManagerAndLogin(manager);
            Assert.That(delegate { manager.ChangePassword("unknown", "whoareyou", "whoami"); },
                        Throws.TypeOf<SessionInvalidException>());
        }

        [Test]
        public void ChangePasswordWithWrongPassword()
        {
            ExternalFileSecurityManager manager = new ExternalFileSecurityManager();
            string session = InitialiseManagerAndLogin(manager);
            Assert.That(delegate { manager.ChangePassword(session, "wrong", "whoami"); },
                        Throws.TypeOf<SecurityException>());
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
        public void ResetPasswordForUnknownSession()
        {
            ExternalFileSecurityManager manager = new ExternalFileSecurityManager();
            string session = InitialiseManagerAndLogin(manager);
            Assert.That(delegate { manager.ResetPassword("unknown", "johndoe", "whoami"); },
                        Throws.TypeOf<SessionInvalidException>());
        }

        [Test]
        public void ResetPasswordWithoutPermission()
        {
            ExternalFileSecurityManager manager = new ExternalFileSecurityManager();
            string session = InitialiseManagerAndLogin(manager);
            Assert.That(delegate { manager.ResetPassword(session, "johndoe", "whoami"); },
                        Throws.TypeOf<PermissionDeniedException>());
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
            LoginRequest credentials = new LoginRequest(userName);
            credentials.AddCredential(LoginRequest.PasswordCredential, "whoareyou");
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
            output.Append("<userPermission user=\"janedoe\" viewSecurity=\"Allow\" modifySecurity=\"Allow\" />");
            output.Append("</security>");
            return output.ToString();
        }
    }
}
