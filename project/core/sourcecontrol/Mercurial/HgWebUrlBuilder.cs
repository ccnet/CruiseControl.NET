using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
    [ReflectorType("hgweb")]
    public class HgWebUrlBuilder : IModificationUrlBuilder
    {
        [ReflectorProperty("url")] public string Url;

        public void SetupModification(Modification[] modifications)
        {
            foreach (Modification modification in modifications)
            {
                modification.Url = Url + "rev/" + modification.Version;
            }
        }
    }
}
