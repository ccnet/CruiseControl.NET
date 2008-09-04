using System.Collections;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    /// <summary>
    /// Plot a set of statistic data.
    /// </summary>
	public interface IPlotter
	{
        /// <summary>
        /// Plot a set of statistic data.
        /// </summary>
        /// <param name="ordinateData">The Y-axis data values.</param>
        /// <param name="abscissaData">The X-axis data values.</param>
        /// <param name="statisticName">The name of the statistic to plot.</param>
		void DrawGraph(IList ordinateData, IList abscissaData, string statisticName);

        /// <summary>
        /// Plot a set of statistic data and write the image to a stream.
        /// </summary>
        /// <param name="ordinateData">The Y-axis data values.</param>
        /// <param name="abscissaData">The X-axis data values.</param>
        /// <param name="stream">The stream to receive the resulting image.</param>
        void WriteToStream(IList ordinateData, IList abscissaData, Stream stream);
	}
}