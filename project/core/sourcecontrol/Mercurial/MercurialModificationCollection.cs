using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
    /// <summary>
    /// 	
    /// </summary>
    [ReflectorType("modifications")]
    public class MercurialModificationCollection
    {
        /// <summary>
        /// Gets or sets the modifications.	
        /// </summary>
        /// <value>The modifications.</value>
        /// <remarks></remarks>
        [ReflectorProperty("array")]
        public MercurialModification[] modifications { get; set; }
    }
}
