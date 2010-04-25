using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Exortech.NetReflector;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class ConditionalTaskTest
    {
        MockRepository mocks = new MockRepository();

        [SetUp]
        protected void SetUp()
        {
        }

        [TearDown]
        protected void TearDown()
        {
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        [Test]
        [Description("XMLInterface Fail")]
        [Category("Category")]
        [ExpectedException(typeof(NetReflectorException))]
        public void ConditionalTaskTest_XMLInterface_Fail(
            [Values(
                "<conditional/>",
                "<Conditional/>",
                "<conditional><Conditions/></conditional>",
                "<conditional><tasks/></conditional>",
                "<conditional><elseTasks/></conditional>",
                "<conditional><tasks/><elseTasks/></conditional>"
                )]
            string xml)
        {
            Debug.WriteLine(xml);
            ConditionalTask result = NetReflector.Read(xml) as ConditionalTask;
        }

        [Test]
        [Description("XMLInterface Success")]
        [Category("Category")]
        public void ConditionalTaskTest_XMLInterface_Success(
            [Values(
                "<conditional><conditions/><tasks/><elseTasks/></conditional>"
                )]
            string xml)
        {
            Debug.WriteLine(xml);
            ConditionalTask result = NetReflector.Read(xml) as ConditionalTask;
        }


        [Test]
        [Description("nullTask throwsArgumentNullException")]
        [Category("null test")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConditionalTaskTest_nullTask_throwsArgumentNullException()
        {
            ConditionalTask result = new ConditionalTask();
            result.Run( IntegrationResultMother.CreateSuccessful());
        }

        [Test]
        [Description("Condition True Run Task")]
        [Category("Category")]
        public void ConditionalTaskTest_ConditionTrue_RunTask()
        {
            ITaskCondition condition = mocks.StrictMock<ITaskCondition>();
            Expect.Call(condition.Eval(null)).IgnoreArguments().Return(true);
            ITask runTask = mocks.StrictMock<ITask>();
            Expect.Call(delegate { runTask.Run(null); }).IgnoreArguments();
            ITask noRunTask = mocks.StrictMock<ITask>();
            IIntegrationResult ResultMock = IntegrationResultMother.CreateSuccessful(); //mocks.StrictMock<IIntegrationResult>();
            //Expect.Call(ResultMock.Status).PropertyBehavior().Return(IntegrationStatus.Success).IgnoreArguments().Repeat.Any();

            mocks.ReplayAll();

            ConditionalTask task = new ConditionalTask();
            task.TaskConditions = new[] { condition };
            task.Tasks = new[] { runTask };
            task.ElseTasks = new[] { noRunTask };
            task.Run(ResultMock);

            mocks.VerifyAll();
        }

        [Test]
        [Description("Condition False Run ElseTask")]
        [Category("Category")]
        public void ConditionalTaskTest_ConditionFalse_RunElseTask()
        {
            ITaskCondition condition = mocks.StrictMock<ITaskCondition>();
            Expect.Call(condition.Eval(null)).IgnoreArguments().Return(false);
            ITask runTask = mocks.StrictMock<ITask>();
            Expect.Call(delegate { runTask.Run(null); }).IgnoreArguments();
            ITask noRunTask = mocks.StrictMock<ITask>();
            IIntegrationResult ResultMock = IntegrationResultMother.CreateSuccessful();
            //mocks.StrictMock<IIntegrationResult>();
            //BuildProgressInformation bpi = mocks.StrictMock<BuildProgressInformation>
            //Expect.Call(ResultMock.Status).PropertyBehavior().Return(IntegrationStatus.Success).IgnoreArguments().Repeat.Any();
            //Expect.Call(ResultMock.BuildProgressInformation).IgnoreArguments().Return()
            mocks.ReplayAll();

            ConditionalTask task = new ConditionalTask();
            task.TaskConditions = new[] { condition };
            task.ElseTasks = new[] { runTask };
            task.Tasks = new[] { noRunTask };
            task.Run(ResultMock);

            mocks.VerifyAll();
        }

        [Test]
        [Description("xmlinterface pass")]
        [Category("Category")]
        public void ConditionalTaskTest_xmlinterface_pass()
        {
            string testXml = "<conditional><conditions>" +
                "<andCondition><conditions>" +
                "<buildCondition value=\"ForceBuild\" />" +
                "<statusCondition value=\"Success\" />" +
                "<folderExistsCondition folder=\"\\\\publicationServer\\c$\\inetpub\\Webproject\\\" />" +
                "<orCondition><conditions>" +
                "<fileExistsCondition file=\"d:\\Buildfolder\\Webproject\\project.csproj\" />" +
                "<fileExistsCondition file=\"d:\\Buildfolder\\Webproject\\webProject.csproj\" />" +
                "</conditions></orCondition>" +
                "<urlPingCondition url=\"http:\\MyWebsite\\WebProject\\default.aspx\" />" +
                "</conditions></andCondition>" +
                "</conditions>" +
                "<tasks><commentTask message=\"publish web site\" /></tasks>" +
                "<elseTasks><commentTask message=\"error deploying website\" FailTask=\"True\" /></elseTasks>" +
                "</conditional>";

            Debug.WriteLine(testXml);
            ConditionalTask result = NetReflector.Read(testXml) as ConditionalTask;
        }
    }
}
