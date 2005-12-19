using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	/// <summary>
	/// Class originally by Joel Matthias, and published into the public domain on
	/// CodeProject.com at:
	/// http://www.codeproject.com/csharp/restoreformstate.asp
	/// 
	/// Adapted for use with ccnet-tray by Grant Drake.
	/// </summary>
	public class PersistWindowState : Component
	{

		public event WindowStateEventHandler LoadState;
		public event WindowStateEventHandler SaveState;

		private Form parent;
		private string regPath;
		private int normalLeft;
		private int normalTop;
		private int normalWidth;
		private int normalHeight;
		private FormWindowState windowState;
		private bool allowSaveMinimized = false;

		public PersistWindowState()
		{
		}

		public Form Parent
		{
			set
			{
				parent = value;

				// subscribe to parent form's events
				parent.Load += new EventHandler(OnLoad);
				parent.Closing += new CancelEventHandler(OnClosing);
				parent.Move += new EventHandler(OnMove);
				parent.Resize += new EventHandler(OnResize);

				// get initial width and height in case form is never resized
				normalWidth = parent.Width;
				normalHeight = parent.Height;
			}
			get { return parent; }
		}

		// registry key should be set in parent form's constructor
		public string RegistryPath
		{
			set { regPath = value; }
			get { return regPath; }
		}

		public bool AllowSaveMinimized
		{
			get { return allowSaveMinimized; }
			set { allowSaveMinimized = value; }
		}

		private void OnLoad(object sender, EventArgs e)
		{
			// attempt to read state from registry
			RegistryKey key = Registry.CurrentUser.OpenSubKey(regPath);
			if (key != null)
			{
				int left = (int) key.GetValue("Left", parent.Left);
				int top = (int) key.GetValue("Top", parent.Top);
				int width = (int) key.GetValue("Width", parent.Width);
				int height = (int) key.GetValue("Height", parent.Height);
				FormWindowState windowState = (FormWindowState) key.GetValue("WindowState", (int) parent.WindowState);

				parent.Location = new Point(left, top);
				parent.Size = new Size(width, height);
				parent.WindowState = windowState;

				// fire LoadState event
				if (LoadState != null)
					LoadState(this, new WindowStateEventArgs(key));
			}
		}

		private void OnClosing(object sender, CancelEventArgs e)
		{
			// save position, size and state
			RegistryKey key = Registry.CurrentUser.CreateSubKey(regPath);
			key.SetValue("Left", normalLeft);
			key.SetValue("Top", normalTop);
			key.SetValue("Width", normalWidth);
			key.SetValue("Height", normalHeight);

			// check if we are allowed to save the state as minimized (not normally)
			if (!allowSaveMinimized)
			{
				if (windowState == FormWindowState.Minimized)
					windowState = FormWindowState.Normal;
			}

			key.SetValue("WindowState", (int) windowState);

			// fire SaveState event
			if (SaveState != null)
				SaveState(this, new WindowStateEventArgs(key));
		}

		private void OnMove(object sender, EventArgs e)
		{
			// save position
			if (parent.WindowState == FormWindowState.Normal)
			{
				normalLeft = parent.Left;
				normalTop = parent.Top;
			}
			// save state
			windowState = parent.WindowState;
		}

		private void OnResize(object sender, EventArgs e)
		{
			// save width and height
			if (parent.WindowState == FormWindowState.Normal)
			{
				normalWidth = parent.Width;
				normalHeight = parent.Height;
			}
		}
	}
}