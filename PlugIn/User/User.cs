using System;
using System.Net.Sockets;
using GHub.Data;
using GHub.EventMessages;

namespace GHub.client.user
{

	public class User : userSendRecieve
	{
		private System.Collections.ArrayList Plugins;
		public User(Socket Soc, ListOfServers serverlist, ListOfLocalUsers clientlist,System.Collections.ArrayList myPlugins, Core thecore):base(Soc,serverlist,clientlist,thecore)
		{
			Plugins = myPlugins;
		}

		protected override void ValidateNick(Message msg)
		{
			bool Handled = false;
			msg.allLocalUsers = this.ClientList;
			msg.allServers = this.ServerList;

			foreach(GHub.plugin.aPlugIn plug in Plugins)
			{
				if (plug.PlugIn.ValidateNick(msg))
					Handled = true;
			}
			if (!Handled)
				base.ValidateNick(msg);
		}

		protected override void Key(Message msg)
		{
			bool Handled = false;
			msg.allLocalUsers = this.ClientList;
			msg.allServers = this.ServerList;

			foreach(GHub.plugin.aPlugIn plug in Plugins)
			{
				if (plug.PlugIn.Key(msg))
					Handled = true;
			}
			if (!Handled)
				base.Key(msg);
		}

		protected override void myInfo(myInfo msg)
		{
			bool Handled = false;
			msg.allLocalUsers = this.ClientList;
			msg.allServers = this.ServerList;

			foreach(GHub.plugin.aPlugIn plug in Plugins)
			{
				if (plug.PlugIn.myInfo(msg))
					Handled = true;
			}
			if (!Handled)
				base.myInfo(msg);
		}

		protected override void AlmostLoggedIn(Message msg)
		{
			bool Handled = false;
			msg.allLocalUsers = this.ClientList;
			msg.allServers = this.ServerList;

			foreach(GHub.plugin.aPlugIn plug in Plugins)
			{
				if (plug.PlugIn.AlmostLoggedIn(msg))
					Handled = true;
			}
			if (!Handled)
				base.AlmostLoggedIn(msg);
		}

		protected override void Version(Message msg)
		{
			bool Handled = false;
			msg.allLocalUsers = this.ClientList;
			msg.allServers = this.ServerList;

			foreach(GHub.plugin.aPlugIn plug in Plugins)
			{
				if (plug.PlugIn.Version(msg))
					Handled = true;
			}
			if (!Handled)
				base.Version(msg);
		}

		protected override void GetNickList(Message msg)
		{
			bool Handled = false;
			msg.allLocalUsers = this.ClientList;
			msg.allServers = this.ServerList;

			foreach(GHub.plugin.aPlugIn plug in Plugins)
			{
				if (plug.PlugIn.GetNickList(msg))
					Handled = true;
			}
			if (!Handled)
				base.GetNickList(msg);
		}

		protected override void UserLeft(Message msg)
		{
			bool Handled = false;
			msg.allLocalUsers = this.ClientList;
			msg.allServers = this.ServerList;

			foreach(GHub.plugin.aPlugIn plug in Plugins)
			{
				if (plug.PlugIn.UserLeft(msg))
					Handled = true;
			}
			if (!Handled)
				base.UserLeft(msg);
		}

		protected override void MainChatMessage(mainChat msg)
		{
			bool Handled = false;
			msg.allLocalUsers = this.ClientList;
			msg.allServers = this.ServerList;

			foreach(GHub.plugin.aPlugIn plug in Plugins)
			{
				if (plug.PlugIn.MainChatMessage(msg))
					Handled = true;
			}
			if (!Handled)
				base.MainChatMessage(msg);
		}

		protected override void PrivateMessage(messageToUser msg)
		{
			bool Handled = false;
			msg.allLocalUsers = this.ClientList;
			msg.allServers = this.ServerList;

			foreach(GHub.plugin.aPlugIn plug in Plugins)
			{
				if (plug.PlugIn.PrivateMessage(msg))
					Handled = true;
			}
			if (!Handled)
				base.PrivateMessage(msg);
		}

		protected override void Search(messageToUser msg)
		{
			bool Handled = false;
			msg.allLocalUsers = this.ClientList;
			msg.allServers = this.ServerList;

			foreach(GHub.plugin.aPlugIn plug in Plugins)
			{
				if (plug.PlugIn.Search(msg))
					Handled = true;
			}
			if (!Handled)
				base.Search(msg);
		}

	}
}
