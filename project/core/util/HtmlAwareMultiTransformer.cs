
using System.Collections;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	// ToDo - something more clever with error cases?
    /// <summary>
    /// 	
    /// </summary>
	public class HtmlAwareMultiTransformer : IMultiTransformer
	{
		private readonly ITransformer delegateTransformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlAwareMultiTransformer" /> class.	
        /// </summary>
        /// <param name="delegateTransformer">The delegate transformer.</param>
        /// <remarks></remarks>
		public HtmlAwareMultiTransformer(ITransformer delegateTransformer)
		{
			this.delegateTransformer = delegateTransformer;
		}

        /// <summary>
        /// Transforms the specified input.	
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="transformerFileNames">The transformer file names.</param>
        /// <param name="xsltArgs">The XSLT args.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string Transform(string input, string[] transformerFileNames, Hashtable xsltArgs)
		{
			StringBuilder builder = new StringBuilder();
			foreach (string transformerFileName in transformerFileNames)
			{
				builder.Append(delegateTransformer.Transform(input, transformerFileName, xsltArgs));				
			}
			return builder.ToString();
		}
	}
}
