

using System;
using GHub.Settings;


namespace GHub.Settings.XML
{
	#region XML

	public class xmlSave
	{
		public xmlSave()
		{
		}
	}

	public class xmlLoad
	{
		public xmlLoad()
		{
		}
	}
	#endregion
}

namespace GHub.Settings.BANS
{
	#region Bans


	public class ipBans
	{
		public ipBans()
		{
		}

		public static void Add(string ip)
		{
		}
	}

	public class tempBans
	{
		private static System.Collections.ArrayList bans = new System.Collections.ArrayList();
		public tempBans()
		{
		}

		public static void Add(string ip, System.DateTime ExpireTime)
		{
			BanInfo banInfo = new BanInfo();
			banInfo.ip = ip;
			banInfo.expireTime = ExpireTime;

			bans.Add(banInfo);
		}

		public static void Remove(string ip)
		{
			
		}

		public static void Remove(int Position)
		{
			bans.RemoveAt(Position);
		}

		public static bool isBanned(string ip)
		{
			BanInfo IP;
			for(int i = 0; i < bans.Count; i++)
			{
				IP = (BanInfo)bans[i];

				if (IP.ip == ip)
				{
					if (IP.expireTime.Ticks > System.DateTime.Now.Ticks)
					{
						return true;
					}
					bans.RemoveAt(i);
					return false;
				}
			}
			return false;
		}
		public static string Get(int position)
		{
			return ((BanInfo)bans[position]).ip;
		}
		public static int Size()
		{
			return bans.Count;
		}
		public static void Clear()
		{
			bans.Clear();
		}

	}

	public class BanInfo
	{
		public string ip;
		public System.DateTime expireTime;

		public BanInfo()
		{
		}
	}

	public class nickBans
	{
		public nickBans()
		{
		}
	}

	#endregion
}

namespace GHub.Settings.ACCOUNTS
{
	#region Accounts

	public class profiles
	{
		private static System.Collections.ArrayList Profiles = new System.Collections.ArrayList();
		private static profile DefaultPro = null;

		public profiles()
		{
		}

		public static profile DefaultProfile
		{
			get
			{
				return DefaultPro;
			}
		}

		public static void LoadFromFile()
		{

			System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
			try
			{
				xmldoc.Load(@"Profiles.xml");
			}
			catch(System.IO.FileNotFoundException)
			{
				SaveToFile(0);
				xmldoc.Load(@"Profiles.xml");
			}

			// Find out how many accounts we are dealing with
			int NumberOfAccounts = xmldoc.DocumentElement.ChildNodes.Count;

			// if the file exists but there are not accounts present in it. exit the function
			if (NumberOfAccounts < 1) return;


			// will hold each account so they can be examined
			System.Xml.XmlNode EachAccount;
			
			// Go through each account in the xml file
			for (int iCount = 0; iCount < NumberOfAccounts; iCount++)
			{
				// hold the data for each profile in EachAccount
				EachAccount = xmldoc.DocumentElement.ChildNodes.Item(iCount);

				profile pro = new profile();

				// Go through each node with in each profile. e.g. nick then pass etc.
				for (int iInnerNodeCount = 0; iInnerNodeCount < EachAccount.ChildNodes.Count; iInnerNodeCount++)
				{
					// find out which item we are currently dealing with and then store the info
					// into userSettings[] array.
					string temp = EachAccount.ChildNodes.Item(iInnerNodeCount).LocalName.ToLower();
					switch (EachAccount.ChildNodes.Item(iInnerNodeCount).LocalName)
					{
						case "isOperator":
						
							try
							{
								pro.isOP = bool.Parse(  EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText  );
							}
							catch(System.Exception)
							{
								pro.isOP = false;
							}
							break;

						case "OnJoinMessage":

							pro.OnJoinMessage = EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText;
							pro.OnJoinMessage = pro.OnJoinMessage.Replace("&#39;",",");
							break;

						case "ProfileLevel":

							try
							{
								pro.profileLevel = int.Parse(EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText);
							}
							catch(System.Exception)
							{
								pro.profileLevel = 0;
							}
							break;

						case "ProfileName":

							pro.profileName = EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText;
							pro.profileName = pro.profileName.Replace("&#39;",",");
							break;

						case "kickLevel":

							pro.kickLevel = int.Parse( EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText );
							break;

					}

				}

				if (pro.profileName == "DefaultProfile")
					DefaultPro = pro;
				Profiles.Add(pro);
				
			}

			if (DefaultPro == null)
				DefaultPro = new profile();

		}

