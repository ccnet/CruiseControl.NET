using System;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface IModificationUrlBuilder
	{
		void SetupModification( Modification[] modifications );
	}
}
