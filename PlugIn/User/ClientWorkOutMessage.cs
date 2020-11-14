using System;
using System.Net.Sockets;
using GHub.Data;
using GHub.EventMessages;

namespace GHub.client.user
{

	public class ClientWorkOutMessage : userInfo
	{
		//private string message;
		private string xmlMessage;
		protected System.Collections.ArrayList recievedMessages;

		public ClientWorkOutMessage(Socket Soc, ListOfServers serverlist, ListOfLocalUsers clientlist, Core thecore):base(Soc,serverlist,clientlist,thecore)
		{

		}

		public override void MessageRecieved(Message msg)
		{
			string recievedMessage = string.Empty;
			if ( (recievedMessages = ConvertToString(msg.RawFormat)) == null)
			{
				recievedMessage = null;
				return;
			}

			for (int eachMessage = recievedMessages.Count - 1; eachMessage > -1; eachMessage--)
			{
				// a plug in may have closed the connection to the socket. therefore we need
				// to do some cleen up.
				if (!this.soc.Connected)
				{
					this.send.SendMessageToAll("$Quit " + this.nick + "|");
					closeAndRemoveUser();
					recievedMessage = null;
					return;
				}
				recievedMessage = (string)recievedMessages[eachMessage];

				// if somthing went wrong with the message or it is not yet ready to be
				// prossesd don't go any further.
				if (recievedMessage == null || recievedMessage == string.Empty)
				{
					recievedMessages.Clear();
					recievedMessages = null;
					return; // might change this from return to continue
				}

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
					
						recievedMessage = null;
						StartOfMessage = null;
						fullMessage = null;
						continue;
				}

				fullMessage.client = this;
				fullMessage.stringFormat += "|";
				Event(fullMessage);

				recievedMessage = null;
				StartOfMessage = null;
				fullMessage = null;
			}

			// a plug in may have closed the connection to the socket. therefore we need
			// to do some cleen up.
			if (!this.soc.Connected)
				closeAndRemoveUser();

			recievedMessage = null;
			recievedMessages.Clear();
			recievedMessages = null;
		}

		protected virtual void Event(Message msg)
		{

		}

		public void closeAndRemoveUser(string MessageToUser)
		{
			this.SendMessage(MessageToUser);
			try
			{
				this.ClientList.Delete(this.soc);
				this.soc.Shutdown(SocketShutdown.Both);
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
				this.ClientList.Delete(this.soc);
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
			{
				abyte = null;
				base64Message = null;
				completeBase64Message = null;
				xmlString = null;
				RecievedMessages = null;
				return null;
			}

			// have to do it this way to get them anoying special charectors
			char[] c= new char[Data.Length];
			for (int i = 0; i < Data.Length; i++)
			{
				c[i] = (char)Data[i];
			}
			
			
			//base64Message = System.Text.UTF8Encoding.ASCII.GetString(Data,0,Data.Length);
			base64Message = new string(c);
			c = null;
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

				abyte = null;
				base64Message = null;
				completeBase64Message = null;
				xmlString = null;

				return RecievedMessages;
			}
			xmlMessage += base64Message;

			// clean up
			abyte = null;
			base64Message = null;
			completeBase64Message = null;
			xmlString = null;
			RecievedMessages = null;

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

			switch (StartOfMessage)
			{


				case "$Supports":

					// Supports string done incorectly so ignore it.
					if (msg.stringFormat.Length < 11)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						splitMessage = null;
						return simpleMessage;
					}

					splitMessage = msg.stringFormat.Substring(firstSpacePosition + 1).Split(' ');

					supports Support = new supports();
					Support.supporList = new string[splitMessage.Length - 1];
					for (int iCount = 0; iCount < splitMessage.Length - 1; iCount++)
					{
						Support.supporList[iCount] = splitMessage[iCount];
					}

					Support.type = (int)Messagetype.Supports;
					Support.RawFormat = msg.RawFormat;
					Support.From = (int)MessageFrom.Client;
					Support.soc = this.soc;
					Support.stringFormat = msg.stringFormat;

					simpleMessage = null;
					splitMessage = null;
					return Support;

				case "$Key":

					// Key string done incorectly so ignore it.
					if (msg.stringFormat.Length < 6)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						splitMessage = null;
						return simpleMessage;
					}

