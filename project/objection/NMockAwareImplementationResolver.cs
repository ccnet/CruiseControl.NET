using System;
using System.Collections;
using System.Reflection;

namespace Objection
{
	public class LoadedTypeList
	{
		private readonly ArrayList namesOfCachedAssemblies = new ArrayList();
		private ArrayList types = null;

		public ArrayList GetTypes()
		{
			if (types == null)
			{
				types = GetTypeListForNewLoadedAssemblies();
			}
			return types;
		}
		
		public ArrayList GetNewTypes()
		{
			ArrayList newTypes = GetTypeListForNewLoadedAssemblies();
			types.AddRange(newTypes);
			return newTypes;
		}
		
		public ArrayList CheckedAssemblies()
		{
			return namesOfCachedAssemblies;
		}
		
		private ArrayList GetTypeListForNewLoadedAssemblies()
		{
			ArrayList newTypes = new ArrayList();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				string assemblyName = assembly.GetName().Name;				
				if (!namesOfCachedAssemblies.Contains(assemblyName))
				{
					namesOfCachedAssemblies.Add(assemblyName);
					foreach (Type type in assembly.GetTypes())
					{
						newTypes.Add(type);
					}
				}
			}
			return newTypes;
		}		
	}
	
	public class NMockAwareImplementationResolver : ImplementationResolver
	{
		private bool ignoreNMockImplementations = false;
		private readonly LoadedTypeList loadedTypesList = new LoadedTypeList();

		public Type ResolveImplementation(Type baseType)
		{
			Type candidateType = FindTypeAssignableToBaseType(baseType, loadedTypesList.GetTypes());
			
			if (candidateType == null)
			{
				candidateType = FindTypeAssignableToBaseType(baseType, loadedTypesList.GetNewTypes());
			}
			
			if (candidateType == null)
			{
				ThrowExceptionForUnfoundImplementation(baseType);
			}
			return candidateType;
		}

		private void ThrowExceptionForUnfoundImplementation(Type baseType)
		{
			string message = "Unable to find implementation for " + baseType.FullName + ". Looked in assemblies: ";
			foreach (string assemblyName in loadedTypesList.CheckedAssemblies())
			{
				message += assemblyName;
				message += " ";
			}
			throw new Exception(message);
		}

		private Type FindTypeAssignableToBaseType(Type baseType, ArrayList types)
		{
			Type candidateType = null;
			foreach (Type type in types)
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
				int indexOfLastUnderscoreInName = FindLastUnderscore(typeNameToCheck);
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

		private int FindLastUnderscore(string nameToCheck)
		{
			return nameToCheck.LastIndexOf('_');
		}
	}
}
