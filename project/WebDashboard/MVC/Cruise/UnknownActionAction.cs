namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
    public class UnknownActionAction : IAction
    {
        public IResponse Execute(IRequest request)
        {
            string actionName = request.FileNameWithoutExtension;
            if (actionName == string.Empty)
            {
                return new HtmlFragmentResponse("Internal Error - 'UnknownActionAction' called but there is no action is request!");
            }
            else
            {
                return new HtmlFragmentResponse("Unknown action requested - " + actionName);
            }
        }
    }
}