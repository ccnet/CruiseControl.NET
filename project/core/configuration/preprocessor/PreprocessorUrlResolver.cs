using System;
using System.IO;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    class PreprocessorUrlResolver : XmlUrlResolver
    {
        private string _base_path;

        public event EventHandler< UrlResolvedArgs > UrlResolved;
        public PreprocessorUrlResolver( string base_path )
        {            
            _base_path = base_path;
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            Uri uri;
            if ( String.IsNullOrEmpty( relativeUri ) )
            {
                uri = new Uri( _base_path );
            }
            else
            {
                Uri uri_rel = new Uri( relativeUri, UriKind.RelativeOrAbsolute );
                if ( uri_rel.IsAbsoluteUri )
                {
                    uri = uri_rel;
                }
                else
                {
                    uri = new Uri( Path.Combine( BaseDir, relativeUri ) );
                }
            }
            if ( UrlResolved != null )
                UrlResolved( this, new UrlResolvedArgs( uri ) );
            return uri;
        }

        public string BaseDir
        {
            get { return Path.GetDirectoryName( _base_path ) + Path.DirectorySeparatorChar; }
            set { _base_path = value; }
        }
    }

    internal class UrlResolvedArgs : EventArgs
    {
        private readonly Uri _uri;
        public UrlResolvedArgs( Uri uri )
        {
            _uri = uri;
        }
        public Uri Uri 
        {
            get { return _uri; }            
        }
    }
}
