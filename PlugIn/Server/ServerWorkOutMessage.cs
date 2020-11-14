using System;
using GHub.EventMessages;
using System.Net.Sockets;
using GHub.Data;

namespace GHub.client.server
{

	public class ServerWorkOutMessage : serverInfo
	{
		//private string message;
		private string xmlMessage;
		protected System.Collections.ArrayList recievedMessages;
		public ListOfUsersToServer usersToServer;

		public ServerWorkOutMessage(Socket Soc, ListOfServers serverlist, ListOfLocalUsers clientlist, Core thecore):base(Soc,serverlist,clientlist,thecore)
		{
			usersToServer = new ListOfUsersToServer();
		}

		public override void MessageRecieved(Message msg)
		{
			string recievedMessage = string.Empty;
			if ( (recievedMessages = ConvertToString(msg.RawFormat)) == null)
				return;

			for (int eachMessage = recievedMessages.Count - 1; eachMessage > -1; eachMessage--)
			{
				// a plug in may have closed the connection to the socket. therefore we need
				// to do some cleen up.
				if (!this.soc.Connected)
				{
					if ((string)recievedMessages[eachMessage] == "$Exit")
					{
						closeAndRemoveUser();

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
						return;
					}

					closeAndRemoveUser();
					this.core.ServerLost(this.Name);
					return;

				}
				try
				{
					recievedMessage = (string)recievedMessages[eachMessage];
				}
				catch(System.Exception e)
				{
					string temp = e.Message;
				}

				// if somthing went wrong with the message or it is not yet rady to be
				// prossesd don't go any further.
				if (recievedMessage == null || recievedMessage == string.Empty)
					return;

				msg.stringFormat = recievedMessage;

				// Find out where the first space accours int the message
				int firstSpacePosition = recievedMessage.IndexOf(" ");
			
				// get the first part of the message
				// e.g. "$To"
				string StartOfMessage = GetFirstPartOfMessage(recievedMessage,firstSpacePosition);

				// returns an event containing all the information needed to do stuff with it.
				Message fullMessage = DeterminMessage(StartOfMessage,firstSpacePosition,msg);

				// we need to just do a check to make sure that an error did not occour while
				// parsing the string. e.g. a server / user may haev sent a wrong string in
				switch (fullMessage.type)
				{
					case -1:
						continue;
				}

				fullMessage.stringFormat += "|";
				Event(fullMessage);
			}
		}

		protected virtual void Event(Message msg)
		{
		}

		public void closeAndRemoveUser(string MessageToUser)
		{
			this.SendMessage(MessageToUser);
			try
			{
				this.ServerList.Delete(this.soc);
				//this.soc.Shutdown(SocketShutdown.Both);
				this.soc.Close();
			}
			finally
			{
				this.recievedMessages.Clear();
			}
		}

		public void closeAndRemoveUser()
		{
			try
			{
				this.ServerList.Delete(this.soc);
				//this.soc.Shutdown(SocketShutdown.Both);
				this.soc.Close();
			}
			finally
			{
				this.recievedMessages.Clear();
			}
		}

