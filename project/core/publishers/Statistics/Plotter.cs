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

		public void DrawGraph(IList ordinateData, IList abscissaData)
		{
			Bitmap bitmap = Plot(ordinateData, abscissaData);
			bitmap.Save(Path.Combine(savePath, fileName));
		}

		public void WriteToStream(IList ordinateData, IList abscissaData, Stream stream)
		{
			Bitmap bitmap = Plot(ordinateData, abscissaData);
			bitmap.Save(stream, ImageFormat.Png);
		}

		private Bitmap Plot(IList ordinateData, IList abscissaData)
		{

			PlotSurface2D plotSurface2D = new PlotSurface2D(200, 200);
			LinePlot linePlot = new LinePlot(ordinateData, abscissaData);
			linePlot.ShadowColor = Color.Beige;
			linePlot.Pen = new Pen(Color.Blue);
			linePlot.Pen.Width = 2.0f;
			plotSurface2D.Add(linePlot);
			plotSurface2D.Refresh();
			return plotSurface2D.Bitmap;
		}
	}
}