using System;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Drawing;

namespace ThoughtWorks.CruiseControl.Remote.monitor
{
	/// <summary>
	/// A class to assist loading embedded resources.  Note that files requested in this fashion
	/// must be marked as 'Embedded resource' in the file properies, accessible from the VS.NET 
	/// solution explorer.
	/// </summary>
	/// <remarks>
	/// If you're having trouble identifying your resource name, compile your application and use
	/// <code>ildasm.exe</code> to view your assembly manifest—the resources are listed in the form
	/// <blockquote><code><pre>
	/// .mresource public &lt;resource-name&gt;
	/// {
	/// }
	/// </pre></code></blockquote>.
	/// </remarks>
	public class ResourceUtil
	{
		#region Private constructor

		/// <summary>
		/// Class cannot be instantiated... it's for static use only.
		/// </summary>
		private ResourceUtil() {}

		#endregion

		#region Loosely typed

		/// <summary>Gets the named resource from the calling assembly.</summary>
		/// <remarks>
		/// The type of object is determined by the filename extension.
		/// For example, ".txt", ".htm[l]" are strings, ".xml" is an 
		/// XmlDocument, ".ico" is an Icon, ".bmp", ".gif", ".jpg", 
		/// ".tif", and ".png" are Bitmaps. Anything else is just a 
		/// stream.
		/// </summary>
		/// <param name="resName">The filename of the embedded resource.</param>
		/// <returns>If recognized, a String, XmlDocument, Icon, or Bitmap. 
		/// Otherwise, a Stream object.</returns>
		public static object GetEmbeddedResource(string resourceName) 
		{
			return GetEmbeddedResource(resourceName, Assembly.GetCallingAssembly());
		}

		/// <summary>Gets the named resource from the specified assembly.</summary>
		/// <remarks>
		/// The type of object is determined by the filename extension.
		/// For example, ".txt", ".htm[l]" are strings, ".xml" is an 
		/// XmlDocument, ".ico" is an Icon, ".bmp", ".gif", ".jpg", 
		/// ".tif", and ".png" are Bitmaps. Anything else is just a 
		/// stream.
		/// </remarks>
		/// <param name="resName">The filename of the embedded resource.</param>
		/// <param name="assemblyContainingResource">The Assembly that contains the embedded resource.</param>
		/// <returns>If recognized, a String, XmlDocument, Icon, or Bitmap. 
		/// Otherwise, a Stream object.</returns>
		public static object GetEmbeddedResource(string resourceName, Assembly assemblyContainingResource) 
		{
			// get the resource's stream
			Stream resourceStream = GetResourceStream(resourceName, assemblyContainingResource);

			// handle the resource based upon its filename extension
			string resourceExtension = resourceName.Substring(resourceName.LastIndexOf(".") + 1);
			switch (resourceExtension.ToLower()) 
			{
				case "txt":
				case "htm":
				case "html":
					return ReadStringFromStream(resourceStream);
				case "xml":
				case "config":
					return ReadXmlDocumentFromStream(resourceStream);
				case "ico":
					return new Icon(resourceStream);
				case "bmp":
				case "gif":
				case "jpg":
				case "jpeg":
				case "exif":
				case "wmf":
				case "emf":
				case "png":
				case "tif":
				case "tiff":
					return new Bitmap(resourceStream);
				default:
					return resourceStream;
			}
		}

		#endregion

		#region Bitmap (Image etc...)

		/// <summary>
		/// Returns a Bitmap object for the specified resource name.
		/// </summary>
		/// <param name="resourceName">The fully namespace-qualified filename of the embedded resource.</param>
		/// <returns>A Bitmap object from the resource specified.</returns>
		public static Bitmap GetEmbeddedBitmap(string resourceName) 
		{
			return new Bitmap(GetResourceStream(resourceName, Assembly.GetCallingAssembly()));
		}

		#endregion

		#region Icon

		/// <summary>
		/// Returns an Icon object for the specified resource name.
		/// </summary>
		/// <param name="resourceName">The fully namespace-qualified filename of the embedded resource.</param>
		/// <returns>An Icon object from the resource specified.</returns>
		public static Icon GetEmbeddedIcon(string resourceName) 
		{
			return new Icon(GetResourceStream(resourceName, Assembly.GetCallingAssembly()));
		}

		#endregion

		#region string

		/// <summary>
		/// Returns the contents of the specified resource file as a string.
		/// </summary>
		/// <param name="resourceName">The fully namespace-qualified filename of the embedded resource.</param>
		/// <returns>The contents of the specified resource file as a string.</returns>
		public static string GetEmbeddedTextFile(string resourceName) 
		{
			return ReadStringFromStream(GetResourceStream(resourceName, Assembly.GetCallingAssembly()));
		}

		#endregion

		#region XmlDocument

		/// <summary>
		/// Returns the contents of the specified resource file as an XmlDocument.
		/// </summary>
		/// <param name="resourceName">The fully namespace-qualified filename of the embedded resource.</param>
		/// <returns>The contents of the specified resource file as an XmlDocument.</returns>
		public static XmlDocument GetEmbeddedXmlDocument(string resourceName) 
		{
			return ReadXmlDocumentFromStream(GetResourceStream(resourceName, Assembly.GetCallingAssembly()));
		}

		#endregion

		#region XslTransform

//		/// <summary>
//		/// Returns the contents of the specified resource file as an XslTransform.
//		/// </summary>
//		/// <param name="resourceName">The fully namespace-qualified filename of the embedded resource.</param>
//		/// <returns>The contents of the specified resource file as an XslTransform.</returns>
//		public static XslTransform GetEmbeddedXslTransform(string resourceName) 
//		{
//			return ReadXslTransformFromStream(GetResourceStream(resourceName, Assembly.GetCallingAssembly()));
//		}

		#endregion

		#region Private helper methods

		static private Stream GetResourceStream(string resourceName, Assembly assembly)
		{
			// replace file system separators with periods
			resourceName = resourceName.Replace("/", ".").Replace(@"\", ".");

			// get the resource's stream
			Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
			
			// check that the resource was found
			if (resourceStream==null) 
			{
				throw new ArgumentException("Resource '"+resourceName+"' not found (note that resource names are case sensitive)");
			}		

			return resourceStream;
		}

		static string ReadStringFromStream(Stream resourceStream)
		{
			StreamReader streamReader = new StreamReader(resourceStream);
			return streamReader.ReadToEnd();
		}

		static XmlDocument ReadXmlDocumentFromStream(Stream resourceStream)
		{
			StreamReader streamReader = new StreamReader(resourceStream);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(streamReader.ReadToEnd());
			return xmlDocument;
		}

//		static XslTransform ReadXslTransformFromStream(Stream resourceStream)
//		{
//			StreamReader streamReader = new StreamReader(resourceStream);
//			XmlTextReader xslReader = new XmlTextReader(resourceStream);
//			XslTransform xslTransform = new XslTransform();
//			// this call is obsolete as of .net fx 1.1
//			xslTransform.Load(xslReader);
//			xslReader.Close();
//			return xslTransform;
//		}

		#endregion
	}
}
