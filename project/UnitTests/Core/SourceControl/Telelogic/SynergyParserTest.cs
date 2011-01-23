using System;
using System.Collections;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Telelogic;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Telelogic
{
	[TestFixture]
	public sealed class SynergyParserTest
	{
		private SynergyConnectionInfo connection;
		private SynergyProjectInfo project;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			connection = new SynergyConnectionInfo();
			connection.Host = "localhost";
			connection.Database = @"\\server\share\mydb";
			connection.Delimiter = '-';
			project = new SynergyProjectInfo();
			project.ProjectSpecification = "MyProject-MyProject_Int";
		}

		[Test]
		public void ConnectionDefaults()
		{
			SynergyConnectionInfo actual = new SynergyConnectionInfo();

			Assert.IsNull(actual.Database, "#A1");
			Assert.IsNull(actual.SessionId, "#A2");
			Assert.AreEqual(3600, actual.Timeout, "#A3");
			Assert.AreEqual("ccm.exe", actual.Executable, "#A4");
			Assert.AreEqual(Environment.ExpandEnvironmentVariables("%USERNAME%"), actual.Username, "#A5");
			Assert.AreEqual("build_mgr", actual.Role, "#A6");
			Assert.AreEqual('-', actual.Delimiter, "#A7");
			Assert.AreEqual(Environment.ExpandEnvironmentVariables(@"%SystemDrive%\cmsynergy\%USERNAME%"), actual.HomeDirectory, "#A8");
			Assert.AreEqual(Environment.ExpandEnvironmentVariables(@"%SystemDrive%\cmsynergy\uidb"), actual.ClientDatabaseDirectory, "#A9");
			Assert.AreEqual(Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\Telelogic\CM Synergy 6.3\bin"), actual.WorkingDirectory, "#A10");
		}

		[Test]
		public void ProjectDefaults()
		{
			SynergyProjectInfo actual = new SynergyProjectInfo();

			Assert.IsNull(actual.Release);
			Assert.IsNull(actual.ProjectSpecification);
			Assert.AreEqual(0, actual.TaskFolder);
			Assert.AreEqual(DateTime.MinValue, actual.LastReconfigureTime);
			Assert.IsFalse(actual.BaseliningEnabled);
			Assert.AreEqual("Integration Testing", actual.Purpose);
		}

		[Test]
		public void CanParseNewTasks()
		{
			SynergyParser parser = new SynergyParser();

			// ngw_de0157~milligan_integrate
			Hashtable actual = parser.ParseTasks(SynergyMother.NewTaskInfo);

			// validate that a collection of 8 comments is returned
			Assert.IsNotNull(actual);
			Assert.AreEqual(6, actual.Count);

			// validate that each comment and timestamp exists, and defaults to String.Empty
			foreach (DictionaryEntry comment in actual)
			{
				Assert.IsNotNull(comment);
				SynergyParser.SynergyTaskInfo info = (SynergyParser.SynergyTaskInfo) comment.Value;
				Assert.IsNotNull(info.TaskNumber);
				Assert.IsNotNull(info.TaskSynopsis);
				Assert.IsNotNull(info.Resolver);
			}

			// test that the right comments are returned, and that the order of retrieval
			// does not matter
			if (null != actual["15"])
			{
				Assert.AreEqual("lorem ipsum dolerem ", ((SynergyParser.SynergyTaskInfo) actual["15"]).TaskSynopsis);
				Assert.AreEqual("Insulated Development projects for release PRODUCT/1.0", ((SynergyParser.SynergyTaskInfo) actual["22"]).TaskSynopsis);
				Assert.AreEqual("jdoe's Insulated Development projects", ((SynergyParser.SynergyTaskInfo) actual["21"]).TaskSynopsis);
				Assert.AreEqual("IGNORE THIS Sample Task ", ((SynergyParser.SynergyTaskInfo) actual["99"]).TaskSynopsis);
				Assert.AreEqual("the quick brown fox jumped over the lazy dog ", ((SynergyParser.SynergyTaskInfo) actual["17"]).TaskSynopsis);
				Assert.AreEqual("0123456789 ~!@#$%^&*()_=", ((SynergyParser.SynergyTaskInfo) actual["1"]).TaskSynopsis);
			}
			else
			{
				Assert.AreEqual("lorem ipsum dolerem ", ((SynergyParser.SynergyTaskInfo) actual["wwdev#15"]).TaskSynopsis);
				Assert.AreEqual("Insulated Development projects for release PRODUCT/1.0", ((SynergyParser.SynergyTaskInfo) actual["wwdev#22"]).TaskSynopsis);
				Assert.AreEqual("jdoe's Insulated Development projects", ((SynergyParser.SynergyTaskInfo) actual["wwdev#21"]).TaskSynopsis);
				Assert.AreEqual("IGNORE THIS Sample Task ", ((SynergyParser.SynergyTaskInfo) actual["wwdev#99"]).TaskSynopsis);
				Assert.AreEqual("the quick brown fox jumped over the lazy dog ", ((SynergyParser.SynergyTaskInfo) actual["wwdev#17"]).TaskSynopsis);
				Assert.AreEqual("0123456789 ~!@#$%^&*()_=", ((SynergyParser.SynergyTaskInfo) actual["wwdev#1"]).TaskSynopsis);
			}

			// assert that tasks not in the original list are null
			Assert.IsNull(actual["123456789"]);
		}

		[Test]
		public void ParseNewObjects()
		{
			ParseNewObjects(SynergyMother.NewTaskInfo, SynergyMother.NewObjects);
		}

		[Test]
		public void ParseDCMObjects()
		{
			ParseNewObjects(SynergyMother.NewDcmTaskInfo, SynergyMother.NewDCMObjects);
		}

		[Test]
		public void ParseWhenTasksAreEmpty()
		{
			SynergyParser parser = new SynergyParser();
			// set the from date to be one week back
			DateTime from = DateTime.Now.AddDays(-7L);

			Modification[] actual = parser.Parse(string.Empty, SynergyMother.NewObjects, from);
			Assert.AreEqual(7, actual.Length);
			Assert.AreEqual("15", actual[0].ChangeNumber);
			Assert.AreEqual("9999", actual[6].ChangeNumber);
		}

		private void ParseNewObjects(string newTasks, string newObjects)
		{
			SynergyParser parser = new SynergyParser();
			// set the from date to be one week back
			DateTime from = DateTime.Now.AddDays(-7L);

			Modification[] actual = parser.Parse(newTasks, newObjects, from);

			Assert.IsNotNull(actual);
			Assert.AreEqual(7, actual.Length);

			foreach (Modification modification in actual)
			{
				Assert.AreEqual("jdoe", modification.EmailAddress);
				Assert.AreEqual("jdoe", modification.UserName);
				Assert.IsNull(modification.Url);
			}

			Assert.AreEqual("15", actual[0].ChangeNumber);
			Assert.AreEqual(@"sourcecontrol-3", actual[0].FileName);
			Assert.AreEqual(@"$/MyProject/CruiseControl.NET/project/core", actual[0].FolderName);
			Assert.AreEqual(@"dir", actual[0].Type);
			Assert.AreEqual(@"lorem ipsum dolerem ", actual[0].Comment);

			// test that the last task number is used when an object is associated with multiple tasks
			Assert.AreEqual("21", actual[1].ChangeNumber);
			Assert.AreEqual(@"Synergy.cs-1", actual[1].FileName);
			Assert.AreEqual(@"$/MyProject/CruiseControl.NET/project/core/sourcecontrol", actual[1].FolderName);
			Assert.AreEqual(@"ms_cs", actual[1].Type);
			// check that trailing spaces are honored
			Assert.AreEqual("jdoe's Insulated Development projects", actual[1].Comment);

			Assert.AreEqual("22", actual[2].ChangeNumber);
			// check that branched version numbers are parsed
			Assert.AreEqual(@"SynergyCommandBuilder.cs-1.1.1", actual[2].FileName);
			Assert.AreEqual(@"$/MyProject/CruiseControl.NET/project/core/sourcecontrol", actual[2].FolderName);
			Assert.AreEqual(@"ms_cs", actual[2].Type);
			Assert.AreEqual("Insulated Development projects for release PRODUCT/1.0", actual[2].Comment);

			Assert.AreEqual("22", actual[3].ChangeNumber);
			Assert.AreEqual(@"SynergyConnectionInfo.cs-2", actual[3].FileName);
			Assert.AreEqual(@"$/MyProject/CruiseControl.NET/project/core/sourcecontrol", actual[3].FolderName);
			Assert.AreEqual(@"ms_cs", actual[3].Type);
			// check that trailing spaces are honored
			Assert.AreEqual("Insulated Development projects for release PRODUCT/1.0", actual[3].Comment);

			Assert.AreEqual("1", actual[4].ChangeNumber);
			// check that branched version numbers are parsed
			Assert.AreEqual(@"SynergyHistoryParser.cs-2.2.1", actual[4].FileName);
			Assert.AreEqual(@"$/MyProject/CruiseControl.NET/project/core/sourcecontrol", actual[4].FolderName);
			Assert.AreEqual(@"ms_cs", actual[4].Type);
			// check that trailing spaces are honored
			Assert.AreEqual(@"0123456789 ~!@#$%^&*()_=", actual[4].Comment);

			Assert.AreEqual("17", actual[5].ChangeNumber);
			// check that branched version numbers are parsed
			Assert.AreEqual(@"SynergyProjectInfo.cs-1", actual[5].FileName);
			Assert.AreEqual(@"$/MyProject/CruiseControl.NET/project/core/sourcecontrol", actual[5].FolderName);
			Assert.AreEqual(@"ms_cs", actual[5].Type);
			// check that reserved regular expression classes are escaped
			Assert.AreEqual(@"the quick brown fox jumped over the lazy dog ", actual[5].Comment);

			Assert.AreEqual("9999", actual[6].ChangeNumber);
			// check that branched version numbers are parsed
			Assert.AreEqual(@"NotUsed-10", actual[6].FileName);
			Assert.AreEqual(@"", actual[6].FolderName);
			Assert.AreEqual(@"dir", actual[6].Type);
			Assert.IsNull(actual[6].Comment);
		}
	}
}