

using System;
using GHub.EventMessages;
using GHub.Data;

namespace GHub
{

	public class SendMessage
	{
		private ListOfServers allServers;
		private ListOfLocalUsers allLocalUsers;

		public SendMessage()
		{

		}

		public SendMessage(ListOfServers servers, ListOfLocalUsers users)
		{
			allServers = servers;
			allLocalUsers = users;

		}

		public ListOfServers serverList
		{
			get
			{
				return allServers;
			}
			set
			{
				allServers = value;
			}
		}

		public ListOfLocalUsers userList
		{
			get
			{
				return allLocalUsers;
			}
			set
			{
				allLocalUsers = value;
			}
		}

		public void DisconnectUser(Message msg)
		{
			GHub.client.user.User clnt;
			switch (msg.From)
			{
				case (int)GHub.EventMessages.MessageFrom.Client:

					clnt = (GHub.client.user.User)msg.client;
					clnt.closeAndRemoveUser();
					break;

				default:

					return;
			}
		}


		public void SendMessageToAllLocalUsers(string message)
		{
			GHub.client.user.User user;
			for (int i = 0; i < allLocalUsers.Size(); i++)
			{
				user = (GHub.client.user.User )allLocalUsers.Get(i);
				if (!user.isLoggedIn)
					continue;
				user.SendMessage(message);
			}
			user = null;
			
		}
		public void SendMessageToAllServers(string message)
		{
			GHub.client.server.Server client;
			for (int i = 0; i < allServers.Size(); i++)
			{
				client = (GHub.client.server.Server)allServers.Get(i);
				if (client.isLoggedIn)
					client.SendMessage(message);
			}
			client = null;
		}
		public void SendMessageToAll(string message)
		{
			SendMessageToAllServers(message);
			SendMessageToAllLocalUsers(message);
		}

		public void sendMessageToIndividual(string nick, string message)
		{
			GHub.client.Client clnt = allLocalUsers.Get(nick);

			// if we have found the user (they exist on our server)
			if (clnt != null)
			{
				clnt.SendMessage(message);
				clnt = null;
				return;
			}

			// if we did not find the user on our server, search for the user
			// on other hubs and then send the message to the server that has
			// the user.  if the user is not found the message will not be sent

			GHub.client.server.Server server;
			GHub.client.server.serverUserInfo info;
			for (int eachServer = 0; eachServer < allServers.Size(); eachServer++)
			{
				server = (GHub.client.server.Server)allServers.Get(eachServer);

				info = server.usersToServer.Get(nick);

				if (info != null)
				{
					server.SendMessage(message);

					server = null;
					info = null;
					return;
				}
			}

			server = null;
			info = null;
		}
	}
}
