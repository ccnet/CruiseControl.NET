using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    /// <summary>
    /// This class foresees for the mappings of a prefix to an xml namespace. For example :
    /// xmlns:mstest=http://microsoft.com/schemas/VisualStudio/TeamTest/2010
    /// </summary>
    /// <title>Namespace Mapping</title>
    [ReflectorType("namespaceMapping")]
    public class StatisticsNamespaceMapping
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public StatisticsNamespaceMapping()
        { }

        /// <summary>
        /// Create with specified settings
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="url"></param>
        public StatisticsNamespaceMapping(string prefix, string url)
        {
            this.Prefix = prefix;
            this.Url = url;
        }


        /// <summary>
        /// the prefix used in the xpath for this namespace
        /// </summary>
        [ReflectorProperty("prefix")]
        public string Prefix { get; set; }

        /// <summary>
        /// the url of the namespace
        /// </summary>
        [ReflectorProperty("url")]
        public string Url { get; set; }

    }
}
