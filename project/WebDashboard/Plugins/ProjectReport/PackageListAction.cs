namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    using System.Collections;
    using System.Collections.Generic;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
    using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

    /// <summary>
    /// Display a list of all the generated packages.
    /// </summary>
    public class PackageListAction
        : ICruiseAction
    {
        #region Public constants
        /// <summary>
        /// The name of the action.
        /// </summary>
        public const string ActionName = "PackageList";
        #endregion

        #region Private fields
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly IFarmService farmService;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new <see cref="PackageListAction"/>.
        /// </summary>
        /// <param name="viewGenerator"></param>
        /// <param name="farmService"></param>
        public PackageListAction(IVelocityViewGenerator viewGenerator,
            IFarmService farmService)
		{
            this.viewGenerator = viewGenerator;
            this.farmService = farmService;
        }
        #endregion
        
        #region Public methods
        #region Execute()
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="cruiseRequest"></param>
        /// <returns></returns>
        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            var velocityContext = new Hashtable();
            velocityContext.Add("projectName", cruiseRequest.ProjectName);
            var packages = farmService.RetrievePackageList(cruiseRequest.ProjectSpecifier, cruiseRequest.RetrieveSessionToken());
            var packageList = new List<PackageDisplay>();
            foreach (var package in packages)
            {
                packageList.Add(
                    new PackageDisplay
                    {
                        Name = package.Name,
                        BuildLabel = package.BuildLabel,
                        NumberOfFiles = package.NumberOfFiles.ToString("#,##0"),
                        Size = FormatSize(package.Size),
                        FileName = package.FileName.Replace("\\", "\\\\")
                    });
            }
            velocityContext.Add("packages", packageList);
            return viewGenerator.GenerateView("PackageList.vm", velocityContext);
        }
        #endregion
        #endregion

        #region Private methods
        #region FormatSize()
        /// <summary>
        /// Formats a size.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private string FormatSize(long size)
        {
            double workingSize = size;
            if (size > 1048576)
            {
                workingSize = workingSize / 1048576;
                return string.Format("{0:0.00}Mb", workingSize);
            }
            else if (size > 1024)
            {
                workingSize = workingSize / 1024;
                return string.Format("{0:0.00}Kb", workingSize);
            }
            else
            {
                return string.Format("{0}b", workingSize);
            }
        }
        #endregion
        #endregion

        #region Public classes
        #region PackageDisplay
        /// <summary>
        /// Display details on a package.
        /// </summary>
        public class PackageDisplay
        {
            public string Name { get; set; }
            public string BuildLabel { get; set; }
            public string NumberOfFiles { get; set; }
            public string Size { get; set; }
            public string FileName { get; set; }
        }
        #endregion
        #endregion
    }
}
