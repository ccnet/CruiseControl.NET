using System;
using System.Collections;
using System.Reflection;

namespace ObjectWizard
{
	public class ManagableObjectGiver : ObjectGiver, ObjectGiverManager
	{
		private Hashtable typedInstances;
		private Hashtable implementationTypes;
		private Hashtable dependencyImplementationTypes;
		private Hashtable implementationMappings;
		private bool ignoreNMockImplementations;
		private Hashtable dependencyImplementationTypesForIdentifers;
		private Hashtable instanceMappings;

		public ManagableObjectGiver()
		{
			typedInstances = new Hashtable();
			implementationTypes = new Hashtable();
			dependencyImplementationTypes = new Hashtable();
			implementationMappings = new Hashtable();
			ignoreNMockImplementations = false;
			dependencyImplementationTypesForIdentifers = new Hashtable();
			instanceMappings = new Hashtable();
		}

		public void AddTypedInstance(Type type, object instance)
		{
			typedInstances[type] = instance;
		}

		public void AddInstances(params object[] objects)
		{
			foreach (object o in objects)
			{
				AddTypedInstance(o.GetType(), o);
			}
		}

		public void SetImplementationType(Type parentType, Type implementationType)
		{
			implementationTypes[parentType] = implementationType;
		}

		public TypeDecoratorable CreateImplementationMapping(string identifier, Type type)
		{
			implementationMappings[identifier] = new DecoratedType(type);
			return (TypeDecoratorable) implementationMappings[identifier];
		}

		public TypeDecoratorable CreateInstanceMapping(string identifier, object instance)
		{
			instanceMappings[identifier] = new DecoratedInstance(instance);
			return (TypeDecoratorable) instanceMappings[identifier];
		}

		public void SetDependencyImplementationForIdentifer(string identifier, Type dependencyType, Type implementationType)
		{
			if (dependencyImplementationTypesForIdentifers[identifier] == null)
				dependencyImplementationTypesForIdentifers[identifier] = new Hashtable();

			((Hashtable) dependencyImplementationTypesForIdentifers[identifier])[dependencyType] = implementationType;
		}

		public void SetDependencyImplementationForType(Type typeWithDependency, Type dependencyParentType, Type dependencyImplementationType)
		{
			if (dependencyImplementationTypes[typeWithDependency] == null)
				dependencyImplementationTypes[typeWithDependency] = new Hashtable();

			((Hashtable) dependencyImplementationTypes[typeWithDependency])[dependencyParentType] = dependencyImplementationType;
		}

		public object GiveObjectByType(Type type)
		{
			return GiveObjectByType(type, null);
		}

		public object GiveObjectById(string id)
		{
			object instance = instanceMappings[id];
			if (instance == null)
			{
				DecoratedType decoratedType = (DecoratedType) implementationMappings[id];
			
				if (decoratedType == null)
					throw new ApplicationException("Unknown object key : " + id);
				else
					return Instantiate(decoratedType, id);
			}
			else
			{
				DecoratedInstance decoratedInstance = (DecoratedInstance) instance;
				if (decoratedInstance.Decorator != null)
				{
					return Instantiate(decoratedInstance.Decorator, id, decoratedInstance.Instance);
				}
				else
				{
					return decoratedInstance.Instance;
				}
			}
		}

		private object GiveObjectByType(Type type, string id)
		{
			object result = typedInstances[type];
			if (result == null)
			{
				result = Instantiate(type, id);
			}
			return result;
		}

		private object Instantiate(DecoratedType decoratedType, string id, params object[] args)
		{
			object baseObject = Instantiate(decoratedType.Type, id, args);

			DecoratedType decorator = decoratedType.Decorator;
			if (decorator != null)
				return Instantiate(decorator, id, baseObject);
			else
				return baseObject;
		}

		private object Instantiate(Type type, string identifier, params object[] args)
		{
			if (implementationTypes[type] != null)
			{
				return Instantiate((Type) implementationTypes[type], identifier, args);
			}
			if (type.IsInterface)
			{
				ResolveImplementation(type);
				return Instantiate((Type) implementationTypes[type], identifier, args);
			}

			ArrayList arguments = new ArrayList();
			ConstructorInfo constructor = GetConstructor(type);

			if (constructor == null)
			{
				throw new Exception("Unable to select constructor for Type " + type.FullName);
			}

			int paramCount = 0;
			foreach (ParameterInfo parameter in constructor.GetParameters())
			{
				if (paramCount < args.Length)
				{
					arguments.Add(args[paramCount]);
				}
				else
				{
					try
					{
						Type dependencyType = OverrideWithSpecifiedDependencyTypeIfNecessary(type, parameter.ParameterType, identifier);
						arguments.Add(GiveObjectByType(dependencyType, identifier));
					}
					catch (Exception e)
					{
						throw new Exception("Failed to instantiate Type " + type.FullName, e);
					}
				}
				paramCount++;
			}

			try
			{
				return constructor.Invoke((object[]) arguments.ToArray(typeof (object)));	
			}
			catch (Exception e)
			{
				string message = "Unable to instante " + type.FullName + " using the following args: ";
				foreach (object arg in arguments)
				{
					message += arg.ToString();
					message += ", ";
				}
				throw new Exception(message, e);
			}			
		}

		private Type OverrideWithSpecifiedDependencyTypeIfNecessary(Type typeBeingInstantiated, Type staticDependencyType, string identifier)
		{
			Hashtable dependencyTypes;
			if (identifier != null)
			{
				dependencyTypes = (Hashtable) dependencyImplementationTypesForIdentifers[identifier];
				if (dependencyTypes != null && dependencyTypes[staticDependencyType] != null)
				{
					return (Type) dependencyTypes[staticDependencyType];
				}			
			}
			dependencyTypes = (Hashtable) dependencyImplementationTypes[typeBeingInstantiated];
			if (dependencyTypes != null && dependencyTypes[staticDependencyType] != null)
			{
				return (Type) dependencyTypes[staticDependencyType];
			}
			return staticDependencyType;
		}

		private ConstructorInfo GetConstructor(Type type)
		{
			ConstructorInfo candidate = null;
			foreach (ConstructorInfo ctor in type.GetConstructors())
			{
				if (candidate == null)
				{
					candidate = ctor;
				}
				else
				{
					if (ctor.GetParameters().Length > candidate.GetParameters().Length)
					{
						candidate = ctor;
					}
				}
			}			
			return candidate;
		}

		private void ResolveImplementation(Type baseType)
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
			implementationTypes[baseType] = candidateType;
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
