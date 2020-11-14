using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ProfilesPlugIn
{
	/// <summary>
	/// Summary description for frmProfiles.
	/// </summary>
	public class frmProfiles : System.Windows.Forms.Panel
	{
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox isOPCheckBox;
		private System.Windows.Forms.TextBox txtOnJoinMessage;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmProfiles()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public bool isOP
		{
			get
			{
				return this.isOPCheckBox.Checked;
			}
			set
			{
				this.isOPCheckBox.Checked = value;
			}
		}

		public string WelcomeMessage
		{
			get
			{
				return this.txtOnJoinMessage.Text;
			}
			set
			{
				this.txtOnJoinMessage.Text = value;
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
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.isOPCheckBox = new System.Windows.Forms.CheckBox();
			this.txtOnJoinMessage = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.Location = new System.Drawing.Point(8, 40);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(80, 199);
			this.listBox1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.TabIndex = 1;
			this.label1.Text = "Profiles";
			// 
			// isOPCheckBox
			// 
			this.isOPCheckBox.Location = new System.Drawing.Point(112, 88);
			this.isOPCheckBox.Name = "isOPCheckBox";
			this.isOPCheckBox.TabIndex = 2;
			this.isOPCheckBox.Text = "Is OP";
			// 
			// txtOnJoinMessage
			// 
			this.txtOnJoinMessage.Location = new System.Drawing.Point(104, 136);
			this.txtOnJoinMessage.Multiline = true;
			this.txtOnJoinMessage.Name = "txtOnJoinMessage";
			this.txtOnJoinMessage.Size = new System.Drawing.Size(200, 104);
			this.txtOnJoinMessage.TabIndex = 3;
			this.txtOnJoinMessage.Text = "";
			// 
			// frmProfiles
			// 
			//this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(584, 502);
			this.Controls.Add(this.txtOnJoinMessage);
			this.Controls.Add(this.isOPCheckBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listBox1);
			this.Name = "frmProfiles";
			this.Text = "frmProfiles";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
