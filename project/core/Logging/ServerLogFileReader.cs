using System.Configuration;
using System.IO;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Logging
{
    /// <summary>
    /// 	
    /// </summary>
    public class ServerLogFileReader
    {
        private const int DefaultMaxLines = 80;
        private string filename;
        private int maxLines;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerLogFileReader" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public ServerLogFileReader()
            : this(ReadFilenameFromConfig(), ReadMaxLinesFromConfig())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerLogFileReader" /> class.	
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="maxLines">The max lines.</param>
        /// <remarks></remarks>
        public ServerLogFileReader(string filename, int maxLines)
        {
            this.filename = filename;
            this.maxLines = maxLines;
        }

        /// <summary>
        /// Reads this instance.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Read()
        {
            return Read(EnumeratorDirection.Forward);
        }

        /// <summary>
        /// Reads the specified direction.	
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Read(EnumeratorDirection direction)
        {
            return Read(direction, null);
        }

        /// <summary>
        /// Reads the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Read(string project)
        {
            return Read(EnumeratorDirection.Forward, '[' + project);
        }

        private string Read(EnumeratorDirection direction, string match)
        {
            CircularArray buffer = new CircularArray(maxLines);
            using (Stream stream = OpenFile())
            {
                using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (match == null || line.IndexOf(match) >= 0)
                            buffer.Add(line);
                        // TODO: Messages can contain embedded newlines (e.g., exception reports).  
                        // TODO: This code should be changed to capture those "follow on" lines as well.
                    }
                }
            }
            return buffer.ToString(direction);
        }

        private Stream OpenFile()
        {
            return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private static string ReadFilenameFromConfig()
        {
            const string log4NetTempPathIndicator = @"${TMP}\";
            string filename = ConfigurationManager.AppSettings["ServerLogFilePath"];
            if (filename.StartsWith(log4NetTempPathIndicator))
            {
                filename = filename.Replace(log4NetTempPathIndicator, "");
                filename = System.IO.Path.Combine( System.IO.Path.GetTempPath(),filename);
            }
            return string.IsNullOrEmpty(filename) ? "ccnet.log" : filename;
        }

        private static int ReadMaxLinesFromConfig()
        {
            string linesToReadConfig = ConfigurationManager.AppSettings["ServerLogFileLines"];
            return (linesToReadConfig != null) ? int.Parse(ConfigurationManager.AppSettings["ServerLogFileLines"], CultureInfo.CurrentCulture) : DefaultMaxLines;
        }
    }
}