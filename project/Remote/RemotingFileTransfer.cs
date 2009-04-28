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
        : IFileTransfer, IDisposable
    {
        #region Private constants
        private const int blockSize = 131072;
        #endregion

        #region Private fields
        private readonly RemotingStreamHolder streamHolder;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="RemotingFileTransfer"/>.
        /// </summary>
        /// <param name="source"></param>
        public RemotingFileTransfer(Stream source)
        {
            streamHolder = new RemotingStreamHolder(source);
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
            streamHolder.Reset();
            int count = blockSize;
            while (count > 0)
            {
                TransferPackage package = streamHolder.TransferData(blockSize);
                count = package.Length;
                destination.Write(package.Data, 0, count);
            }
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Clean up.
        /// </summary>
        public void Dispose()
        {
            streamHolder.Dispose();
        }
        #endregion
        #endregion

        #region Private classes
        #region RemotingStreamHolder
        /// <summary>
        /// The remote reference to the file to download.
        /// </summary>
        private class RemotingStreamHolder
            : MarshalByRefObject, IDisposable
        {
            #region Private fields
            private readonly Stream stream;
            #endregion

            #region Constructors
            /// <summary>
            /// Initialise a new <see cref="RemotingStreamHolder"/>.
            /// </summary>
            /// <param name="source"></param>
            public RemotingStreamHolder(Stream source)
            {
                stream = source;
            }
            #endregion

            #region Public methods
            #region TransferData()
            /// <summary>
            /// Transfer some data.
            /// </summary>
            /// <param name="data"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            public TransferPackage TransferData(int length)
            {
                byte[] data = new byte[length];
                int transferedLength = stream.Read(data, 0, length);
                return new TransferPackage(data, transferedLength);
            }
            #endregion

            #region Reset()
            /// <summary>
            /// Return to the beginning of the stream.
            /// </summary>
            public void Reset()
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            #endregion

            #region Dispose()
            /// <summary>
            /// Clean up.
            /// </summary>
            public void Dispose()
            {
                stream.Dispose();
            }
            #endregion
            #endregion
        }
        #endregion

        #region TransferPackage
        /// <summary>
        /// A package of data.
        /// </summary>
        [Serializable]
        private class TransferPackage
        {
            #region Private fields
            private readonly byte[] data;
            private readonly int length;
            #endregion

            #region Constructors
            /// <summary>
            /// Initialise a new <see cref="TransferPackage"/>.
            /// </summary>
            /// <param name="data"></param>
            /// <param name="length"></param>
            public TransferPackage(byte[] data, int length)
            {
                this.data = data;
                this.length = length;
            }
            #endregion

            #region Public properties
            #region Data
            /// <summary>
            /// The data.
            /// </summary>
            public byte[] Data
            {
                get { return data; }
            }
            #endregion

            #region Length
            /// <summary>
            /// The length of the data.
            /// </summary>
            public int Length
            {
                get { return length; }
            }
            #endregion
            #endregion
        }
        #endregion
        #endregion
    }
}
