using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GHub;

namespace GUI
{
	/// <summary>
	/// Summary description for MultiHubs.
	/// </summary>
	public class MultiHubs : System.Windows.Forms.Panel
	{
		private System.Windows.Forms.ListBox HubsList;
		private GHub.Settings.MultHubs.Hub[] hubs;
		private GHub.Core server;
		private System.Windows.Forms.ContextMenu DeleteMenu;
		private System.Windows.Forms.MenuItem[] menuDelete;

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtPort;
		private System.Windows.Forms.Button cmdConnect;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtPass;
		private System.Windows.Forms.TextBox txtAddress;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Button cmdDisconnect;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button cmdApply;
		private System.Windows.Forms.TextBox txtUserName;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MultiHubs(GHub.Core core)
		{
			server = core;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			cmdDisconnect.Enabled = false;	
			
			hubs = new GHub.Settings.MultHubs.Hub[GHub.Settings.MultHubs.MultiHubs.Size()];
			for (int i = 0; i < hubs.Length; i++)
			{
				hubs[i] = GHub.Settings.MultHubs.MultiHubs.Get(i);
				HubsList.Items.Add(hubs[i].HubName);
			}

			menuDelete = new MenuItem[1];
			menuDelete[0] = new MenuItem("Delete",new System.EventHandler(menuDelete_Press));

			DeleteMenu = new ContextMenu(menuDelete);
			HubsList.ContextMenu = DeleteMenu;


			server.AddServerConnectionMade(new MessageReceivedEventHandler(server_ConnectionToServerMade));
			server.AddServerLost(new MessageReceivedEventHandler(server_ConnectionLostToServer));
			server.AddConnectingToServer(new MessageReceivedEventHandler(server_ConnectingToServer));
		
		}

		// when a connection between this server and another server has been made for 
		// multihubbing.
		private void server_ConnectionToServerMade(object obj, MessageReceivedEventArgs e)
		{
			string hubName = (string)e.msg;

			foreach (GHub.Settings.MultHubs.Hub hub in hubs)
			{
				if (hub.HubName == hubName)
				{
					hub.ConnectionState = 2;

					if (txtName.Text == hubName)
					{
						cmdConnect.Enabled = false;
						cmdDisconnect.Enabled = true;
					}
					break;
				}
			}
		}

		private void server_ConnectionLostToServer(object obj, MessageReceivedEventArgs e)
		{
			string hubName = (string)e.msg;

			foreach (GHub.Settings.MultHubs.Hub hub in hubs)
			{
				if (hub.HubName == hubName)
				{
					hub.ConnectionState = 0;

					if (txtName.Text == hubName)
					{
						cmdConnect.Enabled = true;
						cmdDisconnect.Enabled = false;
					}
					break;
				}
			}
		}

