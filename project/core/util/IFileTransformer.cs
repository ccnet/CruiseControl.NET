namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IFileTransformer
	{
        /// <summary>
        /// Transforms the specified input file name.	
        /// </summary>
        /// <param name="inputFileName">Name of the input file.</param>
        /// <param name="transformerFileName">Name of the transformer file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string Transform(string inputFileName, string transformerFileName);
	}
}
