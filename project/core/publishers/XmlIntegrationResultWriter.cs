using System;
using System.IO;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// 	
    /// </summary>
    public class XmlIntegrationResultWriter : IDisposable
    {
        private XmlFragmentWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlIntegrationResultWriter" /> class.	
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <remarks></remarks>
        public XmlIntegrationResultWriter(TextWriter textWriter)
        {
            writer = new XmlFragmentWriter(textWriter);
        }

        /// <summary>
        /// Writes the specified result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public void Write(IIntegrationResult result)
        {
            writer.WriteStartElement(Elements.CRUISE_ROOT);
            writer.WriteAttributeString("project", result.ProjectName);
            WriteRequest(result.IntegrationRequest);
            WriteModifications(result.Modifications);
            WriteIntegrationProperties(result);
            WriteBuildElement(result);
            WriteException(result);
            writer.WriteEndElement();
        }

        private void WriteRequest(IntegrationRequest request)
        {
            if (request == null) return;
            writer.WriteStartElement(Elements.Request);
            writer.WriteAttributeString("source", request.Source);
            writer.WriteAttributeString("buildCondition", request.BuildCondition.ToString());
            writer.WriteString(request.ToString());
            writer.WriteEndElement();

            // Output the parameters
            if ((request.BuildValues != null) && (request.BuildValues.Count > 0))
            {
                writer.WriteStartElement(Elements.Parameters);
                foreach (string key in request.BuildValues.Keys)
                {
                    writer.WriteStartElement(Elements.Parameter);
                    writer.WriteAttributeString("name", key);
                    writer.WriteAttributeString("value", request.BuildValues[key]);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void WriteTaskResults(IIntegrationResult result)
        {
            foreach (ITaskResult taskResult in result.TaskResults)
            {
                WriteOutput(taskResult.Data);

                var fileTaskResult = taskResult as FileTaskResult;
                if(fileTaskResult != null)
                {
                    // check whether the file should be deleted after merging
                    if(fileTaskResult.DeleteAfterMerge)
                    {
                        Log.Info("Delete merged file '{0}'.", fileTaskResult.File.FullName);
                        //TODO: enqueue for deleting
                    }
                }
            }
        }

        /// <summary>
        /// Writes the build element.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public void WriteBuildElement(IIntegrationResult result)
        {
            writer.WriteStartElement(Elements.BUILD);
            writer.WriteAttributeString("date", DateUtil.FormatDate(result.StartTime));

            // hide the milliseconds
            TimeSpan time = result.TotalIntegrationTime;
            writer.WriteAttributeString("buildtime", string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0:d2}:{1:d2}:{2:d2}", time.Hours, time.Minutes, time.Seconds));
            if (result.Failed)
            {
                writer.WriteAttributeString("error", "true");
            }
            writer.WriteAttributeString("buildcondition", result.BuildCondition.ToString());
            WriteTaskResults(result);
            writer.WriteEndElement();
        }

        private void WriteOutput(string output)
        {
            writer.WriteNode(output);
        }

        private void WriteException(IIntegrationResult result)
        {
            if (result.ExceptionResult == null)
            {
                return;
            }

            writer.WriteStartElement(Elements.EXCEPTION);
            writer.WriteCData(result.ExceptionResult.ToString());
            writer.WriteEndElement();
        }

        void IDisposable.Dispose()
        {
            writer.Close();
        }

        /// <summary>
        /// Writes the modifications.	
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <remarks></remarks>
        public void WriteModifications(Modification[] mods)
        {
            writer.WriteStartElement(Elements.MODIFICATIONS);
            if (mods == null)
            {
                return;
            }
            foreach (Modification mod in mods)
            {
                mod.ToXml(writer);
            }
            writer.WriteEndElement();
        }

        private void WriteIntegrationProperties(IIntegrationResult result)
        {

            writer.WriteStartElement(Elements.IntegrationProps);
            
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetArtifactDirectory],
                                                            IntegrationPropertyNames.CCNetArtifactDirectory);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetBuildCondition],
                                                            IntegrationPropertyNames.CCNetBuildCondition);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetBuildDate],
                                                            IntegrationPropertyNames.CCNetBuildDate);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetBuildTime],
                                                            IntegrationPropertyNames.CCNetBuildTime);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetFailureUsers],
                                                            IntegrationPropertyNames.CCNetFailureUsers);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetFailureTasks],
                                                            IntegrationPropertyNames.CCNetFailureTasks, "task");
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetIntegrationStatus],
                                                            IntegrationPropertyNames.CCNetIntegrationStatus);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetLabel],
                                                            IntegrationPropertyNames.CCNetLabel);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetLastIntegrationStatus],
                                                            IntegrationPropertyNames.CCNetLastIntegrationStatus);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetListenerFile],
                                                            IntegrationPropertyNames.CCNetListenerFile);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetModifyingUsers],
                                                            IntegrationPropertyNames.CCNetModifyingUsers);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetNumericLabel],
                                                            IntegrationPropertyNames.CCNetNumericLabel);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetProject],
                                                            IntegrationPropertyNames.CCNetProject);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetProjectUrl],
                                                            IntegrationPropertyNames.CCNetProjectUrl);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetRequestSource],
                                                            IntegrationPropertyNames.CCNetRequestSource);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetWorkingDirectory],
                                                            IntegrationPropertyNames.CCNetWorkingDirectory);
            WriteIntegrationProperty(result.IntegrationProperties[IntegrationPropertyNames.CCNetUser],
                                                            IntegrationPropertyNames.CCNetUser);

            WriteIntegrationProperty(result.LastChangeNumber, "LastChangeNumber");
            WriteIntegrationProperty(result.LastIntegrationStatus, "LastIntegrationStatus");
            WriteIntegrationProperty(result.LastSuccessfulIntegrationLabel, "LastSuccessfulIntegrationLabel");
            WriteIntegrationProperty(result.LastModificationDate, "LastModificationDate");

            var buildParameters = NameValuePair.ToDictionary(result.Parameters);
            if (buildParameters.ContainsKey("CCNetForceBuildReason"))
            {
                WriteIntegrationProperty(buildParameters["CCNetForceBuildReason"], "CCNetForceBuildReason");
            }

            writer.WriteEndElement();
        }

        private void WriteIntegrationProperty(object value, string propertyName)
        {
            WriteIntegrationProperty(value, propertyName, "user");
        }

        private void WriteIntegrationProperty(object value, string propertyName, string arrayElementName)
        {
            if (value == null) return ;


            writer.WriteStartElement(propertyName);

            if ((value is string) || (value is int) || (value is Enum) || value is DateTime )
            {
                writer.WriteString(value.ToString());
            }
            else
            {
                var dummy = value as System.Collections.ArrayList;
                if (dummy != null)
                {
                    string[] tmp = (string[])dummy.ToArray(typeof(string));

                    foreach (string s in tmp)
                    {
                        WriteIntegrationProperty(s, arrayElementName);
                    }
                }
                else
                {
                    throw new ArgumentException(
                                    string.Format(System.Globalization.CultureInfo.CurrentCulture,"The IntegrationProperty type {0} is not supported yet", value.GetType()));
                }
            
            }

            writer.WriteEndElement();
        }


        private class Elements
        {
            private  Elements()
            {}

            public const string BUILD = "build";
            public const string CRUISE_ROOT = "cruisecontrol";
            public const string MODIFICATIONS = "modifications";
            public const string EXCEPTION = "exception";
            public const string Request = "request";
            public const string Parameters = "parameters";
            public const string Parameter = "parameter";
            public const string IntegrationProps = "integrationProperties";
        }

        /// <summary>
        /// Sets the formatting.	
        /// </summary>
        /// <value>The formatting.</value>
        /// <remarks></remarks>
        public Formatting Formatting
        {
            set { writer.Formatting = value; }
        }
    }
}