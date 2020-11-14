using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace GUI
{
	/// <summary>
	/// Summary description for HubSettings.
	/// </summary>
	public class HubSettings : System.Windows.Forms.Panel
	{
		private System.Windows.Forms.Label lblMaxMessageLength;
		private System.Windows.Forms.Label lblMessageLengthValue;
		private System.Windows.Forms.TrackBar MessageLengthSlider;
		private System.Windows.Forms.Button cmdApply;
		private System.Windows.Forms.Label lblChatMessageLength;
		private System.Windows.Forms.TrackBar ChatLengthSlider;
		private System.Windows.Forms.Label lblChatLengthValue;
		private System.Windows.Forms.Label lblNickLength;
		private numberTextBox txtNickLength;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public HubSettings()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			lblMessageLengthValue.Text = MessageLengthSlider.Value.ToString();
			lblChatLengthValue.Text = ChatLengthSlider.Value.ToString();
			txtNickLength.Text = "20";
			cmdApply.Enabled = false;
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
			this.lblMaxMessageLength = new System.Windows.Forms.Label();
			this.lblMessageLengthValue = new System.Windows.Forms.Label();
			this.MessageLengthSlider = new System.Windows.Forms.TrackBar();
			this.cmdApply = new System.Windows.Forms.Button();
			this.lblChatMessageLength = new System.Windows.Forms.Label();
			this.ChatLengthSlider = new System.Windows.Forms.TrackBar();
			this.lblChatLengthValue = new System.Windows.Forms.Label();
			this.lblNickLength = new System.Windows.Forms.Label();
			this.txtNickLength = new numberTextBox();
			((System.ComponentModel.ISupportInitialize)(this.MessageLengthSlider)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ChatLengthSlider)).BeginInit();
			this.SuspendLayout();
			// 
			// lblMaxMessageLength
			// 
			this.lblMaxMessageLength.Location = new System.Drawing.Point(24, 32);
			this.lblMaxMessageLength.Name = "lblMaxMessageLength";
			this.lblMaxMessageLength.Size = new System.Drawing.Size(112, 23);
			this.lblMaxMessageLength.TabIndex = 1;
			this.lblMaxMessageLength.Text = "Max Message Length";
			// 
			// lblMessageLengthValue
			// 
			this.lblMessageLengthValue.Location = new System.Drawing.Point(328, 64);
			this.lblMessageLengthValue.Name = "lblMessageLengthValue";
			this.lblMessageLengthValue.Size = new System.Drawing.Size(72, 16);
			this.lblMessageLengthValue.TabIndex = 4;
			// 
			// MessageLengthSlider
			// 
			this.MessageLengthSlider.Location = new System.Drawing.Point(144, 24);
			this.MessageLengthSlider.Maximum = 8000;
			this.MessageLengthSlider.Minimum = 3000;
			this.MessageLengthSlider.Name = "MessageLengthSlider";
			this.MessageLengthSlider.Size = new System.Drawing.Size(456, 45);
			this.MessageLengthSlider.TabIndex = 5;
			this.MessageLengthSlider.TickFrequency = 150;
			this.MessageLengthSlider.Value = 3000;
			this.MessageLengthSlider.ValueChanged += new System.EventHandler(this.MessageLengthSlider_ValueChanged);
			// 
			// cmdApply
			// 
			this.cmdApply.Enabled = false;
			this.cmdApply.Location = new System.Drawing.Point(448, 296);
			this.cmdApply.Name = "cmdApply";
			this.cmdApply.TabIndex = 6;
			this.cmdApply.Text = "Apply";
			this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
			// 
			// lblChatMessageLength
			// 
			this.lblChatMessageLength.Location = new System.Drawing.Point(24, 96);
			this.lblChatMessageLength.Name = "lblChatMessageLength";
			this.lblChatMessageLength.Size = new System.Drawing.Size(120, 23);
			this.lblChatMessageLength.TabIndex = 7;
			this.lblChatMessageLength.Text = "Chat Message Length";
			// 
			// ChatLengthSlider
			// 
			this.ChatLengthSlider.Location = new System.Drawing.Point(144, 88);
			this.ChatLengthSlider.Maximum = 2000;
			this.ChatLengthSlider.Minimum = 200;
			this.ChatLengthSlider.Name = "ChatLengthSlider";
			this.ChatLengthSlider.Size = new System.Drawing.Size(456, 45);
			this.ChatLengthSlider.TabIndex = 8;
			this.ChatLengthSlider.TickFrequency = 50;
			this.ChatLengthSlider.Value = 200;
			this.ChatLengthSlider.ValueChanged += new System.EventHandler(this.ChatLengthSlider_ValueChanged);
			// 
			// lblChatLengthValue
			// 
			this.lblChatLengthValue.Location = new System.Drawing.Point(328, 128);
			this.lblChatLengthValue.Name = "lblChatLengthValue";
			this.lblChatLengthValue.Size = new System.Drawing.Size(72, 23);
			this.lblChatLengthValue.TabIndex = 9;
			// 
			// lblNickLength
			// 
			this.lblNickLength.Location = new System.Drawing.Point(24, 160);
			this.lblNickLength.Name = "lblNickLength";
			this.lblNickLength.Size = new System.Drawing.Size(72, 23);
			this.lblNickLength.TabIndex = 10;
			this.lblNickLength.Text = "Nick Length";
			// 
			// txtNickLength
			// 
			this.txtNickLength.Location = new System.Drawing.Point(152, 160);
			this.txtNickLength.Name = "txtNickLength";
			this.txtNickLength.Size = new System.Drawing.Size(56, 20);
			this.txtNickLength.TabIndex = 11;
			this.txtNickLength.Text = "";
			this.txtNickLength.TextChanged +=new EventHandler(txtNickLength_TextChanged);
			// 
			// HubSettings
			// 
			//this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(624, 382);
			this.Controls.Add(this.txtNickLength);
			this.Controls.Add(this.lblNickLength);
			this.Controls.Add(this.lblChatLengthValue);
			this.Controls.Add(this.ChatLengthSlider);
			this.Controls.Add(this.lblChatMessageLength);
			this.Controls.Add(this.cmdApply);
			this.Controls.Add(this.lblMessageLengthValue);
			this.Controls.Add(this.MessageLengthSlider);
			this.Controls.Add(this.lblMaxMessageLength);
			this.Name = "HubSettings";
			this.Text = "HubSettings";
			((System.ComponentModel.ISupportInitialize)(this.MessageLengthSlider)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ChatLengthSlider)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void MessageLengthSlider_ValueChanged(object sender, System.EventArgs e)
		{
			lblMessageLengthValue.Text = MessageLengthSlider.Value.ToString();
			cmdApply.Enabled = true;
		}

		private void cmdApply_Click(object sender, System.EventArgs e)
		{
			
				//GHub.Settings.Synchronization.clientArray.WaitOne();
				//	GHub.Settings.Synchronization.serverArray.WaitOne();

						GHub.Settings.Hub.hubSettings.MaxMessageLength = MessageLengthSlider.Value;
						GHub.Settings.Hub.hubSettings.MaxMainChatLength = ChatLengthSlider.Value;
				//	GHub.Settings.Synchronization.serverArray.ReleaseMutex();
				//GHub.Settings.Synchronization.clientArray.ReleaseMutex();

				cmdApply.Enabled = false;
		}

		private void ChatLengthSlider_ValueChanged(object sender, System.EventArgs e)
		{
			lblChatLengthValue.Text = ChatLengthSlider.Value.ToString();
			cmdApply.Enabled = true;
		}

		private void txtNickLength_TextChanged(object sender, System.EventArgs e)
		{
			cmdApply.Enabled = true;
		}




	}
}
