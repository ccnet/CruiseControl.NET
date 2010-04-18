namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    using System.Collections.Generic;
    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    /// Defines an item that can be packaged.
    /// </summary>
    public interface IPackageItem
    {
        /// <summary>
        /// Packages the specified items.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="zipStream">The zip stream.</param>
        /// <returns>The name of the files that were packaged.</returns>
        IEnumerable<string> Package(IIntegrationResult result, ZipOutputStream zipStream);
    }
}