		private void server_ConnectingToServer(object obj, MessageReceivedEventArgs e)
		{
			string hubName = (string)e.msg;

			foreach (GHub.Settings.MultHubs.Hub hub in hubs)
			{
				if (hub.HubName == hubName)
				{
					hub.ConnectionState = 1;

					if (txtName.Text == hubName)
					{
						cmdConnect.Enabled = false;
						cmdDisconnect.Enabled = false;
					}
					break;
				}
			}
		}

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
			this.HubsList = new System.Windows.Forms.ListBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label3 = new System.Windows.Forms.Label();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.cmdConnect = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.txtPass = new System.Windows.Forms.TextBox();
			this.txtAddress = new System.Windows.Forms.TextBox();
			this.txtName = new System.Windows.Forms.TextBox();
			this.cmdDisconnect = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.cmdApply = new System.Windows.Forms.Button();
			this.txtUserName = new System.Windows.Forms.TextBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.SuspendLayout();
			// 
			// HubsList
			// 
			this.HubsList.Location = new System.Drawing.Point(8, 8);
			this.HubsList.Name = "HubsList";
			this.HubsList.Size = new System.Drawing.Size(120, 381);
			this.HubsList.TabIndex = 0;
			this.HubsList.SelectedIndexChanged += new System.EventHandler(this.HubsList_SelectedIndexChanged);
			// 
			// tabControl1
			// 
			this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new System.Drawing.Point(136, 8);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(624, 384);
			this.tabControl1.TabIndex = 14;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.txtPort);
			this.tabPage1.Controls.Add(this.cmdConnect);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.txtPass);
			this.tabPage1.Controls.Add(this.txtAddress);
			this.tabPage1.Controls.Add(this.txtName);
			this.tabPage1.Controls.Add(this.cmdDisconnect);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.cmdApply);
			this.tabPage1.Controls.Add(this.txtUserName);
			this.tabPage1.Location = new System.Drawing.Point(4, 25);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(616, 355);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Connection";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(176, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 16);
			this.label3.TabIndex = 11;
			this.label3.Text = "Port";
			// 
			// txtPort
			// 
			this.txtPort.Location = new System.Drawing.Point(176, 104);
			this.txtPort.Name = "txtPort";
			this.txtPort.TabIndex = 3;
			this.txtPort.Text = "";
			// 
			// cmdConnect
			// 
			this.cmdConnect.Location = new System.Drawing.Point(48, 288);
			this.cmdConnect.Name = "cmdConnect";
			this.cmdConnect.TabIndex = 6;
			this.cmdConnect.Text = "Connect";
			this.cmdConnect.Click += new System.EventHandler(this.cmdConnect_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(40, 144);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 16);
			this.label4.TabIndex = 12;
			this.label4.Text = "User Name";
			// 
			// txtPass
			// 
			this.txtPass.Location = new System.Drawing.Point(176, 160);
			this.txtPass.Name = "txtPass";
			this.txtPass.TabIndex = 4;
			this.txtPass.Text = "";
			// 
			// txtAddress
			// 
			this.txtAddress.Location = new System.Drawing.Point(40, 104);
			this.txtAddress.Name = "txtAddress";
			this.txtAddress.TabIndex = 2;
			this.txtAddress.Text = "";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(40, 48);
			this.txtName.Name = "txtName";
			this.txtName.TabIndex = 1;
			this.txtName.Text = "";
			// 
			// cmdDisconnect
			// 
			this.cmdDisconnect.Location = new System.Drawing.Point(144, 288);
			this.cmdDisconnect.Name = "cmdDisconnect";
			this.cmdDisconnect.TabIndex = 7;
			this.cmdDisconnect.Text = "Disconnect";
			this.cmdDisconnect.Click += new System.EventHandler(this.cmdDisconnect_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(40, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 16);
			this.label2.TabIndex = 10;
			this.label2.Text = "Address";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(40, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(416, 23);
			this.label1.TabIndex = 8;
			this.label1.Text = "Hub Name (Must be unique to any other hub name that you are connecting to)";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(176, 144);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 16);
			this.label5.TabIndex = 13;
			this.label5.Text = "Password";
			// 
			// cmdApply
			// 
			this.cmdApply.Location = new System.Drawing.Point(152, 232);
			this.cmdApply.Name = "cmdApply";
			this.cmdApply.Size = new System.Drawing.Size(72, 32);
			this.cmdApply.TabIndex = 9;
			this.cmdApply.Text = "Apply Changes";
			// 
			// txtUserName
			// 
			this.txtUserName.Location = new System.Drawing.Point(40, 160);
			this.txtUserName.Name = "txtUserName";
			this.txtUserName.TabIndex = 5;
			this.txtUserName.Text = "";
			// 
			// tabPage3
			// 
			this.tabPage3.Location = new System.Drawing.Point(4, 25);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(616, 355);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Add";
			// 
			// MultiHubs
			// 
			//this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(864, 446);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.HubsList);
			this.Name = "MultiHubs";
			this.Text = "MultiHubs";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void cmdConnect_Click(object sender, System.EventArgs e)
		{
			if (!server.Connected)
				return;

			if (txtName.Text == string.Empty)
				return;
			if (txtAddress.Text == string.Empty)
				return;
			if (txtPort.Text == string.Empty)
				return;
			if (txtPass.Text == string.Empty)
				return;
			if (txtUserName.Text == string.Empty)
				return;

			switch (server.ConnectToServer(txtAddress.Text,int.Parse(txtPort.Text),txtName.Text))
			{
					// connection was sucsessfull.
				case 1:
					cmdConnect.Enabled = false;
					cmdDisconnect.Enabled = false;

					GHub.Settings.MultHubs.MultiHubs.Get(txtName.Text).ConnectionState = 1;
					break;

					// connection failed.
				default:
					break;
			}
						
		}

		private void HubsList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (HubsList.SelectedIndex == -1)
				return;

			string selectedHub = (string)HubsList.Items[HubsList.SelectedIndex];

			foreach (GHub.Settings.MultHubs.Hub hub in hubs)
			{
				if (hub.HubName == selectedHub)
				{
					txtName.Text = hub.HubName;
					txtAddress.Text = hub.address;
					txtPort.Text = hub.port.ToString();
					txtPass.Text = hub.pass;
					txtUserName.Text = hub.userName;

					switch(hub.ConnectionState)
					{
						case 0:

							cmdConnect.Enabled = true;
							cmdDisconnect.Enabled = false;
							break;

						case 1:

							cmdConnect.Enabled = false ;
							cmdDisconnect.Enabled = false;
							break;

						case 2:

							cmdConnect.Enabled = false;
							cmdDisconnect.Enabled = true;
							break;
					}

					break;
				}
			}


		}

		private void menuDelete_Press(object sender, EventArgs e)
		{
			int SelectedHub = HubsList.SelectedIndex;
			string SelectedHubName;

			switch(SelectedHub)
			{
				case -1:

					return;

				default:
					SelectedHubName = (string)HubsList.Items[SelectedHub];
					foreach (GHub.Settings.MultHubs.Hub hub in hubs)
					{
						if (hub.HubName == SelectedHubName)
						{
							if (hub.ConnectionState > 0)
							{
								System.Windows.Forms.MessageBox.Show("Can't delete while connected to server");
								return;
							}
							switch ( System.Windows.Forms.MessageBox.Show("Remove " + hub.HubName + " from the list ?","",System.Windows.Forms.MessageBoxButtons.YesNo) )
							{
								case System.Windows.Forms.DialogResult.Yes:

									HubsList.Items.RemoveAt(SelectedHub);
									GHub.Settings.MultHubs.MultiHubs.Delete(hub.HubName);
									break;

								default:

									break;
							}

							return;
						}
					}

					break;
			}
		}

		private void cmdDisconnect_Click(object sender, System.EventArgs e)
		{
			try
			{
				server.DisconnectFromServer(txtName.Text);
				cmdConnect.Enabled = true;
				cmdDisconnect.Enabled = false;
				GHub.Settings.MultHubs.MultiHubs.Get(txtName.Text).ConnectionState = 0;
			}
			catch(System.Exception f)
			{
				string temp = f.Message;
			}
		}

	}
}
