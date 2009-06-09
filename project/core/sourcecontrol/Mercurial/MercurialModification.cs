using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
    [ReflectorType("modification")]
    public class MercurialModification
    {
        [ReflectorProperty("node")] public string Version;

        [ReflectorProperty("date")] public string ModificationTime;

        public string Type
        {
            get { return "Changeset"; }
        }

        [ReflectorProperty("files")] public string FileName;

        public DateTime ModifiedTime
        {
            get { return DateTime.Parse(ModificationTime); }
            set { ModificationTime = value.ToString(); }
        }

        [ReflectorProperty("author")] public string UserName;
        [ReflectorProperty("rev")] public int ChangeNumber;
        [ReflectorProperty("desc")] public string Comment;
        [ReflectorProperty("email")] public string EmailAddress;

        public static implicit operator Modification(MercurialModification hg)
        {
            Modification modification = new Modification();
            modification.Version = hg.Version;
            modification.ModifiedTime = hg.ModifiedTime;
            modification.Type = hg.Type;
            modification.FileName = hg.FileName;
            modification.UserName = hg.UserName;
            modification.ChangeNumber = hg.Version;
            modification.Comment = hg.Comment;
            modification.EmailAddress = hg.EmailAddress;
            return modification;
        }
    }
}
