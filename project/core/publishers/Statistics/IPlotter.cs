using System.Collections;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	public interface IPlotter
	{
		void DrawGraph(IList ordinateData, IList abscissaData, string statisticName);
		void WriteToStream(IList ordinateData, IList abscissaData, Stream stream);
	}
}