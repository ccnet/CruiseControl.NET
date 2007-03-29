using System;
using System.Collections;
using System.Reflection;

namespace Objection
{
	public class NMockAwareImplementationResolver : ImplementationResolver
	{
		private bool ignoreNMockImplementations = false;

		public Type ResolveImplementation(Type baseType)
		{
			ArrayList assemblyNames = new ArrayList();
			Type candidateType = null;
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				assemblyNames.Add(assembly.GetName().Name);
				foreach (Type type in assembly.GetTypes())
				{
					if (baseType.IsAssignableFrom(type) && baseType != type)
					{
						if (! IgnoreType(type))
						{
							if (candidateType != null)
							{
								throw new Exception(string.Format("Ambiguous type {0}, implemented by {1} and {2}", baseType.FullName, candidateType.FullName, type.FullName));	
							}
						
							candidateType = type;
						}
					}
				}
			}
			if (candidateType == null)
			{
				string message = "Unable to find implementation for " + baseType.FullName + ". Looked in assemblies: ";
				foreach (string assemblyName in assemblyNames)
				{
					message += assemblyName;
					message += " ";
				}
				throw new Exception(message);
			}
			return candidateType;
		}

		public bool IgnoreNMockImplementations
		{
			set { ignoreNMockImplementations = value; }
		}

		private bool IgnoreType(Type typeToCheck)
		{
			if (ignoreNMockImplementations)
			{
				return IsNMockProxyType(typeToCheck);	
			}
			return false;
		}

		// ToDo - In theory we should think about Full Names here in case someone is actually using the name 'Proxy' as part of their class or Namespace name
		private bool IsNMockProxyType(Type typeToCheck)
		{
			// NMock Types are of the format 'ProxyMyOriginalType_4' where the original type was 'MyOriginalType'
			string typeNameToCheck = typeToCheck.Name;

			if (typeNameToCheck.StartsWith("Proxy"))
			{
				int indexOfLastUnderscoreInName = findLastUnderscore(typeNameToCheck);
				if (indexOfLastUnderscoreInName > -1 && indexOfLastUnderscoreInName < (typeNameToCheck.Length - 1) )
				{
					try
					{
						int.Parse(typeNameToCheck.Substring(indexOfLastUnderscoreInName + 1));
						return true;
					}
					catch (FormatException) 
					{ 
						// Not a NMock type in this case
					}
				}
			}

			return false;
		}

		private int findLastUnderscore(string nameToCheck)
		{
			return findLastUnderscore(nameToCheck, 0);
		}

		private int findLastUnderscore(string nameToCheck, int startPosition)
		{
			int lastUnderscore = nameToCheck.IndexOf('_', startPosition);
			if (lastUnderscore != -1)
			{
				int nextUnderscore = findLastUnderscore(nameToCheck, lastUnderscore + 1);
				if (nextUnderscore > -1)
				{
					return nextUnderscore;
				}
			}
			return lastUnderscore;
		}
	}
}
