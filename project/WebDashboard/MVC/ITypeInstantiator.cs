using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public interface ITypeInstantiator
	{
		object GetInstance(ITypeSpecification typeSpecification);

	}
}
