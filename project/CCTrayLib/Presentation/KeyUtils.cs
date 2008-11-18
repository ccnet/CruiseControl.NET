using System;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public static class KeyUtils
	{
		public static bool PressedControlA(KeyEventArgs e)
		{
			return (e.Modifiers == Keys.Control) && (e.KeyCode == Keys.A);
		}
	}
}
