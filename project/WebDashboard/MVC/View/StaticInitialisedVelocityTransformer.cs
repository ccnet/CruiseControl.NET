using System.Collections;
using System.IO;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.View
{
	public class StaticInitialisedVelocityTransformer : IVelocityTransformer
	{
		private readonly IPathMapper pathMapper;

		public StaticInitialisedVelocityTransformer(IPathMapper pathMapper)
		{
			this.pathMapper = pathMapper;
		}

		public string Transform(string transformerFileName, Hashtable transformable)
		{
			InitialiseVelocityEngine();
			string output = "";
			using(TextWriter writer = new StringWriter())
			{
				Velocity.MergeTemplate(transformerFileName, new VelocityContext(transformable), writer);
				output = writer.ToString();
			}
			return output;
		}

		// The nasty staticsies stole our nice TDD, yes, precious...
		private static bool VelocityInitialised = false;

		private void InitialiseVelocityEngine()
		{
			lock(this.GetType())
			{
				if (!VelocityInitialised)
				{
					VelocityInitialised = true;
					Velocity.SetProperty(RuntimeConstants_Fields.RUNTIME_LOG_LOGSYSTEM_CLASS, "NVelocity.Runtime.Log.NullLogSystem");
					Velocity.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, Path.Combine(pathMapper.PhysicalApplicationPath, "templates"));
					Velocity.SetProperty(RuntimeConstants_Fields.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl");
					Velocity.Init();
				}
			}
		}
	}
}
