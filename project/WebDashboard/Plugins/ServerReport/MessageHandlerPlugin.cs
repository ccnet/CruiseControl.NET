using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
    /// <summary>
    /// A pass-through plug-in that hands any message-type requests to the cruise server for processing.
    /// </summary>
    public class MessageHandlerPlugin
        : ICruiseAction, IPlugin
    {
        #region Public constants
        public const string ActionName = "RawXmlMessage";
        #endregion

        #region Private fields
        private readonly IFarmService farmService;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="MessageHandlerPlugin"/>.
        /// </summary>
        /// <param name="farmService">The <see cref="IFarmService"/> to use in the plug-in.</param>
        public MessageHandlerPlugin(IFarmService farmService)
        {
            this.farmService = farmService;
        }
        #endregion

        #region Public properties
        #region NamedActions
        /// <summary>
        /// The actions that are available from this plug-in.
        /// </summary>
        public INamedAction[] NamedActions
        {
            get
            {
                return new INamedAction[] { 
                    new ImmutableNamedActionWithoutSiteTemplate(ActionName, this) 
                };
            }
        }
        #endregion

        #region LinkDescription
        /// <summary>
        /// The description for any links to this plug-in.
        /// </summary>
        public string LinkDescription
        {
            get { return "Process XML message"; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Execute()
        /// <summary>
        /// Processes an incoming request.
        /// </summary>
        /// <param name="cruiseRequest">The request to process.</param>
        /// <returns>An XML fragment containing the response from the server.</returns>
        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            string action = cruiseRequest.Request.GetText("action");
            string message = cruiseRequest.Request.GetText("message");
            string response = farmService.ProcessMessage(cruiseRequest.ServerSpecifier,
                action,
                message);
            return new XmlFragmentResponse(response);
        }
        #endregion
        #endregion
    }
}