		public static void SaveToFile(int saveType)
		{


			System.Xml.XmlTextWriter writer;
			writer = new System.Xml.XmlTextWriter(@"Profiles.xml",System.Text.Encoding.GetEncoding("windows-1252"));

			writer.Formatting = System.Xml.Formatting.Indented;
			writer.WriteStartDocument();

			// this will normly happen when there is no xml file
			// and one needs creating
			if (saveType == 0)
			{
				writer.WriteStartElement("Profiles");

					writer.WriteStartElement("Profile");

						writer.WriteElementString("isOperator","false");
						writer.WriteElementString("OnJoinMessage","Join message for normle users");
						writer.WriteElementString("ProfileLevel","0");
						writer.WriteElementString("ProfileName","DefaultProfile");
						writer.WriteElementString("kickLevel","0");

					writer.WriteEndElement();

				writer.WriteEndElement();

				writer.WriteEndDocument();
				writer.Flush();
				writer.Close();
				writer = null;
				return;
			}

			writer.WriteStartElement("Profiles");

			System.Xml.XmlElement[] Account = new System.Xml.XmlElement[Profiles.Count];

			for (int iCount = 0; iCount < Profiles.Count; iCount++)
			{
				profile pro = (profile)Profiles[iCount];

				pro.OnJoinMessage = pro.OnJoinMessage.Replace("'","&#39;");
				pro.profileName = pro.profileName.Replace("'","&#39;");

				writer.WriteStartElement("Profile");

				writer.WriteElementString("isOperator",pro.isOP.ToString());
				writer.WriteElementString("OnJoinMessage",pro.OnJoinMessage);
				writer.WriteElementString("ProfileLevel",pro.profileLevel.ToString());
				writer.WriteElementString("ProfileName",pro.profileName);

				writer.WriteEndElement();
			}

			writer.WriteEndElement();


			writer.WriteEndDocument();
			writer.Flush();
			writer.Close();
			writer = null;
		

		}

		public static profile GetProfile(string profileName)
		{
			profile Profile;

			for (int i = 0; i < Profiles.Count; i++)
			{
				Profile = (profile)Profiles[i];
				if (Profile.profileName.ToLower() == profileName.ToLower())
					return Profile;
			}
			return null;
		}

		public static profile GetProfile(int position)
		{
			return (profile)Profiles[position];
		}

		public static int Size()
		{
			return Profiles.Count;
		}
	}

	public class usersAccounts
	{
		private static System.Collections.ArrayList accounts = new System.Collections.ArrayList();
		public usersAccounts()
		{
	
		}

