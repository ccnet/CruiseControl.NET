using System;
using System.Globalization;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Telelogic;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Telelogic
{
	[TestFixture]
	public sealed class SynergyTest
	{
		[Test]
		public void VerifyDefaultValues()
		{
			Synergy synergy = new Synergy();
			Assert.AreEqual("ccm.exe", synergy.Connection.Executable, "#A1");
			Assert.AreEqual("localhost", synergy.Connection.Host, "#A2");
			Assert.IsNull(synergy.Connection.Database, "#A3");
			Assert.IsNull(synergy.Connection.SessionId, "#A4");
			Assert.AreEqual(3600, synergy.Connection.Timeout, "#A5");
			Assert.AreEqual('-', synergy.Connection.Delimiter, "#A6");
			Assert.IsNull(synergy.Project.Release, "#A7");
			Assert.AreEqual(0, synergy.Project.TaskFolder, "#A8");
			Assert.AreEqual(Environment.ExpandEnvironmentVariables("%USERNAME%"), synergy.Connection.Username, "#A9");
			Assert.AreEqual(String.Empty, synergy.Connection.Password, "#A10");
			Assert.AreEqual("build_mgr", synergy.Connection.Role, "#A11");
			Assert.IsFalse(synergy.Connection.PollingEnabled, "#A12");
			Assert.IsFalse(synergy.Project.BaseliningEnabled, "#A13");
			Assert.IsFalse(synergy.Project.TemplateEnabled, "#A14");
			Assert.IsNull(synergy.Project.ReconcilePaths, "#A15");
			Assert.AreEqual("Integration Testing", synergy.Project.Purpose, "#A16");
		}

		[Test]
		public void PopulateFromConfigurationXml()
		{
			Synergy synergy = (Synergy) NetReflector.Read(SynergyMother.ConfigValues);
			Assert.AreEqual("ccm.cmd", synergy.Connection.Executable);
			Assert.AreEqual("myserver", synergy.Connection.Host);
			Assert.AreEqual(@"\\myserver\share\mydatabase", synergy.Connection.Database);
			Assert.AreEqual(600, synergy.Connection.Timeout);
			Assert.AreEqual("Product/1.0", synergy.Project.Release);
			Assert.AreEqual(1234, synergy.Project.TaskFolder);
			Assert.AreEqual("jdoe", synergy.Connection.Username);
			Assert.AreEqual("password", synergy.Connection.Password);
			Assert.AreEqual("developer", synergy.Connection.Role);
			Assert.IsTrue(synergy.Connection.PollingEnabled);
			Assert.IsTrue(synergy.Project.BaseliningEnabled);
			Assert.IsTrue(synergy.Project.TemplateEnabled);
			Assert.IsNotNull(synergy.Project.ReconcilePaths);
			Assert.AreEqual(2, synergy.Project.ReconcilePaths.Length);
			Assert.AreEqual(@"Product\bin", synergy.Project.ReconcilePaths[0]);
			Assert.AreEqual(@"Product\temp.txt", synergy.Project.ReconcilePaths[1]);
			Assert.AreEqual("Custom Purpose", synergy.Project.Purpose);
			Assert.AreEqual(@"D:\cmsynergy\jdoe", synergy.Connection.HomeDirectory);
			Assert.AreEqual(@"D:\cmsynergy\uidb", synergy.Connection.ClientDatabaseDirectory);
		}

		[Test]
		public void ProtectedDatabase()
		{
			const string status = @"
Warning: Database \\myserver\share\mydatabase on host myserver is protected.  Starting a session is not allowed.
Warning: CM Synergy startup failed.
";
			Synergy synergy = (Synergy) NetReflector.Read(SynergyMother.ConfigValues);
			SynergyCommand command = new SynergyCommand(synergy.Connection, synergy.Project);

			Assert.IsTrue(command.IsDatabaseProtected(status, synergy.Connection.Host, synergy.Connection.Database));
		}

		[Test]
		public void UnprotectedDatabase()
		{
			const string status = @"
Warning: CM Synergy startup failed.
";
			Synergy synergy = (Synergy) NetReflector.Read(SynergyMother.ConfigValues);
			SynergyCommand command = new SynergyCommand(synergy.Connection, synergy.Project);

			Assert.IsFalse(command.IsDatabaseProtected(status, synergy.Connection.Host, synergy.Connection.Database));
		}

		[Test]
		public void DatabaseName()
		{
			// test non-default configured values
			SynergyConnectionInfo info = new SynergyConnectionInfo();
			info.Database = @"\\myserver\share\mydatabase";
			Assert.AreEqual(@"mydatabase", info.DatabaseName);

			info.Database = @"\\myserver\share\mydatabase\";
			Assert.AreEqual(@"mydatabase", info.DatabaseName);
		}

		[Test]
		public void DeadSession()
		{
			const string status = @"
Sessions for user jdoe:

	No sessions found.

Current project could not be identified.
";
			AssertSession(status, "COMPUTERNAME:1234:127.0.0.1", @"\\myserver\share\mydatabase", false);
		}

		[Test]
		public void WrongSession()
		{
			const string status = @"
Sessions for user cruisecontrol:

Developer Interface @ 127.0.0.1:4567
Database: \\myserver\share\mydatabase

Command Interface @ COMPUTERNAME:8888:127.0.0.1
Database: \\myserver\share\mydatabase

Graphical Interface @ COMPUTERNAME:1234:127.0.0.1 (current session)
Database: \\myserver\share\mydatabase

Current project could not be identified.
";
			AssertSession(status, "COMPUTERNAME:3333:127.0.0.1", @"\\myserver\share\mydatabase", false);
		}

		[Test]
		public void AliveCurrentSession()
		{
			const string status = @"
Sessions for user cruisecontrol:

Developer Interface @ 127.0.0.1:4567
Database: \\myserver\share\mydatabase

Command Interface @ COMPUTERNAME:8888:127.0.0.1
Database: \\myserver\share\mydatabase

Graphical Interface @ COMPUTERNAME:1234:127.0.0.1 (current session)
Database: \\myserver\share\mydatabase

Current project could not be identified.
";
			AssertSession(status, "COMPUTERNAME:1234:127.0.0.1", @"\\myserver\share\mydatabase", true);
		}

		[Test]
		public void AliveSession()
		{
			const string status = @"
Sessions for user cruisecontrol:

Developer Interface @ 127.0.0.1:4567
Database: \\myserver\share\mydatabase

Command Interface @ COMPUTERNAME:8888:127.0.0.1
Database: \\myserver\share\mydatabase

Graphical Interface @ COMPUTERNAME:1234:127.0.0.1 (current session)
Database: \\myserver\share\mydatabase

Current project could not be identified.
";
			AssertSession(status, "COMPUTERNAME:8888:127.0.0.1", @"\\myserver\share\mydatabase", true);
		}
		
		[Test]
		public void GetModifications()
		{
			IMock mockCommand = new DynamicMock(typeof(ISynergyCommand));
			mockCommand.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult("output"), new IsAnything());
			mockCommand.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult("output"), new IsAnything(), false);
			mockCommand.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult("output"), new IsAnything(), false);
			mockCommand.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult("output"), new IsAnything(), false);
			IMock mockParser = new DynamicMock(typeof(SynergyParser));
			mockParser.ExpectAndReturn("Parse", new Modification[0], new IsAnything(), new IsAnything(), new NotNull());

			Synergy synergy = new Synergy(new SynergyConnectionInfo(), new SynergyProjectInfo(), (ISynergyCommand) mockCommand.MockInstance, (SynergyParser) mockParser.MockInstance);
			synergy.GetModifications(new IntegrationResult(), new IntegrationResult());
			mockCommand.Verify();
		}

		[Test]
		public void ApplyLabel()
		{
			IMock mockCommand = new DynamicMock(typeof(ISynergyCommand));
			mockCommand.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult(DateTime.MinValue.ToString(CultureInfo.InvariantCulture)), new IsAnything());
			mockCommand.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult("output"), new IsAnything());
			mockCommand.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult("output"), new IsAnything(), false);
			mockCommand.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult("output"), new IsAnything(), false);
			mockCommand.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult("output"), new IsAnything(), false);
			IMock mockParser = new DynamicMock(typeof(SynergyParser));
			mockParser.ExpectAndReturn("Parse", new Modification[0], new IsAnything(), new IsAnything(), new NotNull());

			SynergyConnectionInfo connectionInfo = new SynergyConnectionInfo();
			connectionInfo.FormatProvider = CultureInfo.InvariantCulture;
			Synergy synergy = new Synergy(connectionInfo, new SynergyProjectInfo(), (ISynergyCommand) mockCommand.MockInstance, (SynergyParser) mockParser.MockInstance);
			IntegrationResult integrationResult = new IntegrationResult();
			integrationResult.Status = ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Success;
			synergy.LabelSourceControl(integrationResult);
			mockCommand.Verify();
		}

		[Test]
		public void GetReconfigureTimeShouldHandleNonUSDates()
		{
			string dateString = "samedi 2 décembre 2006";
			IMock mockCommand = new DynamicMock(typeof(ISynergyCommand));
			mockCommand.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult(dateString), new IsAnything());
			SynergyConnectionInfo connectionInfo = new SynergyConnectionInfo();
			connectionInfo.FormatProvider = new CultureInfo("FR-fr");
			Synergy synergy = new Synergy(connectionInfo, new SynergyProjectInfo(), (ISynergyCommand) mockCommand.MockInstance, null);
			DateTime time = synergy.GetReconfigureTime();
			mockCommand.Verify();
		}

		private void AssertSession(string status, string sessionId, string database, bool isAlive)
		{
			SynergyCommand command = new SynergyCommand(null, null);
			bool result = command.IsSessionAlive(status, sessionId, database);
			Assert.IsTrue(isAlive == result, "IsSessionAlive checked failed");
		}
	}
}
