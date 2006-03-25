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

		public LazilyInitialisingVelocityTransformer(IPhysicalApplicationPathProvider physicalApplicationPathProvider)
		{
			this.physicalApplicationPathProvider = physicalApplicationPathProvider;
		}

		public string Transform(string transformerFileName, Hashtable transformable)
		{
			string output = "";
			using(TextWriter writer = new StringWriter())
			{
				try
				{
					VelocityEngine.MergeTemplate(transformerFileName, RuntimeConstants.ENCODING_DEFAULT, new VelocityContext(transformable), writer);
				}
				catch (Exception baseException)
				{
					throw new CruiseControlException(string.Format(@"Exception calling NVelocity for template: {0}
Template path is {1}", transformerFileName, Path.Combine(physicalApplicationPathProvider.PhysicalApplicationPath, "templates")), baseException);
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
						lazilyInitialisedEngine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, Path.Combine(physicalApplicationPathProvider.PhysicalApplicationPath, "templates"));
						lazilyInitialisedEngine.SetProperty(RuntimeConstants.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl");
						lazilyInitialisedEngine.Init();
					}
				}
				return lazilyInitialisedEngine;
			}
		}
	}
}
