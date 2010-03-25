using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
    [ReflectorType("modification")]
    public class MercurialModification
    {
        [ReflectorProperty("node")]
        public string Version { get; set; }

        [ReflectorProperty("date")]
        public string ModificationTime { get; set; }

        public string Type
        {
            get { return "Changeset"; }
        }

        [ReflectorProperty("files")]
        public string FileName { get; set; }

        public DateTime ModifiedTime
        {
            get { return DateTime.Parse(ModificationTime); }
            set { ModificationTime = value.ToString(); }
        }

        [ReflectorProperty("author")]
        public string UserName { get; set; }

        [ReflectorProperty("rev")]
        public int ChangeNumber { get; set; }

        [ReflectorProperty("desc")]
        public string Comment { get; set; }

        [ReflectorProperty("email")]
        public string EmailAddress { get; set; }


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
