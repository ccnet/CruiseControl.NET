using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	public class NUnitArgument
	{
		public readonly string[] assemblies;
		private readonly string outputfile;

		public NUnitArgument(string[] assemblies, string outputfile)
		{
			if (assemblies == null || assemblies.Length == 0)
			{
				throw new CruiseControlException("No unit test assemblies are specified. Please use the <assemblies> element to specify the test assemblies to run.");
			}
			this.assemblies = assemblies;
			this.outputfile = outputfile;
		}

		public override string ToString()
		{
			StringBuilder argsBuilder = new StringBuilder();
			argsBuilder.AppendFormat(@" /xml={0} ", outputfile);
			argsBuilder.Append(" /nologo ");

			foreach (string assemblyName in assemblies)
			{
				argsBuilder.AppendFormat(" {0} ", assemblyName);
			}
			return argsBuilder.ToString();
		}
	}
}