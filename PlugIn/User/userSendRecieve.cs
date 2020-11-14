using System;
using System.Net.Sockets;
using GHub.EventMessages;
using GHub.Data;


namespace GHub.client.user
{

	public class userSendRecieve : ClientWorkOutMessage
	{
		public userSendRecieve(Socket Soc, ListOfServers serverlist, ListOfLocalUsers clientlist, Core thecore):base(Soc,serverlist,clientlist, thecore)
		{

		}

		protected override void Event(Message msg)
		{
			switch (msg.type)
			{
				case (int)Messagetype.ValidateNick:

					ValidateNick(msg);
					break;

				case (int)Messagetype.Key:

					Key(msg);
					break;

				case (int)Messagetype.Supports:

					Supports(msg);
					break;

				case (int)Messagetype.MyINFO:

					myInfo((myInfo)msg);
					break;

				case (int)Messagetype.Version:

					break;
					
				case (int)Messagetype.GetNickList:

					GetNickList(msg);
					break;

				case (int)Messagetype.GetINFO:

					GetInfo((messageToUser)msg);
					break;

				case (int)Messagetype.MainChatMessage:

					MainChatMessage((mainChat)msg);
					break;
/*
									case (int)Messagetype.Unknown:

										break;
*/
				case (int)Messagetype.ChatPM:

					PrivateMessage((messageToUser)msg);
					break;

				case (int)Messagetype.MyPass:

					MyPass(msg);
					break;

				case (int)Messagetype.ConnectToMe:

					ConnectToMe((messageToUser)msg);
					break;

				case (int)Messagetype.Search:

					Search((messageToUser)msg);
					break;

				case (int)Messagetype.RevConnectToMe:

					RevConnectToMe((messageToUser)msg);
					break;

				case (int)Messagetype.Kick:

					Kick(msg);
					break;

				case (int)Messagetype.QUIT:

					UserLeft(msg);
					break;
			}
		}

		// the Nick a user sends when they are loggin in.
		protected virtual void ValidateNick(Message msg)
		{
			//$ValidateNick nick|
			string nick = msg.stringFormat.Substring(14,msg.stringFormat.Length - 15);

			if (nick.Length > GHub.Settings.Hub.hubSettings.NickLength)
			{
				this.closeAndRemoveUser("your nick is too long|");
				return;
			}

			if (nick.IndexOf(" ",0) != -1)
			{
				this.closeAndRemoveUser("spaces are not allowed in a nick");
				return;
			}

			if(!this.supportsSupport)
			{
				this.closeAndRemoveUser("You need to support NoGetINFO and NoHello to be able to enter this hub|");
				return;
			}
			GHub.Settings.ACCOUNTS.account Account;

			#region check if loogged into this server
			if (  this.ClientList.Get(nick) != null )
			{
				this.closeAndRemoveUser("$ValidateDenide|");
				return;
			}
			#endregion


			#region check if logged into other server
			GHub.client.server.Server eachServer;
			GHub.client.server.serverUserInfo info;
			for (int i = 0; i < this.ServerList.Size(); i++)
			{
				eachServer = (GHub.client.server.Server)this.ServerList.Get(i);

				for (int j = 0; j < eachServer.usersToServer.Size(); j++)
				{
					info = eachServer.usersToServer.Get(j);
					// if the user exists on another server they can not log into here.
					if (info.nick == nick)
					{
						this.closeAndRemoveUser("<GBot> You are allready logged into a hub that is connected to this one|$ValidateDenide|");
					}
				}
			}
			#endregion


			#region check if reged user (i.e. they need to send a pass)

			this.nick = nick;
			Account = GHub.Settings.ACCOUNTS.usersAccounts.GetAccount(nick);
			if (Account == null)
			{
				// log user and send user list.
				this.SendMessage("$HubName " + GHub.Settings.Hub.hubSettings.HubName + "|<GHub> GHub Version:" + GHub.Settings.Hub.hubSettings.VersionNo + ".|");
				this.SendMessage("$Hello " + nick +"|");
				this.loggedInCorrectly = true;
				this.Profile = GHub.Settings.ACCOUNTS.profiles.DefaultProfile;
				AlmostLoggedIn(msg);
			}
			// we need to ask the user for a password
			else
			{
				this.Profile = Account.Profile;
				this.SendMessage("<GHub> Your name is registered. Please supply pass|$GetPass|");
			}
			#endregion
			
		}

