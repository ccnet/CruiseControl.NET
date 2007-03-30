using System;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Telelogic;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Telelogic
{
	[TestFixture]
	public sealed class SynergyCommandBuilderTest : IntegrationFixture
	{
		private SynergyConnectionInfo connection;
		private SynergyProjectInfo project;
		private IIntegrationResult result;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Synergy synergy = (Synergy) NetReflector.Read(SynergyMother.ConfigValues);

			connection = synergy.Connection;
			connection.Host = "localhost";
			connection.Username = "jdoe";
			connection.Password = "password";
			connection.Role = "build_mgr";
			connection.Database = @"\\server\share\dbname";
			connection.Delimiter = '-';
			connection.WorkingDirectory = @"C:\Program Files\Telelogic\CM Synergy 6.3\bin";
			connection.HomeDirectory = @"C:\cmsynergy\jdoe";
			connection.ClientDatabaseDirectory = @"C:\cmsynergy\uidb";
			connection.SessionId = "LOCALHOST:1234:127.0.0.1";
			project = synergy.Project;
			project.ProjectSpecification = "MyProject-MyProject_Int";
			project.ObjectName = "MyProject-MyProject_Int:project:1";
			project.Purpose = "Integration Testing";
			project.Release = "MyProduct/1.0";
			project.TaskFolder = 1234;
		}

		[SetUp]
		public void SetUp()
		{
			result = Integration("MyCCNETProject", String.Empty);

			result.Modifications = new Modification[]
				{
					new Modification(), new Modification(), new Modification(), new Modification()
				};
			result.Modifications[0].ChangeNumber = 100;
			result.Modifications[1].ChangeNumber = 2000;
			result.Modifications[2].ChangeNumber = 30000;
			// duplicate the last change number
			result.Modifications[3].ChangeNumber = 30000;

			result.Label = "765";
			((IntegrationResult) result).StartTime = new DateTime(1970, 1, 1);
		}

		[Test]
		public void Heartbeat()
		{
			ProcessInfo actual = SynergyCommandBuilder.Heartbeat(connection);
			ValidateProcessInfo(actual, "status");
		}

		[Test]
		public void Start()
		{
			ProcessInfo actual = SynergyCommandBuilder.Start(connection, project);
			ValidateProcessInfo(actual, @"start -nogui -q -m -h ""localhost"" -d ""\\server\share\dbname"" -p ""MyProject-MyProject_Int"" -n ""jdoe"" -pw ""password"" -r ""build_mgr"" -u ""C:\cmsynergy\uidb"" -home ""C:\cmsynergy\jdoe""");
		}

		[Test]
		public void Stop()
		{
			ProcessInfo actual = SynergyCommandBuilder.Stop(connection);
			ValidateProcessInfo(actual, "stop");
		}

		[Test]
		public void GetProjectName()
		{
			ProcessInfo actual = SynergyCommandBuilder.GetProjectFullName(connection, project);
			ValidateProcessInfo(actual, @"properties /format ""%objectname"" /p ""MyProject-MyProject_Int""");
		}

		[Test]
		public void GetSubProjects()
		{
			ProcessInfo actual = SynergyCommandBuilder.GetSubProjects(connection, project);
			ValidateProcessInfo(actual, @"query hierarchy_project_members('MyProject-MyProject_Int:project:1', 'none')");
		}

		[Test]
		public void SetProjectRelease()
		{
			ProcessInfo actual = SynergyCommandBuilder.SetProjectRelease(connection, project);
			ValidateProcessInfo(actual, @"attribute /m release /v ""MyProduct/1.0"" @ ");
		}

		[Test]
		public void GetLastReconfigureTime()
		{
			ProcessInfo actual = SynergyCommandBuilder.GetLastReconfigureTime(connection, project);
			ValidateProcessInfo(actual, @"attribute /show reconf_time ""MyProject-MyProject_Int:project:1""");
		}

		[Test]
		public void GetDelimiter()
		{
			ProcessInfo actual = SynergyCommandBuilder.GetDelimiter(connection);
			ValidateProcessInfo(actual, @"delimiter");
		}

		[Test]
		public void GetDcmDelimiter()
		{
			ProcessInfo actual = SynergyCommandBuilder.GetDcmDelimiter(connection);
			ValidateProcessInfo(actual, @"dcm /show /delimiter");
		}

		[Test]
		public void GetDcmSettings()
		{
			ProcessInfo actual = SynergyCommandBuilder.GetDcmSettings(connection);
			ValidateProcessInfo(actual, @"dcm /show /settings");
		}

		[Test]
		public void GetWorkArea()
		{
			ProcessInfo actual = SynergyCommandBuilder.GetWorkArea(connection, project);
			ValidateProcessInfo(actual, @"info /format ""%wa_path\%name"" /project ""MyProject-MyProject_Int""");
		}

		[Test]
		public void UseReconfigureTemplate()
		{
			ProcessInfo actual = SynergyCommandBuilder.UseReconfigureTemplate(connection, project);
			ValidateProcessInfo(actual, @"reconfigure_properties /reconf_using template /recurse ""MyProject-MyProject_Int""");
		}

		[Test]
		public void Reconfigure()
		{
			ProcessInfo actual = SynergyCommandBuilder.Reconfigure(connection, project);
			ValidateProcessInfo(actual, @"reconfigure /recurse /keep_subprojects /project ""MyProject-MyProject_Int""");
		}

		[Test]
		public void Reconcile()
		{
			ProcessInfo actual = SynergyCommandBuilder.Reconcile(connection, project, @"c:\ccm_wa\project-ver\project\folder");
			ValidateProcessInfo(actual, @"reconcile /consider_uncontrolled /missing_wa_file /recurse /update_wa ""c:\ccm_wa\project-ver\project\folder""");
		}

		[Test]
		public void UpdateReconfigureProperites()
		{
			ProcessInfo actual = SynergyCommandBuilder.UpdateReconfigureProperites(connection, project);
			ValidateProcessInfo(actual, @"reconfigure_properties /refresh /recurse ""MyProject-MyProject_Int""");
		}

		[Test]
		public void GetNewTasks()
		{
			ProcessInfo actual = SynergyCommandBuilder.GetNewTasks(connection, project, DateTime.MinValue);
			string expectedDateString = SynergyCommandBuilder.FormatCommandDate(DateTime.MinValue);
			string expectedCommand = string.Format(@"query /type task /format ""%displayname #### %task_number #### %completion_date #### %resolver #### %task_synopsis #### "" /nf /u /no_sort ""status != 'task_automatic' and status != 'excluded' and completion_date >= time('{0}') and not ( is_task_in_folder_of(folder('1234')) or is_task_in_folder_of(is_folder_in_rp_of(is_baseline_project_of('MyProject-MyProject_Int:project:1'))) or is_task_in_rp_of(is_baseline_project_of('MyProject-MyProject_Int:project:1')) ) and (is_task_in_folder_of(is_folder_in_rp_of('MyProject-MyProject_Int:project:1')) or is_task_in_rp_of('MyProject-MyProject_Int:project:1'))""", expectedDateString);
			ValidateProcessInfo(actual, expectedCommand);
		}

		[Test]
		public void GetTaskObjects()
		{
			ProcessInfo actual = SynergyCommandBuilder.GetTaskObjects(connection, project);
			ValidateProcessInfo(actual, @"task /show objects /no_sort /u @");
		}

		[Test]
		public void GetNewObjects()
		{
			ProcessInfo actual = SynergyCommandBuilder.GetObjectPaths(connection, project);
			ValidateProcessInfo(actual, @"finduse @");
		}

		[Test]
		public void AddTasksToFolder()
		{
			ProcessInfo actual = SynergyCommandBuilder.AddTasksToFolder(connection, project, result);
			ValidateProcessInfo(actual, @"folder /modify /add_tasks ""100,2000,30000"" /y ""1234""");
		}

		[Test]
		public void AddLabelToTaskComment()
		{
			ProcessInfo actual = SynergyCommandBuilder.AddLabelToTaskComment(connection, project, result);
			string expected = string.Format(@"task /modify /description ""Integrated Successfully with CruiseControl.NET project 'MyCCNETProject' build '765' on {0}"" ""100,2000,30000""", result.StartTime);
			ValidateProcessInfo(actual, expected);
		}

		[Test]
		public void CreateBaseline()
		{
			ProcessInfo actual = SynergyCommandBuilder.CreateBaseline(connection, project, result);
			string expected = string.Format(@"baseline /create ""19700101 CCNET build 765 "" /description ""Integrated Successfully with CruiseControl.NET project 'MyCCNETProject' build '765' on {0}"" /release ""MyProduct/1.0"" /purpose ""Integration Testing"" /p ""MyProject-MyProject_Int"" /subprojects", result.StartTime);
			ValidateProcessInfo(actual, expected);
		}

		private void ValidateProcessInfo(ProcessInfo actual, string arguments)
		{
			Assert.IsNotNull(actual);
			Assert.AreEqual(connection.Executable, actual.FileName);
			Assert.AreEqual(connection.WorkingDirectory, actual.WorkingDirectory);
			Assert.AreEqual(arguments, actual.Arguments);
		}
	}
}