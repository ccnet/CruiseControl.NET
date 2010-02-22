using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using System.Xml;
using System.Text;
using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    /// <summary>
    /// Returns an XML fragment containing the current status of the project.
    /// </summary>
    public class ProjectStatusAction : ICruiseAction
    {
        public const string ActionName = "ProjectStatus";
        private readonly IFarmService farmServer;

        /// <summary>
        /// Initialise a new <see cref="ProjectStatusAction"/>.
        /// </summary>
        /// <param name="farmServer">The farm service to use.</param>
        public ProjectStatusAction(IFarmService farmServer)
        {
            this.farmServer = farmServer;
        }

        /// <summary>
        /// Generates the XML response for the specified project.
        /// </summary>
        /// <param name="cruiseRequest"></param>
        /// <returns></returns>
        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            // Retrieve the actual snapshot
            IProjectSpecifier projectSpecifier = cruiseRequest.ProjectSpecifier;
            ProjectStatusSnapshot snapshot = farmServer.TakeStatusSnapshot(projectSpecifier, cruiseRequest.RetrieveSessionToken());

            // See what type of output is required
            IResponse output = null;
            string outputType = (cruiseRequest.Request.GetText("view") ?? string.Empty).ToLower();
            switch (outputType)
            {
                case "json":
                    // Generate the output as JSON
                    string json = ConvertStatusToJson(snapshot);
                    output = new JsonFragmentResponse(json);
                    break;
                default:
                    // Default output is XML
                    string xml = snapshot.ToString();
                    output = new XmlFragmentResponse(xml);
                    break;
            }
            return output;
        }

        /// <summary>
        /// Simple method to convert a project status to JSON (since we're not using .NET 3.5).
        /// </summary>
        /// <param name="status">The status to convert.</param>
        /// <returns></returns>
        private string ConvertStatusToJson(ProjectStatusSnapshot status)
        {
            StringBuilder jsonText = new StringBuilder();
            jsonText.Append("{");
            jsonText.AppendFormat("time:{0},", ToJsonDate(status.TimeOfSnapshot));
            AppendStatusDetails(status, jsonText);
            jsonText.Append("}");
            return jsonText.ToString();
        }

        /// <summary>
        /// Simple method to convert am item status to JSON (since we're not using .NET 3.5).
        /// </summary>
        /// <param name="status">The status to convert.</param>
        /// <returns></returns>
        private string ConvertStatusToJson(ItemStatus status)
        {
            StringBuilder jsonText = new StringBuilder();
            jsonText.Append("{");
            AppendStatusDetails(status, jsonText);
            jsonText.Append("}");
            return jsonText.ToString();
        }

        /// <summary>
        /// Simple method to append status information to a builder
        /// </summary>
        /// <param name="status">The status to append.</param>
        /// <param name="builder">The builder to append the details to.</param>
        private void AppendStatusDetails(ItemStatus status, StringBuilder builder)
        {
            builder.AppendFormat("id:'{0}'", status.Identifier);
            builder.AppendFormat(",name:'{0}'", ToJsonString(status.Name));
            builder.AppendFormat(",status:'{0}'", status.Status);
            if (!string.IsNullOrEmpty(status.Description)) builder.AppendFormat(",description:'{0}'", ToJsonString(status.Description));
            if (status.TimeStarted.HasValue) builder.AppendFormat(",started:{0}", ToJsonDate(status.TimeStarted.Value));
            if (status.TimeCompleted.HasValue) builder.AppendFormat(",completed:{0}", ToJsonDate(status.TimeCompleted.Value));
            if (status.TimeOfEstimatedCompletion.HasValue) builder.AppendFormat(",estimated:{0}", ToJsonDate(status.TimeOfEstimatedCompletion.Value));
            if (status.ChildItems.Count > 0)
            {
                builder.Append(",children:[");
                int count = 0;
                foreach (ItemStatus child in status.ChildItems)
                {
                    if (count++ > 0) builder.Append(",");
                    builder.Append(ConvertStatusToJson(child));
                }
                builder.Append("]");
            }
        }

        /// <summary>
        /// Convert a <see cref="String"/> to a JSON safe version.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ToJsonString(string value)
        {
            string json = value.Replace("\'", "\'\'");
            return json;
        }

        /// <summary>
        /// Convert a <see cref="DateTime"/> to a JSON representation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ToJsonDate(DateTime value)
        {
            string json = string.Format("new Date({0}, {1}, {2}, {3}, {4}, {5})",
                value.Year,
                value.Month - 1,
                value.Day,
                value.Hour,
                value.Minute,
                value.Second);
            return json;
        }
    }
}
