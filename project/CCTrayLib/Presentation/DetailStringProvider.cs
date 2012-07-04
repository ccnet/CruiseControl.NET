using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class DetailStringProvider : IDetailStringProvider
	{
		public string FormatDetailString(ISingleProjectDetail projectStatus)
		{
			if (projectStatus.ProjectState == ProjectState.NotConnected)
			{
				if (projectStatus.ConnectException == null)
					return "Connecting...";

				return string.Format(System.Globalization.CultureInfo.CurrentCulture,"Error: {0}", projectStatus.ConnectException.Message);
			}

			string message = GetTimeRemainingMessage(projectStatus);



            // show first breaking task and the breaking users
            // search from the end, to get the most recent messages of the specified kind.
		    bool breakerFound = false;
		    bool breakingTaskFound = false;
		    bool aborterFound = false;
            bool fixerFound = false;
            

            for (int i = projectStatus.Messages.Length - 1; i >= 0; i--)
            {

                if (projectStatus.Messages[i].Kind == Message.MessageKind.Fixer)
                {
                    if (!fixerFound)
                    {
                        if (projectStatus.CurrentMessage != projectStatus.Messages[i].Text)
                        {
                            if (message.Length > 0) message += " - ";
                            message += projectStatus.Messages[i].Text;
                        }

                        fixerFound = true;
                    }
                }

                
                if (projectStatus.Messages[i].Kind == Message.MessageKind.Breakers)
                {
                    if (!breakerFound)
                    {
                        if (projectStatus.CurrentMessage != projectStatus.Messages[i].Text)
                        {
                            if (message.Length > 0) message += " - Breaker(s) ";
                            message += projectStatus.Messages[i].Text;
                        }

                        breakerFound = true;
                    }                                        
                }

                if (projectStatus.Messages[i].Kind == Message.MessageKind.FailingTasks)
                {
                    if (!breakingTaskFound)
                    {
                        if (projectStatus.CurrentMessage != projectStatus.Messages[i].Text)
                        {
                            if (message.Length > 0) message += " - ";
                            message += projectStatus.Messages[i].Text;
                        }
                        breakingTaskFound = true;
                    }
                }


                if (projectStatus.Messages[i].Kind == Message.MessageKind.BuildAbortedBy)
                {
                    if (!aborterFound)
                    {
                        if (projectStatus.CurrentMessage != projectStatus.Messages[i].Text)
                        {
                            if (message.Length > 0) message += " - Aborted By :";
                            message += projectStatus.Messages[i].Text;
                        }
                        aborterFound = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(projectStatus.CurrentMessage))
            {
                message += " - " + projectStatus.CurrentMessage;
            }

			return message;
		}

		private static string GetTimeRemainingMessage(ISingleProjectDetail projectStatus)
		{
			if (projectStatus.Activity.IsSleeping())
			{
				if (projectStatus.NextBuildTime == DateTime.MaxValue)
					return "Project is not automatically triggered";

                if (projectStatus.NextBuildTime.Date != DateTime.Now.Date)
                {
                    return string.Format(System.Globalization.CultureInfo.CurrentCulture,"Next build check: {0:G}", projectStatus.NextBuildTime);
                }
                else
                {
                    return string.Format(System.Globalization.CultureInfo.CurrentCulture,"Next build check: {0:T}", projectStatus.NextBuildTime);
                }
			}

            if (!projectStatus.Activity.IsPending())
            {
			TimeSpan durationRemaining = projectStatus.EstimatedTimeRemainingOnCurrentBuild;

			if (durationRemaining != TimeSpan.MaxValue)
			{
				if (durationRemaining <= TimeSpan.Zero)
				{
					return string.Format(System.Globalization.CultureInfo.CurrentCulture,"Taking {0} longer than last build", new CCTimeFormatter(durationRemaining.Negate()));
				}

				return string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0} estimated remaining", new CCTimeFormatter(durationRemaining));
			}
            }

			return string.Empty;
		}
	}
}
