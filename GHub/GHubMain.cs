using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using GHub;

namespace GUI
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class GHubMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TreeView Menu;
		private System.Windows.Forms.Panel mainPanel;
		private System.ComponentModel.IContainer components;

		private GUI.connection frmConnection;
		private GUI.plugin frmPlugIn;
		private GUI.HubSettings frmHubSettings;
		private GUI.MultiHubs frmMultiHubs;

		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private GHub.Core server;

		public GHubMain()
		{

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			server = new GHub.Core();
			frmPlugIn = new plugin(server);

            // add event for when the plug ins are loaded.
            // with in this event we will look to see what plugins are loaded and for
            // each one that is loaded it will be added to the Menu on the left hand side
            // of the GUI under the PlugIns section.
            server.AddPlugInLoaded(new MessageReceivedEventHandler(server_PlugInLoaded));

			server.LoadPlugIns();
			server.LoadSettings();
			//server.Connect(411);
			frmConnection = new connection(server);
			
			frmHubSettings = new HubSettings();
			frmMultiHubs = new MultiHubs(server);


			this.MinimumSize = new Size(800,500);

			this.Text = GHub.Settings.Hub.hubSettings.HubName;



            
		}

        private void server_PlugInLoaded(object obj, MessageReceivedEventArgs e)
        {
            Menu.Nodes["PlugIns"].Nodes.Add((string)e.secondMsg);
            frmPlugIn.AddPlugin(e);
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Connection");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Plug-Ins");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Settings");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Multi Hubs");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GHubMain));
            this.Menu = new System.Windows.Forms.TreeView();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // Menu
            // 
            this.Menu.Location = new System.Drawing.Point(0, 16);
            this.Menu.Name = "Menu";
            treeNode1.Name = "";
            treeNode1.Text = "Connection";
            treeNode2.Name = "PlugIns";
            treeNode2.Text = "Plug-Ins";
            treeNode3.Name = "";
            treeNode3.Text = "Settings";
            treeNode4.Name = "";
            treeNode4.Text = "Multi Hubs";
            this.Menu.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            this.Menu.Scrollable = false;
            this.Menu.Size = new System.Drawing.Size(121, 416);
            this.Menu.TabIndex = 0;
            this.Menu.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.Menu_AfterSelect);
            // 
            // mainPanel
            // 
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mainPanel.Location = new System.Drawing.Point(128, 16);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(688, 416);
            this.mainPanel.TabIndex = 1;
            this.mainPanel.Resize += new System.EventHandler(this.mainPanel_Resize);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "GHub";
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // GHubMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(832, 446);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.Menu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GHubMain";
            this.Text = "GHub";
            this.Resize += new System.EventHandler(this.GHubMain_Resize);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.GHubMain_Closing);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new GHubMain());
		}

		private void GHubMain_Resize(object sender, System.EventArgs e)
		{
			if (FormWindowState.Minimized == WindowState)
			{
				Hide();
				notifyIcon1.Visible = true;
				return;
			}

			Menu.Height = this.Height - 80;
			mainPanel.Height = this.Height - 80;
			mainPanel.Width = this.Width - 140;
			
		}

		private void Menu_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			mainPanel.Controls.Clear();
			switch (Menu.SelectedNode.Text)
			{
				case "Connection":

					mainPanel.Controls.Add(frmConnection);
					frmConnection.Size = new System.Drawing.Size(mainPanel.Size.Width - 2,mainPanel.Size.Height - 2);
					//HubPage.BackColor = System.Drawing.Color.Red;
					frmConnection.Show();
					break;

				case "Plug-Ins":
                    /*
					mainPanel.Controls.Add(frmPlugIn);
					frmPlugIn.Size = new System.Drawing.Size(mainPanel.Size.Width - 2,mainPanel.Size.Height - 2);
					//HubPage.BackColor = System.Drawing.Color.Red;
					frmPlugIn.Show();
                     */
					break;

				case "Settings":

					mainPanel.Controls.Add(frmHubSettings);
					frmHubSettings.Size = new System.Drawing.Size(mainPanel.Size.Width - 2,mainPanel.Size.Height - 2);
					//HubPage.BackColor = System.Drawing.Color.Red;
					frmHubSettings.Show();
					break;

				case "Multi Hubs":

					mainPanel.Controls.Add(frmMultiHubs);
					frmMultiHubs.Size = new System.Drawing.Size(mainPanel.Size.Width - 2,mainPanel.Size.Height - 2);
					//HubPage.BackColor = System.Drawing.Color.Red;
					frmMultiHubs.Show();
					break;

                // if we got this far one of the plugins have been clicked on.
                default:

                    frmPlugIn.PluginChanged(Menu.SelectedNode.Text);
                    mainPanel.Controls.Add(frmPlugIn);
                    frmPlugIn.Size = new System.Drawing.Size(mainPanel.Size.Width - 2, mainPanel.Size.Height - 2);
                    //HubPage.BackColor = System.Drawing.Color.Red;
                    frmPlugIn.Show();
                    break;

			}
		}

		private void mainPanel_Resize(object sender, System.EventArgs e)
		{
			if (mainPanel.Controls.Count > 0)
			{
				System.Windows.Forms.Control currentWindow =  (System.Windows.Forms.Control)mainPanel.Controls[0];
				currentWindow.Width = mainPanel.Width;
				currentWindow.Height = mainPanel.Height;
			}
		}

		private void GHubMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (server.Connected)
			{
				switch(System.Windows.Forms.MessageBox.Show("Hub is still running. Are you sure you wish to exit?","",System.Windows.Forms.MessageBoxButtons.YesNo))
				{
					case System.Windows.Forms.DialogResult.Yes:

						server.Disconnect();
						break;

					case System.Windows.Forms.DialogResult.No:

						e.Cancel = true;
						break;
				}
				
			}
		}

		private void notifyIcon1_DoubleClick(object sender, System.EventArgs e)
		{
			Show();
			WindowState = FormWindowState.Normal;
			notifyIcon1.Visible = false;
		}



	}
}
