using System;
using System.Collections;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using System.Runtime.InteropServices;




namespace tw.ccnet.core.publishers
{
	/// <summary>
	/// Summary description for YahooPublisher.
	/// Yahoo Publisher uses YPluginDLL.DLL for sending the Yahoo IM Message to any one. It works only on Windows Platforms
	/// 
	/// Configuring Yahoo Publisher
	/// 1.	Yahoo Publisher Expects a already Logged in User
	///		a.	Start Yahoo Messenger
	///		b.	Set to login when systems starts, and to remember your user id and password
	///	2.	Make sure that Yahoo Messenger is there and you can send message using Yahoo Messenger.
	///	3.	In config file, add users and their IDs, a sample of yahoo plugin config is given below.
	///	4.	I have removed left angular bracket from begning of the line because of parsing problems of the compiler, Please add brackets.
	///	
	///	@	yplugin from=""ccnet@thoughtworks.com"" projectUrl=""http://localhost/ccnet""
	///	@		yahoousers>
	///	@			yuser ame=""buildmaster"" group=""buildmaster"" id=""itsajey""/>
	///	@			yuser name=""netbuzzme"" group=""developers"" id=""netbuzzme""/>
	///	@			yuser name=""itsajey""	group=""developers"" id=""ajey.gore""/>
	///	@			yuser name=""narsi"" group=""developers"" id=""narsi321""/>
	///	@			yuser name=""asubrama"" group=""developers"" id=""ashok_subramanian""/>
	///	@		/yahoousers>
	///	@		yahoogroups>
	///	@			ygroup	name=""developers""	notification=""change""/>
	///	@			ygroup	name=""buildmaster"" notification=""always""/>				
	///	@		/yahoogroups>
	///	@	/yplugin>
	///		
	/// </summary>
	/// 
	public unsafe class YahooWrap
	{
		[ DllImport( "YpluginDLL.DLL", EntryPoint="SendYahooMessage")]
		//, CharSet=CharSet.Ansi commented CharSet because it may fail on some systems, 
		// if we experience problem we will add this again (Interop Services comments)
		
		public static extern int SendYahooMessage(String to, String message);
				
	}	
	[ReflectorType("yplugin")]
	public class YahooPublisher : PublisherBase
	{
		private string _projectUrl;
	
		private Hashtable _users = new Hashtable();
		private Hashtable _groups = new Hashtable();

		public YahooPublisher()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public override void Publish(object source, IntegrationResult result)
		{
		
			String Message = CreateMessage(result);
			SendMessageToNotifyGroupMembers(result, Message);
			SendMessageToModifiersWhoCheckedInTheFiles(result.Modifications, Message);

		
		}
		public void SendMessageToNotifyGroupMembers(IntegrationResult result, String Message)
		{
			foreach (YahooUser yuser in YahooUserIDs.Values)
			{
				YahooGroup group = GetYahooGroup(yuser.Group);
				if ( group!= null && group.Notification.Equals(YahooGroup.NotificationType.Always))
				{
					YahooWrap.SendYahooMessage(yuser.ID, Message);
				}
			}
		}

		public void SendMessageToModifiersWhoCheckedInTheFiles(Modification[] modifications, String Message)
		{
			foreach (Modification modification in modifications)
			{
				YahooUser yuser = GetYahooUser(modification.UserName);
				if (yuser!=null)
				{
					YahooWrap.SendYahooMessage(yuser.ID, Message);
				}
			}

		}

		public void SendYahooMessage(String name, String message)
		{
			YahooWrap.SendYahooMessage(name, message);
		}

		public YahooUser GetYahooUser(string username)
		{
			return (YahooUser)_users[username];
		}

		public YahooGroup GetYahooGroup(string groupname)
		{
			return (YahooGroup)_groups[groupname];
		}
		
		internal string CreateMessage(IntegrationResult result)
		{
			// TODO Add culprit to message text -- especially if modifier is not an email user
			return String.Format(@"CC.NET Build Results for {0}: {1}", 
				result.ProjectName, LogFile.CreateUrl(ProjectUrl, result));
		}

		#region Configuration Properties
		

		[ReflectorProperty("projectUrl")]
		public string ProjectUrl
		{
			get { return _projectUrl; }
			set { _projectUrl = value; }
		}

		[ReflectorHash("yahoousers", "id")]
		public Hashtable YahooUserIDs
		{
			get { return _users; }
			set { _users = value; }
		}

		[ReflectorHash("yahoogroups", "name")]
		public Hashtable YahooGroups
		{
			get { return _groups; }
			set { _groups = value; }
		}
		#endregion

	}
}
