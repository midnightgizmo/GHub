using System;

namespace GUI
{
	/// <summary>
	/// Summary description for numberTextBox.
	/// </summary>
	public class numberTextBox : System.Windows.Forms.TextBox
	{
		public numberTextBox()
		{
			this.KeyPress +=new System.Windows.Forms.KeyPressEventHandler(numberTextBox_KeyPress);
			this.ContextMenu = new System.Windows.Forms.ContextMenu();
		}

		private void numberTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			int i = e.KeyChar;


			switch(i)
			{
					// ctrl v (i.e. paste
				case 22:
					e.Handled = true;
					return;

					// ctrl c (copy)
				case 3:
					return;

					// ctrl x (i.e. cut)
				case 24:
					return;

					// back space was pressed
				case 8:
					return;
			}

			if (char.IsNumber(e.KeyChar))
				return;

			e.Handled = true;

		}

	}
}
