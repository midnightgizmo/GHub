using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GHub;

namespace GUI
{
	/// <summary>
	/// Summary description for plugin.
	/// </summary>
	public class plugin : System.Windows.Forms.Panel
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ListBox LoadedPlugIns;
		private GHub.Core server;
		private System.Windows.Forms.Panel guiPanel;
		private System.Collections.ArrayList PlugInList;

		public plugin(GHub.Core core)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			server = core;

			PlugInList = new ArrayList();

            // old way of dealing with the GUI side of plugins. . no longer needed
			//server.AddPlugInLoaded(new MessageReceivedEventHandler(server_PlugInLoaded));

            /*			string[] plugings = server.GetPlugInNames();

			if (plugings != null)
				LoadedPlugIns.Items.AddRange(plugings);*/
		}

        // Adds to plugin to a list so its GUI conponet can
        // be called upon.
        public void AddPlugin(MessageReceivedEventArgs e)
        {
            pluginInfo plug = new pluginInfo();

            // if the plug in does not have a graphica interface to it.
            if (e.msg == null)
            {
                LoadedPlugIns.Items.Add(e.secondMsg);

                plug.name = (string)e.secondMsg;
                plug.GUI = null;
                PlugInList.Add(plug);
                return;
            }


            // if we get this far, then there is a GUI to go with this plug in.
            //LoadedPlugIns.Items.Add(e.secondMsg);

            plug.name = (string)e.secondMsg;
            plug.GUI = (System.Windows.Forms.Panel)e.msg;
            PlugInList.Add(plug);
        }

        // when the user clicks on one of the plugins from the
        // tree menu on the left this function is called.
        // This function will Display the GUI for the current
        // plugin that was clicked on.
        public void PluginChanged(string PluginName)
        {
            foreach (pluginInfo info in PlugInList)
            {
                if (PluginName == info.name)
                {
                    guiPanel.Controls.Clear();

                    // if the current plugin does not have a GUI to it we don't need
                    // to display anything so just return;
                    if (info.GUI == null)
                        return;

                    guiPanel.Controls.Add(info.GUI);
                    info.GUI.Size = new System.Drawing.Size(guiPanel.Size.Width - 2, guiPanel.Size.Height - 2);
                    //HubPage.BackColor = System.Drawing.Color.Red;
                    info.GUI.Show();

                    return;
                }
            }
        }

        /*
         // old way of dealing with the GUI side of plugins. . no longer needed
		private void server_PlugInLoaded(object obj, MessageReceivedEventArgs e)
		{
			pluginInfo plug = new pluginInfo();

			// if the plug in does not have a graphica interface to it.
			if (e.msg == null)
			{
				LoadedPlugIns.Items.Add(e.secondMsg);
				
				plug.name = (string)e.secondMsg;
				plug.GUI = null;
				PlugInList.Add(plug);
				return;
			}

			// the Graphical interface must be a panel, if not we will ignore it.
            //if (e.msg.GetType() != typeof(System.Windows.Forms.Panel))
            //{
            //    LoadedPlugIns.Items.Add(e.secondMsg);
				
            //    plug.name = (string)e.secondMsg;
            //    plug.GUI = null;
            //    PlugInList.Add(plug);
            //    return;
            //}

			// if we get this far, then there is a GUI to go with this plug in.
			LoadedPlugIns.Items.Add(e.secondMsg);
				
			plug.name = (string)e.secondMsg;
			plug.GUI = (System.Windows.Forms.Panel)e.msg;
			PlugInList.Add(plug);

		}
        */
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            // old way of dealing with the GUI side of plugins. . no longer needed
			//this.LoadedPlugIns = new System.Windows.Forms.ListBox();
			this.guiPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();

            // old way of dealing with the GUI side of plugins. . no longer needed
			// 
			// LoadedPlugIns
			// 
			//this.LoadedPlugIns.Location = new System.Drawing.Point(8, 8);
			//this.LoadedPlugIns.Name = "LoadedPlugIns";
			//this.LoadedPlugIns.Size = new System.Drawing.Size(100, 264);
			//this.LoadedPlugIns.TabIndex = 0;
			//this.LoadedPlugIns.SelectedIndexChanged += new System.EventHandler(this.LoadedPlugIns_SelectedIndexChanged);
			// 
			// guiPanel
			// 
			this.guiPanel.Location = new System.Drawing.Point(8, 8);
			this.guiPanel.Name = "guiPanel";
			this.guiPanel.Size = new System.Drawing.Size(416, 264);
			this.guiPanel.TabIndex = 1;
			// 
			// plugin
			// 
			//this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(560, 286);
			this.Controls.Add(this.guiPanel);
			this.Controls.Add(this.LoadedPlugIns);
			this.Name = "plugin";
			this.Text = "plugin";
			this.Resize += new System.EventHandler(this.plugin_Resize);
			this.ResumeLayout(false);

		}
		#endregion

		private void plugin_Resize(object sender, System.EventArgs e)
		{
            // old way of dealing with the GUI side of plugins. . no longer needed
            //
			//LoadedPlugIns.Height = this.Height - 10;
			//guiPanel.Width = this.Width - LoadedPlugIns.Width - 10;

            guiPanel.Width = this.Width - 10;
			guiPanel.Height = this.Height - 10;
		}
        
        /*
        // old way of dealing with the GUI side of plugins. . no longer needed
                private void LoadedPlugIns_SelectedIndexChanged(object sender, System.EventArgs e)
                {
                    foreach (pluginInfo info in PlugInList)
                    {
                        if (LoadedPlugIns.Text == info.name)
                        {
                            guiPanel.Controls.Clear();

                            // if the current plugin does not have a GUI to it we don't need
                            // to display anything so just return;
                            if (info.GUI == null)
                                return;

                            guiPanel.Controls.Add(info.GUI);
                            info.GUI.Size = new System.Drawing.Size(guiPanel.Size.Width - 2,guiPanel.Size.Height - 2);
                            //HubPage.BackColor = System.Drawing.Color.Red;
                            info.GUI.Show();

                            return;
                        }
                    }
                }*/

    }

	





		

	struct pluginInfo
	{
		public string name;
		public System.Windows.Forms.Panel GUI;
	}
}
