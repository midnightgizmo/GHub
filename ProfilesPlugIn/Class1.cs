using System;
using GHub.plugin;
using GHub.EventMessages;

namespace ProfilesPlugIn
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Start:IPlugin
	{
		frmProfiles profiles;
		public Start()
		{

			profiles = new frmProfiles();
			//
			// TODO: Add constructor logic here
			//
		}

		public System.Windows.Forms.Panel PluginLoaded()
		{
			return profiles;
		}

		public void ConnectionMade(GHub.Data.ListOfServers ser, GHub.Data.ListOfLocalUsers usr)
		{
		}
		public bool ValidateNick(Message msg)
		{
			return false;
		}
		public bool Key(Message msg)
		{
			return false;
		}
		public bool myInfo(myInfo msg)
		{
			return false;
		}
		public bool AlmostLoggedIn(Message msg)
		{
			return false;
		}
		public bool Version(Message msg)
		{
			return false;
		}
		public bool GetNickList(Message msg)
		{
			return false;
		}
		public bool UserLeft(Message msg)
		{
			return false;
		}
		public bool MainChatMessage(mainChat msg)
		{
			return false;
		}

		public bool PrivateMessage(messageToUser msg)
		{
			return false;
		}
		public bool Search(messageToUser msg)
		{
			return false;
		}
	}
}
