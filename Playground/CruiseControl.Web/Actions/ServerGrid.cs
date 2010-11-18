namespace CruiseControl.Web.Actions
{
    using System.ComponentModel.Composition;
    using System.Web.Mvc;
    using NLog;

    /// <summary>
    /// Displays the projects for a server.
    /// </summary>
    [Export(typeof(ActionHandler))]
    [ActionHandler("serverGrid", ActionHandlerTargets.Root | ActionHandlerTargets.Server)]
    public class ServerGrid
        : ActionHandler
    {
        #region Private fields
        private static readonly Logger sLogger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Public methods
        #region Generate()
        /// <summary>
        /// Generates the result from this handler.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The result.</returns>
        public override ActionResult Generate(ActionRequestContext context)
        {
            throw new System.NotImplementedException();
        }
        #endregion
        #endregion
    }
}