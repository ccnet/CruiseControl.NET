namespace ThoughtWorks.CruiseControl.Core.Util
{
	public interface IMultiTransformer
	{
		string Transform(string input, string[] transformerFileNames);
	}
}
