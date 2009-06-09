using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
    [ReflectorType("modifications")]
    public class MercurialModificationCollection
    {
        [ReflectorArray("array")] public MercurialModification[] modifications;
    }
}
