using System;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class VaultTest : CustomAssertion
	{
		private Vault _vault;
		private Vault _sslVault;
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
			_vault = CreateNoSslVault();
			_sslVault = CreateSslVault();
		}

		[Test]
		public void CreateHistoryProcess()
		{
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			DateTime to = new DateTime(2002, 2, 22, 20, 0, 0);

			ProcessInfo actual = _vault.CreateHistoryProcessInfo(from, to);

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

			ProcessInfo actual = _sslVault.CreateHistoryProcessInfo(from, to);

			string expectedExecutable = @"c:\program files\sourcegear\vault client\vault.exe";
			string expectedArgs = string.Format(COMMAND_LINE_SSL, "$", "host", "username", "password", "repository");

			Assert.IsNotNull(actual);
			Assert.AreEqual(expectedExecutable, actual.FileName);
			Assert.AreEqual(expectedArgs, actual.Arguments);
		}

		[Test]
		public void ValuesSet()
		{
			Assert.AreEqual(@"c:\program files\sourcegear\vault client\vault.exe", _vault.Executable);
			Assert.AreEqual("username", _vault.Username);
			Assert.AreEqual("password", _vault.Password);
			Assert.AreEqual("host", _vault.Host);
			Assert.AreEqual("repository", _vault.Repository);
			Assert.AreEqual("$", _vault.Folder);
		}

		private Vault CreateNoSslVault()
		{
			Vault vault = new Vault();
			NetReflector.Read(ST_XML_NOSSL, vault);
			return vault;
		}

		private Vault CreateSslVault()
		{
			Vault vault = new Vault();
			NetReflector.Read(ST_XML_SSL, vault);
			return vault;
		}
	}
}