		protected virtual void MyPass(Message msg)
		{
			// $MyPass pass|
			string pass = msg.stringFormat.Substring(8,msg.stringFormat.Length - 9);

			GHub.Settings.ACCOUNTS.account acc = GHub.Settings.ACCOUNTS.usersAccounts.GetAccount(this.nick);

			// if it was an incorect pass
			if (acc.pass != pass)
			{
				this.closeAndRemoveUser("<GHub> Incorrect password|$BadPass|");
				return;
			}

			this.loggedInCorrectly = true;
			this.SendMessage("$LogedIn " + this.nick + "|");
			this.SendMessage("$HubName " + GHub.Settings.Hub.hubSettings.HubName + "|<GHub> GHub Version:" + GHub.Settings.Hub.hubSettings.VersionNo + ".|");
			this.SendMessage("$Hello " + this.nick + "|");

			if ( (this.isOP = acc.Profile.isOP)  )
				this.send.SendMessageToAll(GetOPlist());

			AlmostLoggedIn(msg);

		}

		// this function will be called just after the user has been sent the 
		// "$Hello string".  Its function is to send the user a nick list and
		// an op list.
		// WARNING.  I do not consider the user to be logged in yet (i.e. they 
		// will not be able to recieve chat) because they have yet to send me
		// a myINFO string. As soon as they send that they will be marked as
		// logged in.  Until then they can not chat search etc.
		protected virtual void AlmostLoggedIn(Message msg)
		{
			string userInfoList = GetUserInfoList();
			string opList = GetOPlist();

			if (userInfoList != null)
				this.SendMessage(userInfoList);

			if (opList != null)
				this.SendMessage(opList);
		}
		// The key that is sent in when a user logs in.
		protected virtual void Key(Message msg)
		{

			//this.SendMessage("$HubName Gizmos Hub|<GHub> GHub Version: Beta 0.01. Writen by Gizmo|");
		}

		protected virtual void Supports(Message msg)
		{
			// must do a check here to see if the user supports NoGetINFO 
			// and NoHello.
			// IF THE USER DOES NOT SUPPORT THESE THEY MUST BE DISCONNECTED	
			// FROM THE HUB. we won't want them because they are probs
			// too old a client.
 
			if (  (msg.stringFormat.IndexOf("NoGetINFO") != -1) &&  (msg.stringFormat.IndexOf("NoHello") != -1)  )
				this.supportsSupport = true;
		}

		// when a user logs in, they send there version number
		// that is this.
		protected virtual void Version(Message msg)
		{
		}

		protected virtual void GetInfo(messageToUser msg)
		{
			this.SendMessage( ((GHub.client.user.User)this.ClientList.Get(msg.reciever)).rawUserInfo );
		}

		// a user is asking us for a nick list
		protected virtual void GetNickList(Message msg)
		{
			string nickList = GetNickList();
			string opList = GetOPlist();

			if (nickList != null)
				this.SendMessage(nickList);

			if (opList != null)
				this.SendMessage(opList);

			
		}

