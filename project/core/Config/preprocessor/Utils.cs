/*
Purpose: definitinon of the XmlPreprocessor.Utils class
Author: Jeremy Lew
Created: 2008.03.24
*/
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    /// <summary>
    /// Internal static utility methods
    /// </summary>
    internal sealed class Utils
    {
        private  Utils()
        {}

        /// <summary>
        /// Create a XML writer with the following settings:
        /// Indent = true;
        /// UTF-8 encoding
        /// Auto conformance level
        /// NewlineChars = "\n"
        /// NewlineHandling = Replace
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static XmlWriter CreateWriter(string url)
        {
            var writer_settings = new XmlWriterSettings();
            writer_settings.Indent = true;
            writer_settings.Encoding = Encoding.UTF8;
            writer_settings.ConformanceLevel = ConformanceLevel.Document;
            writer_settings.NewLineChars = "\n";
            writer_settings.NewLineHandling = NewLineHandling.Replace;
            return XmlWriter.Create(url, writer_settings);
        }

        /// <summary>
        /// Throws an ApplicationException with a message constructed from the
        /// given formatting arguments
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="args"></param>
        internal static void ThrowAppException(string fmt, params object[] args)
        {
            throw new ApplicationException(String.Format(CultureInfo.CurrentCulture, fmt, args));
        }

        /// <summary>
        /// Construct an exception from the given factory, using the message constructed from the given
        /// formatting arguments.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="fmt"></param>
        /// <param name="args"></param>
        internal static void ThrowException(ExceptionFactory factory, string fmt,
                                            params object[] args)
        {
            throw factory(String.Format(CultureInfo.CurrentCulture, fmt, args));
        }

        /// <summary>
        /// Transforms <paramref name="input"/> using the supplied XSLT and associated support objects.  Result is returned as
        /// a XmlDocument DOM object.
        /// </summary>
        /// <param name="input">Document to transform</param>
        /// <param name="transform">The XSLT to apply</param>
        /// <param name="args">The XSLT arguments</param>
        /// <param name="resolver">The XmlResolver used during the transform</param>
        /// <returns>The transformed document as a DOM</returns>
        internal static XmlDocument TransformToDocument(XmlReader input,
                                                        XslCompiledTransform transform,
                                                        XsltArgumentList args, XmlResolver resolver)
        {
            var doc = new XmlDocument();
            using (XmlWriter output = doc.CreateNavigator().AppendChild())
            {
                transform.Transform(input, args, output, resolver);
            }
            return doc;
        }

        /// <summary>
        /// Returns manifest resource stream qualified by the given type and name.
        /// </summary>
        /// <param name="type">Qualifying type</param>
        /// <param name="resourceName">Resource name</param>
        /// <returns></returns>
        public static Stream GetAssemblyResourceStream(Type type, string resourceName)
        {
            return type.Assembly.GetManifestResourceStream(type, resourceName);
        }

        /// <summary>
        /// Returns the manifest resource stream with the given name.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static Stream GetAssemblyResourceStream(string resourceName)
        {
            Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new ApplicationException(
                    String.Format(CultureInfo.CurrentCulture, "Could not find manifest resource stream: {0}", resourceName));
            return stream;
        }

        public static string ResolvePathToAssemblyLocation(string relPath, Assembly assembly)
        {
            return
                Path.Combine(
                    Path.GetDirectoryName(assembly.Location), relPath);
        }
    }
}
