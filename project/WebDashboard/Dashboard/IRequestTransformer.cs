using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IRequestTransformer
	{
		Control Transform(ICruiseRequest cruiseRequest, params string[] transformerFileNames);
	}
}
