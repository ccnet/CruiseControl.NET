
using System;
using System.Text;
using System.IO;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
    public class BuildProgressInformation
    {
        private string _listenerFile;
        private string _buildInformation = string.Empty;
        private DateTime _lastTimeQueried;
        private const Int32 buildStageCheckIntervalInSeconds = 5;
        private readonly object lockObject = new object();
        private System.Collections.Generic.List<BuildProgressInformationData> Progress;
        private const Int32 MaxItemsInQueue = 10;


        /// <summary>
        /// Initializes a new instance of the <see cref="BuildProgressInformation" /> class.	
        /// </summary>
        /// <param name="artifactDirectory">The artifact directory.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <remarks></remarks>
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
        public virtual void SignalStartRunTask(string information)
        {
            lock (lockObject)
            {
                RemoveListenerFile();

                Progress = new System.Collections.Generic.List<BuildProgressInformationData>();
                AddToInternalQueue(information);

                this._buildInformation = GetQueueDataAsXml();
                this._lastTimeQueried = DateTime.Now.AddYears(-10);

            }
        }


        /// <summary>
        /// Adds the task information.	
        /// </summary>
        /// <param name="information">The information.</param>
        /// <remarks></remarks>
        public virtual void AddTaskInformation(string information)
        {
            lock (lockObject)
            {
                AddToInternalQueue(information);
            }
        }


        /// <summary>
        /// Gets the build progress information.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual string GetBuildProgressInformation()
        {
            lock (lockObject)
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
            else
            {
                this._buildInformation = GetQueueDataAsXml();
            }

            this._lastTimeQueried = DateTime.Now;
            return this._buildInformation;
            }
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
                            string.Format(System.Globalization.CultureInfo.CurrentCulture,"Failed to delete {0} after {1} attempts", this._listenerFile, RetryCounter), e);
                }
            }
        }
      
        private void AddToInternalQueue(string info)
        {
            Progress.Add(new BuildProgressInformationData(info));

            if (Progress.Count > MaxItemsInQueue) Progress.RemoveAt(1); // keep the first 1 because this contains the taks name (signal start run)

        }

        private string GetQueueDataAsXml()
        {
            System.Text.StringBuilder ListenData = new StringBuilder();

            ListenData.AppendLine("<data>");
            
            foreach( BuildProgressInformationData bpi in Progress)
            {
                ListenData.AppendLine(
                    string.Format(System.Globalization.CultureInfo.CurrentCulture,"<Item Time=\"{0}\" Data=\"{1}\" />", bpi.At ?? string.Empty, bpi.Information ?? string.Empty));
            }

            ListenData.AppendLine("</data>");

            return ListenData.ToString();
        }


    }

    struct BuildProgressInformationData
    {
        private DateTime at;
        private string information;

        public string At
        {
            get { return at.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture); }
        }

        public string Information
        {
            get { return CleanUpMessageForXMLLogging(information); }
        }

        public BuildProgressInformationData(string info)
        {
            at = DateTime.Now;
            information = info;
        }

        private string CleanUpMessageForXMLLogging(string msg)
        {
            return msg.Replace("\"", string.Empty);
        }
    }


}



