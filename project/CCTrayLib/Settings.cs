using System;
using System.IO;
using System.Runtime.Remoting;
using System.Xml.Serialization;
using Drew.Agents;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebServiceProxy;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	/// <summary>
	/// Encapsulates all user-settings for the CruiseControl.NET Monitor.  This class
	/// is designed to work with Xml serialisation, for persisting user settings.
	/// </summary>
	[XmlRoot("CruiseControlMonitor", Namespace="http://www.sf.net/projects/ccnet", IsNullable=false)]
	public class Settings
	{
		public int PollingIntervalSeconds;
		public string RemoteServerUrl;

		public string ProjectName;

		public NotificationBalloon NotificationBalloon;

		public Sounds Sounds = new Sounds();

		public Messages Messages = new Messages();

		public Agents Agents = new Agents();
		public StatusIcons Icons = new StatusIcons();

		public ConnectionMethod ConnectionMethod;

		public bool ShowExceptions = true;

		public Settings()
		{
		}

		public static Settings CreateDefaultSettings()
		{
			Settings defaults = new Settings();

			defaults.ProjectName = "ProjectName";

			defaults.ConnectionMethod = ConnectionMethod.Remoting;
			defaults.ShowExceptions = true;

			defaults.Sounds = Sounds.CreateDefaultSettings();
			defaults.NotificationBalloon = NotificationBalloon.CreateDefaultSettings();
			defaults.Messages = Messages.CreateDefaultSettings();
			defaults.Agents = Agents.CreateDefaultSettings();
			defaults.Icons = StatusIcons.CreateDefaultSettings();
			defaults.PollingIntervalSeconds = 15;
			defaults.RemoteServerUrl = "tcp://localhost:21234/CruiseManager.rem";

			return defaults;
		}
		[XmlIgnore]
		public virtual ICruiseManager CruiseManager
		{
			get
			{
				if (ConnectionMethod == ConnectionMethod.WebService)
				{
					return new CCNetManagementProxy(RemoteServerUrl);
				}
				if (ConnectionMethod == ConnectionMethod.Remoting)
				{
					return (ICruiseManager) RemotingServices.Connect(typeof (ICruiseManager), RemoteServerUrl);
				}
				throw new NotImplementedException("Connection method " + ConnectionMethod + " is not implemented.");

			}
		}
	}

	#region NotificationBalloon

	public class NotificationBalloon
	{
		[XmlAttribute] public bool ShowBalloon;

		public static NotificationBalloon CreateDefaultSettings()
		{
			NotificationBalloon defaults = new NotificationBalloon();
			defaults.ShowBalloon = true;
			return defaults;
		}
	}

	#endregion

	#region Messages

	public class Messages
	{
		[XmlArrayItem("Message", typeof (string))] public string[] AnotherSuccess = new string[0];

		[XmlArrayItem("Message", typeof (string))] public string[] AnotherFailure = new string[0];

		[XmlArrayItem("Message", typeof (string))] public string[] Fixed = new string[0];

		[XmlArrayItem("Message", typeof (string))] public string[] Broken = new string[0];

		public static Messages CreateDefaultSettings()
		{
			Messages defaults = new Messages();
			defaults.AnotherSuccess = new string[] {"Yet another successful build!"};
			defaults.AnotherFailure = new string[] {"The build is still broken..."};
			defaults.Fixed = new string[] {"Recent checkins have fixed the build."};
			defaults.Broken = new string[] {"Recent checkins have broken the build."};
			return defaults;
		}

		// TODO Sreekanth Need to move it into the Build transition class
		public string GetMessageForTransition(BuildTransition buildTransition)
		{
			if (buildTransition == BuildTransition.StillSuccessful)
				return SelectRandomString(AnotherSuccess);
			if (buildTransition == BuildTransition.StillFailing)
				return SelectRandomString(AnotherFailure);
			if (buildTransition == BuildTransition.Broken)
				return SelectRandomString(Broken);
			if (buildTransition == BuildTransition.Fixed)
				return SelectRandomString(Fixed);
			throw new Exception("Unsupported build transition.");
		}

		private string SelectRandomString(string[] messages)
		{
			if (messages.Length == 0)
				return "No message available.";

			int index = new Random().Next(messages.Length);
			return messages[index];
		}
	}

	#endregion

	public class StatusIcons
	{
		public string Unknown;
		public string BuildSuccessful;
		public string BuildFailed;
		public string NowBuilding;
		public string Error;
		[XmlAttribute] public bool UseDefaultIcons = true;

		public static StatusIcons CreateDefaultSettings()
		{
			StatusIcons defaults = new StatusIcons();
			defaults.UseDefaultIcons = true;
			return defaults;
		}
	}

	#region Agents

	public class Agents
	{
		[XmlAttribute] public string CurrentAgentName;

		[XmlAttribute] public bool ShowAgent = false;

		[XmlAttribute] public bool HideAfterMessage = true;

		[XmlArray("AvailableAgents")]
		[XmlArrayItem("Agent", typeof (AgentDetails))] public AgentDetails[] AvailableAgents = new AgentDetails[0];

		public static Agents CreateDefaultSettings()
		{
			Agents defaults = new Agents();
			defaults.CurrentAgentName = "Peedy";
			defaults.ShowAgent = false;
			defaults.AvailableAgents = new AgentDetails[] {CreatePeedyAgent()};
			return defaults;
		}

		private static AgentDetails CreatePeedyAgent()
		{
			AgentDetails peedy = new AgentDetails();
			peedy.AcsFileName = "Peedy.acs";
			peedy.Name = "Peedy";
			peedy.SpeakOutLoud = false;
			peedy.AnotherSuccessAction = "TODO";
			peedy.AnotherFailureAction = "TODO";
			peedy.FixedAction = "TODO";
			peedy.BrokenAction = "TODO";
			return peedy;
		}

		public Agent CreateAgent()
		{
			if (!ShowAgent)
				return null;

			if (CurrentAgentName == null || CurrentAgentName.Trim().Length == 0)
				throw new Exception("No CurrentAgentName has been set in configuration data.");

			AgentDetails currentAgentDetails = GetCurrentAgentDetails();

			if (currentAgentDetails == null)
				throw new Exception("Error in configuration of agents.  CurrentAgentName does not correspond to an available agents.");

			return new Agent(currentAgentDetails.AcsFileName);
		}

		private AgentDetails GetCurrentAgentDetails()
		{
			foreach (AgentDetails details in AvailableAgents)
			{
				if (details.Name == CurrentAgentName)
					return details;
			}
			return null;
		}
	}

	public class AgentDetails
	{
		[XmlAttribute] public string Name = string.Empty;
		[XmlAttribute] public string AcsFileName = string.Empty;
		[XmlAttribute] public bool SpeakOutLoud = false;

		public string AnotherSuccessAction = string.Empty;
		public string AnotherFailureAction = string.Empty;
		public string FixedAction = string.Empty;
		public string BrokenAction = string.Empty;
	}

	#endregion
}