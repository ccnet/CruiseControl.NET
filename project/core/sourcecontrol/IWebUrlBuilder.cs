using System;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface IUrlBuilder
	{
		void SetupModification( Modification[] modifications );
	}
}
