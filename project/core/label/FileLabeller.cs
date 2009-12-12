using System;
using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Label
{
    /// <summary>
    /// The File Labeller is used to generate labels based on the content of a disk file. The labeller is configured with the location of the
    /// file, and it reads the file content to generate the label for CCNet. The file is read just before the first pre-build task is executed.
    /// </summary>
    /// <title>File Labeller</title>
    /// <version>1.3</version>
    /// <example>
    /// <code>
    /// &lt;labeller type="fileLabeller"&gt;
    /// &lt;labelFilePath&gt;xxx&lt;/labelFilePath&gt;
    /// &lt;prefix&gt;Foo-&lt;/prefix&gt;
    /// &lt;allowDuplicateSubsequentLabels&gt;true&lt;/allowDuplicateSubsequentLabels&gt;
    /// &lt;/labeller&gt;
    /// </code>
    /// </example>
    [ReflectorType("fileLabeller")]
    public class FileLabeller 
        : LabellerBase
    {
        private readonly FileReader fileReader;
        private bool allowDuplicateSubsequentLabels = true;
        private string labelFilePath = string.Empty;
        private string prefix = string.Empty;

        /// <summary>
        /// Create a new FileLabeller with the default FileReader.
        /// </summary>
        public FileLabeller() : this(new FileReader())
        {
        }

        /// <summary>
        /// Create a new FileLabeller with a specified FileReader.
        /// </summary>
        /// <param name="fileReader">the Filereader.</param>
        public FileLabeller(FileReader fileReader)
        {
            this.fileReader = fileReader;
        }

        /// <summary>
        /// The pathname of the file to read. This can be the absolute path or one relative to the project's working directory. 
        /// </summary>
        /// <version>1.3</version>
        /// <default>n/a</default>
        [ReflectorProperty("labelFilePath", Required = true)]
        public string LabelFilePath
        {
            get { return labelFilePath; }
            set { labelFilePath = value; }
        }

        /// <summary>
        /// Any string to be put in front of all labels.
        /// </summary>
        /// <version>1.3</version>
        /// <default>None</default>
        [ReflectorProperty("prefix", Required = false)]
        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        /// <summary>
        /// Controls whether duplicate labels are permitted or not.  If true, duplicate labels are left
        /// intact.  If false, the label will be suffixed with "-n", where "n" is incremented for each
        /// successive duplication.  Defaults to "true"
        /// </summary>
        /// <version>1.3</version>
        /// <default>true</default>
        [ReflectorProperty("allowDuplicateSubsequentLabels", Required = false)]
        public bool AllowDuplicateSubsequentLabels
        {
            get { return allowDuplicateSubsequentLabels; }
            set { allowDuplicateSubsequentLabels = value; }
        }

        #region ILabeller Members
        /// <summary>
        /// Generate and return a label from the file content.
        /// </summary>
        /// <param name="integrationResult">the current integration result</param>
        /// <returns>the label</returns>
        public override string Generate(IIntegrationResult integrationResult)
        {
            string label = fileReader.GetLabel(integrationResult.BaseFromWorkingDirectory(labelFilePath));
            string suffix = GetSuffixBasedOn(label, integrationResult.LastIntegration.Label);
            return string.Format("{0}{1}{2}", prefix, label, suffix);
        }
        #endregion

        /// <summary>
        /// Generate a suffix to differentiate between two labels.
        /// </summary>
        /// <param name="currentLabel">The new label value</param>
        /// <param name="lastIntegrationLabel">The previous label value</param>
        /// <returns>The suffix string (including a leading "-"), or String.Empty if no suffix is necessary.
        /// </returns>
        /// <remarks>
        /// The two labels are considered to be the same (and thus requiring a suffix) if the currentLabel
        /// matches the lastIntegrationLabel after any prefix is removed from it.  Thus "banana" matches
        /// "banana-2".  The converse is not true - "banana-2" does not match "banana", because the suffix 
        /// is not stripped from currentLabel.
        /// If the lastIntegrationLabel does not contain a suffix, the generated suffix will be 1, 
        /// otherwise it will be the lastIntegrationLabel suffix + 1.
        /// </remarks>
        private string GetSuffixBasedOn(string currentLabel, string lastIntegrationLabel)
        {
            int lastLabelSuffix = 1;
            string[] splits = lastIntegrationLabel.Split('-');
            string labelWithoutSuffix = splits[0];
            if (!allowDuplicateSubsequentLabels && currentLabel != null && Equals(currentLabel, labelWithoutSuffix))
            {
                if (splits.Length > 1)
                {
                    lastLabelSuffix = Int32.Parse(splits[splits.Length - 1]) + 1;
                }
                return "-" + lastLabelSuffix;
            }
            return string.Empty;
        }

        #region Nested type: FileReader

        public class FileReader
        {
            /// <summary>
            /// Read the label text.
            /// </summary>
            /// <param name="labelFilePath">the file pathname</param>
            /// <returns>the label from the file</returns>
            /// <remarks>
            /// The label will have all leading and trailing whitespace removed.
            /// </remarks>
            public string GetLabel(string labelFilePath)
            {
                string label = ReadLabel(labelFilePath);
                // Convert all whitespace to blanks
                char[] nonBlankWhiteSpace = { '\r', '\n', '\v', '\f', '\t' };
                for (int i = 0; i < nonBlankWhiteSpace.Length; i++)
                {
                    label = label.Replace(nonBlankWhiteSpace[i], ' ');
                }
                // Remove leading and trailing blanks
                label = label.Trim();
                if (label == string.Empty)
                    throw new CruiseControlException("Label only contains whitespace.");
                return label;
            }

            /// <summary>
            /// Read the label text from the specified file.
            /// </summary>
            /// <param name="labelFilePath">the file pathname</param>
            /// <returns>the label from the file</returns>
            /// <remarks>
            /// The label consists of the entire contents of the file.
            /// </remarks>
            public virtual string ReadLabel(string labelFilePath)
            {
                string ver;
                try
                {
                    TextReader tr = new StreamReader(labelFilePath);
                    ver = tr.ReadToEnd();
                    tr.Close();
                }
                catch (Exception e)
                {
                    throw new CruiseControlException(
                        String.Format("Error reading file {0}: {1}", labelFilePath, e.Message),
                        e);
                }
                if (ver == string.Empty)
                    throw new CruiseControlException(
                        String.Format("File {0} only contains whitespace.", labelFilePath));
                return ver;
            }
        }

        #endregion
    }
}
