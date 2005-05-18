using System;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class VaultTest : CustomAssertion
	{
		private Vault vault;
		private Vault sslVault;
		private const string COMMAND_LINE_NOSSL = @"history ""{0}"" -host ""{1}"" -user ""{2}"" -password ""{3}"" -repository ""{4}"" -rowlimit 0";
		private const string COMMAND_LINE_SSL = @"history ""{0}"" -host ""{1}"" -user ""{2}"" -password ""{3}"" -repository ""{4}"" -rowlimit 0 -ssl";

		private const string ST_XML_SSL = @"<sourceControl type=""vault"">
				<executable>c:\program files\sourcegear\vault client\vault.exe</executable>
				<username>username</username>
				<password>password</password>
				<host>host</host>
				<repository>repository</repository>
				<folder>$</folder>
				<ssl>True</ssl>
			</sourceControl>";

		private const string ST_XML_NOSSL = @"<sourceControl type=""vault"">
				<executable>c:\program files\sourcegear\vault client\vault.exe</executable>
				<username>username</username>
				<password>password</password>
				<host>host</host>
				<repository>repository</repository>
				<folder>$</folder>
			</sourceControl>";

		[SetUp]
		public void SetUp()
		{
			vault = CreateVault(ST_XML_NOSSL);
			sslVault = CreateVault(ST_XML_SSL);
		}

		[Test]
		public void CreateHistoryProcess()
		{
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			DateTime to = new DateTime(2002, 2, 22, 20, 0, 0);

			ProcessInfo actual = vault.CreateHistoryProcessInfo(from, to);

			string expectedExecutable = @"c:\program files\sourcegear\vault client\vault.exe";
			string expectedArgs = string.Format(COMMAND_LINE_NOSSL, "$", "host", "username", "password", "repository");

			Assert.IsNotNull(actual);
			Assert.AreEqual(expectedExecutable, actual.FileName);
			Assert.AreEqual(expectedArgs, actual.Arguments);
		}

		[Test]
		public void CreateHistoryProcessSsl()
		{
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			DateTime to = new DateTime(2002, 2, 22, 20, 0, 0);

			ProcessInfo actual = sslVault.CreateHistoryProcessInfo(from, to);

			string expectedExecutable = @"c:\program files\sourcegear\vault client\vault.exe";
			string expectedArgs = string.Format(COMMAND_LINE_SSL, "$", "host", "username", "password", "repository");

			Assert.IsNotNull(actual);
			Assert.AreEqual(expectedExecutable, actual.FileName);
			Assert.AreEqual(expectedArgs, actual.Arguments);
		}

		[Test]
		public void ValuesSet()
		{
			Assert.AreEqual(@"c:\program files\sourcegear\vault client\vault.exe", vault.Executable);
			Assert.AreEqual("username", vault.Username);
			Assert.AreEqual("password", vault.Password);
			Assert.AreEqual("host", vault.Host);
			Assert.AreEqual("repository", vault.Repository);
			Assert.AreEqual("$", vault.Folder);
		}

		[Test]
		public void ConfigureWithMinimumSettings()
		{
			Vault vault = CreateVault(@"<sourceControl type=""vault"">
				<executable>c:\program files\sourcegear\vault client\vault.exe</executable>
				<folder>$</folder>
			</sourceControl>");

			ProcessInfo actual = vault.CreateHistoryProcessInfo(DateTime.Now, DateTime.Now);
			Assert.AreEqual(@"c:\program files\sourcegear\vault client\vault.exe", actual.FileName);
			Assert.AreEqual(@"history ""$"" -rowlimit 0", actual.Arguments);
		}

		private Vault CreateVault(string xml)
		{
			Vault vault = new Vault();
			NetReflector.Read(xml, vault);
			return vault;
		}
	}
}