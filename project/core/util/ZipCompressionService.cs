namespace ThoughtWorks.CruiseControl.Core.Util
{
    using System;
    using System.IO;
    using System.Text;
    using ICSharpCode.SharpZipLib.Core;
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

    /// <summary>
    /// An <see cref="ICompressionService"/> using ZIP compression.
    /// </summary>
    public class ZipCompressionService
        : ICompressionService
    {
        #region Public methods
        #region CompressString()
        /// <summary>
        /// Compresses a string.
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <returns>The compressed string.</returns>
        public string CompressString(string value)
        {
            // The input value must be non-null
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var outputData = string.Empty;
            var inputData = UTF8Encoding.UTF8.GetBytes(value);
            using (var inputStream = new MemoryStream(inputData))
            {
                using (var outputStream = new MemoryStream())
                {
                    // Zip the string
                    using (var zipStream = new DeflaterOutputStream(outputStream))
                    {
                        zipStream.IsStreamOwner = false;
                        StreamUtils.Copy(inputStream, zipStream, new byte[4096]);
                    }

                    // Convert to a string
                    outputData = Convert.ToBase64String(outputStream.GetBuffer(), 0, Convert.ToInt32(outputStream.Length));
                }
            }

            // Return the compressed string
            return outputData;
        }
        #endregion

        #region ExpandString()
        /// <summary>
        /// Expands (de-compresses) a string.
        /// </summary>
        /// <param name="value">The string to expanded.</param>
        /// <returns>The expanded string.</returns>
        public string ExpandString(string value)
        {
            // The input value must be non-null
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var outputData = string.Empty;
            var inputData = Convert.FromBase64String(value);
            using (var inputStream = new MemoryStream(inputData))
            {
                using (var outputStream = new MemoryStream())
                {
                    // Zip the string
                    using (var zipStream = new InflaterInputStream(inputStream))
                    {
                        zipStream.IsStreamOwner = false;
                        StreamUtils.Copy(zipStream, outputStream, new byte[4096]);
                    }

                    // Convert to a string
                    outputData = UTF8Encoding.UTF8.GetString(outputStream.GetBuffer(), 0, Convert.ToInt32(outputStream.Length));
                }
            }

            // Return the compressed string
            return outputData;
        }
        #endregion
        #endregion
    }
}
