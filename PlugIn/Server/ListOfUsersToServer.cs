using System;

namespace GHub.client.server
{

	public class ListOfUsersToServer
	{
		private System.Collections.ArrayList users;
		private serverUserInfo info;

		public ListOfUsersToServer()
		{
			users = new System.Collections.ArrayList();
		}

		public void Add(serverUserInfo user)
		{
			users.Add(user);
		}

		public void Delete(string nick)
		{
			for (int i = 0; i < users.Count; i++)
			{
				info = (serverUserInfo)users[i];
				if (info.nick.ToString() == nick.ToLower())
				{
					users.RemoveAt(i);
					break;
				}
			}
		}

		public void Delete(int position)
		{
			users.RemoveAt(position);
		}

		public serverUserInfo Get(int position)
		{
			try
			{
				return (serverUserInfo)users[position];
			}
			catch
			{
				string temp = "Ddf!";
				return null;
			}
		}

		public serverUserInfo Get(string nick)
		{
			for (int i = 0; i < users.Count; i++)
			{
				info = (serverUserInfo)users[i];
				if (info.nick.ToString() == nick.ToLower())
				{
					return info;
				}
			}
			return null;
		}

		public int Size()
		{
			return users.Count;
		}

	}
}