		// I riped this out of another program i did.  Therefor some of the
		// veriable names at the moment will look at bit odd.  However it does the job
		public System.Collections.ArrayList ConvertToString(byte[] Data)
		{

			Byte[] abyte = new Byte[1];
			string base64Message = string.Empty;
			string completeBase64Message = string.Empty;
			string xmlString = string.Empty;
			int Position = 0;
			System.Collections.ArrayList RecievedMessages = new System.Collections.ArrayList();

			if (Data == null)
				return null;

			// have to do it this way to get them anoying special charectors
			char[] c= new char[Data.Length];
			for (int i = 0; i < Data.Length; i++)
			{
				c[i] = (char)Data[i];
			}
			
			
			//base64Message = System.Text.UTF8Encoding.ASCII.GetString(Data,0,Data.Length);
			base64Message = new string(c);
			//			base64Message = System.Text.ASCIIEncoding.ASCII.GetString(Data,0,Data.Length);

			// we might be dealing with more than one xml message. Therefore we need to check
			// the entier message to see if there is an equals "=" any where with in it.
			// An equals indicates the complesion of an xml message.
			while (  (Position = base64Message.IndexOf("|")) != -1  )
			{

				//while(base64Message.IndexOf("=",Position + 1) != -1)
				if (Position < (base64Message.Length - 1))
				{
					while(  base64Message[Position + 1] == '|'  )
					{
						Position++;
						if (Position + 1 == base64Message.Length)
						{
							break;
						}
					}
				}
				completeBase64Message = base64Message.Substring(0,Position);
				completeBase64Message = xmlMessage + completeBase64Message;

				// any data after Postion is to be stored in xmlMessage for next time around
				xmlMessage = base64Message.Substring(Position + 1);


				if (xmlMessage.Length > 0)
				{
					byte[] NextDataToBeLookedAt = new byte[xmlMessage.Length];
					

					for(int i = 0; i < xmlMessage.Length; i++)
						NextDataToBeLookedAt[i] = Convert.ToByte(xmlMessage[i]);
					xmlMessage = string.Empty;
					System.Collections.ArrayList secondRecievedMessages = ConvertToString(NextDataToBeLookedAt);
					NextDataToBeLookedAt = null;


					// this function can return null. secondRecievedMessages holds
					// a return value of this function. Therefore we need to check
					// incase the value is null or not.
					if (secondRecievedMessages != null)
					{
						for (int iCount = 0; iCount < secondRecievedMessages.Count; iCount++)
						{
							RecievedMessages.Add(secondRecievedMessages[iCount]);
						}
						secondRecievedMessages.Clear();
					}


					secondRecievedMessages = null;

				}
	
				if (completeBase64Message.Length > GHub.Settings.Hub.hubSettings.MaxMessageLength)
					completeBase64Message = completeBase64Message.Substring(0,3000);
				RecievedMessages.Add(completeBase64Message);

				completeBase64Message = null;
				return RecievedMessages;
			}
			xmlMessage += base64Message;

			// clean up
			base64Message = null;
			RecievedMessages = null;
			abyte = null;

			return null;
		}

		
		private string GetFirstPartOfMessage(string message,int position)
		{
			switch (position)
			{
					// if no space was found in the message we are dealing
					// with a simple message
				case -1:

					return message;

					// if a space was found, the message will later need to be
					// split up into seconds and worked out.
				default:

					return message.Substring(0,position);
			}
		}

