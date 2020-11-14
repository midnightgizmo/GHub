using System;
using System.Net.Sockets;
using GHub.Data;

namespace GHub.client.server
{

	public class serverInfo : Client
	{
		//protected string name;
		private bool LoggedIn;
		private string serverName; // a Uneek name used to identify this server.

		public serverInfo(Socket Soc, ListOfServers serverlist, ListOfLocalUsers clientlist,Core theCore):base(Soc,serverlist,clientlist,theCore)
		{

		}

		public bool isLoggedIn
		{
			get
			{
				return LoggedIn;
			}
			set
			{
				LoggedIn = value;
			}
		}

		public string Name
		{
			get
			{
				return serverName;
			}
			set
			{
				serverName = value;
			}
		}
	}
}
