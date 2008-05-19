using System;
using System.Text;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    public class BuildProgressInformation
    {
        private string _listenerFile;
        private string _buildInformation;
        private DateTime _lastTimeQueried;
        private const Int32 buildStageCheckIntervalInSeconds = 5;

        public BuildProgressInformation(string artifactDirectory, string projectName)
        {
            this._listenerFile = System.IO.Path.Combine(artifactDirectory, StringUtil.RemoveInvalidCharactersFromFileName(projectName) + "_ListenFile.xml");
        }

        /// <summary>
        /// Returns the location of the listenerfile to be used by external programs
        /// </summary>
        public string ListenerFile
        {
            get { return this._listenerFile; }
        }


        /// <summary>
        /// Signals the start of a new task, so initialise all needed actions for monitoring this tasks progress
        /// </summary>
        /// <param name="information"></param>
        public void SignalStartRunTask(string information)
        {
            RemoveListenerFile();

            System.Text.StringBuilder ListenData = new StringBuilder();

            ListenData.AppendLine("<data>");
            ListenData.AppendLine(string.Format("<Item Time=\"{0}\" Data=\"{1}\" />",
                                    System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    CleanUpMessageForXMLLogging(information)));
            ListenData.AppendLine("</data>");

            this._buildInformation = ListenData.ToString();

            this._lastTimeQueried = DateTime.Now.AddYears(-10);
        }

        public string GetBuildProgressInformation()
        {
            if (DateTime.Now.AddSeconds(-buildStageCheckIntervalInSeconds) <= this._lastTimeQueried)
                return this._buildInformation;

            if (File.Exists(this._listenerFile))
            {
                try
                {
                    using (StreamReader FileReader = new StreamReader(this._listenerFile))
                    {
                        this._buildInformation = FileReader.ReadToEnd();
                    }
                }
                catch
                { }
            }

            this._lastTimeQueried = DateTime.Now;
            return this._buildInformation;
        }


        /// <summary>
        /// Deletes the listenerfile
        /// </summary>        
        private void RemoveListenerFile()
        {
            const int MaxAmountOfRetries = 10;
            int RetryCounter = 0;


            while (System.IO.File.Exists(this._listenerFile) && (RetryCounter <= MaxAmountOfRetries))
            {
                try
                {
                    System.IO.File.Delete(this._listenerFile);
                }
                catch (Exception e)
                {
                    RetryCounter += 1;
                    System.Threading.Thread.Sleep(200);

                    if (RetryCounter > MaxAmountOfRetries)
                        throw new CruiseControlException(
                            string.Format("Failed to delete {0} after {1} attempts", this._listenerFile, RetryCounter), e);
                }
            }
        }

        private string CleanUpMessageForXMLLogging(string msg)
        {
            return msg.Replace("\"", string.Empty);
        }

    }
}