		// checks to see what message we are dealing with.
		// e.g. "$To" , "$Search" etc
		private Message DeterminMessage(string StartOfMessage,int firstSpacePosition, Message msg)
		{
			Message simpleMessage = new Message();
			string[] splitMessage;
			serverUserInfo userInfo;
			GHub.client.userInfo usinfo;
			string nick;

			switch (StartOfMessage)
			{

					// another server is connecting to us and have sent
					// there username and pass.
					//
					//$LogginIn UserName Pass
				case "$LogginIn":

					splitMessage = msg.stringFormat.Split(' ');
					if (splitMessage.Length < 3)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						return simpleMessage;
					}
					serverLogInInfo logIn = new serverLogInInfo();

					logIn.userName = splitMessage[1];
					logIn.pass = splitMessage[2];

					logIn.type = (int)ServerMessageTypes.LoggingIn;
					logIn.RawFormat = msg.RawFormat;
					logIn.From = (int)MessageFrom.Server;
					logIn.soc = this.soc;
					logIn.stringFormat = msg.stringFormat;

					return logIn;

				// we are telling this server that it has send a correct
				// user name and password.
				case "$loggedIn":

					simpleMessage.type = (int)ServerMessageTypes.loggedIn;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Server;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;
					return simpleMessage;

				case "$Badpass|":

					simpleMessage.type = (int)ServerMessageTypes.BadLogIn;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Server;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;
					return simpleMessage;

				// server is closing down
				case "$Exit":
	
					simpleMessage.type = (int)ServerMessageTypes.Exit;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Server;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;
					return simpleMessage;

					// another server is asking us for our username and pass
					// to be able to log into there server
				case "$Lock":

					simpleMessage.type = (int)ServerMessageTypes.LogInInfoPlz;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Server;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;
					return simpleMessage;

					// a new user has logged into another server and they are
					// telling us about it. the format of the user is as follows.
					//
					// $NewUser IsOP=<True/False> $MyINFO $ALL <nick> <interest>$ $<speed>$<e-mail>$<sharesize>$ 
				case "$NewUser":

					newUser NewUser = new newUser();

					userInfo = new serverUserInfo();

					nick = userInfo.parseNickFromMyInfo(  msg.stringFormat.Substring(msg.stringFormat.IndexOf("$MyINFO"))  );
					if (   (usinfo = GHub.client.Client.getnickInfo(nick)) != null)
					{
						this.SendMessage("$Exit " + usinfo.nick + "|");
						//this.closeAndRemoveUser();
						recievedMessages.Clear();
						simpleMessage.type = -1;
						return simpleMessage;
					}

					// add the nick to the list of users held on another server
					userInfo.rawUserInfo = msg.stringFormat.Substring(msg.stringFormat.IndexOf("$MyINFO"));
					usersToServer.Add(userInfo);


					usinfo = new userInfo(msg.soc,this.ServerList,this.ClientList,this.core);
					usinfo.rawUserInfo = userInfo.rawUserInfo;
					GHub.client.Client.AddToUserInfoList(usinfo);
					// add the new usersinfo string to the main array list that holds all the myINFO's
//					GHub.client.Client.AddToUserInfoList(  usinfo  );

					NewUser.type = (int)ServerMessageTypes.NewUser;
					NewUser.RawFormat = msg.RawFormat;
					NewUser.From = (int)MessageFrom.Server;
					NewUser.soc = this.soc;
					NewUser.stringFormat = msg.stringFormat;

					NewUser.slots = userInfo.slots;
					NewUser.normHubs = userInfo.normHubs;
					NewUser.regHubs = userInfo.regHubs;
					NewUser.opHubs = userInfo.opHubs;
					NewUser.description = userInfo.description;
					NewUser.email = userInfo.eMail;
					NewUser.nick = userInfo.nick;
					return NewUser;

					// the server is sending us a users MyINFO that exists
					// there end.
				case "$MyINFO":

					// myINFO string done incorectly so ignore it.
					if (msg.stringFormat.Length < 9)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						return simpleMessage;
					}

					userInfo = new serverUserInfo();
					

					nick = userInfo.parseNickFromMyInfo(msg.stringFormat); 
					// if this the the time were 2 servers are connecting to each other
					// there should be a chance bother servers could have the same nick logged
					// into there server. We can not let this happen.  If this does happen
					// we will inform the other server we are droping the connection and let them
					// know why
/*					
					if (   (usinfo = GHub.client.Client.getnickInfo(nick)) != null)
					{
						this.SendMessage("$Exit " + usinfo.nick + "|");
						//this.closeAndRemoveUser();
						recievedMessages.Clear();
						simpleMessage.type = -1;
						return simpleMessage;
					}*/
					usinfo = GHub.client.Client.getnickInfo(nick);
					usinfo.rawUserInfo = msg.stringFormat;

					userInfo = usersToServer.Get( nick );
					userInfo.rawUserInfo = msg.stringFormat;

					GHub.client.Client.upDateInfoList();

					myInfo info = new myInfo();

					
					info.type =(int)Messagetype.MyINFO;
					info.RawFormat = msg.RawFormat;
					info.From = (int)MessageFrom.Server;
					info.soc = this.soc;
					info.stringFormat = msg.stringFormat;

					info.slots = userInfo.slots;
					info.normHubs = userInfo.normHubs;
					info.regHubs = userInfo.regHubs;
					info.opHubs = userInfo.opHubs;
					info.description = userInfo.description;
					info.email = userInfo.eMail;
					info.nick = userInfo.nick;

