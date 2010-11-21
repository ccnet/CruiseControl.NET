using System;
using Exortech.NetReflector;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
    /// <summary>
    /// 	
    /// </summary>
    [ReflectorType("modification")]
    public class MercurialModification
    {
        /// <summary>
        /// Gets or sets the version.	
        /// </summary>
        /// <value>The version.</value>
        /// <remarks></remarks>
        [ReflectorProperty("node")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the modification time.	
        /// </summary>
        /// <value>The modification time.</value>
        /// <remarks></remarks>
        [ReflectorProperty("date")]
        public string ModificationTime { get; set; }

        /// <summary>
        /// Gets the type.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Type
        {
            get { return "Changeset"; }
        }

        /// <summary>
        /// Gets or sets the name of the file.	
        /// </summary>
        /// <value>The name of the file.</value>
        /// <remarks></remarks>
        [ReflectorProperty("files")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the modified time.	
        /// </summary>
        /// <value>The modified time.</value>
        /// <remarks></remarks>
        public DateTime ModifiedTime
        {
            get { return DateTime.Parse(ModificationTime, CultureInfo.CurrentCulture); }
            set { ModificationTime = value.ToString(CultureInfo.CurrentCulture); }
        }

        /// <summary>
        /// Gets or sets the name of the user.	
        /// </summary>
        /// <value>The name of the user.</value>
        /// <remarks></remarks>
        [ReflectorProperty("author")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the change number.	
        /// </summary>
        /// <value>The change number.</value>
        /// <remarks></remarks>
        [ReflectorProperty("rev")]
        public int ChangeNumber { get; set; }

        /// <summary>
        /// Gets or sets the comment.	
        /// </summary>
        /// <value>The comment.</value>
        /// <remarks></remarks>
        [ReflectorProperty("desc")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the email address.	
        /// </summary>
        /// <value>The email address.</value>
        /// <remarks></remarks>
        [ReflectorProperty("email")]
        public string EmailAddress { get; set; }


        /// <summary>
        /// Implements the operator Modification.	
        /// </summary>
        /// <param name="hg">The hg.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
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
