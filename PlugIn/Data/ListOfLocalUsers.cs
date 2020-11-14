using System;
using GHub.client;

namespace GHub.Data
{

	public class ListOfLocalUsers
	{
		private System.Collections.ArrayList Clients;

		public ListOfLocalUsers()
		{
			Clients = new System.Collections.ArrayList();
		}

		public void Add(Client newUser)
		{
			Clients.Add(newUser);
		}

		public void Delete(int position)
		{
			GHub.client.userInfo clnt = (GHub.client.userInfo)Clients[position];
			GHub.client.Client.RemoveFromUserInfoList(clnt.nick);
			Clients.RemoveAt(position);
		}

		public void Delete(string nick)
		{
			GHub.client.user.User clnt;

			for (int i = 0; i < Clients.Count; i++)
			{
				clnt = (GHub.client.user.User)Clients[i];
				if (clnt.nick.ToLower() == nick.ToLower())
				{
					GHub.client.Client.RemoveFromUserInfoList(nick);
					Clients.RemoveAt(i);
					break;
				}
			}
		}

		public void Delete(System.Net.Sockets.Socket soc)
		{
			GHub.client.user.User clnt;

			for (int i = 0; i < Clients.Count; i++)
			{
				clnt = (GHub.client.user.User)Clients[i];
				if (clnt.soc == soc)
				{
					GHub.client.Client.RemoveFromUserInfoList(clnt.nick);
					Clients.RemoveAt(i);
					break;
				}
			}
		}

		public Client Get(int position)
		{
			return (Client)Clients[position];
		}

		public Client Get(string nick)
		{
			GHub.client.userInfo clnt;

			for (int i = 0; i < Clients.Count; i++)
			{
				clnt = (GHub.client.userInfo)Clients[i];
				if (clnt.nick.ToLower() == nick.ToLower())
				{
					return clnt;
				}
			}
			return null;
		}

		public int Size()
		{
			return Clients.Count;
		}

		public System.Collections.ArrayList GetArray()
		{
			return Clients;
		}

		public void CloseAllSockets()
		{
			GHub.client.userInfo clnt;

			for (int i = 0; i < Clients.Count; i++)
			{
				clnt = (GHub.client.userInfo)Clients[i];

				try
				{
					clnt.soc.Shutdown(System.Net.Sockets.SocketShutdown.Both);
					clnt.soc.Close();
				}
				catch
				{
				}
			}
		}
	}
}
