using System;
using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Label
{
    /// <summary>
    /// A file labeller which works on the concept of your build process generating label
    /// stored in a file. The labeller is configured with the location of the file, and it
    /// reads the file content to generate the label for CCNet.
    /// </summary>
    [ReflectorType("fileLabeller")]
    public class FileLabeller : ILabeller
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
        /// The pathname of the file containing the label.  If the pathname is relative, it will be
        /// interpreted relative to the proejct working directory.
        /// </summary>
        [ReflectorProperty("labelFilePath", Required = true)]
        public string LabelFilePath
        {
            get { return labelFilePath; }
            set { labelFilePath = value; }
        }

        /// <summary>
        /// A prefix to be added to the label read from the file.  Defaults to "".
        /// </summary>
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
        public string Generate(IIntegrationResult integrationResult)
        {
            string label = fileReader.GetLabel(integrationResult.BaseFromWorkingDirectory(labelFilePath));
            string suffix = GetSuffixBasedOn(label, integrationResult.LastIntegration.Label);
            return string.Format("{0}{1}{2}", prefix, label, suffix);
        }

        /// <summary>
        /// Generate a label from the file content and save it in the integration result.
        /// </summary>
        /// <param name="result">the current integration result</param>
        public void Run(IIntegrationResult result)
        {
            result.Label = Generate(result);
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
                if (label == "")
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
                if (ver == "")
                    throw new CruiseControlException(
                        String.Format("File {0} only contains whitespace.", labelFilePath));
                return ver;
            }
        }

        #endregion
    }
}
