using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
    [ReflectorType("modifications")]
    public class MercurialModificationCollection
    {
        [ReflectorProperty("array")]
        public MercurialModification[] modifications { get; set; }
    }
}
