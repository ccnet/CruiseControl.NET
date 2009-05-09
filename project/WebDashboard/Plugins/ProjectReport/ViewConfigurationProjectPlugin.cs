using System.IO;
using System.Web;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    [ReflectorType("viewConfigurationProjectPlugin")]
    public class ViewConfigurationProjectPlugin : ICruiseAction, IPlugin
    {
        private readonly ICruiseManagerWrapper cruiseManager;

        private bool hidePasswords = true;

        [ReflectorProperty("hidePasswords", Required = false)]
        public bool HidePasswords
        {
            get { return hidePasswords; }
            set { hidePasswords = value; }
        }

        public ViewConfigurationProjectPlugin(ICruiseManagerWrapper cruiseManager)
        {
            this.cruiseManager = cruiseManager;
        }

        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            IProjectSpecifier projectSpecifier = cruiseRequest.ProjectSpecifier;
            string projectXml = cruiseManager.GetProject(projectSpecifier, cruiseRequest.RetrieveSessionToken());
            return new HtmlFragmentResponse("<pre>" + HttpUtility.HtmlEncode(FormatXml(projectXml)) + "</pre>");
        }

        private string FormatXml(string projectXml)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(projectXml);
            StringWriter buffer = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(buffer);
            writer.Formatting = Formatting.Indented;
            document.WriteTo(writer);

            string Result;
            if (hidePasswords)
            {
                Result = SecureProjectView(buffer.ToString());
            }
            else
            {
                Result = buffer.ToString();
            }


            return Result;
        }

        public string LinkDescription
        {
            get { return "Project Configuration"; }
        }

        public INamedAction[] NamedActions
        {
            get { return new INamedAction[] { new ImmutableNamedAction("ViewProjectConfiguration", this) }; }
        }


        private string SecureProjectView(string project)
        {
            const string PasswordReplacement = "*****";

            System.IO.StringReader projectReader = new StringReader(project);
            string projectLine = projectReader.ReadLine();

            System.IO.StringWriter result = new StringWriter();
            string replacedPassword="";

            int startPos;
            int endPos;


            while (!(projectLine == null))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(projectLine,"password", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    endPos = projectLine.IndexOf("</");

                    if (endPos > 0)
                    {
                        //structure : <password>somevalue</password>

                        startPos = projectLine.IndexOf(">");
                        replacedPassword = projectLine.Substring(0,startPos+1);
                        replacedPassword += PasswordReplacement;
                        replacedPassword += projectLine.Substring(endPos);
                    }
                    else
                    {
                        //structure : <password />
                        replacedPassword = projectLine.Replace(" /","");
                        string temp = replacedPassword.Trim();

                        replacedPassword += PasswordReplacement;
                        replacedPassword += temp.Insert(1,"/");
                    }

                    result.WriteLine(replacedPassword);
                }
                else
                {
                    result.WriteLine(projectLine);
                }

                projectLine = projectReader.ReadLine();
            }

            return result.ToString();
        }


    }
}