					simpleMessage.type = (int)Messagetype.Key;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Client;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;

					splitMessage = null;
					return simpleMessage;

				case "$ValidateNick":

					// ValidateNick string done incorectly so ignore it.
					if (msg.stringFormat.Length < 15)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						splitMessage = null;
						return simpleMessage;
					}

					simpleMessage.type = (int)Messagetype.ValidateNick;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Client;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;

					splitMessage = null;
					return simpleMessage;

				case "$GetNickList":

					simpleMessage.type = (int)Messagetype.GetNickList;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Client;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;
					
					splitMessage = null;
					return simpleMessage;

				case "$GetINFO":

					splitMessage = msg.stringFormat.Split(' ');
					if (splitMessage.Length < 3)
					{
						simpleMessage.type = -1;
						simpleMessage.RawFormat = msg.RawFormat;
						simpleMessage.From = (int)MessageFrom.Client;
						simpleMessage.soc = this.soc;
						simpleMessage.stringFormat = msg.stringFormat;

						splitMessage = null;
						return simpleMessage;
					}

					 messageToUser getInfo = new messageToUser();

					getInfo.type = (int)Messagetype.GetINFO;
					getInfo.RawFormat = msg.RawFormat;
					getInfo.From = (int)MessageFrom.Client;
					getInfo.soc = this.soc;
					getInfo.stringFormat = msg.stringFormat;
					getInfo.sender = splitMessage[2];
					getInfo.reciever = splitMessage[1];

					splitMessage = null;
					simpleMessage = null;
					return getInfo;

				case "$MyPass":


					// MyPass string done incorectly so ignore it.
					if (msg.stringFormat.Length < 9)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						splitMessage = null;
						return simpleMessage;
					}

					simpleMessage.type = (int)Messagetype.MyPass;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Client;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;

					splitMessage = null;
					return simpleMessage;

				case "$Version":

					// Version string done incorectly so ignore it.
					if (msg.stringFormat.Length < 10)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						return simpleMessage;
					}

					simpleMessage.type = (int)Messagetype.Version;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Client;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;

					splitMessage = null;
					return simpleMessage;

					// a client is sending us a users MyINFO list 
				case "$MyINFO":

					// myINFO string done incorectly so ignore it.
					if (msg.stringFormat.Length < 9)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						splitMessage = null;
						return simpleMessage;
					}
							
					myInfo myINFO = new myInfo();
					myINFO.nick = this.nick;
					// convert the myinfo string to something
					// readable
					this.rawUserInfo = msg.stringFormat;

					// if the user has tried to change there nick while logged in.
					if (myINFO.nick != this.nick)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						splitMessage = null;
						myINFO = null;
						return simpleMessage;
					}

					myINFO.type =(int)Messagetype.MyINFO;
					myINFO.RawFormat = msg.RawFormat;
					myINFO.From = (int)MessageFrom.Client;
					myINFO.soc = this.soc;
					myINFO.stringFormat = msg.stringFormat;

					myINFO.slots = this.slots;
					myINFO.normHubs = this.normHubs;
					myINFO.regHubs = this.regHubs;
					myINFO.opHubs = this.opHubs;
					myINFO.description = this.description;
					myINFO.email = this.eMail;
					myINFO.nick = this.nick;

					splitMessage = null;
					simpleMessage = null;

					return myINFO;
					
					// a user is leaving on another server
				case"$Quit":

					// Quit string done incorectly so ignore it.
					if (msg.stringFormat.Length < 7)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						return simpleMessage;
					}

					simpleMessage.type = (int)Messagetype.QUIT;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Client;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;

					closeAndRemoveUser();
					splitMessage = null;
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

						splitMessage = null;
						FirstPartOfSplit = null;
						return simpleMessage;
					}

					FirstPartOfSplit = splitMessage[1].Split(' ');

					// not enough parameters in the string for it to be right
					if (FirstPartOfSplit.Length < 5)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;

						splitMessage = null;
						FirstPartOfSplit = null;
						return simpleMessage;
					}

					// user trying to send a pm to some one using a nick other
					// than there own.
					if (FirstPartOfSplit[3] != this.nick)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;

						splitMessage = null;
						FirstPartOfSplit = null;
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

					splitMessage = null;
					FirstPartOfSplit = null;
					simpleMessage = null;
					return PrivateMessage;

					// a user on another server is searching for somthing
					// this message must be relayed to all the clients on this
					// server.

				case "$Kick":

					if (msg.stringFormat.Length < 7)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						splitMessage = null;
						return simpleMessage;
					}

					simpleMessage.type = (int)Messagetype.Kick;
					simpleMessage.RawFormat = msg.RawFormat;
					simpleMessage.From = (int)MessageFrom.Client;
					simpleMessage.soc = this.soc;
					simpleMessage.stringFormat = msg.stringFormat;

					splitMessage = null;
					return simpleMessage;

				case "$Search":


					splitMessage = msg.stringFormat.Split(' ');
					if (splitMessage.Length < 3)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						splitMessage = null;
						return simpleMessage;
					}

					messageToUser Search = new messageToUser();
					Search.sender = splitMessage[1];

					Search.type = (int)Messagetype.Search;
					Search.RawFormat = msg.RawFormat;
					Search.From = (int)MessageFrom.Client;
					Search.soc = this.soc;
					Search.stringFormat = msg.stringFormat;

					splitMessage = null;
					return simpleMessage;

				case "$ConnectToMe":
					//$ConnectToMe <remoteNick> <senderIp>:<senderPort> 
					splitMessage = msg.stringFormat.Split(' ');

					if (splitMessage.Length != 3)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						splitMessage = null;
						return simpleMessage;
					}

					messageToUser connectToMe = new messageToUser();

					connectToMe.sender = this.nick;
					connectToMe.reciever = splitMessage[1];
				
					connectToMe.type = (int)Messagetype.ConnectToMe;
					connectToMe.RawFormat = msg.RawFormat;
					connectToMe.From = (int)MessageFrom.Client;
					connectToMe.soc = this.soc;
					connectToMe.stringFormat = msg.stringFormat;

					splitMessage = null;
					simpleMessage = null;
					return connectToMe;

				case "$RevConnectToMe":
					//$RevConnectToMe <nick> <remoteNick>
					splitMessage = msg.stringFormat.Split(' ');

					if (splitMessage.Length != 3)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						splitMessage = null;
						return simpleMessage;
					}
					messageToUser ReConnectToMe = new messageToUser();

					ReConnectToMe.sender = splitMessage[1];
					ReConnectToMe.reciever = splitMessage[2];
				
					ReConnectToMe.type = (int)Messagetype.RevConnectToMe;
					ReConnectToMe.RawFormat = msg.RawFormat;
					ReConnectToMe.From = (int)MessageFrom.Client;
					ReConnectToMe.soc = this.soc;
					ReConnectToMe.stringFormat = msg.stringFormat;

					splitMessage = null;
					simpleMessage = null;
					return ReConnectToMe;

				default:

					if (StartOfMessage.StartsWith("$"))
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;
						splitMessage = null;
						return simpleMessage;
					}

					mainChat MainChatMessage = new mainChat();

					MainChatMessage.type = (int)Messagetype.MainChatMessage;
					MainChatMessage.RawFormat = msg.RawFormat;
					MainChatMessage.From = (int)MessageFrom.Client;
					MainChatMessage.soc = this.soc;
					MainChatMessage.stringFormat = msg.stringFormat;
					MainChatMessage.sender = this.nick;

					if (msg.stringFormat.Length < this.nick.Length + 3)
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;

						splitMessage = null;
						MainChatMessage = null;
						return simpleMessage;
					}
					// if the message does not contain <nick>
					if (  MainChatMessage.stringFormat.Substring(0,3 + this.nick.Length) != "<" + this.nick + "> "  )
					{
						// -1 indicates we are just going to ignore this message
						simpleMessage.type = -1;

						splitMessage = null;
						MainChatMessage = null;
						return simpleMessage;
					}

					splitMessage = null;
					simpleMessage = null;
					return MainChatMessage;
			}

		}

	
	}
}
