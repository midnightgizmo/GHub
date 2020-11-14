using System;
using GHub.plugin;
using GHub.EventMessages;


namespace TestPlugIn
{
	/// <summary>
	/// This plug in does nothing at the moment, but i decied to create it in case some one
	/// wanted to mess about with it.
	/// </summary>
	public class Start:IPlugin
	{
		private GHub.SendMessage sendMessage;
		private pluginGUI gui;

		public Start()
		{
			gui = new pluginGUI();
		}


		public System.Windows.Forms.Panel PluginLoaded()
		{
			return gui;
		}

		public void ConnectionMade(GHub.Data.ListOfServers ser, GHub.Data.ListOfLocalUsers usr)
		{
			sendMessage = new GHub.SendMessage(ser,usr);
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

			string message = msg.stringFormat.Substring(msg.sender.Length + 3, msg.stringFormat.Length - msg.sender.Length - 4);
			
			if (message[0] == '!')
				DeterminMessage(message,msg);
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


		//////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////
		
		private void DeterminMessage(string Msg,Message msg)
		{
			string message;
			int position;

			// check to see if there is anything after the ! e.g. <nick> !kick
			switch(Msg.Length)
			{
				case 1:
					return;
			}
            
			position = Msg.IndexOf(" ",1);

			switch (position)
			{
				// the message has no spaces in it and so the message will look
				// somthing like this.
				// "!myip"
				case -1:

					message = Msg.Substring(1);
					break;

				default:

					message = Msg.Substring(1,position - 1);
					break;
			}
			
			switch(message.ToLower())
			{
				case "kick":

					if (position == -1)
						return;

					kick(Msg,msg);
					break;

				case "drop":

					break;
			}
		}

		private void kick(string Msg,Message msg)
		{// !kick nick reasong for kicking
			string nickBeingKicked = string.Empty;
			string kickMessage = string.Empty;
			int position;
			GHub.client.Client kickedClnt;
			GHub.client.user.User clntKicking;
			GHub.client.server.Server serv;

			if (Msg.Length < 7)
				return;

			//if (  !((GHub.client.userInfo)msg.client).isOP  )
			//	return;

			// find out where the space is after the nick of the person being kicked
			switch(position = Msg.IndexOf(' ',6))
			{
				// if no space was found after the name then there probs was no reason given
				case -1:
					nickBeingKicked = Msg.Substring(6);
					kickMessage = "$To: " + nickBeingKicked + " From: " + msg.sender + " $<" + msg.sender + "> You have been kicked from the hub" + "|";
					break;

				// found a space after the nick being kicked so work out the nick being kicked.
				default:
                    nickBeingKicked = Msg.Substring(6,position -6);
					kickMessage = "$To: " + nickBeingKicked + " From: " + msg.sender + " $<" + msg.sender + ">" + Msg.Substring(position) + "|";
					break;
			}
			
			// check to see if we can find the user on this hub.
			if (  (kickedClnt = msg.client.ClientList.Get(nickBeingKicked)) == null  )
			{
				// we have not found the user on this hub so now look for the user on
				// the rest of the hubs.
				for (int i = 0; i < msg.client.ServerList.Size(); msg.client.ServerList.Get(i))
				{
					serv = (GHub.client.server.Server)msg.client.ServerList.Get(i);
					if(serv.usersToServer.Get(nickBeingKicked) != null)
					{
						serv.SendMessage(msg.stringFormat + "|");
						return;
					}
						
				}
				return;
			}

			// the person we are trying to kick is on this server
			
			GHub.client.user.User clntKicked = (GHub.client.user.User)msg.allLocalUsers.Get(nickBeingKicked);
			
			clntKicking = (GHub.client.user.User)msg.client;

			switch (clntKicking.Profile.kickLevel)
			{
				// user is not allowed to use the kick command
				case 0:
					return;

				// user can kick any one below his profile
				case 1:

					if (  clntKicking.Profile.profileLevel > ((GHub.client.user.User)kickedClnt).Profile.profileLevel )
					{
					}
					else
					{
						return;
					}
					break;

				// user can kick any one below and at the same level as his profile
				case 2:

					if (  clntKicking.Profile.profileLevel >= ((GHub.client.user.User)kickedClnt).Profile.profileLevel )
					{
					}
					else
					{
						return;
					}
					break;

				// user can kick any one below at and above his profile (i.e. any one)
				case 3:

					break;
			}

			if (  clntKicking.Profile.profileLevel < clntKicked.Profile.profileLevel)
			{
				msg.client.SendMessage("$To: " + msg.sender + " From: " + GHub.Settings.Hub.hubSettings.BotName + " $<" + GHub.Settings.Hub.hubSettings.BotName + "> You do not have access to kick this user|");
				return;
			}

			// send message to the user being kicked.
			kickedClnt.SendMessage(kickMessage);
			// Disconnect the user from the hub.
			sendMessage.DisconnectUser(msg);

			System.DateTime ban = System.DateTime.Now;
			ban = ban.AddMinutes(5);
			// add a temp ban to the user
			GHub.Settings.BANS.tempBans.Add(((GHub.client.userInfo)kickedClnt).ipAddress,ban);
		}

	}

	public class pluginGUI : System.Windows.Forms.Panel
	{
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox textBox;

		public pluginGUI()
		{
			this.BackColor = System.Drawing.Color.Yellow;

			textBox = new System.Windows.Forms.TextBox();
			textBox.Location = new System.Drawing.Point(15,15);
			this.Controls.Add(textBox);
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
	}
}
