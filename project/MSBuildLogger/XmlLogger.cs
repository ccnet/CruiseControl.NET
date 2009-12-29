#region Using directives

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Xml;
#endregion

namespace ThoughtWorks.CruiseControl.MSBuild
{
    /// <summary>
    /// Implements an XML logger for MSBuild.
    /// </summary>
    public class XmlLogger : Logger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlLogger"/> class.
        /// </summary>
        public XmlLogger()
        { }

        private string outputPath;
        private XmlDocument outputDoc;
        private XmlElement currentElement;

        /// <summary>
        /// Initializes the logger by attaching events and parsing command line.
        /// </summary>
        /// <param name="eventSource">The event source.</param>
        public override void Initialize(IEventSource eventSource)
        {
            outputPath = this.Parameters;

            this.outputDoc = new XmlDocument();
            this.outputDoc.AppendChild(this.outputDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
            this.outputDoc.AppendChild(this.outputDoc.CreateElement(XmlLoggerElements.Build));
            this.currentElement = this.outputDoc.DocumentElement;

            eventSource.ErrorRaised += new BuildErrorEventHandler(eventSource_ErrorRaised);
            eventSource.WarningRaised += new BuildWarningEventHandler(eventSource_WarningRaised);

            eventSource.BuildStarted += new BuildStartedEventHandler(eventSource_BuildStartedHandler);
            eventSource.BuildFinished += new BuildFinishedEventHandler(eventSource_BuildFinishedHandler);

            if (Verbosity != LoggerVerbosity.Quiet) // minimal and above
            {
                eventSource.MessageRaised += new BuildMessageEventHandler(eventSource_MessageHandler);
                eventSource.CustomEventRaised += new CustomBuildEventHandler(eventSource_CustomBuildEventHandler);

                eventSource.ProjectStarted += new ProjectStartedEventHandler(eventSource_ProjectStartedHandler);
                eventSource.ProjectFinished += new ProjectFinishedEventHandler(eventSource_ProjectFinishedHandler);

                if (Verbosity != LoggerVerbosity.Minimal) // normal and above
                {
                    eventSource.TargetStarted += new TargetStartedEventHandler(eventSource_TargetStartedHandler);
                    eventSource.TargetFinished += new TargetFinishedEventHandler(eventSource_TargetFinishedHandler);

                    if (Verbosity != LoggerVerbosity.Normal) // only detailed and diagnostic
                    {
                        eventSource.TaskStarted += new TaskStartedEventHandler(eventSource_TaskStartedHandler);
                        eventSource.TaskFinished += new TaskFinishedEventHandler(eventSource_TaskFinishedHandler);
                    }
                }
            }
        }

        public override void Shutdown()
        {
            if (String.IsNullOrEmpty(this.outputPath))
            {
                // Output to console.
                Console.WriteLine(this.outputDoc.OuterXml);
            }
            else
            {
                this.outputDoc.Save(this.outputPath);
            }
        }

        #region Event Handlers

        private void eventSource_BuildStartedHandler(object sender, BuildStartedEventArgs e)
        {
            LogStageStarted(XmlLoggerElements.Build, "", "", e.Timestamp);
        }

        private void eventSource_BuildFinishedHandler(object sender, BuildFinishedEventArgs e)
        {
            LogStageFinished(e.Succeeded, e.Timestamp);
        }

        private void eventSource_ProjectStartedHandler(object sender, ProjectStartedEventArgs e)
        {
            LogStageStarted(XmlLoggerElements.Project, e.TargetNames, e.ProjectFile, e.Timestamp);
        }

        private void eventSource_ProjectFinishedHandler(object sender, ProjectFinishedEventArgs e)
        {
            LogStageFinished(e.Succeeded, e.Timestamp);
        }

        private void eventSource_TargetStartedHandler(object sender, TargetStartedEventArgs e)
        {
            LogStageStarted(XmlLoggerElements.Target, e.TargetName, "", e.Timestamp);
        }

        private void eventSource_TargetFinishedHandler(object sender, TargetFinishedEventArgs e)
        {
            LogStageFinished(e.Succeeded, e.Timestamp);
        }

        private void eventSource_TaskStartedHandler(object sender, TaskStartedEventArgs e)
        {
            LogStageStarted(XmlLoggerElements.Task, e.TaskName, e.ProjectFile, e.Timestamp);
        }

        private void eventSource_TaskFinishedHandler(object sender, TaskFinishedEventArgs e)
        {
            LogStageFinished(e.Succeeded, e.Timestamp);
        }

        void eventSource_WarningRaised(object sender, BuildWarningEventArgs e)
        {
            LogErrorOrWarning(XmlLoggerElements.Warning, e.Message, e.Code, e.File, e.LineNumber, e.ColumnNumber, e.Timestamp);
        }

        void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            LogErrorOrWarning(XmlLoggerElements.Error, e.Message, e.Code, e.File, e.LineNumber, e.ColumnNumber, e.Timestamp);
        }

