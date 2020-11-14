using System;
using System.Net.Sockets;
using GHub.EventMessages;
using GHub.Data;

namespace GHub.client
{
	// used for debug only, will be taken out.
	public class Log
	{
		//private System.IO.StreamWriter aFile; 
		private System.IO.StringWriter aFile;
		private System.Text.StringBuilder stringBuilder;

		public Log(string fileName)
		{/*
			stringBuilder = new System.Text.StringBuilder();
			aFile = new System.IO.StringWriter(stringBuilder);
			//aFile = new System.IO.StreamWriter(@"C:\GHub\GH ReWrite\Version 0.02\Logs\" + fileName + ".txt");
			*/
		}

		public void Write(string text)
		{
			//aFile.WriteLine(text);
			//aFile.Flush();
		}

		public void close()
		{
			//aFile.Close();
		}

		public string Get()
		{
			return "disabled at the moment";
			//return stringBuilder.ToString();
		}

	}

	public class Client
	{
		public Socket soc;
		public ListOfServers ServerList;
		public ListOfLocalUsers ClientList;
		public SendMessage send;
		public Log log;

		private static System.Collections.ArrayList infoList = new System.Collections.ArrayList();
		private static string InfListAsString = string.Empty;

		protected Core core;

		public Client(Socket Soc, ListOfServers serverlist, ListOfLocalUsers clientlist, Core theCore)
		{
			soc = Soc;
			ServerList = serverlist;
			ClientList = clientlist;
			core = theCore;
			send = new SendMessage(ServerList,ClientList);
		}

		public void SendMessage(string message)
		{
/*
try
{
	this.log.Write("<Server> " + message);
}
catch{}*/

			// will hold the string message in a byte array
			Byte[] data;
			data = new Byte[message.Length];//256

			for (int i = 0; i < data.Length; i++)
			{
				data[i] = (byte)message[i];
			}

			int total = 0;
			int nbBytesLeft = data.Length;
			int ret = 0;

			try
			{
				while(total < data.Length)
				{		
					ret = soc.Send(data,total,nbBytesLeft,SocketFlags.Partial);
					if (ret <= 0)
						break;

					total += ret;
					nbBytesLeft -= ret;
				}
			}
			catch(System.Exception e)
			{
				this.log.Write(e.Message);
			}

			data = null;

		}

		public virtual void MessageRecieved(Message msg)
		{
		}

		public static void AddToUserInfoList(GHub.client.userInfo nick)
		{
			GHub.Settings.Synchronization.userInfo.WaitOne();
			infoList.Add(nick);
			//upDateInfoList();
			InfListAsString += nick.rawUserInfo + "|";
			GHub.Settings.Synchronization.userInfo.ReleaseMutex();
		}

		public static void RemoveFromUserInfoList(string nick)
		{

			for (int i = 0; i < infoList.Count; i++)
			{
				if (((GHub.client.userInfo)infoList[i]).nick == nick)
					infoList.RemoveAt(i);
			}

			upDateInfoList();

		}

		public static void upDateInfoList()
		{

			InfListAsString = string.Empty;
			foreach (GHub.client.userInfo nick in infoList)
			{
				InfListAsString += nick.rawUserInfo + "|";
			}

		}

		public static GHub.client.userInfo getnickInfo(string nick)
		{
			GHub.Settings.Synchronization.userInfo.WaitOne();
			for (int i = 0; i < infoList.Count; i++)
			{
				if (((GHub.client.userInfo)infoList[i]).nick == nick)
					return (GHub.client.userInfo)infoList[i];
			}
			GHub.Settings.Synchronization.userInfo.ReleaseMutex();
			return null;
		}

		public static string GetUserInfoList()
		{

			switch (infoList.Count)
			{
				case 0:
					return null;
			}

			return InfListAsString;
		}

		public static void ClearInfoList()
		{
			GHub.Settings.Synchronization.userInfo.WaitOne();
			InfListAsString = string.Empty;
			GHub.Settings.Synchronization.userInfo.ReleaseMutex();
		}
	}
}
