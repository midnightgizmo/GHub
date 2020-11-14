using System;
using System.Net.Sockets;
using GHub.client.server;
using GHub.Data;
namespace GHub.client.server
{

	public class Server : serverSendRecieve
	{
		
		private System.Collections.ArrayList Plugins;
		public Server(Socket Soc, ListOfServers serverlist, ListOfLocalUsers clientlist, System.Collections.ArrayList myPlugins, Core thecore):base(Soc,serverlist,clientlist,thecore)
		{
			Plugins = myPlugins;
		}



	}
}
