using System;
using System.Diagnostics;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    internal class TempFile : IDisposable
    {
        private readonly string _path = String.Empty;

        public TempFile()
        {
            _path = Path.GetTempFileName();
        }

        public TempFile(Stream contents) : this()
        {
            using ( Stream output_stream = File.OpenWrite( _path ) )
            {
                const int buflen = 4096;
                var buffer = new byte[buflen];
                int bytesread;
                while ( ( bytesread = contents.Read( buffer, 0, buflen ) ) > 0 )
                {
                    output_stream.Write( buffer, 0, bytesread );
                }
            }
        }

        public string TempFileName
        {
            get { return _path; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if ( File.Exists( _path ) )
                {
                    File.Delete( _path );
                }
            }
            catch ( Exception ex )
            {
                Trace.TraceError(
                    "Could not delete temp file '{0}'\nReason: {1}", _path,
                    ex.Message );
            }
        }

        #endregion

        public Stream OpenRead()
        {
            return File.OpenRead( _path );
        }
    }
}
