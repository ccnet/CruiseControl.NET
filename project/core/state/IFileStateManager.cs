using ThoughtWorks.CruiseControl.Core.State;

namespace ThoughtWorks.CruiseControl.Core.State
{
	public interface IFileStateManager : IStateManager
	{
		string Filename
		{
			get; set;
		}
	}
}
