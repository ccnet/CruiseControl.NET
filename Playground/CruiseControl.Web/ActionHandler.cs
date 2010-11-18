namespace CruiseControl.Web
{
    using System.Web.Mvc;

    /// <summary>
    /// Handles an action.
    /// </summary>
    public abstract class ActionHandler
    {
        #region Public methods
        #region Generate()
        /// <summary>
        /// Generates the result from this handler.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The result.</returns>
        public abstract ActionResult Generate(ActionRequestContext context);
        #endregion
        #endregion
    }
}