        private void eventSource_MessageHandler(object sender, BuildMessageEventArgs e)
        {
            LogMessage(XmlLoggerElements.Message, e.Message, e.Importance, e.Timestamp);
        }

        private void eventSource_CustomBuildEventHandler(object sender, CustomBuildEventArgs e)
        {
            LogMessage(XmlLoggerElements.Custom, e.Message, MessageImportance.Normal, e.Timestamp);
        }

        #endregion

        #region Logging

        private void LogStageStarted(string elementName, string stageName, string file, DateTime timeStamp)
        {
            // use the default root for the build element
            if (elementName != XmlLoggerElements.Build)
            {
                XmlElement stageElement = this.outputDoc.CreateElement(elementName);
                this.currentElement.AppendChild(stageElement);
                this.currentElement = stageElement;
            }

            SetAttribute(currentElement, stageName, XmlLoggerAttributes.Name);
            SetAttribute(currentElement, file, XmlLoggerAttributes.File);
            SetAttribute(currentElement, timeStamp, XmlLoggerAttributes.StartTime);

        }

        private void LogStageFinished(bool succeded, DateTime timeStamp)
        {
            DateTime startTime = DateTime.Parse(currentElement.GetAttribute(XmlLoggerAttributes.StartTime), DateTimeFormatInfo.InvariantInfo);
            SetAttribute(currentElement, timeStamp - startTime, XmlLoggerAttributes.ElapsedTime);

            SetAttribute(currentElement, succeded, XmlLoggerAttributes.Success);

            if (this.currentElement.ParentNode is XmlElement)
            {
                XmlElement parentElement = (XmlElement)this.currentElement.ParentNode;

                // don't put element's that don't contain any messages
                if (!currentElement.HasChildNodes && Verbosity != LoggerVerbosity.Detailed && Verbosity != LoggerVerbosity.Diagnostic)
                    parentElement.RemoveChild(currentElement);

                this.currentElement = parentElement;
            }
        }

        private void LogErrorOrWarning(string messageType, string message, string code, string file, int lineNumber, int columnNumber, DateTime timestamp)
        {
            XmlElement messageElement = this.outputDoc.CreateElement(messageType);
            SetAttribute(messageElement, code, XmlLoggerAttributes.Code);

            SetAttribute(messageElement, file, XmlLoggerAttributes.File);
            SetAttribute(messageElement, lineNumber, XmlLoggerAttributes.LineNumber);
            SetAttribute(messageElement, columnNumber, XmlLoggerAttributes.ColumnNumber);

            SetAttribute(messageElement, timestamp, XmlLoggerAttributes.TimeStamp);

            // Escape < and > if this is not a "Properties" message.  This is because in a Properties
            // message, we want the ability to insert legal XML, but otherwise we can get malformed
            // XML that will cause the parser to fail.
            WriteMessage(messageElement, message, code != "Properties");

            this.currentElement.AppendChild(messageElement);
        }