		public static void LoadFromFile()
		{
			System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();

			try
			{
				xmldoc.Load(@"Accounts.xml");
			}
			catch(System.IO.FileNotFoundException)
			{
				SaveToFile(0);
				xmldoc.Load(@"Accounts.xml");
			}

			// Find out how many accounts we are dealing with
			int NumberOfAccounts = xmldoc.DocumentElement.ChildNodes.Count;

			// if the file exists but there are not accounts present in it. exit the function
			if (NumberOfAccounts < 1) return;

			// will hold each account so they can be examined
			System.Xml.XmlNode EachAccount;
			
			// Go through each account in the xml file
			for (int iCount = 0; iCount < NumberOfAccounts; iCount++)
			{
				// hold the data for each Account in EachAccount
				EachAccount = xmldoc.DocumentElement.ChildNodes.Item(iCount);

				// create an instance for each user account, other wise when we try to
				// add data to userAccounts it will error
				account acc = new account();

				// Go through each node with in each account. e.g. nick then pass etc.
				for (int iInnerNodeCount = 0; iInnerNodeCount < EachAccount.ChildNodes.Count; iInnerNodeCount++)
				{
					// find out which item we are currently dealing with and then store the info
					// into userSettings[] array.
					string temp = EachAccount.ChildNodes.Item(iInnerNodeCount).LocalName.ToLower();
					switch (EachAccount.ChildNodes.Item(iInnerNodeCount).LocalName)
					{
						case "Nick":
						
							// The inner text contains the name of the person
							acc.nick = EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText;
							break;

						case "Pass":

							acc.pass = EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText;
							break;

						case "ProfileName":

							profile pro = profiles.GetProfile( EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText  );
							if (pro == null)
								acc.Profile = new profile();
							else
								acc.Profile = pro;

							break;

						case "CreatedBy":

							acc.createdBy = EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText;
							break;

						case "DateCreated":

							acc.DateCreated = EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText;
							break;
					}

				}

				accounts.Add(acc);
				
			}

		}

		// 0 = the file does not exist yet and so needs to be created
		// 1 = file exist and we just need to update it
		public static void SaveToFile(int saveType)
		{/*
			System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
			System.Xml.XmlElement Nick,Pass,Profile,CreatedBy,DateCreated;
			string xmlData = "<Accounts Version=\"1.0\"> </Accounts>";
			xmldoc.Load(new System.IO.StringReader(xmlData));
*/



			System.Xml.XmlTextWriter writer;
			writer = new System.Xml.XmlTextWriter(@"Accounts.xml",System.Text.Encoding.GetEncoding("windows-1252"));

			writer.Formatting = System.Xml.Formatting.Indented;
			writer.WriteStartDocument();

			// this will normly happen when there is no xml file
			// and one needs creating
			if (saveType == 0)
			{

				writer.WriteStartElement("Accounts");
/*
				writer.WriteStartElement("Account");
				writer.WriteElementString("Nick","n");
				writer.WriteElementString("Pass","pg");
				writer.WriteElementString("ProfileName","pn");
				writer.WriteElementString("CreatedBy","c");
				writer.WriteElementString("DateCreated","d");
				writer.WriteEndElement();
*/

				writer.WriteEndElement();

				writer.WriteEndDocument();
				writer.Flush();
				writer.Close();
				return;
			}

			writer.WriteStartElement("Accounts");
			for (int iCount = 0; iCount < accounts.Count; iCount++)
			{
				account acc = (account)accounts[iCount];

				writer.WriteStartElement("Account");
				writer.WriteElementString("Nick",acc.nick);
				writer.WriteElementString("Pass",acc.pass);
				writer.WriteElementString("ProfileName",acc.Profile.profileName);
				writer.WriteElementString("CreatedBy",acc.createdBy);
				writer.WriteElementString("DateCreated",acc.DateCreated);
				writer.WriteEndElement();

			}

			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();
			writer.Close();


/*
			// The following code will take the all the accounts that are in
			// settings and write them to an xml file called Accounts.
			// NOTE. at the moment, it completly rewrite the old xml file.
			// this seems a bit inifishent.
			System.Xml.XmlElement[] Account = new System.Xml.XmlElement[accounts.Count];

			for (int iCount = 0; iCount < accounts.Count; iCount++)
			{
				Account[iCount] = xmldoc.CreateElement("","Account","");

				account acc = (account)accounts[iCount];
				Nick = xmldoc.CreateElement("","Nick","");
				Nick.InnerText = acc.nick;

				Pass = xmldoc.CreateElement("","Pass","");
				Pass.InnerText = acc.pass;

				Profile = xmldoc.CreateElement("","ProfileName","");
				Profile.InnerText = acc.Profile.profileName;

				CreatedBy = xmldoc.CreateElement("","CreatedBy","");
				CreatedBy.InnerText = acc.createdBy;

				DateCreated = xmldoc.CreateElement("","DateCreated","");
				DateCreated.InnerText = acc.DateCreated;

				Account[iCount].AppendChild(Nick);
				Account[iCount].AppendChild(Pass);
				Account[iCount].AppendChild(Profile);
				Account[iCount].AppendChild(CreatedBy);
				Account[iCount].AppendChild(DateCreated);

				xmldoc.DocumentElement.AppendChild(Account[iCount]);
			}

			xmldoc.Save(@"Accounts.xml");
			// add code for saving the accounts.
			
			// Also add code incase settings value that is passed is null.
			// if null is passed it means the file does not exist and we
			// want to create one but with no current accounts in it.
		*/
	
			
		}

