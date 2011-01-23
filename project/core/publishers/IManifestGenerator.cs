using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// Defines a manifest generator for packages.
    /// </summary>
    /// <title>Manifest Generators</title>
    public interface IManifestGenerator
    {
        #region Methods
        #region Generate()
        /// <summary>
        /// Generate a manifest for a package.
        /// </summary>
        /// <param name="result">The result of the build.</param>
        /// <param name="packagedFiles">The files that were packaged.</param>
        /// <returns>An <see cref="XmlDocument"/> containing the manifest.</returns>
        XmlDocument Generate(IIntegrationResult result, string[] packagedFiles);
        #endregion
        #endregion
    }
}
