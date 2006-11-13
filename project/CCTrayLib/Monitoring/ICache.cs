using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Interface for caching that allows for cache material to be cleared
	/// </summary>
	public interface ICache
	{
		void Clear();
	}
}
