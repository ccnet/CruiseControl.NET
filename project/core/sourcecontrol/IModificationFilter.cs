namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public interface IModificationFilter
	{
		bool Accept(Modification m);
	}
}