					return info;

/*
					// the user does not exist in the array list so add it
					if (userInfo == null)
					{
						userInfo = new serverUserInfo();
						userInfo.rawUserInfo = msg.stringFormat;
						usersToServer.Add(userInfo);

						info.type =(int)Messagetype.MyINFO;
						info.RawFormat = msg.RawFormat;
						info.From = (int)MessageFrom.Server;
						info.soc = this.soc;
						info.stringFormat = msg.stringFormat;

						info.slots = userInfo.slots;
						info.normHubs = userInfo.normHubs;
						info.regHubs = userInfo.regHubs;
						info.opHubs = userInfo.opHubs;
						info.description = userInfo.description;
						info.email = userInfo.eMail;
						info.nick = userInfo.nick;

						usinfo = new userInfo(msg.soc,this.ServerList,this.ClientList);
						usinfo.rawUserInfo = msg.stringFormat;
						// add the new usersinfo string to the main array list that holds all the myINFO's
						GHub.client.Client.AddToUserInfoList(  usinfo  );

						return info;
					}*/
/*
					// update the users info with there new myINFO string.
					userInfo.rawUserInfo = msg.stringFormat;

					usinfo = GHub.client.Client.getnickInfo(userInfo.nick);
					usinfo.rawUserInfo = userInfo.rawUserInfo;
					GHub.client.Client.upDateInfoList();
					

					info.type =(int)Messagetype.MyINFO;
					info.RawFormat = msg.RawFormat;
					info.From = (int)MessageFrom.Server;
					info.soc = this.soc;
					info.stringFormat = msg.stringFormat;

					info.slots = userInfo.slots;
					info.normHubs = userInfo.normHubs;
					info.regHubs = userInfo.regHubs;
					info.opHubs = userInfo.opHubs;
					info.description = userInfo.description;
					info.email = userInfo.eMail;
					info.nick = userInfo.nick;

					return info;*/


				// this will only be sent during server to server log in.
				// We will find out about any new user that connects after that
				// through $NewUser
				case "$OpList":

					// OpList string done incorectly so ignore it.
					if (msg.stringFormat.Length < 9)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						return simpleMessage;
					}

					simpleMessage.type = (int)ServerMessageTypes.OPList;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Server;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;
					return simpleMessage;

					// a server is asking for our users list
					// this will happen at the log in process.
				case "$AllUserInfo":

					simpleMessage.type = (int)ServerMessageTypes.AllUsersInfo;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Server;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;
					return simpleMessage;
					
					// a user is leaving on another server
				case"$Quit":

					// Quit string done incorectly so ignore it.
					if (msg.stringFormat.Length < 7)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						return simpleMessage;
					}

					simpleMessage.type = (int)ServerMessageTypes.UserLeft;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Server;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;

					return simpleMessage;

					// a user on another server is atempting to chat to a user
					// on our server.
				case "$To:":

					splitMessage = msg.stringFormat.Split('$');
					string[] FirstPartOfSplit;

					// not enough parameters in the string for it to be right
					if (splitMessage.Length < 3)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						return simpleMessage;
					}

					FirstPartOfSplit = splitMessage[1].Split(' ');

					// not enough parameters in the string for it to be right
					if (FirstPartOfSplit.Length < 5)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						return simpleMessage;
					}

					messageToUser PrivateMessage = new messageToUser();

					PrivateMessage.type = (int)Messagetype.ChatPM;
					PrivateMessage.RawFormat = msg.RawFormat;
					PrivateMessage.From = (int)MessageFrom.Client;
					PrivateMessage.soc = this.soc;

					PrivateMessage.sender = FirstPartOfSplit[3];
					PrivateMessage.reciever = FirstPartOfSplit[1];
					PrivateMessage.messageToReciever = splitMessage[2];
					PrivateMessage.stringFormat = msg.stringFormat;

					return PrivateMessage;

					// a user on another server is searching for somthing
					// this message must be relayed to all the clients on this
					// server.
				case "$Search":

					splitMessage = msg.stringFormat.Split(' ');
					if (splitMessage.Length < 3)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						return simpleMessage;
					}

					messageToUser Search = new messageToUser();
					Search.sender = splitMessage[1];

					Search.type = (int)Messagetype.Search;
					Search.RawFormat = msg.RawFormat;
					Search.From = (int)MessageFrom.Client;
					Search.soc = this.soc;
					Search.stringFormat = msg.stringFormat;

					return simpleMessage;

				default:

					if (StartOfMessage.StartsWith("$"))
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						return simpleMessage;
					}

					mainChat MainChatMessage = new mainChat();

					MainChatMessage.type = (int)Messagetype.MainChatMessage;
					MainChatMessage.RawFormat = msg.RawFormat;
					MainChatMessage.From = (int)MessageFrom.Client;
					MainChatMessage.soc = this.soc;
					MainChatMessage.stringFormat = msg.stringFormat;

					return MainChatMessage;
			}

		}

	}
}
