using System;
using GHub.client;

namespace GHub.Data
{

	public class ListOfServers
	{
		private System.Collections.ArrayList servers;

		public ListOfServers()
		{
			servers = new System.Collections.ArrayList();
		}

		public void Add(Client newServer)
		{
			servers.Add(newServer);
		}

		public void Delete(int position)
		{
			try
			{
				servers.RemoveAt(position);
			}
			catch(System.Exception e)
			{
				string temp = e.Message;
			}
		}

		public void Delete(System.Net.Sockets.Socket soc)
		{
			try
			{
				GHub.client.server.ServerWorkOutMessage clnt;

				for (int i = 0; i < servers.Count; i++)
				{
					clnt = (GHub.client.server.ServerWorkOutMessage)servers[i];
					if (clnt.soc == soc)
					{/*
					for (int z = 0; i < clnt.usersToServer.Size(); z++)
					{
						GHub.client.Client.RemoveFromUserInfoList(clnt.usersToServer.Get(z).nick);
					}*/
						servers.RemoveAt(i);
						return;
					}
				}
			}
			catch(System.Exception e)
			{
				string temp = e.Message;
			}
		}
/*
		public void Delete(Client clnt)
		{
		}
*/
		public Client Get(int position)
		{
			try
			{
				return (Client)servers[position];
			}
			catch(System.Exception e)
			{
				string temp = e.Message;
				return null;
			}
		}

		public int Size()
		{
			return servers.Count;
		}

		
		public System.Collections.ArrayList GetArray()
		{
			return servers;
		}

		public void CloseAllSockets()
		{
			Client clnt;
			
			for (int i = 0; i < servers.Count; i++)
			{
				clnt = (Client)servers[i];

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
