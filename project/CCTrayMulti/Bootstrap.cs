using System;
using System.Windows.Forms;

namespace CCTrayMulti
{
	public class Bootstrap
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.EnableVisualStyles();
			Application.DoEvents();
			Application.Run(new MainForm());
		}

	}
}
