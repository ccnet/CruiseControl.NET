using System.Collections;
using System.IO;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class StaticInitialisedVelocityTransformer : IHashtableTransformer
	{
		private readonly IPathMapper pathMapper;

		public StaticInitialisedVelocityTransformer(IPathMapper pathMapper)
		{
			this.pathMapper = pathMapper;
		}

		public string Transform(Hashtable transformable, string transformerFileName)
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
					Velocity.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, pathMapper.PhysicalApplicationPath);
					Velocity.SetProperty(RuntimeConstants_Fields.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl");
					Velocity.Init();
				}
			}
		}
	}
}
