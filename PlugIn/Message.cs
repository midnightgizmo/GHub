
namespace GHub.EventMessages
{

	public enum Messagetype 
	{
		Key=1, ValidateNick=2, MyPass, Supports, MyINFO, Version, GetNickList,
		MainChatMessage, ChatPM, Unknown, BadPass, ERROR, ValidateDenide, ConnectToMe,
		RevConnectToMe, QUIT, SocketClosed, Search, GetINFO, Kick};


	public enum ServerMessageTypes
	{
		LoggingIn = 100, Exit = 101, BadLogIn, OPList, UserLeft, LogInInfoPlz, AllUsersInfo,loggedIn,NewUser
	}

	public enum MessageFrom
	{
		Server = 200, Client = 2001
	}

	public class Message
	{
		public int type;
		public int From;
		public byte[] RawFormat;
		public System.Net.Sockets.Socket soc;
		public string stringFormat;

		public GHub.Data.ListOfServers allServers;
		public GHub.Data.ListOfLocalUsers allLocalUsers;
		
		public GHub.client.Client client;

		public string sender;
		public string reciever;

		public Message()
		{

		}
	}

	public class messageToUser : Message
	{
		//public string sender;
		//public string reciever;
		public string messageToReciever;

		public messageToUser()
		{

		}
	}

	public class serverLogInInfo : Message
	{
		public string userName;
		public string pass;
		public string hubName;

		public serverLogInInfo()
		{

		}
	}

	public class mainChat : Message
	{
		//public string sender;
	}

	public class supports : Message
	{
		public string[] supporList;
	}

	public class myInfo : Message
	{
		public int slots;
		public int normHubs;
		public int regHubs;
		public int opHubs;
		public string description;
		public string email;
		public string nick;
	}

	public class newUser : myInfo
	{
		public bool isOP;
	}
}