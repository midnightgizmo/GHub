using System;
using System.Net.Sockets;
using GHub.client;
using GHub.client.server;
using GHub.client.user;
using GHub.Data;
using GHub.EventMessages;


namespace GHub
{

	public class Core
	{
		#region Atributes
		private ListOfServers serverArray;
		private ListOfLocalUsers clientArray;

		private Socket serverSock;
		private int port;
		private bool ServerState;

		// thread that listens for new connections
		private System.Threading.Thread connectionThread;
		// thread checks for incoming data from servers
		private System.Threading.Thread DataRecievedThread;

		private System.Threading.Thread GCThread; // collects rubish

		private System.AsyncCallback ConectionRecivedCallBack;

		private System.Threading.Mutex mutexServerArray;
		private System.Threading.Mutex mutexClientArray;

		// plug in stuff
		// holds an array of all plug ins that were loaded.
		private System.Collections.ArrayList myPlugins;

		#endregion


		private MessageReceivedEventHandler ehServerLost = null;
		private MessageReceivedEventHandler ehServerConnectMade = null;
		private MessageReceivedEventHandler ehServerConnecting = null;
		private MessageReceivedEventHandler ehPlugInLoaded = null;

		private System.Threading.ManualResetEvent newConnection;

		public Core()
		{

			ServerState = false;

			ConectionRecivedCallBack = new AsyncCallback(AcceptNewConnection);

			mutexServerArray = new System.Threading.Mutex(false,"server_array");
			mutexClientArray = new System.Threading.Mutex(false,"client_array");
			newConnection = new System.Threading.ManualResetEvent(false);

			myPlugins = new System.Collections.ArrayList();
		}

		public bool Connected
		{
			get
			{
				return ServerState;
			}
		}

		// returns -1 if faild and 1 if sucsess.
		public int Connect(int thePort)
		{
			port = thePort;
			// set up new instances of all veriables
			serverArray = new ListOfServers();
			clientArray = new ListOfLocalUsers();

			System.Net.IPEndPoint endpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any,port);
			serverSock = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

			System.GC.Collect();
			try
			{
				serverSock.Bind(endpoint);
			}
			catch (System.Exception)
			{
				serverArray = null;
				clientArray = null;
				endpoint = null;
				serverSock = null;

				return -1; // an error accound, most probs port is int use
			}

			serverSock.Listen(100);
			ServerState = true;

			// Setting up Threads //////////////////////////////////////////////////////////////////////
			connectionThread = new System.Threading.Thread(new System.Threading.ThreadStart(AcceptLoop));
			connectionThread.Name = "Accept Loop Thread";
			connectionThread.IsBackground = true;
			connectionThread.Start();

			DataRecievedThread = new System.Threading.Thread(new System.Threading.ThreadStart(RecieveData));
			DataRecievedThread.Name = "data recieved";
			DataRecievedThread.IsBackground = true;
			DataRecievedThread.Start();

			GCThread = new System.Threading.Thread(new System.Threading.ThreadStart(GarbageCollection));
			GCThread.Name = "GCThread";
			GCThread.IsBackground = true;
			GCThread.Start();

			endpoint = null;


			foreach (GHub.plugin.aPlugIn plug in myPlugins)
			{
				plug.PlugIn.ConnectionMade(serverArray,clientArray);
			}
			return 1;

		}

		public void Disconnect()
		{
			ServerState = false;
			newConnection.Set();

			while (DataRecievedThread.ThreadState != System.Threading.ThreadState.Stopped)
				System.Threading.Thread.Sleep(50);

			while (connectionThread.ThreadState != System.Threading.ThreadState.Stopped)
				System.Threading.Thread.Sleep(50);

			mutexServerArray.WaitOne();
			mutexClientArray.WaitOne();

				serverArray.CloseAllSockets();
				clientArray.CloseAllSockets();

			mutexClientArray.ReleaseMutex();
			mutexServerArray.ReleaseMutex();

			//serverSock.Shutdown(System.Net.Sockets.SocketShutdown.Both);
			serverSock.Close();
			GHub.client.Client.ClearInfoList();
			GHub.Settings.ACCOUNTS.profiles.SaveToFile(1);
			GHub.Settings.ACCOUNTS.usersAccounts.SaveToFile(1);

			serverArray = null;
			clientArray = null;
			serverSock = null;
			connectionThread = null;
			DataRecievedThread = null;

			System.GC.Collect();

		}