		public static account GetAccount(string nick)
		{
			account Account;

			for (int i = 0; i < accounts.Count; i++)
			{
				Account = (account)accounts[i];
				if (Account.nick.ToLower() == nick.ToLower())
					return Account;
				
			}
			return null;
		}

		public static account GetAccount(int position)
		{
			return (account)accounts[position];
		}

		public static int Size()
		{
			return accounts.Count;
		}
	}

	public class account
	{
		public string nick;
		public string pass;
		public profile Profile;	
		public string createdBy;
		public string DateCreated;

		public account()
		{
			nick = string.Empty;
			pass = string.Empty;
			createdBy = string.Empty;
			DateCreated = string.Empty;
		}
	}

	public class profile
	{
		public int profileLevel;
		public bool isOP;
		public string profileName;
		public string OnJoinMessage;

		// 0 = not able to kick
		// 1 = kick any one below this profileLevel
		// 2 = kick any one below and equal to this profileLevel
		// 3 = kick any one inculding any one above this profileLevel;
		public int kickLevel;

		public profile()
		{
			profileLevel = 0;
			isOP = false;
			profileName = "DefaultProfile";
			OnJoinMessage = "Welcome";
			kickLevel = 0;
		}

	}

	#endregion
}

namespace GHub.Settings.MultHubs
{
	public class MultiHubs
	{
		private static System.Collections.ArrayList Hubs = new System.Collections.ArrayList();

		public static Hub Get(int position)
		{	
			return (Hub)Hubs[position];
		}

		public static Hub Get(string hubName)
		{
			Hub hub;
			for (int i = 0; i < Hubs.Count; i++)
			{
				hub = (Hub)Hubs[i];

				if (hub.HubName == hubName)
				{
					return (Hub)Hubs[i];
				}
			}
			return null;
		}

		public static void Delete(string HubName)
		{

			Hub hub;
			for (int i = 0; i < Hubs.Count; i++)
			{
				hub = (Hub)Hubs[i];

				if (hub.HubName == HubName)
				{
					Hubs.RemoveAt(i);
					return;
				}
			}
		}

		public static int Size()
		{
			return Hubs.Count;
		}

		public static void LoadHubs()
		{
			System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();

			try
			{
				xmldoc.Load(@"MultiHubs.xml");
			}
			catch(System.IO.FileNotFoundException)
			{
				SaveToFile(0);
				xmldoc.Load(@"MultiHubs.xml");
			}

			// Find out how many accounts we are dealing with
			int NumberOfAccounts = xmldoc.DocumentElement.ChildNodes.Count;

			// if the file exists but there are not accounts present in it. exit the function
			if (NumberOfAccounts < 1) return;

			// will hold each account so they can be examined
			System.Xml.XmlNode EachAccount;
			
			// Go through each account in the xml file
			for (int iCount = 0; iCount < NumberOfAccounts; iCount++)
			{
				// hold the data for each Account in EachAccount
				EachAccount = xmldoc.DocumentElement.ChildNodes.Item(iCount);

				// create an instance for each user account, other wise when we try to
				// add data to userAccounts it will error
				Hub hub = new Hub();

				// Go through each node with in each account. e.g. nick then pass etc.
				for (int iInnerNodeCount = 0; iInnerNodeCount < EachAccount.ChildNodes.Count; iInnerNodeCount++)
				{
					// find out which item we are currently dealing with and then store the info
					// into userSettings[] array.
					string temp = EachAccount.ChildNodes.Item(iInnerNodeCount).LocalName.ToLower();
					switch (EachAccount.ChildNodes.Item(iInnerNodeCount).LocalName)
					{
						case "Address":
						

							hub.address = EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText;
							break;

						case "Port":
						
							try
							{
								hub.port = int.Parse( EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText );
							}
							catch
							{
								hub.port = 411;
							}
							break;

						case "Pass":

							hub.pass = EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText;
							break;

						case "UserName":

							hub.userName = EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText;
							break;

						case "HubName":

							hub.HubName = EachAccount.ChildNodes.Item(iInnerNodeCount).InnerText;
							break;

					}

				}

				hub.address = hub.address.Replace("&#39;",",");
				hub.pass = hub.pass.Replace("&#39;",",");
				hub.userName = hub.userName.Replace("&#39;",",");
				hub.HubName = hub.HubName.Replace("&#39;",",");

				Hubs.Add(hub);
				
			}
		}

