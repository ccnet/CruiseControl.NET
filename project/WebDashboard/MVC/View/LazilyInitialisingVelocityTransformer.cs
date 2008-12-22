using System;
using System.Collections;
using System.IO;
using System.Text;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.View
{
    public class LazilyInitialisingVelocityTransformer : IVelocityTransformer
    {
        private readonly IPhysicalApplicationPathProvider physicalApplicationPathProvider;

        private VelocityEngine lazilyInitialisedEngine = null;
        private VelocityEngine lazilyCustomInitialisedEngine = null;
        private System.Collections.Generic.Dictionary<string, TemplateLocation> FoundTemplates = new System.Collections.Generic.Dictionary<string, TemplateLocation>();

        public enum TemplateLocation
        {
            Templates, CustomTemplates
        }



        public LazilyInitialisingVelocityTransformer(IPhysicalApplicationPathProvider physicalApplicationPathProvider)
        {
            this.physicalApplicationPathProvider = physicalApplicationPathProvider;
        }

        public string Transform(string transformerFileName, Hashtable transformable)
        {
            string output = "";
            using (TextWriter writer = new StringWriter())
            {
                try
                {

                    if (DetermineTemplateLocation(transformerFileName) == TemplateLocation.CustomTemplates )
                    {
                        VelocityEngineCustom.MergeTemplate(transformerFileName, RuntimeConstants.ENCODING_DEFAULT, new VelocityContext(transformable), writer);

                    }
                    else
                    {
                        VelocityEngine.MergeTemplate(transformerFileName, RuntimeConstants.ENCODING_DEFAULT, new VelocityContext(transformable), writer);
                    }
                }
                catch (Exception baseException)
                {
                    throw new CruiseControlException(string.Format(@"Exception calling NVelocity for template: {0}
Template path is {1}", transformerFileName, physicalApplicationPathProvider.GetFullPathFor("templates")), baseException);
                }
                output = writer.ToString();
            }
            return output;
        }

        private VelocityEngine VelocityEngine
        {
            get
            {
                lock (this)
                {
                    if (lazilyInitialisedEngine == null)
                    {
                        lazilyInitialisedEngine = new VelocityEngine();
                        lazilyInitialisedEngine.SetProperty(RuntimeConstants.RUNTIME_LOG_LOGSYSTEM_CLASS, "NVelocity.Runtime.Log.NullLogSystem");
                        lazilyInitialisedEngine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, physicalApplicationPathProvider.GetFullPathFor("templates"));
                        lazilyInitialisedEngine.SetProperty(RuntimeConstants.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl");
                        lazilyInitialisedEngine.Init();
                    }
                }
                return lazilyInitialisedEngine;
            }
        }



        private VelocityEngine VelocityEngineCustom
        {
            get
            {
                lock (this)
                {
                    if (lazilyCustomInitialisedEngine == null)
                    {
                        lazilyCustomInitialisedEngine = new VelocityEngine();
                        lazilyCustomInitialisedEngine.SetProperty(RuntimeConstants.RUNTIME_LOG_LOGSYSTEM_CLASS, "NVelocity.Runtime.Log.NullLogSystem");
                        lazilyCustomInitialisedEngine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, physicalApplicationPathProvider.GetFullPathFor("customtemplates"));
                        lazilyCustomInitialisedEngine.SetProperty(RuntimeConstants.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl");
                        lazilyCustomInitialisedEngine.Init();
                    }
                }
                return lazilyCustomInitialisedEngine;
            }
        }


        private TemplateLocation DetermineTemplateLocation(string transformerFileName)
        {
            if (!FoundTemplates.ContainsKey(transformerFileName))
            {
                string filelocation = System.IO.Path.Combine(physicalApplicationPathProvider.GetFullPathFor("customtemplates"),transformerFileName);
                if (System.IO.File.Exists(filelocation))
                {
                    FoundTemplates.Add(transformerFileName, TemplateLocation.CustomTemplates);
                }
                else
                {
                    FoundTemplates.Add(transformerFileName, TemplateLocation.Templates);
                }
            }

            return FoundTemplates[transformerFileName];
        }


    }
}