        private void LogMessage(string messageType, string message, MessageImportance importance, DateTime timestamp)
        {
            if (importance == MessageImportance.Low
                && Verbosity != LoggerVerbosity.Detailed
                && Verbosity != LoggerVerbosity.Diagnostic)
                return;

            if (importance == MessageImportance.Normal
                && (Verbosity == LoggerVerbosity.Minimal
                    || Verbosity == LoggerVerbosity.Quiet))
                return;

            XmlElement messageElement = this.outputDoc.CreateElement(messageType);

            SetAttribute(messageElement, importance, XmlLoggerAttributes.Importance);

            if (Verbosity == LoggerVerbosity.Diagnostic)
                SetAttribute(messageElement, timestamp, XmlLoggerAttributes.TimeStamp);

            WriteMessage(messageElement, message, false);

            this.currentElement.AppendChild(messageElement);
        }

        private void WriteMessage(XmlElement element, string message, bool escapeLtGt)
        {
            if (!string.IsNullOrEmpty(message))
            {
                string temp = message.Replace("&", "&amp;");

                if (escapeLtGt)
                {
                    temp = temp.Replace("<", "&lt;");
                    temp = temp.Replace(">", "&gt;");
                }
                element.AppendChild(this.outputDoc.CreateCDataSection(temp));
            }
        }

        private void SetAttribute(XmlElement element, object obj, string name)
        {
            if (obj == null)
                return;

            Type t = obj.GetType();
            if (t == typeof(int))
            {
                if (Int32.Parse(obj.ToString()) > 0)	//????
                {
                    element.SetAttribute(name, obj.ToString());
                }
            }
            else if (t == typeof(DateTime))
            {
                DateTime dateTime = (DateTime)obj;
                element.SetAttribute(name, dateTime.ToString("G", DateTimeFormatInfo.InvariantInfo));
            }
            else if (t == typeof(TimeSpan))
            {
                // format TimeSpan to show only integral seconds
                double seconds = ((TimeSpan)obj).Seconds;
                TimeSpan whole = TimeSpan.FromSeconds(Math.Truncate(seconds));
                //int rest = (int)(Math.Round(seconds - Math.Truncate(seconds), 2) * 100.0);
                //if (rest!=0)
                //    element.SetAttribute(name, whole.ToString()+"."+rest.ToString(NumberFormatInfo.InvariantInfo));
                //else
                element.SetAttribute(name, whole.ToString());
            }
            else if (t == typeof(Boolean))
            {
                element.SetAttribute(name, obj.ToString().ToLower());
            }
            else if (t == typeof(MessageImportance))
            {
                MessageImportance importance = (MessageImportance)obj;
                element.SetAttribute(name, importance.ToString().ToLower());
            }
            else
            {
                if (obj.ToString().Length > 0)
                {
                    element.SetAttribute(name, obj.ToString());
                }
            }
        }

        #endregion

        #region Constants

        internal sealed class XmlLoggerElements
        {
            private XmlLoggerElements()
            { }

            public const string Build = "msbuild";
            public const string Error = "error";
            public const string Warning = "warning";
            public const string Message = "message";
            public const string Project = "project";
            public const string Target = "target";
            public const string Task = "task";
            public const string Custom = "custom";
        }

        internal sealed class XmlLoggerAttributes
        {
            private XmlLoggerAttributes()
            { }

            public const string Name = "name";
            public const string File = "file";
            public const string StartTime = "startTime";
            public const string ElapsedTime = "elapsedTime";
            public const string TimeStamp = "timeStamp";
            public const string Code = "code";
            public const string LineNumber = "line";
            public const string ColumnNumber = "column";
            public const string Importance = "level";
            public const string Processor = "processor";
            public const string HelpKeyword = "help";
            public const string SubCategory = "category";
            public const string Success = "success";
        }

        #endregion
    }
}
