namespace ThoughtWorks.CruiseControl.UnitTests.IntegrationTests
{
    using System.Xml;

    /// <summary>
    /// Helper class for working with embedded data.
    /// </summary>
    public static class EmbeddedData
    {
        #region Helper methods
        /// <summary>
        /// Loads an embedded XML document.
        /// </summary>
        /// <param name="documentName">Name of the document.</param>
        /// <returns>The loaded XML data.</returns>
        public static XmlDocument LoadEmbeddedXml(string documentName)
        {
            var document = new XmlDocument();
            var actualName = "ThoughtWorks.CruiseControl.UnitTests.IntegrationTests.Data." + documentName;
            using (var stream = typeof(DynamicValuesTests).Assembly.GetManifestResourceStream(actualName))
            {
                document.Load(stream);
            }

            return document;
        }
        #endregion
    }
}
