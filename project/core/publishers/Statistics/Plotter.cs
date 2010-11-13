using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using NPlot;
using PlotSurface2D = NPlot.Bitmap.PlotSurface2D;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    /// A 2-dimensional surface plotter.
    public class Plotter : IPlotter
	{
        /// <summary>
        /// The directory where the file will be created.
        /// </summary>
		private string savePath;
        /// <summary>
        /// The disk file extension (should make sense for <see cref="imageFormat"/>.
        /// </summary>
	    private readonly string fileExtension;
        /// <summary>
        /// The type of image to generate.
        /// </summary>
	    private ImageFormat imageFormat;

        /// <summary>
        /// Create a 2-dimensional surface plotter with the specified disk location and image format.
        /// </summary>
        /// <param name="savePath">The directory where the file will be created.</param>
        /// <param name="fileExtension">The disk file extension (should make sense
        /// for <paramref name="imageFormat"/>.</param>
        /// <param name="imageFormat">The type of image to generate.</param>
	    public Plotter(string savePath, string fileExtension, ImageFormat imageFormat)
		{
			this.savePath = savePath;
	        this.fileExtension = fileExtension;
	        this.imageFormat = imageFormat;
		}

        /// <summary>
        /// Plot a set of statistic data and save it to the configured disk file.
        /// </summary>
        /// <param name="ordinateData">The Y-axis data values.</param>
        /// <param name="abscissaData">The X-axis data values.</param>
        /// <param name="statisticName">The name of the statistic to plot.</param>
        /// <remarks>
        /// The disk file name will be
        /// <i><see cref="savePath"/></i>.<i><paramref name="statisticName"/></i>.<i><see cref="fileExtension"/></i>. 
        /// </remarks>
        public void DrawGraph(IList ordinateData, IList abscissaData, string statisticName)
		{
			Bitmap bitmap = Plot(ordinateData, abscissaData);
			bitmap.Save(Path.Combine(savePath, string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}.{1}", statisticName, fileExtension)));
		}

		/// <summary>
        /// Plot a set of statistic data and write the image to a stream.
		/// </summary>
        /// <param name="ordinateData">The Y-axis data values.</param>
        /// <param name="abscissaData">The X-axis data values.</param>
        /// <param name="stream">The stream to receive the resulting image.</param>
        public void WriteToStream(IList ordinateData, IList abscissaData, Stream stream)
		{
			Bitmap bitmap = Plot(ordinateData, abscissaData);
			bitmap.Save(stream, imageFormat);
		}

        /// <summary>
        /// Create a 2-dimensional surface plot of the specified statistic data.
        /// </summary>
        /// <param name="ordinateData">The Y-axis data values.</param>
        /// <param name="abscissaData">The X-axis data values.</param>
        /// <returns>The plot image bitmap.</returns>
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