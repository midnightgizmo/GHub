using System;
using System.Net.Sockets;
using GHub.Data;

namespace GHub.client
{

	public class userInfo : Client
	{
		private string rawInfo;
		private string Description;
		private string Tag;
		private int Slots;
		private int NormHubs;
		private int RegHubs;
		private int OPhubs;
		private long shareSize;
		private string EMail;
		private string Connection;
		private string Nick;
		private bool LoggedIn;
		private bool LoggedInCorrectly;
		private bool SupportsSupport;// used to indicate if the client supports NoGetINFO NoHello.
		private bool IsOP;
		private string IPaddress;
		private GHub.Settings.ACCOUNTS.profile pro;


		public userInfo(Socket Soc, ListOfServers serverlist, ListOfLocalUsers clientlist,Core thecore):base(Soc,serverlist,clientlist,thecore)
		{
			rawInfo = string.Empty;
			Description = string.Empty;
			Tag = string.Empty;
			EMail = string.Empty;
			Connection = string.Empty;
			Nick = string.Empty;
			LoggedIn = false;
			LoggedInCorrectly = false;
			SupportsSupport = false;
			IsOP = false;
		}

		//"$MyINFO $ALL Gizmo <++ V:0.668,M:A,H:1/0/1,S:4>$ $DSL$$65390146729$"
		public string rawUserInfo
		{
			get
			{
				return rawInfo;
			}
			set
			{
				rawInfo = value;
				ParseMyINFO();
			}
		}

		private void ParseMyINFO()
		{
			//[2] name and description //"ALL Gizmo abc<++ V:0.668,M:A,H:1/0/1,S:4>"
			//[4] connection type //"DSL"
			//[6] share size //"65390146729"
			string[] infoSplitUp = rawInfo.Split('$');
			int position;
			// now looks like following
			// "Gizmo abc<++ V:0.668,M:A,H:1/0/1,S:4>"
			infoSplitUp[2] = infoSplitUp[2].Substring(4);
			// find where the first space position is. this will let us know
			// where the nick ends and the description begins.
			position = infoSplitUp[2].IndexOf(' ',0);
			
			this.Nick = infoSplitUp[2].Substring(0,  infoSplitUp[2].Length  - (infoSplitUp[2].Length - position)  );
			// "abc<++ V:0.668,M:A,H:1/0/1,S:4>"
			infoSplitUp[2] = infoSplitUp[2].Substring(position + 1);
			
			// find out where the tag starts so that the description and
			// tag can be split up.
			position = -1;
			for (int i = infoSplitUp[2].Length - 1; i > -1; i--)
			{
				if (infoSplitUp[2][i] == '<')
				{
					// holds the position of where the start of the tag is.
					position = i;
					break;
				}
			}

			// if we found a tag split the description from it
			if (position != -1)
			{
				// if there is no description with the tag
				if (position == 0)
					this.Description = string.Empty;
				// if there is a description and a tag
				else
					this.Description = infoSplitUp[2].Substring(0,  infoSplitUp[2].Length - (infoSplitUp[2].Length - position)  );
				this.Tag = infoSplitUp[2].Substring(position);
				splitTag();
			}
				// if no tag found.
			else
				this.Description = infoSplitUp[2];

			Connection = infoSplitUp[4];
			
			shareSize = long.Parse(infoSplitUp[6]);

					
			
		}

		private void splitTag()
		{
		}

		public string description
		{
			get
			{
				return Description;
			}
			set
			{
				Description = value;
			}
		}

		public string tag
		{
			get
			{
				return Tag;
			}
			set
			{
				Tag = value;
			}
		}

		public int slots
		{
			get
			{
				return Slots;
			}
			set
			{
				Slots = value;
			}
		}

		public int normHubs
		{
			get
			{
				return NormHubs;
			}
			set
			{
				NormHubs = value;
			}
		}

		public int regHubs
		{
			get
			{
				return RegHubs;
			}
			set
			{
				RegHubs = value;
			}
		}

		public int opHubs
		{
			get
			{
				return OPhubs;
			}
			set
			{
				OPhubs = value;
			}
		}

		public string eMail
		{
			get
			{
				return EMail;
			}
			set
			{
				EMail = value;
			}
		}

		public string connection
		{
			get
			{
				return Connection;
			}
			set
			{
				Connection = value;
			}
		}

		public string nick
		{
			get
			{
				return Nick;
			}
			set
			{
				Nick = value;
			}
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

		public bool loggedInCorrectly
		{
			get
			{
				return LoggedInCorrectly;
			}
			set
			{
				LoggedInCorrectly = value;
			}
		}

		public bool isOP
		{
			get
			{
				return IsOP;
			}
			set
			{
				IsOP = value;
			}
		}

		public bool supportsSupport
		{
			get
			{
				return SupportsSupport;
			}
			set
			{
				SupportsSupport = value;
			}
		}

		public string ipAddress
		{
			get
			{
				return IPaddress;
			}
			set
			{
				IPaddress = value;
			}
		}

		public GHub.Settings.ACCOUNTS.profile Profile
		{
			get
			{
				return pro;
			}
			set
			{
				pro = value;
			}
		}

	}
}
