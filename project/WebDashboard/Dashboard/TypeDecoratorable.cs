using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface TypeDecoratorable
	{
		TypeDecoratorable Decorate(Type type);
	}
}
