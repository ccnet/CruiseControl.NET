using System.Drawing;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ProjectGridRow
	{
		private readonly ProjectStatus status;
		private readonly IServerSpecifier serverSpecifier;
		private readonly string url;

		public ProjectGridRow(ProjectStatus status, IServerSpecifier serverSpecifier, string url)
		{
			this.status = status;
			this.serverSpecifier = serverSpecifier;
			this.url = url;
		}

		public string Name
		{
			get { return status.Name; }
		}

		public string ServerName
		{
			get { return serverSpecifier.ServerName; }
		}

		public string Category
		{
			get { return status.Category; }
		}

		public string BuildStatus
		{
			get { return status.BuildStatus.ToString(); }
		}

		public string BuildStatusHtmlColor
		{
			get { return CalculateHtmlColor(status.BuildStatus); }
		}

		public string LastBuildDate
		{
			get { return DateUtil.FormatDate(status.LastBuildDate); }
		}

        public string NextBuildTime
        {
            get
            {
                if (status.NextBuildTime == System.DateTime.MaxValue)
                {
                    return "Force Build Only";
                }
                else
                {
                    return DateUtil.FormatDate(status.NextBuildTime);
                }
            }
        }

		public string LastBuildLabel
		{
			get { return (status.LastBuildLabel != null ? status.LastBuildLabel : "no build available"); }
		}

		public string Status
		{
			get { return status.Status.ToString(); }
		}

		public string Activity
		{
			get { return status.Activity.ToString(); }
		}

		public string CurrentMessage
		{
			get { return status.CurrentMessage; }
		}

		public string Url
		{
			get { return url; }
		}


        public string Queue
        {
            get { return status.Queue; }
        }


        public int QueuePriority
        {
            get { return status.QueuePriority; }
        }


		public string StartStopButtonName
		{
			get { return (status.Status == ProjectIntegratorState.Running) ? "StopBuild" : "StartBuild"; }
		}

		public string StartStopButtonValue
		{
			get { return (status.Status == ProjectIntegratorState.Running) ? "Stop" : "Start"; }
		}

		public string ForceAbortBuildButtonName
		{
			get { return (status.Activity != ProjectActivity.Building) ? "ForceBuild" : "AbortBuild"; }
		}

		public string ForceAbortBuildButtonValue
		{
			get { return (status.Activity != ProjectActivity.Building) ? "Force" : "Abort"; }
		}

		public bool AllowForceBuild
		{
			get { return serverSpecifier.AllowForceBuild; }
		}

		public bool AllowStartStopBuild
		{
			get { return serverSpecifier.AllowStartStopBuild; }
		}

		private string CalculateHtmlColor(IntegrationStatus integrationStatus)
		{
			if (integrationStatus == IntegrationStatus.Success)
			{
				return Color.Green.Name;
			}
			else if (integrationStatus == IntegrationStatus.Unknown)
			{
				return Color.Blue.Name;
			}
			else
			{
				return Color.Red.Name;
			}
		}

        public string BuildStage
        {
            get
            {
                string CurrentBuildStage = status.BuildStage;

                if (CurrentBuildStage.Length == 0)
                { return ""; }
                else
                { return GetFormattedBuildStage(CurrentBuildStage); }
            }
        }

        private string GetFormattedBuildStage(string buildStageData)
        {
            System.Xml.XmlDocument XDoc = new System.Xml.XmlDocument();
            System.Xml.XmlTextReader XReader;
            System.Text.StringBuilder Result = new System.Text.StringBuilder();

            try
            {

                XDoc.LoadXml(buildStageData);
                XReader = new System.Xml.XmlTextReader(XDoc.OuterXml, System.Xml.XmlNodeType.Document, null);
                XReader.WhitespaceHandling = System.Xml.WhitespaceHandling.None;

                Result.Append("<table width=\"80%\">");
                Result.AppendLine();

                while (XReader.Read())
                {
                    XReader.MoveToContent();

                    if (XReader.AttributeCount > 0)
                    {
                        Result.AppendFormat("<tr><td NOWRAP>{0}</td> ", XReader.GetAttribute("Time"));
                        Result.AppendFormat("<td>{0}</td></tr>", XReader.GetAttribute("Data"));
                        Result.AppendLine();
                    }
                }                
                
                Result.Append("</table>");

                XReader.Close();
            }
            catch
            {
                Result = new System.Text.StringBuilder();
            }
            return Result.ToString();
        }                                                                                                              
                                                                                                                   


	}
}
