using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NPlot;
using PlotSurface2D = NPlot.Bitmap.PlotSurface2D;

namespace ThoughtWorks.CruiseControl.Core.publishers.Statistics
{
	public class Plotter : IPlotter
	{
		private string savePath;
		private string fileName;

		public Plotter(string savePath, string fileName)
		{
			this.savePath = savePath;
			this.fileName = fileName;
		}

		public void DrawGraph(IList ordinateData, IList abscissaData, double sigma)
		{
			Bitmap bitmap = Plot(ordinateData, abscissaData, sigma);
			bitmap.Save(Path.Combine(savePath, fileName));
		}

		public void WriteToStream(IList ordinateData, IList abscissaData, double sigma, Stream stream)
		{
			Bitmap bitmap = Plot(ordinateData, abscissaData, sigma);
			bitmap.Save(stream, ImageFormat.Png);
		}

		private Bitmap Plot(IList ordinateData, IList abscissaData, double sigma)
		{

			PlotSurface2D plotSurface2D = new PlotSurface2D(200, 200);
			LinePlot linePlot = new LinePlot(ordinateData, abscissaData);
			linePlot.ShadowColor = Color.Beige;
			linePlot.Pen = new Pen(Color.Blue);
			linePlot.Pen.Width = 2.0f;
			plotSurface2D.Add(linePlot);

			ArrayList sigmaData = new ArrayList();
			for (int i = 0; i < abscissaData.Count; i++)
			{
				sigmaData.Add(sigma);
			}
			LinePlot sigmaLine = new LinePlot(sigmaData, abscissaData);
			sigmaLine.Pen = new Pen(Color.Red);
			sigmaLine.Pen.Width = 2.0f;
			plotSurface2D.Add(sigmaLine);
			plotSurface2D.Refresh();
			return plotSurface2D.Bitmap;
		}
	}
}