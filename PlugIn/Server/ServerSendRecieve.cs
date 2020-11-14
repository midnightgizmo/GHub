using System;
using GHub.EventMessages;
using GHub.Data;
using System.Net.Sockets;

namespace GHub.client.server
{

	public class serverSendRecieve : ServerWorkOutMessage
	{


		public serverSendRecieve(Socket Soc, ListOfServers serverlist, ListOfLocalUsers clientlist, Core thecore):base(Soc,serverlist,clientlist,thecore)
		{

		}

		protected override void Event(Message msg)
		{
			switch (msg.type)
			{
				case (int)ServerMessageTypes.LogInInfoPlz:

					// send user name and password
					LogInInfo(msg);
					break;

				case (int)ServerMessageTypes.LoggingIn:

					// recieve user name and password.
					LoggingIn((serverLogInInfo)msg);
					break;

				case (int)ServerMessageTypes.AllUsersInfo:

					AllUsersInfo(msg);
					break;

				case (int)ServerMessageTypes.loggedIn:

					LoggeedIn(msg);
					break;

				case (int)ServerMessageTypes.BadLogIn:

					BadPass(msg);
					break;

				case (int)Messagetype.MyINFO:

					myINFO((myInfo)msg);
					break;

				case (int)ServerMessageTypes.NewUser:

					NewUser(msg);
					break;

				case (int)ServerMessageTypes.UserLeft:

					UserLeft(msg);
					break;

				case (int)Messagetype.MainChatMessage:

					MainChat(msg);
					break;

				case (int)ServerMessageTypes.Exit:

					ServerDisconnecting(msg);
					break;
			}
		}

		protected void LogInInfo(Message msg)
		{
			this.SendMessage("$LogginIn "+ GHub.Settings.Hub.hubSettings.MultiHubUserName + " " + GHub.Settings.Hub.hubSettings.MultiHubPass + "|");
		}

		protected void LoggingIn(serverLogInInfo msg)
		{
			int size = GHub.Settings.MultHubs.MultiHubs.Size();
			GHub.Settings.MultHubs.Hub aHub;
			string ListOfUsers = string.Empty;



			for (int i = 0; i < size; i++)
			{
				aHub = GHub.Settings.MultHubs.MultiHubs.Get(i);

				// we know of this server. the next step is to see if they sent the 
				// right password.
				if (aHub.userName == msg.userName)
				{
					// if they sent the right password.
					if (aHub.pass == msg.pass)
					{
						this.Name = aHub.HubName;

						this.core.ConnectingToServer(aHub.HubName);

						GHub.client.userInfo clnt;
						for (int eachUser = 0; eachUser < this.ClientList.Size(); eachUser++)
						{
							clnt = (GHub.client.userInfo)this.ClientList.Get(eachUser);
							ListOfUsers += "$NewUser IsOP=" + clnt.isOP.ToString() + " " + clnt.rawUserInfo + "|";
						}

						this.SendMessage("$AllUserInfo|");
						if (ListOfUsers != string.Empty)
							this.SendMessage( ListOfUsers );
						/////////////////////////////////////////////////////////////////////////////
						/////////////////////////////////////////////////////////////////////////////
						/////////////////////////////////////////////////////////////////////////////
						//
						//
						//need to send op list as well
						//
						//
						/////////////////////////////////////////////////////////////////////////////
						////////////////////////////////////////////////////////////////////////////////
						////////////////////////////////////////////////////////////////////////////////
						///
						this.SendMessage("$loggedIn|");
						
						return;
					}
				}
			}

			this.closeAndRemoveUser("$Badpass|");
		}

		protected virtual void BadPass(Message msg)
		{
			this.closeAndRemoveUser();
		}

		protected virtual void AllUsersInfo(Message msg)
		{
			string ListOfUsers = string.Empty;

			GHub.client.userInfo clnt;
			for (int eachUser = 0; eachUser < this.ClientList.Size(); eachUser++)
			{
				clnt = (GHub.client.userInfo)this.ClientList.Get(eachUser);
				ListOfUsers += "$NewUser IsOP=" + clnt.isOP.ToString() + " " + clnt.rawUserInfo + "|";
			}
			if (ListOfUsers != string.Empty)
				this.SendMessage( ListOfUsers );
			/////////////////////////////////////////////////////////////////////////////
			/////////////////////////////////////////////////////////////////////////////
			/////////////////////////////////////////////////////////////////////////////
			//
			//
			//need to send op list as well
			//
			//
			/////////////////////////////////////////////////////////////////////////////
			////////////////////////////////////////////////////////////////////////////////
			////////////////////////////////////////////////////////////////////////////////
			///
			this.SendMessage("$loggedIn|");
			

		}


		protected virtual void LoggeedIn(Message msg)
		{
			// set the log in value to true (need to create one).
			// this will let us know we can send and receive text and searches and all
			// that other stuff.
			this.isLoggedIn = true;
			this.core.ConnctionToServerMade(this.Name);

		}

		protected virtual void myINFO(myInfo msg)
		{

			this.send.SendMessageToAllLocalUsers(msg.stringFormat);

		}

		protected virtual void NewUser(Message msg)
		{
			this.send.SendMessageToAllLocalUsers(msg.stringFormat.Substring(msg.stringFormat.IndexOf("$MyINFO")) + "|");
		}

		protected virtual void UserLeft(Message msg)
		{
			string nick = msg.stringFormat.Substring(6,msg.stringFormat.Length - 7);
			this.usersToServer.Delete(nick);
			GHub.client.Client.RemoveFromUserInfoList(nick);
			this.send.SendMessageToAllLocalUsers(msg.stringFormat);
		}

		protected virtual void MainChat(Message msg)
		{
			this.send.SendMessageToAllLocalUsers(msg.stringFormat);
		}

		protected virtual void ServerDisconnecting(Message msg)
		{
			
			if (msg.stringFormat.Length > 8)
			{
				string message = "<GHub> Connection to server " + this.Name +  " has been stopped because a user named " + msg.stringFormat.Substring(msg.stringFormat.IndexOf(" ")) + " was found to be in both hubs.  Remove this user from one of the hubs and then try again.|";
				this.send.SendMessageToAllLocalUsers(message);
				GHub.Settings.MultHubs.MultiHubs.Get(this.Name).ConnectionState = 0;
			}

			this.closeAndRemoveUser();

			string nick;
			for (int i = 0; i < this.usersToServer.Size(); i++)
			{
				nick = this.usersToServer.Get(i).nick;
				this.usersToServer.Delete(nick);
				GHub.client.Client.RemoveFromUserInfoList(nick);
				this.send.SendMessageToAllLocalUsers("$Quit " + nick + "|");
				i--;
			}
			this.core.ServerLost(this.Name);
		}
						 

		protected virtual void Kick(Message msg)
		{
		}

		protected virtual void Ban(Message msg)
		{
		}

		protected virtual void PublicMessage(Message msg)
		{
		}

		protected virtual void PrivateMessage(Message msg)
		{
		}
	}
}
