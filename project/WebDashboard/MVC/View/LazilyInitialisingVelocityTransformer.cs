using System.Collections;
using System.IO;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.View
{
	public class LazilyInitialisingVelocityTransformer : IVelocityTransformer
	{
		private readonly IPathMapper pathMapper;

		private VelocityEngine lazilyInitialisedEngine = null;

		public LazilyInitialisingVelocityTransformer(IPathMapper pathMapper)
		{
			this.pathMapper = pathMapper;
		}

		public string Transform(string transformerFileName, Hashtable transformable)
		{
			string output = "";
			using(TextWriter writer = new StringWriter())
			{
				VelocityEngine.MergeTemplate(transformerFileName, new VelocityContext(transformable), writer);
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
						lazilyInitialisedEngine.SetProperty(RuntimeConstants_Fields.RUNTIME_LOG_LOGSYSTEM_CLASS, "NVelocity.Runtime.Log.NullLogSystem");
						lazilyInitialisedEngine.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, Path.Combine(pathMapper.PhysicalApplicationPath, "templates"));
						lazilyInitialisedEngine.SetProperty(RuntimeConstants_Fields.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl");
						lazilyInitialisedEngine.Init();
					}
				}
				return lazilyInitialisedEngine;
			}
		}
	}
}
