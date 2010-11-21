using System.Collections;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IMultiTransformer
	{
        /// <summary>
        /// Transforms the specified input.	
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="transformerFileNames">The transformer file names.</param>
        /// <param name="xsltArgs">The XSLT args.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string Transform(string input, string[] transformerFileNames, Hashtable xsltArgs);
	}
}