		public static void SaveToFile(int saveType)
		{


			System.Xml.XmlTextWriter writer;
			writer = new System.Xml.XmlTextWriter(@"MultiHubs.xml",System.Text.Encoding.GetEncoding("windows-1252"));

			writer.Formatting = System.Xml.Formatting.Indented;
			writer.WriteStartDocument();

			// this will normly happen when there is no xml file
			// and one needs creating
			if (saveType == 0)
			{
				writer.WriteStartElement("MultiHubs");

				writer.WriteStartElement("Hub");

				writer.WriteElementString("Address","uri goes hear");
				writer.WriteElementString("Port","411");
				writer.WriteElementString("Pass","temp");
				writer.WriteElementString("UserName","username hear");
				writer.WriteElementString("HubName","a hub name");

				writer.WriteEndElement();

				writer.WriteEndElement();

				writer.WriteEndDocument();
				writer.Flush();
				writer.Close();
				writer = null;
				return;
			}

			writer.WriteStartElement("MultiHubs");

			for (int iCount = 0; iCount < Hubs.Count; iCount++)
			{
				Hub hub = (Hub)Hubs[iCount];

				hub.address = hub.address.Replace("'","&#39;");
				hub.pass = hub.pass.Replace("'","&#39;");
				hub.userName = hub.userName.Replace("'","&#39;");
				hub.HubName = hub.HubName.Replace("'","&#39;");

				writer.WriteStartElement("Hub");

				writer.WriteElementString("Address",hub.address);
				writer.WriteElementString("Port",hub.port.ToString());
				writer.WriteElementString("Pass",hub.pass);
				writer.WriteElementString("UserName",hub.userName);
				writer.WriteElementString("HubName",hub.HubName);


				writer.WriteEndElement();
			}

			writer.WriteEndElement();


			writer.WriteEndDocument();
			writer.Flush();
			writer.Close();
			writer = null;
		}
	}

	public class Hub
	{
		public string address;
		public int port;
		public string pass;
		public string userName;
		public string HubName;
		public int ConnectionState; // 0 = not connected, 1 = connecting, 2 = connected.
	}
}

namespace GHub.Settings.Hub
{
	public class hubSettings
	{
		public static string VersionNo = "beta 0.044";
		public static string HubName = "Welcome to GHub - " + VersionNo;
		public static int NickLength = 20;
		public static int MaxMessageLength = 3000; // the maximum length a message can be that is sent to the server before a "|" is reached.
		public static int MaxMainChatLength = 1000;
		public static string MultiHubUserName = "Gizmo";
		public static string MultiHubPass = "temp";
		public static string BotName = "GHub";

		public hubSettings()
		{
		}

		public static void Load()
		{
		}

		public static void Save(int type)
		{

		}
	}
}

namespace GHub.Settings
{
	public class Synchronization
	{
		public static System.Threading.Mutex serverArray = new System.Threading.Mutex();
		public static System.Threading.Mutex clientArray = new System.Threading.Mutex();

		// used when dealing with things like altering the list of myINFO strings
		public static System.Threading.Mutex userInfo = new System.Threading.Mutex();

		public Synchronization()
		{
		}
	}
}