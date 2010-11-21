namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <title>Modification Filters</title>
	public interface IModificationFilter
	{
		bool Accept(Modification modification);
	}
}