using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Transfer a file over .NET remoting.
    /// </summary>
    [Serializable]
    public class RemotingFileTransfer
        : IFileTransfer
    {
        #region Private constants
        private const int blockSize = 131072;
        #endregion

        #region Private fields
        private byte[] fileData = { };
        private int fileLength/* = 0*/;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="RemotingFileTransfer"/>.
        /// </summary>
        /// <param name="source"></param>
        public RemotingFileTransfer(Stream source)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Load all the data into the memory stream
                byte[] data = new byte[blockSize];
                var byteCount = source.Read(data, 0, blockSize);
                while (byteCount > 0)
                {
                    memoryStream.Write(data, 0, byteCount);
                    byteCount = source.Read(data, 0, blockSize);
                }

                // Store the data so it will be transferred
                this.fileData = memoryStream.GetBuffer();
                this.fileLength = Convert.ToInt32(source.Length);
            }
        }
        #endregion

        #region Public methods
        #region Download()
        /// <summary>
        /// Download a file.
        /// </summary>
        /// <param name="destination">The destination to download the file to.</param>
        public void Download(Stream destination)
        {
            destination.Write(fileData, 0, this.fileLength);
        }
        #endregion
        #endregion
    }
}
