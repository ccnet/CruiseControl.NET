using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class VaultTest : CustomAssertion
	{
		private Vault _vault;
		private Vault _sslVault;
		private const string COMMAND_LINE_NOSSL = @"history ""{0}"" -host {1} -user {2} -password {3} -repository {4} -rowlimit 0";
		private const string COMMAND_LINE_SSL = @"history ""{0}"" -host {1} -user {2} -password {3} -repository {4} -rowlimit 0 -ssl";

		private const string ST_XML_SSL =
			@"<sourceControl type=""vault"">
				<executable>c:\program files\sourcegear\vault client\vault.exe</executable>
				<username>username</username>
				<password>password</password>
				<host>host</host>
				<repository>repository</repository>
				<folder>$</folder>
				<ssl>True</ssl>
			</sourceControl>";

		private const string ST_XML_NOSSL =
			@"<sourceControl type=""vault"">
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
		public void TestCreateHistoryProcess()
		{				
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			DateTime to = new DateTime(2002, 2, 22, 20, 0, 0);

			ProcessInfo actual = _vault.CreateHistoryProcessInfo(from, to);

			string expectedExecutable = @"c:\program files\sourcegear\vault client\vault.exe";
			string expectedArgs = string.Format(
				COMMAND_LINE_NOSSL,
				"$",
				"host",
				"username",
				"password",
				"repository");

			AssertNotNull("process was null", actual);
			AssertEquals(expectedExecutable, actual.FileName);
			AssertEquals(expectedArgs, actual.Arguments);
		}

		[Test]
		public void TestCreateHistoryProcessSsl()
		{				
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			DateTime to = new DateTime(2002, 2, 22, 20, 0, 0);

			ProcessInfo actual = _sslVault.CreateHistoryProcessInfo(from, to);

			string expectedExecutable = @"c:\program files\sourcegear\vault client\vault.exe";
			string expectedArgs = string.Format(
				COMMAND_LINE_SSL,
				"$",
				"host",
				"username",
				"password",
				"repository");

			AssertNotNull("process was null", actual);
			AssertEquals(expectedExecutable, actual.FileName);
			AssertEquals(expectedArgs, actual.Arguments);
		}

		[Test]
		public void TestValuesSet()
		{			
			AssertEquals(@"c:\program files\sourcegear\vault client\vault.exe", _vault.Executable);
			AssertEquals("username", _vault.Username);
			AssertEquals("password", _vault.Password);
			AssertEquals("host", _vault.Host);
			AssertEquals("repository", _vault.Repository);
			AssertEquals("$", _vault.Folder);
		}

		[Test]
		[Ignore("user-specific vault test settings here")]
		public void HistoryDatesTest()
		{
			// change these values to reflect your live vault repository
			// checked in IPopup.cs on 4/20/2004 10:56:46 AM
			// so check for changes between 10 and 11
			DateTime from = new DateTime(2004, 4, 20, 10, 0, 0);
			DateTime to = new DateTime(2004, 4, 20, 11, 0, 0);
			
			Modification[] mods = _vault.GetModifications(from, to);
			AssertNotNull(mods);

			// change this value to reflect your live vault repository
			AssertEquals(1, mods.Length);
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
