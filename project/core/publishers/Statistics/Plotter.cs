using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using NPlot;
using PlotSurface2D = NPlot.Bitmap.PlotSurface2D;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	public class Plotter : IPlotter
	{
		private string savePath;
	    private readonly string fileExtension;
	    private ImageFormat imageFormat;

	    public Plotter(string savePath, string fileExtension, ImageFormat imageFormat)
		{
			this.savePath = savePath;
	        this.fileExtension = fileExtension;
	        this.imageFormat = imageFormat;
		}

		public void DrawGraph(IList ordinateData, IList abscissaData, string statisticName)
		{
			Bitmap bitmap = Plot(ordinateData, abscissaData);
			bitmap.Save(Path.Combine(savePath, string.Format("{0}.{1}", statisticName, fileExtension)));
		}

		public void WriteToStream(IList ordinateData, IList abscissaData, Stream stream)
		{
			Bitmap bitmap = Plot(ordinateData, abscissaData);
			bitmap.Save(stream, imageFormat);
		}

		private static Bitmap Plot(IList ordinateData, IList abscissaData)
		{

			PlotSurface2D plotSurface2D = new PlotSurface2D(200, 200);
		    plotSurface2D.SmoothingMode = SmoothingMode.HighQuality;

		    LinePlot linePlot = new LinePlot(ordinateData, abscissaData);
			linePlot.ShadowColor = Color.Beige;
			linePlot.Pen = new Pen(Color.Blue);
			linePlot.Pen.Width = 1.0f;
			plotSurface2D.Add(linePlot);
			plotSurface2D.Refresh();
			return plotSurface2D.Bitmap;
		}
	}
}