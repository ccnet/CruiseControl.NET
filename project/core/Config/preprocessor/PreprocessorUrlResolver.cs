using System;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    class PreprocessorUrlResolver : XmlUrlResolver
    {
        public event EventHandler< UrlResolvedArgs > UrlResolved;
        public PreprocessorUrlResolver( )
        {            
        }

        /// <summary>
        /// Resolve the absolute URI from the base and relative URIs, evaluating the relative URI
        /// in the base URI's context.
        /// </summary>
        /// <param name="baseUri">The base URI used to resolve the relative URI.</param>
        /// <param name="relativeUri">
        /// The URI string to resolve. The URI can be absolute or relative. If absolute, this value
        /// effectively replaces the baseUri value. If relative, it combines with the baseUri 
        /// (following the rules of <see cref="System.Uri(Uri, Uri)"/>) to make an absolute URI.
        /// </param>
        /// <returns>
        /// A <see cref="System.Uri"/> representing the absolute URI, or the base URI if the relative URI
        /// is null or empty.
        /// </returns>
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            Uri uri;
            if ( String.IsNullOrEmpty( relativeUri ) )
            {
                uri = baseUri;
            }
            else
            {
                string uriString = Uri.UnescapeDataString(relativeUri);
                Uri uri_rel = new Uri(uriString, UriKind.RelativeOrAbsolute);
                uri = new Uri(baseUri, uri_rel); 
            }
            if ( UrlResolved != null )
                UrlResolved( this, new UrlResolvedArgs( uri ) );
            return uri;
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
