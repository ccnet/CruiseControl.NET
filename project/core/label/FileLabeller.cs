using System;
using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Label
{
    [ReflectorType("fileLabeller")]
    public class FileLabeller : ILabeller
    {
        private readonly FileReader fileReader;
        private bool allowDuplicateSubsequentLabels = true;
        private string labelFilePath = string.Empty;
        private string prefix = string.Empty;

        public FileLabeller() : this(new FileReader())
        {
        }

        public FileLabeller(FileReader fileReader)
        {
            this.fileReader = fileReader;
        }

        [ReflectorProperty("labelFilePath", Required = true)]
        public string LabelFilePath
        {
            get { return labelFilePath; }
            set { labelFilePath = value; }
        }

        [ReflectorProperty("prefix", Required = false)]
        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        [ReflectorProperty("allowDuplicateSubsequentLabels", Required = false)]
        public bool AllowDuplicateSubsequentLabels
        {
            get { return allowDuplicateSubsequentLabels; }
            set { allowDuplicateSubsequentLabels = value; }
        }

        #region ILabeller Members

        public string Generate(IIntegrationResult integrationResult)
        {
            string label = fileReader.GetLabel(integrationResult.BaseFromWorkingDirectory(labelFilePath));
            string suffix = GetSuffixBasedOn(label, integrationResult.LastIntegration.Label);
            return string.Format("{0}{1}{2}", prefix, label, suffix);
        }

        public void Run(IIntegrationResult result)
        {
            result.Label = Generate(result);
        }

        #endregion

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
            public virtual string GetLabel(string labelFilePath)
            {
                TextReader tr = new StreamReader(labelFilePath);
                string ver = tr.ReadLine();
                tr.Close();
                return ver;
            }
        }

        #endregion
    }
}