		protected virtual void MainChatMessage(mainChat msg)
		{
			if (this.isLoggedIn == false)
				return;
			if (msg.stringFormat.Length > GHub.Settings.Hub.hubSettings.MaxMainChatLength)
				return;
			this.send.SendMessageToAll(msg.stringFormat);


//			if (!msg.stringFormat.StartsWith("<" + this.nick + ">"))
//				return;

			if (msg.stringFormat.IndexOf("showlogs")!= -1)
			{
				for (int i = 0; i< this.ClientList.Size(); i++)
				{
					this.send.SendMessageToAll(  this.ClientList.Get(i).log.Get() + "|"  );
				}

				for (int i = 0; i < this.ServerList.Size(); i++)
				{
					this.send.SendMessageToAll(  this.ServerList.Get(i).log.Get() + "|" );
				}
			}
			if (msg.stringFormat.IndexOf("Collect rubish")!= -1)
			{
				this.send.SendMessageToAll("<GHub> Im on it. Garbage collection starting now|");

				this.send.SendMessageToAll("<GHub> Memory used before collection: " + GC.GetTotalMemory(false) + "|");
         
				// Collect all generations of memory.
				GC.Collect();
				this.send.SendMessageToAll("<GHub> Memory used after full collection: " + GC.GetTotalMemory(true) + "|");

			}

			if (msg.stringFormat.IndexOf("showlocal") != -1)
			{
				for (int i = 0; i < this.ClientList.Size(); i++)
				{
					this.send.SendMessageToAll("<GBot> " +  ((GHub.client.userInfo)this.ClientList.Get(i)).nick + "|" );
				}
			}

			if (msg.stringFormat.IndexOf("showserver") != -1)
			{
				GHub.client.server.ListOfUsersToServer list;
				for (int i = 0; i < this.ServerList.Size(); i++)
				{
					list = ((GHub.client.server.Server)this.ServerList.Get(i)).usersToServer;

					for (int u = 0; u < list.Size(); u++)
					{
						this.send.SendMessageToAll("<GBot> " +  list.Get(u).nick + "|" );
					}
					
				}
			}

			if (msg.stringFormat.IndexOf("servercount") != -1)
			{
				int size = 0;
				for (int i = 0; i < this.ServerList.Size(); i++)
				{
					size += ((GHub.client.server.Server)this.ServerList.Get(i)).usersToServer.Size();
				}
				this.send.SendMessageToAll( size.ToString() + "|");
			}
			if (msg.stringFormat.IndexOf("clientcount") != -1)
			{
				this.send.SendMessageToAll( this.ClientList.Size().ToString() + "|");
			}
		}
		// user has send eather there first myInfo or
		// is just sending another to update there
		// nick, share size, description etc.
		protected virtual void myInfo(myInfo msg)
		{
			// not really much to do. the myInfo string has
			// allready been updated in the ClientWorkOutMessage.

			// now just need to let the other users know that a user
			// has changed there myInfo string.

			if (!this.loggedInCorrectly)
			{
				this.closeAndRemoveUser("Incorect log in|");
				return;
			}

			
			// Ok this is very important. we are saying, if the user has not logged in
			// this will be there first myINFO string they are sending.
			// this means it does not edist in the myINFO array held in the Client class.
			// Because of this we need to add it to the array. Simple enough so far.
			// however the user is allowed to send myINFO as many times as he likes
			// during his connection to the hub.  When this happens the myINFO string
			// will be updated.  Now if im right when its update it will also update
			// the one that was added to the user info array because i am hoping
			// it is just a pointer that is tored in the user info array (don't see
			// what else it could be).
			if (!this.isLoggedIn)
			{
				this.isLoggedIn = true;
				GHub.client.Client.AddToUserInfoList((GHub.client.userInfo)this);
				LoggedIn();
			}
			else
			{
				// user has changed there myINFO string so we need to update it in the
				// InfoList.
				GHub.client.Client.upDateInfoList();
			}
			

			this.send.SendMessageToAll(this.rawUserInfo + "|");

			
			
		}

		protected virtual void LoggedIn()
		{
			GHub.Settings.ACCOUNTS.account acc = GHub.Settings.ACCOUNTS.usersAccounts.GetAccount(this.nick);
			if (acc == null)
			{
				this.SendMessage( "<GHub> " + GHub.Settings.ACCOUNTS.profiles.DefaultProfile.OnJoinMessage + "|");
			}
			else
			{
				this.SendMessage( "<GHub> " + GHub.Settings.ACCOUNTS.usersAccounts.GetAccount(this.nick).Profile.OnJoinMessage + "|");
			}
				
			string newUser = "$NewUser IsOP=" + this.isOP.ToString() + " " + this.rawUserInfo + "|";
			// we can't use the normle serach to send this message because the user is not fully
			// logged in yet. therefor we have to send it the old way

			GHub.client.server.Server client;
			for (int i = 0; i < this.ServerList.Size(); i++)
			{
				client = (GHub.client.server.Server)ServerList.Get(i);
				if (client.isLoggedIn)
					client.SendMessage(newUser);
			}
		}

		// the user has allready left and has been removed from the
		// list of users logged in (i hope). so don't sending
		// anything to them on the socket other wise it will just
		// go poof.
		protected virtual void UserLeft(Message msg)
		{
			// send a message to all users on this server and 
			// other servers to let them know a user has left on
			// this server.
			this.send.SendMessageToAll(msg.stringFormat);
		}

		protected virtual void Kick(Message msg)
		{
			if (!this.isOP)
				return;
			string nick = msg.stringFormat.Substring(6,msg.stringFormat.Length - 7);

			GHub.client.user.User clnt = (GHub.client.user.User)this.ClientList.Get(nick);

			if (clnt != null)
			{
				clnt.closeAndRemoveUser();
				return;
			}

	
			for (int eachServer = 0; eachServer < this.ServerList.Size(); eachServer++)
			{
				GHub.client.server.Server serv = (GHub.client.server.Server)this.ServerList.Get(eachServer);

				for (int eachUser = 0; eachUser < serv.usersToServer.Size(); eachUser++)
				{
					if (serv.usersToServer.Get(nick) != null)
					{
						serv.SendMessage(msg.stringFormat);
					}

				}

			}

			
		}


		protected virtual void Disconnect(Message msg)
		{
		}