		public void AcceptLoop()
		{
			System.IAsyncResult asyncResult;

serverSock.Blocking = false;
			while (ServerState)
			{

					newConnection.Reset();
						asyncResult = serverSock.BeginAccept(new AsyncCallback(ConectionRecivedCallBack), serverSock);
					newConnection.WaitOne();
					
			}
			
			return;
		}

		private void AcceptNewConnection(System.IAsyncResult asyncResult)
		{
			Socket servSock;
			Socket clntSock;
			byte[] recieved;
			int length;

			GHub.client.Client client;

			
			try
			{
				servSock = (Socket)asyncResult.AsyncState;
				clntSock = servSock.EndAccept(asyncResult);
				newConnection.Set();
				clntSock.Blocking = true;
				clntSock.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket,System.Net.Sockets.SocketOptionName.ReceiveTimeout, 10000); //10 seconds.
				clntSock.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket,System.Net.Sockets.SocketOptionName.SendTimeout, 10000); //10 seconds.

				

			}
			catch(System.InvalidOperationException)
			{
				servSock = null;
				clntSock = null;
				recieved = null;
				newConnection.Set();
				return;
			}
			catch(System.Exception e)
			{
				string a = e.Message;
				newConnection.Set();
				return;
			}

			client = new Client(clntSock,serverArray,clientArray,this);
			try
			{
				client.SendMessage("$Lock EXTENDEDPROTOCOL::This_hub_was_written_by_Gizmo} Pk=GHub|");
				client.SendMessage("$Supports NoGetINFO NoHello|");

				recieved = new byte[9];
				length = clntSock.Receive(recieved,0,recieved.Length,SocketFlags.Peek);
			}
			catch
			{
				//clntSock.Shutdown(SocketShutdown.Both);
				clntSock.Close();

				servSock = null;
				clntSock = null;
				recieved = null;
				return;
			}

			switch(length)
			{
				case 0:
					servSock = null;
					clntSock = null;
					recieved = null;
					return;
			}
			byte[] exactRecieved = new byte[length];
			for (int i = 0; i < length; i++)
				exactRecieved[i] = recieved[i];

			string textData = System.Text.ASCIIEncoding.ASCII.GetString(exactRecieved);

			//clntSock.Blocking = false;
			// if we are dealing with a server
			if (textData.IndexOf("$LogginIn") != -1)
			{
				Server newServer = new Server(clntSock,serverArray,clientArray,myPlugins,this);

				GHub.Settings.Synchronization.serverArray.WaitOne();
				serverArray.Add(newServer);
				GHub.Settings.Synchronization.serverArray.ReleaseMutex();

				newServer.log = new Log(  ((System.Net.IPEndPoint)client.soc.RemoteEndPoint).Address.ToString ()  );
				newServer.log.Write("<Server to server> $Lock EXTENDEDPROTOCOL::This_hub_was_written_by_Gizmo} Pk=GHub|");
			}
				// if we are dealing with a client
			else
			{
				User newUser = new User(clntSock,serverArray,clientArray,myPlugins,this);
				newUser.ipAddress = ((System.Net.IPEndPoint)newUser.soc.RemoteEndPoint).Address.ToString ();

				// if the user if banned we must deni them access to the hub.
				if ( GHub.Settings.BANS.tempBans.isBanned(newUser.ipAddress)  )
				{

					try
					{
						clntSock.Shutdown(System.Net.Sockets.SocketShutdown.Both);
						clntSock.Close();
					}
					catch{}
					finally
					{
						servSock = null;
						clntSock = null;
						recieved = null;
						textData = null;
						exactRecieved = null;
					}

					return;
				}

				GHub.Settings.Synchronization.clientArray.WaitOne();
				clientArray.Add(newUser);
				GHub.Settings.Synchronization.clientArray.ReleaseMutex();
				newUser.log = new Log(  ((System.Net.IPEndPoint)client.soc.RemoteEndPoint).Address.ToString ()  );
				newUser.log.Write("<Server to client> $Lock EXTENDEDPROTOCOL::This_hub_was_written_by_Gizmo} Pk=GHub|");
			}
			
			servSock = null;
			clntSock = null;
			recieved = null;
			textData = null;
			exactRecieved = null;
			
		}

	
		public int ConnectToServer(string address,int serverPort, string serverName)
		{
			Socket conToSer = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			System.Net.IPHostEntry hostEntry;// = System.Net.Dns.Resolve(address);
			System.Net.IPEndPoint endPoint = null;// = new System.Net.IPEndPoint(hostEntry.AddressList[0],serverPort);

			// atempt to resolve the DNS to an IP address
			try
            {
                hostEntry = System.Net.Dns.GetHostEntry(address);
                // was used in .net version 1.1
				//hostEntry = System.Net.Dns.Resolve(address);
			}
			// If an error occours it is mostly likely because the DNS could not resolve the name to
			// an ip address and so returned back an error.
			//
			// We now must stop this connection from going any further and inform the user that
			// an error occound.
			catch(System.Net.Sockets.SocketException)
			{
				conToSer = null;
				hostEntry = null;
			}

			endPoint = new System.Net.IPEndPoint(hostEntry.AddressList[0],serverPort);

			try
			{
				// set the ip adderss and port we will be reciving data on
				conToSer.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any,0));
			}
			catch (System.Exception)
			{
			
				conToSer = null;
				hostEntry = null;
				endPoint = null;

				return -1;
			}

			try
			{
				conToSer.Connect(endPoint);
			}
			catch(System.Exception)
			{
				conToSer = null;
				hostEntry = null;
				endPoint = null;

				return -1;
			}

			Server newServer = new Server(conToSer,serverArray,clientArray,myPlugins,this);

			newServer.Name = serverName;
			GHub.Settings.Synchronization.serverArray.WaitOne();
			serverArray.Add(newServer);
			GHub.Settings.Synchronization.serverArray.ReleaseMutex();


			conToSer = null;
			hostEntry = null;
			endPoint = null;
			newServer = null;

			return 1;

		}

		// the GUI is asking us to shut down a connection to a connecting server.
		public void DisconnectFromServer(string serverName)
		{
			Server ser;
			string nick;

			GHub.Settings.Synchronization.serverArray.WaitOne();
			GHub.Settings.Synchronization.clientArray.WaitOne();
			if (GHub.Settings.MultHubs.MultiHubs.Get(serverName).ConnectionState != 2)
			{
				ser = null;
				return;
			}

			for (int eachServer = 0; eachServer < serverArray.Size(); eachServer++)
			{
				ser = (Server)serverArray.Get(eachServer);
				if (ser.Name == serverName)
				{
					ser.SendMessage("$Exit|");

					try
					{
						serverArray.Delete(ser.soc);
						ser.soc.Close();

						

	
						for (int i = 0; i < ser.usersToServer.Size(); i++)
						{
							nick = ser.usersToServer.Get(i).nick;
							ser.usersToServer.Delete(nick);
							GHub.client.Client.RemoveFromUserInfoList(nick);
							ser.send.SendMessageToAllLocalUsers("$Quit " + nick + "|");
							i--;
						}
						
					}
					catch(System.Exception e)
					{

					}
					
					GHub.Settings.Synchronization.serverArray.ReleaseMutex();
					GHub.Settings.Synchronization.clientArray.ReleaseMutex();
					
					nick = null;
					ser = null;
					// indicates we have disconnected from other server.
					return;
				}
			}
			GHub.Settings.Synchronization.serverArray.ReleaseMutex();
			ser = null;
			nick = null;
			return; // indicates the server could not be found.
		}

		private void RecieveData()
		{
			System.IO.StreamWriter file = new System.IO.StreamWriter("Error_Log.txt");

			Client aClient = null;
			Message Event;
			int bytesRecieved;
			string quitMessage;

			while (ServerState)
			{
				System.Threading.Thread.Sleep(100);
				try
				{
					for (int eachServer = 0; eachServer < serverArray.Size(); eachServer++)
					{
						mutexServerArray.WaitOne();
						aClient = serverArray.Get(eachServer);

						try
						{

							// if there is NO data to be read, go to the next one in the for loop
							if(  !aClient.soc.Poll(0,System.Net.Sockets.SelectMode.SelectRead)  )
							{// do a check to see last time we recieved info from this user
								mutexServerArray.ReleaseMutex();
								continue;
							}
						
							bytesRecieved = aClient.soc.Receive(new byte[2],0,2,System.Net.Sockets.SocketFlags.Peek);
						}
						catch(System.Exception s)
						{


							quitMessage = "$Exit|";
							Event = new Message();
							Event.RawFormat = System.Text.ASCIIEncoding.ASCII.GetBytes(quitMessage.ToCharArray());
							Event.type = (int)MessageFrom.Server;

							aClient.MessageRecieved(Event);

							mutexServerArray.ReleaseMutex();

							
							quitMessage = null;
							Event = null;
							aClient = null;
							continue;
						}

						

						if (  bytesRecieved  == 0)
						{
							quitMessage = "$Exit|";
							Event = new Message();
							Event.RawFormat = System.Text.ASCIIEncoding.ASCII.GetBytes(quitMessage.ToCharArray());
							Event.type = (int)MessageFrom.Server;

							aClient.MessageRecieved(Event);
							mutexServerArray.ReleaseMutex();

							quitMessage = null;
							Event = null;
							aClient = null;
							continue;
						}
						mutexServerArray.ReleaseMutex();

						Event = new Message();
						mutexServerArray.WaitOne();
						Event.RawFormat = GetData(aClient.soc);
						mutexServerArray.ReleaseMutex();
						Event.type = (int)MessageFrom.Server;

						mutexServerArray.WaitOne();
						aClient.MessageRecieved(Event);
						mutexServerArray.ReleaseMutex();

						quitMessage = null;
						Event = null;
						aClient = null;
						
					}
				


					for (int eachClient = 0; eachClient < clientArray.Size(); eachClient++)
					{				
						GHub.Settings.Synchronization.clientArray.WaitOne();
						aClient = clientArray.Get(eachClient);
						

						try 
						{
							// if there is NO data to be read, go to the next one in the for loop
							if(  !aClient.soc.Poll(0,System.Net.Sockets.SelectMode.SelectRead)  )
							{// do a check to see last time we recieved info from this user
								//if (((GHub.client.userInfo)aClient).nick == string.Empty)
								//	throw new Exception("User not allowed");
								GHub.Settings.Synchronization.clientArray.ReleaseMutex();

								quitMessage = null;
								Event = null;
								aClient = null;
								continue;
							}

							bytesRecieved = aClient.soc.Receive(new byte[2],0,2,System.Net.Sockets.SocketFlags.Peek);
						}
						catch(System.Exception e)
						{

							if (  ((userInfo)aClient).nick == "")
							{
								quitMessage = "Deletethis";
							}
							quitMessage = "$Quit " + ((GHub.client.userInfo)aClient).nick + "|";
							Event = new Message();
							Event.RawFormat = System.Text.ASCIIEncoding.ASCII.GetBytes(quitMessage.ToCharArray());
							Event.type = (int)MessageFrom.Client;

							aClient.MessageRecieved(Event);
							GHub.Settings.Synchronization.clientArray.ReleaseMutex();

							quitMessage = null;
							Event = null;
							aClient = null;
							continue;
						}

						GHub.Settings.Synchronization.clientArray.ReleaseMutex();


			
						GHub.Settings.Synchronization.clientArray.WaitOne();
						if (  bytesRecieved  == 0)
						{
							if (  ((userInfo)aClient).nick == "")
							{
								quitMessage = "Deletethis";
							}
							quitMessage = "$Quit " + ((GHub.client.userInfo)aClient).nick + "|";
							Event = new Message();
							Event.RawFormat = System.Text.ASCIIEncoding.ASCII.GetBytes(quitMessage.ToCharArray());
							Event.type = (int)MessageFrom.Client;

							aClient.MessageRecieved(Event);
							GHub.Settings.Synchronization.clientArray.ReleaseMutex();

							quitMessage = null;
							Event = null;
							aClient = null;
							continue;
						}
						GHub.Settings.Synchronization.clientArray.ReleaseMutex();
					

						Event = new Message();
						GHub.Settings.Synchronization.clientArray.WaitOne();
						Event.RawFormat = GetData(aClient.soc);
						GHub.Settings.Synchronization.clientArray.ReleaseMutex();
						Event.type = (int)MessageFrom.Client;

						GHub.Settings.Synchronization.clientArray.WaitOne();
						aClient.MessageRecieved(Event);
						GHub.Settings.Synchronization.clientArray.ReleaseMutex();
					
						quitMessage = null;
						Event = null;
						aClient = null;
					}
				}
				catch(System.Exception e)
				{
					#region error log
					try
					{
						file.WriteLine("Current user count = " + clientArray.Size());
						file.Flush();
						file.WriteLine("ErrorType = " + e.Message);
						file.Flush();
						file.WriteLine("Stack trace = " + e.StackTrace);
						file.WriteLine("Error source " + e.Source);
						file.Flush();
						file.WriteLine("Error Target " + e.TargetSite);
						file.Flush();
						if (aClient != null)
						{
							file.WriteLine("Current user we cort error on = " + ((GHub.client.userInfo)aClient).rawUserInfo);
							file.Flush();
						}

						file.WriteLine("-----------------------------------------------------------");
						file.WriteLine("-----------------------------------------------------------");
						file.WriteLine("-----------------------------------------------------------");
						file.Flush();
					}
					catch(System.Exception a)
					{
						file.WriteLine("unable to write error log - " + a.Message);
						file.WriteLine("-----------------------------------------------------------");
						file.WriteLine("-----------------------------------------------------------");
						file.WriteLine("-----------------------------------------------------------");
						file.Flush();
					}
					#endregion

				}


			}
			file.Close();
		}

		private void GarbageCollection()
		{
			while(ServerState)
			{
				GC.Collect();
				Client aClient = null;

				mutexServerArray.WaitOne();
				for (int eachServer = 0; eachServer < serverArray.Size(); eachServer++)
				{
					try
					{
						aClient = clientArray.Get(eachServer);
						aClient.SendMessage("$KeepAlive|");
					}
					catch
					{
					}
					
				}
				mutexServerArray.ReleaseMutex();
				System.Threading.Thread.Sleep(300000);// 5 mins
			}
		}
		
		private byte[] GetData(Socket soc)
		{
			byte[] buffer = new byte[300];
			byte[] data;
			int length;
			
			try
			{
				length = soc.Receive(buffer,0,buffer.Length,SocketFlags.None);
			}
			catch
			{
				buffer = null;
				data = null;
				return null;
			}

			data = new byte[length];
			for (int i = 0; i < length; i++)
				data[i] = buffer[i];
			
			buffer = null;
			return data;
		}

		public void LoadPlugIns()
		{
			myPlugins = GHub.plugin.plugin.PluginLoader();

			foreach (GHub.plugin.aPlugIn plug in myPlugins)
			{
				
				PlugInLoaded(plug.PlugIn.PluginLoaded(),plug.plugInName);
			}
		}

		public string[] GetPlugInNames()
		{
			string[] listOfPlugIns = new string[myPlugins.Count];
			for (int i = 0; i < myPlugins.Count; i++)
			{
				listOfPlugIns[i] = ((GHub.plugin.aPlugIn)myPlugins[i]).plugInName;
			}

			if (listOfPlugIns.Length == 0)
			{
				return null;
			}
			return listOfPlugIns;
		}

		public void LoadSettings()
		{
			GHub.Settings.ACCOUNTS.profiles.LoadFromFile();
			GHub.Settings.ACCOUNTS.usersAccounts.LoadFromFile();
			GHub.Settings.MultHubs.MultiHubs.LoadHubs();
		}
	
	
	///////////////////////////////////////////////////////////////////////////////////////
	
		#region connection to server lost
		public void AddServerLost(MessageReceivedEventHandler handler)
		{
			ehServerLost=(MessageReceivedEventHandler)Delegate.Combine(ehServerLost, handler);
		}
		public void RemoveServerLost(MessageReceivedEventHandler handler)
		{
			ehServerLost=(MessageReceivedEventHandler)Delegate.Remove(ehServerLost, handler);
		}

		protected virtual void OnServerLost(MessageReceivedEventArgs e)
		{
			if (ehServerLost!=null)
			{
				ehServerLost(this, e);
			}
		}

		// If for some reason a server that we were connected to got disconected.
		// This could be because we chose to drop the server from our end.
		// OR
		// Ther server on the other end decied to drop the connection to us.
		//
		// This would normly be private but i need to access it some where else.
		// this should not be called any where in the GUI part
		public void ServerLost(object message)
		{
			MessageReceivedEventArgs args=new MessageReceivedEventArgs(message,null);
			OnServerLost(args);
		}
		#endregion

		#region connection to Server Made
		public void AddServerConnectionMade(MessageReceivedEventHandler handler)
		{
			ehServerConnectMade=(MessageReceivedEventHandler)Delegate.Combine(ehServerConnectMade, handler);
		}
		public void RemoveServerConnectionMade(MessageReceivedEventHandler handler)
		{
			ehServerConnectMade=(MessageReceivedEventHandler)Delegate.Remove(ehServerConnectMade, handler);
		}

		protected virtual void OnServerConnectionMade(MessageReceivedEventArgs e)
		{
			if (ehServerConnectMade!=null)
			{
				ehServerConnectMade(this, e);
			}
		}

		public void ConnctionToServerMade(object message)
		{
			MessageReceivedEventArgs args = new MessageReceivedEventArgs(message,null);
			OnServerConnectionMade(args);
		}
		#endregion
	

		#region connecting to server
		public void AddConnectingToServer(MessageReceivedEventHandler handler)
		{
			ehServerConnecting=(MessageReceivedEventHandler)Delegate.Combine(ehServerConnecting, handler);
		}
		public void RemoveConnectingToServer(MessageReceivedEventHandler handler)
		{
			ehServerConnecting=(MessageReceivedEventHandler)Delegate.Remove(ehServerConnecting, handler);
		}

		protected virtual void OnConnectingToServer(MessageReceivedEventArgs e)
		{
			if (ehServerConnecting!=null)
			{
				ehServerConnecting(this, e);
			}
		}

		public void ConnectingToServer(object message)
		{
			MessageReceivedEventArgs args = new MessageReceivedEventArgs(message,null);
			OnConnectingToServer(args);
		}
		#endregion



		#region Plug in loaded event
		public void AddPlugInLoaded(MessageReceivedEventHandler handler)
		{
			ehPlugInLoaded=(MessageReceivedEventHandler)Delegate.Combine(ehPlugInLoaded, handler);
		}
		public void RemoveCPlugInLoaded(MessageReceivedEventHandler handler)
		{
			ehPlugInLoaded=(MessageReceivedEventHandler)Delegate.Remove(ehPlugInLoaded, handler);
		}

		protected virtual void OnPlugInLoaded(MessageReceivedEventArgs e)
		{
			if (ehPlugInLoaded!=null)
			{
				ehPlugInLoaded(this, e);
			}
		}

		public void PlugInLoaded(object message,object secMessage)
		{
			MessageReceivedEventArgs args = new MessageReceivedEventArgs(message,secMessage);
			OnPlugInLoaded(args);
		}
		#endregion


	}


	public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

	public class MessageReceivedEventArgs : EventArgs
	{      
		public object msg; //identifies the action
		public object secondMsg;
		public MessageReceivedEventArgs(object message, object secondMessage)
		{
			msg=message; 
			secondMsg = secondMessage;
		}
	}

}
