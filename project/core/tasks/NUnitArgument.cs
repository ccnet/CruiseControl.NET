using System;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	public class NUnitArgument 
	{
		private string[] _assemblies = new string[0];

		public NUnitArgument (string[] assemblies)
		{
			_assemblies = assemblies;
		}

	  public string[] Assemblies
		{
			get
			{
				return _assemblies;
			}
		}

	  public override string ToString ()
	  {
		  if( _assemblies == null || _assemblies.Length == 0)
		  {
			  return String.Empty;
		  }
		
		  StringBuilder argsBuilder=new StringBuilder();
		  argsBuilder.Append(" /xmlConsole ");
		  argsBuilder.Append(" /nologo ");
		
		  foreach(string assemblyName in Assemblies)
		  {
		      argsBuilder.AppendFormat(" {0} ",assemblyName);			    
		  }
		  return argsBuilder.ToString();
	  }
	}
}
						   