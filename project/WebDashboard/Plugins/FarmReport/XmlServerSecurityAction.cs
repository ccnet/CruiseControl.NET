using System.IO;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport
{
    public class XmlServerSecurityAction : IAction
    {
        public const string ACTION_NAME = "XmlSecurity";

        private readonly IFarmService farmService;

        public XmlServerSecurityAction(IFarmService farmService)
        {
            this.farmService = farmService;
        }

        public IResponse Execute(IRequest request)
        {
            string action = request.GetText("action");
            string result = null;

            switch (action.ToLower())
            {
                case "login":
                    result = PerformLogin(request);
                    break;
                case "logout":
                    result = PerformLogout(request);
                    break;
                default:
                    result = GenerateResult("failure", "Unknown action");
                    break;
            }
            return new XmlFragmentResponse(result);
        }

        private string PerformLogin(IRequest request)
        {
            try
            {
                LoginRequest credentials = Deserialise(request.GetText("credentials"));
                string sessionToken = farmService.Login(request.GetText("server"), credentials);
                return GenerateResult("success", string.Format(System.Globalization.CultureInfo.CurrentCulture,"<session>{0}</session>", sessionToken));
            }
            catch
            {
                return GenerateResult("failure", "Login failure");
            }
        }

        private LoginRequest Deserialise(string data)
        {
            XmlSerializer serialiser = new XmlSerializer(typeof(LoginRequest));
            StringReader reader = new StringReader(data);
            LoginRequest request = serialiser.Deserialize(reader) as LoginRequest;
            return request;
        }

        private string PerformLogout(IRequest request)
        {
            try
            {
                string sessionToken = request.GetText("sessionToken");
                farmService.Logout(request.GetText("server"), sessionToken);
                return GenerateResult("success", string.Empty);
            }
            catch
            {
                return GenerateResult("failure", "Login failure");
            }
        }

        private string GenerateResult(string outcome, string contents)
        {
            string result = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<security result=\"{0}\">{1}</security>", outcome, contents);
            return result;
        }
    }
}
