using System;
using System.Drawing;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public interface IIconProvider
	{
		Icon Icon { get; }
		event EventHandler IconChanged;
	}
}