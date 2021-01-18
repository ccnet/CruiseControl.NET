using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class ProjectStatusTests
    {
        #region Test methods
        #region Properties
        [Test]
        public void BuildStageGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.BuildStage = "testing";
            Assert.AreEqual("testing", activity.BuildStage);
        }

        [Test]
        public void StatusGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.Status = ProjectIntegratorState.Running;
            Assert.AreEqual(ProjectIntegratorState.Running, activity.Status);
        }

        [Test]
        public void BuildStatusGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.BuildStatus = IntegrationStatus.Cancelled;
            Assert.AreEqual(IntegrationStatus.Cancelled, activity.BuildStatus);
        }

        [Test]
        public void ActivityGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.Activity = ProjectActivity.Building;
            Assert.AreEqual(ProjectActivity.Building, activity.Activity);
        }

        [Test]
        public void NameGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.Name = "testing";
            Assert.AreEqual("testing", activity.Name);
        }

        [Test]
        public void CategoryGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.Category = "testing";
            Assert.AreEqual("testing", activity.Category);
        }

        [Test]
        public void WebURLGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.WebURL = "testing";
            Assert.AreEqual("testing", activity.WebURL);
        }

        [Test]
        public void LastBuildLabelGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.LastBuildLabel = "testing";
            Assert.AreEqual("testing", activity.LastBuildLabel);
        }

        [Test]
        public void LastSuccessfulBuildLabelGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.LastSuccessfulBuildLabel = "testing";
            Assert.AreEqual("testing", activity.LastSuccessfulBuildLabel);
        }

        [Test]
        public void LastBuildDateGetSetTest()
        {
            DateTime timeNow = DateTime.Now;
            ProjectStatus activity = new ProjectStatus();
            activity.LastBuildDate = timeNow;
            Assert.AreEqual(timeNow, activity.LastBuildDate);
        }

        [Test]
        public void NextBuildTimeGetSetTest()
        {
            DateTime timeNow = DateTime.Now;
            ProjectStatus activity = new ProjectStatus();
            activity.NextBuildTime = timeNow;
            Assert.AreEqual(timeNow, activity.NextBuildTime);
        }

        [Test]
        public void StreamToXMLDefaultConstructorTest()
        {
            ProjectStatus projectStatus = new ProjectStatus();

            XmlSerializer serializer = new XmlSerializer(typeof(ProjectStatus));
            TextWriter writer = new StringWriter();
            XmlSerializerNamespaces nmsp = new XmlSerializerNamespaces();
            nmsp.Add("", "");

            serializer.Serialize(writer, projectStatus, nmsp);

            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?>" + Environment.NewLine +
                            "<projectStatus " +
                            "showForceBuildButton=\"true\" " +
                            "showStartStopButton=\"true\" " +
                            "serverName=\"" + Environment.MachineName + "\" " +
                            "status=\"Running\" " +
                            "buildStatus=\"Success\" " +
                            "queuePriority=\"0\" " +
                            "lastBuildDate=\"0001-01-01T00:00:00\" " +
                            "nextBuildTime=\"0001-01-01T00:00:00\"" +
                            ">" + Environment.NewLine +
                            "  <activity type=\"Sleeping\" />" + Environment.NewLine +
                            "  <parameters />" + Environment.NewLine +
                            "</projectStatus>",
                            writer.ToString());
        }

        [Test]
        public void StreamToXMLPartialConstructorTest()
        {
            string projectName = "test project";
            IntegrationStatus buildStatus = IntegrationStatus.Exception;
            DateTime lastBuildDate = DateTime.Now;
            ProjectStatus projectStatus = new ProjectStatus(projectName, buildStatus, lastBuildDate);

            XmlSerializer serializer = new XmlSerializer(typeof(ProjectStatus));
            TextWriter writer = new StringWriter();
            XmlSerializerNamespaces nmsp = new XmlSerializerNamespaces();
            nmsp.Add("", "");

            serializer.Serialize(writer, projectStatus, nmsp);

            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?>" + Environment.NewLine +
                            "<projectStatus " +
                            "showForceBuildButton=\"true\" " +
                            "showStartStopButton=\"true\" " +
                            "serverName=\"" + Environment.MachineName + "\" " +
                            "status=\"Running\" " +
                            "buildStatus=\"" + buildStatus.ToString() + "\" " +
                            "name=\"" + projectName + "\" " +
                            "queuePriority=\"0\" " +
                            "lastBuildDate=\"" + lastBuildDate.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF") + "\" " +
                            "nextBuildTime=\"0001-01-01T00:00:00\"" +
                            ">" + Environment.NewLine +
                            "  <activity type=\"Sleeping\" />" + Environment.NewLine +
                            "  <parameters />" + Environment.NewLine +
                            "</projectStatus>",
                            writer.ToString());
        }

        [Test]
        public void StreamToXMLFullConstructorTest()
        {
            string projectName = "full test";
            string category = "categ1";
            ProjectActivity activity = ProjectActivity.Building;
            IntegrationStatus buildStatus = IntegrationStatus.Failure;
            ProjectIntegratorState status = ProjectIntegratorState.Stopped;
            string webURL = "someurl";
            DateTime lastBuildDate = DateTime.Now;
            string lastBuildLabel = "lastLabel";
            string lastSuccessfulBuildLabel = "lastSuccess";
            DateTime nextBuildTime = DateTime.Now.AddDays(2);
            string buildStage = "some stage";
            string queue = "someQueue";
            int queuePriority = 25;
            List<ParameterBase> parameters = new List<ParameterBase>();

            ProjectStatus projectStatus = new ProjectStatus(projectName, category, activity, buildStatus,
                                                            status, webURL, lastBuildDate, lastBuildLabel,
                                                            lastSuccessfulBuildLabel, nextBuildTime, buildStage,
                                                            queue, queuePriority, parameters);

            XmlSerializer serializer = new XmlSerializer(typeof(ProjectStatus));
            TextWriter writer = new StringWriter();
            XmlSerializerNamespaces nmsp = new XmlSerializerNamespaces();
            nmsp.Add("", "");

            serializer.Serialize(writer, projectStatus, nmsp);

            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?>" + Environment.NewLine +
                            "<projectStatus " +
                            "stage=\"" + buildStage + "\" " +
                            "showForceBuildButton=\"true\" " +
                            "showStartStopButton=\"true\" " +
                            "serverName=\"" + Environment.MachineName + "\" " +
                            "status=\"" + status.ToString() + "\" " +
                            "buildStatus=\"" + buildStatus.ToString() + "\" " +
                            "name=\"" + projectName + "\" " +
                            "category=\"" + category + "\" " +
                            "queueName=\"" + queue + "\" " +
                            "queuePriority=\"" + queuePriority.ToString() + "\" " +
                            "url=\"" + webURL + "\" " +
                            "lastBuildDate=\"" + lastBuildDate.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF") + "\" " +
                            "lastBuildLabel=\"" + lastBuildLabel + "\" " +
                            "lastSuccessfulBuildLabel=\"" + lastSuccessfulBuildLabel + "\" " +
                            "nextBuildTime=\"" + nextBuildTime.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF") + "\"" +
                            ">" + Environment.NewLine +
                            "  <activity type=\"" + activity.ToString() + "\" />" + Environment.NewLine +
                            "  <parameters />" + Environment.NewLine +
                            "</projectStatus>",
                            writer.ToString());
        }

        [Test]
        public void StreamToXMLFullConstructorWithParametersTest()
        {
            string projectName = "full test";
            string category = "categ1";
            ProjectActivity activity = ProjectActivity.Building;
            IntegrationStatus buildStatus = IntegrationStatus.Failure;
            ProjectIntegratorState status = ProjectIntegratorState.Stopped;
            string webURL = "someurl";
            DateTime lastBuildDate = DateTime.Now;
            string lastBuildLabel = "lastLabel";
            string lastSuccessfulBuildLabel = "lastSuccess";
            DateTime nextBuildTime = DateTime.Now.AddDays(2);
            string buildStage = "some stage";
            string queue = "someQueue";
            int queuePriority = 25;
            List<ParameterBase> parameters = new List<ParameterBase> { new TextParameter("textParam"), new BooleanParameter("boolParam") };

            ProjectStatus projectStatus = new ProjectStatus(projectName, category, activity, buildStatus,
                                                            status, webURL, lastBuildDate, lastBuildLabel,
                                                            lastSuccessfulBuildLabel, nextBuildTime, buildStage,
                                                            queue, queuePriority, parameters);

            string streamedParameters = String.Empty;
            foreach (ParameterBase parameter in parameters)
            {
                XmlSerializerNamespaces parameternmsp = new XmlSerializerNamespaces();
                parameternmsp.Add("", "");

                XmlSerializer parameterSerializer = new XmlSerializer(parameter.GetType());
                TextWriter parameterWriter = new StringWriter();

                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.OmitXmlDeclaration = true;
                using (XmlWriter xmlWriter = XmlWriter.Create(parameterWriter, writerSettings))
                {
                    parameterSerializer.Serialize(xmlWriter, parameter, parameternmsp);
                }
                string streamedParameter = parameterWriter.ToString();
                streamedParameter = Regex.Replace(streamedParameter, parameter.GetType().Name, "parameter d3p1:type=\"" + parameter.GetType().Name + "\"", RegexOptions.IgnoreCase);
                streamedParameter = Regex.Replace(streamedParameter, "/>", "xmlns:d3p1=\"http://www.w3.org/2001/XMLSchema-instance\" />", RegexOptions.IgnoreCase);
                streamedParameters += "    " + streamedParameter + "" + Environment.NewLine;
            }

            XmlSerializerNamespaces nmsp = new XmlSerializerNamespaces();
            nmsp.Add("", "");

            XmlSerializer serializer = new XmlSerializer(typeof(ProjectStatus));
            TextWriter writer = new StringWriter();

            serializer.Serialize(writer, projectStatus, nmsp);

            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?>" + Environment.NewLine +
                            "<projectStatus " +
                            "stage=\"" + buildStage + "\" " +
                            "showForceBuildButton=\"true\" " +
                            "showStartStopButton=\"true\" " +
                            "serverName=\"" + Environment.MachineName + "\" " +
                            "status=\"" + status.ToString() + "\" " +
                            "buildStatus=\"" + buildStatus.ToString() + "\" " +
                            "name=\"" + projectName + "\" " +
                            "category=\"" + category + "\" " +
                            "queueName=\"" + queue + "\" " +
                            "queuePriority=\"" + queuePriority.ToString() + "\" " +
                            "url=\"" + webURL + "\" " +
                            "lastBuildDate=\"" + lastBuildDate.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF") + "\" " +
                            "lastBuildLabel=\"" + lastBuildLabel + "\" " +
                            "lastSuccessfulBuildLabel=\"" + lastSuccessfulBuildLabel + "\" " +
                            "nextBuildTime=\"" + nextBuildTime.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF") + "\"" +
                            ">" + Environment.NewLine +
                            "  <activity type=\"" + activity.ToString() + "\" />" + Environment.NewLine +
                            "  <parameters>" + Environment.NewLine + 
                            streamedParameters +
                            "  </parameters>" + Environment.NewLine +
                            "</projectStatus>",
                            writer.ToString());
        }
        #endregion
        #endregion
    }
}
