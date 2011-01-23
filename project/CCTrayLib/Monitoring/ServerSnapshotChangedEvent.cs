using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
    /// <summary>
    /// The arguments for when a server snapshot has changed.
    /// </summary>
    public class ServerSnapshotChangedEventArgs
    {
        #region Private fields
        private List<string> addedProjects = new List<string>();
        private List<string> deletedProjects = new List<string>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises the args.
        /// </summary>
        /// <param name="addedProjects"></param>
        /// <param name="deletedProjects"></param>
        public ServerSnapshotChangedEventArgs(string server,
            BuildServer configuration,
            IEnumerable<string> addedProjects,
            IEnumerable<string> deletedProjects)
        {
            this.Server = server;
            this.Configuration = configuration;
            this.addedProjects.AddRange(addedProjects ?? new string[0]);
            this.deletedProjects.AddRange(deletedProjects ?? new string[0]);
        }
        #endregion
        
        #region Public properties
        #region Server
        /// <summary>
        /// The server that has changed.
        /// </summary>
        public string Server { get; private set; }
        #endregion

        #region Configuration
        /// <summary>
        /// The server configuration.
        /// </summary>
        public BuildServer Configuration { get; private set; }
        #endregion

        #region AddedProjects
        /// <summary>
        /// The projects that have been added to the server.
        /// </summary>
        public IList<string> AddedProjects
        {
            get { return addedProjects; }
        }
        #endregion

        #region DeletedProjects
        /// <summary>
        /// The projects that have been deleted from the server.
        /// </summary>
        public IList<string> DeletedProjects
        {
            get { return deletedProjects; }
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// The delegate for handling a server snapshot changed event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ServerSnapshotChangedEventHandler(object sender, ServerSnapshotChangedEventArgs args);
}
