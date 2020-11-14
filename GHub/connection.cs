using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GHub;

namespace GUI
{
	/// <summary>
	/// Summary description for connection.
	/// </summary>
	public class connection : System.Windows.Forms.Panel
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button cmdStart;
		private System.Windows.Forms.Button cmdStop;
		private System.Windows.Forms.Label lblPort;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TextBox txtPort;
		private GHub.Core server;

		public connection(GHub.Core core)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			server = core;

			cmdStop.Enabled = false;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(connection));
			this.cmdStart = new System.Windows.Forms.Button();
			this.cmdStop = new System.Windows.Forms.Button();
			this.lblPort = new System.Windows.Forms.Label();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// cmdStart
			// 
			this.cmdStart.BackColor = System.Drawing.Color.WhiteSmoke;
			this.cmdStart.Location = new System.Drawing.Point(64, 64);
			this.cmdStart.Name = "cmdStart";
			this.cmdStart.TabIndex = 0;
			this.cmdStart.Text = "Start Hub";
			this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
			// 
			// cmdStop
			// 
			this.cmdStop.BackColor = System.Drawing.Color.WhiteSmoke;
			this.cmdStop.Location = new System.Drawing.Point(64, 96);
			this.cmdStop.Name = "cmdStop";
			this.cmdStop.TabIndex = 1;
			this.cmdStop.Text = "Stop Hub";
			this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
			// 
			// lblPort
			// 
			this.lblPort.BackColor = System.Drawing.Color.Transparent;
			this.lblPort.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblPort.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.lblPort.Location = new System.Drawing.Point(24, 24);
			this.lblPort.Name = "lblPort";
			this.lblPort.Size = new System.Drawing.Size(48, 24);
			this.lblPort.TabIndex = 2;
			this.lblPort.Text = "Port";
			// 
			// txtPort
			// 
			this.txtPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtPort.Location = new System.Drawing.Point(64, 24);
			this.txtPort.Name = "txtPort";
			this.txtPort.TabIndex = 0;
			this.txtPort.Text = "";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(848, 472);
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			// 
			// connection
			// 
			//this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ClientSize = new System.Drawing.Size(872, 486);
			this.Controls.Add(this.txtPort);
			this.Controls.Add(this.lblPort);
			this.Controls.Add(this.cmdStop);
			this.Controls.Add(this.cmdStart);
			this.Controls.Add(this.pictureBox1);
			this.Name = "connection";
			this.Text = "connection";
			this.ResumeLayout(false);

		}
		#endregion

		private void cmdStart_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (server.Connect(int.Parse(txtPort.Text)) == -1)
				{
					System.Windows.Forms.MessageBox.Show("Unable to connect, may be the port is allready in use");
					return;
				}
				cmdStart.Enabled = false;
				cmdStop.Enabled = true;
			}
			catch
			{
				System.Windows.Forms.MessageBox.Show("Unrecognised port");
			}
		
		}

		private void cmdStop_Click(object sender, System.EventArgs e)
		{
			cmdStop.Enabled = false;
			server.Disconnect();
			cmdStart.Enabled = true;
		}
	}
}
