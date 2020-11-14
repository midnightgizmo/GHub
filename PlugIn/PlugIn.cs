

using System;
using GHub.EventMessages;
//using GHub.Data;

namespace GHub.plugin
{
	/// <summary>
	/// Summary description for plugin.
	/// </summary>
	public class plugin
	{
		public plugin()
		{

		}

		public static System.Collections.ArrayList PluginLoader()
		{
			//create an array for my plugins
			System.Collections.ArrayList myPlugins = new System.Collections.ArrayList();

			string plugInDirectory = GetAppDir();
			string[] SubDirectorys = System.IO.Directory.GetDirectories(plugInDirectory,"PlugIns");

			//bool PlugInDirectoryExist = false;
			if (SubDirectorys.Length == 0)
			{
				System.IO.Directory.CreateDirectory(GetAppDir() + "\\PlugIns");
			}/*
			else
			{
				for (int i = 0; i < SubDirectorys.Length; i++)
				{
					if (SubDirectorys[i].ToLower() == "plugins")
					{
						PlugInDirectoryExist = true;
						break;
					}
				}
				if (!PlugInDirectoryExist)
					System.IO.Directory.CreateDirectory(GetAppDir() + "\\PlugIns");
			}*/

			plugInDirectory += "\\PlugIns";

			//load all .dll-files in this directory as assemblies
			foreach(string file in System.IO.Directory.GetFiles(plugInDirectory + "\\", "*.dll"))
			{
			//	System.Reflection.Assembly a = System.Reflection.Assembly.LoadFile(
			//		System.IO.Directory.GetCurrentDirectory() + "\\" + file);

				System.Reflection.Assembly a = System.Reflection.Assembly.LoadFile(file);
				try
				{
					//iterate over all types in the assembly
					foreach(Type t1 in a.GetTypes()) 
					{
						//retrieve all interfaces of the current type
						foreach(Type t2 in t1.GetInterfaces())
						{
							//if the interface matches exactly the plugin-interface...
							if(t2 == typeof(IPlugin))
							{
								try 
								{
									//...then create an instance of the class
									object plugObject = Activator.CreateInstance(t1);
									
									aPlugIn plug = new aPlugIn();
									plug.plugInName = a.FullName.Split(',')[0];
									plug.PlugIn = (IPlugin)plugObject;
									myPlugins.Add(plug);
								}
								catch(Exception)
								{
								}
							}
						}
					}
				}
				catch
				{
					continue;
				}
			}

			return myPlugins;
		}

		public static string GetAppDir()
		{
				string appDirCache = null;
			// return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
			if (appDirCache == null)
			{
				appDirCache = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
			}
			return appDirCache;
		}
	}


	public class aPlugIn
	{
		public string plugInName;
		public IPlugin PlugIn;

		public aPlugIn()
		{
		}
	}
	// if any of the functions return true it means they have handled
	// the message and the core does not need to handle them.
	// Be carfull if u do this thow because the core is responsable
	// the making the hub opperate properly. Make sure you know
	// what you are doing.
	public interface IPlugin
	{
		System.Windows.Forms.Panel PluginLoaded();
		void ConnectionMade(GHub.Data.ListOfServers ser, GHub.Data.ListOfLocalUsers usr);
		bool ValidateNick(Message msg);
		bool Key(Message msg);
		bool myInfo(myInfo msg);
		bool AlmostLoggedIn(Message msg);
		bool Version(Message msg);
		bool GetNickList(Message msg);
		bool UserLeft(Message msg);
		bool MainChatMessage(mainChat msg);
		bool PrivateMessage(messageToUser msg);
		bool Search(messageToUser msg);

	}
	
}
