using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	class DefaultStatusIconLoader : IStatusIconLoader
	{
		private IIconStore _iconStore ;

	    public DefaultStatusIconLoader(IIconStore iconStore)
	    {
			_iconStore = iconStore;
	    }

	    public StatusIcon LoadIcon(ProjectStatus status)
	    {
	        return _iconStore[status];
	    }
	}
}
