namespace ThoughtWorks.CruiseControl.Core.Util
{
	public interface IFileTransformer
	{
		string Transform(string inputFileName, string transformerFileName);
	}
}
