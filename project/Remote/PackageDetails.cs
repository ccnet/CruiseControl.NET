namespace ThoughtWorks.CruiseControl.Remote
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Details on a package.
    /// </summary>
    [XmlRoot("package")]
    [Serializable]
    public class PackageDetails
    {
        #region Private fields
        private string name;
        private string buildLabel;
        private DateTime dateTime;
        private int numberOfFiles;
        private long size;
        private string fileName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new blank <see cref="PackageDetails"/>.
        /// </summary>
        public PackageDetails()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="PackageDetails"/> with a package.
        /// </summary>
        /// <param name="package">The location of the package.</param>
        public PackageDetails(string package)
        {
            this.fileName = package;
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// The name of the package.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

        #region BuildLabel
        /// <summary>
        /// The label of the build this package is for.
        /// </summary>
        [XmlElement("buildLabel")]
        public string BuildLabel
        {
            get { return buildLabel; }
            set { buildLabel = value; }
        }
        #endregion

        #region DateTime
        /// <summary>
        /// The date and time the package was generated.
        /// </summary>
        [XmlAttribute("dateTime")]
        public DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }
        #endregion

        #region NumberOfFiles
        /// <summary>
        /// The number of files in the package.
        /// </summary>
        [XmlAttribute("numberOfFiles")]
        public int NumberOfFiles
        {
            get { return numberOfFiles; }
            set { numberOfFiles = value; }
        }
        #endregion

        #region Size
        /// <summary>
        /// The size of the package.
        /// </summary>
        [XmlAttribute("size")]
        public long Size
        {
            get { return size; }
            set { size = value; }
        }
        #endregion

        #region FileName
        /// <summary>
        /// The actual name of the file on the server.
        /// </summary>
        [XmlElement("fileName")]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        #endregion
        #endregion
    }
}
