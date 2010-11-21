using System.Collections;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public interface ITransformer
	{
        /// <summary>
        /// Transforms the specified input.	
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="transformerFileName">Name of the transformer file.</param>
        /// <param name="xsltArgs">The XSLT args.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string Transform(string input, string transformerFileName, Hashtable xsltArgs);
	}
}