		protected virtual void PrivateMessage(messageToUser msg)
		{
			if (this.isLoggedIn == false)
				return;
			this.send.sendMessageToIndividual(msg.reciever,msg.stringFormat);
		}

		protected virtual void Search(messageToUser msg)
		{	
			if (this.isLoggedIn == false)
				return;

			if ((msg.sender.IndexOf(this.ipAddress,0) == -1) || (msg.sender.IndexOf(this.nick,0) == -1))
			{
				return;
			}
			//if (msg != this.nick)
			//	return;
			this.send.SendMessageToAll(msg.stringFormat);
		}

		protected virtual void ConnectToMe(messageToUser msg)
		{
			this.send.sendMessageToIndividual(msg.reciever,msg.stringFormat + "|");
		}

		protected virtual void RevConnectToMe(messageToUser msg)
		{
			if (msg.sender != this.nick)
				return;
			this.send.sendMessageToIndividual(msg.reciever,msg.stringFormat + "|");
		}


/*
		private string GetUserInfoList()
		{
			string users = string.Empty;
			for (int eachUser = 0; eachUser < this.ClientList.Size(); eachUser++)
			{
				users += ((GHub.client.userInfo)this.ClientList.Get(eachUser)).rawUserInfo + "|";
			}

			GHub.client.server.ListOfUsersToServer usersToServer;
			for (int eachServer = 0; eachServer < this.ServerList.Size(); eachServer++)
			{
				usersToServer = ((GHub.client.server.Server)this.ServerList.Get(eachServer)).usersToServer;

				for (int eachUser = 0; eachUser < usersToServer.Size(); eachUser++)
				{
					users += usersToServer.Get(eachUser).rawUserInfo + "|";
	
				}

			}

			if (users == string.Empty)
				return null;
	
			// an extra "|" is getting added onto the end of this message.
			// not to much of a prob but i really should cleen it up at some point.coul
			return users;
		}*/

		private string GetNickList()
		{
			string users = string.Empty;
			GHub.client.userInfo info;
			for (int eachUser = 0; eachUser < this.ClientList.Size(); eachUser++)
			{
				info = (GHub.client.userInfo)this.ClientList.Get(eachUser);
				if (info.loggedInCorrectly)
					users += info.nick + "$$";
			}

			GHub.client.server.ListOfUsersToServer usersToServer;
			for (int eachServer = 0; eachServer < this.ServerList.Size(); eachServer++)
			{
				usersToServer = ((GHub.client.server.Server)this.ServerList.Get(eachServer)).usersToServer;

				for (int eachUser = 0; eachUser < usersToServer.Size(); eachUser++)
				{
					users += usersToServer.Get(eachUser).nick + "$$";
				}

			}

			if (users == string.Empty)
				return null;
			return "$NickList " + users + "|";
		}

		// need to change this function so that it does not have to search eveytime a 
		// new user enters.  will make it so that it only seraches when an op enters
		// or leaves. but for now it will do.
		// Think ill make it so that they are added to an array, and then when one
		// leaves/enters it will just eather add or remove that user from the array.
		private string GetOPlist()
		{/*
			string OPs = string.Empty;
			GHub.Settings.ACCOUNTS.account Account;
			for (int i = 0; i < GHub.Settings.ACCOUNTS.usersAccounts.Size(); i++)
			{
				Account = GHub.Settings.ACCOUNTS.usersAccounts.GetAccount(i);
				if (Account.Profile.isOP)
					OPs += Account.nick + "$$";
			}

			if (OPs == string.Empty)
				return null;
			return "$OpList " + OPs + "|";
*/

			string OPs = string.Empty;
			GHub.client.userInfo info;
			for (int eachUser = 0; eachUser < this.ClientList.Size(); eachUser++)
			{
				info = (GHub.client.userInfo)this.ClientList.Get(eachUser);
				if (info.isOP)
					if (info.loggedInCorrectly)
						OPs += info.nick + "$$";
			}

			GHub.client.server.ListOfUsersToServer usersToServer;
			GHub.client.server.serverUserInfo serverInfo;
			for (int eachServer = 0; eachServer < this.ServerList.Size(); eachServer++)
			{
				usersToServer = ((GHub.client.server.Server)this.ServerList.Get(eachServer)).usersToServer;

				for (int eachUser = 0; eachUser < usersToServer.Size(); eachUser++)
				{
					serverInfo = usersToServer.Get(eachUser);
					if (serverInfo.isOP)
						OPs += usersToServer.Get(eachUser).nick + "$$";

				}

			}

			if (OPs == string.Empty)
				return null;
			return "$OpList " + OPs + "|";

		}
	}
}
