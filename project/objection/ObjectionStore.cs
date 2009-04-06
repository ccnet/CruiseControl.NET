using System;
using System.Collections;
using System.Reflection;

namespace Objection
{
	public class ObjectionStore : ObjectSource, ObjectionManager
	{
		private ImplementationResolver implementationResolver;
		private ConstructorSelectionStrategy constructorSelectionStrategy;
		private Hashtable typedInstances;
		private Hashtable implementationTypes;
		private Hashtable dependencyImplementations;
		private Hashtable namedTypes;
		private Hashtable dependencyImplementationsForNames;
		private Hashtable namedInstances;

		public ObjectionStore() : this (
			new NMockAwareImplementationResolver(),
			new MaxLengthConstructorSelectionStrategy()) 
		{ }

		public ObjectionStore(ImplementationResolver implementationResolver, ConstructorSelectionStrategy constructorSelectionStrategy)
		{
			this.implementationResolver = implementationResolver;
			this.constructorSelectionStrategy = constructorSelectionStrategy;
			typedInstances = new Hashtable();
			implementationTypes = new Hashtable();
			dependencyImplementations = new Hashtable();
			namedTypes = new Hashtable();
			dependencyImplementationsForNames = new Hashtable();
			namedInstances = new Hashtable();
			AddInstanceForType(typeof(ObjectSource), this);
			AddInstanceForType(typeof(ObjectionManager), this);
			AddInstanceForType(typeof(ObjectionStore), this);
		}

		public void AddInstanceForType(Type type, object instance)
		{
			typedInstances[type] = instance;
		}

		public void AddInstance(params object[] objects)
		{
			foreach (object o in objects)
			{
				AddInstanceForType(o.GetType(), o);
			}
		}

		public void SetImplementationType(Type parentType, Type implementationType)
		{
			implementationTypes[parentType] = implementationType;
		}

		public DecoratableByType AddTypeForName(string name, Type type)
		{
			namedTypes[name] = new ObjectionType(type);
			return (DecoratableByType) namedTypes[name];
		}

		public DecoratableByType AddInstanceForName(string name, object instance)
		{
			namedInstances[name] = new ObjectionObject(instance);
			return (DecoratableByType) namedInstances[name];
		}

		public void SetDependencyImplementationForName(string name, Type dependencyType, Type implementationType)
		{
			if (dependencyImplementationsForNames[name] == null)
				dependencyImplementationsForNames[name] = new Hashtable();

			((Hashtable) dependencyImplementationsForNames[name])[dependencyType] = implementationType;
		}

		public void SetDependencyImplementationForType(Type typeWithDependency, Type dependencyParentType, Type dependencyImplementationType)
		{
			if (dependencyImplementations[typeWithDependency] == null)
				dependencyImplementations[typeWithDependency] = new Hashtable();

			((Hashtable) dependencyImplementations[typeWithDependency])[dependencyParentType] = dependencyImplementationType;
		}

		public object GetByType(Type type)
		{
			return GiveObjectByType(type, null);
		}

		public object GetByName(string name)
		{
			ObjectionObject namedInstance = namedInstances[name] as ObjectionObject;
			if (namedInstance == null)
			{
				ObjectionType namedType = (ObjectionType) namedTypes[name];
			
				if (namedType == null)
					throw new ApplicationException("Unknown object name : " + name);
				else
					return Instantiate(namedType, name);
			}
			else
			{
				if (namedInstance.Decorator != null)
				{
					return Instantiate(namedInstance.Decorator, name, namedInstance.Instance);
				}
				else
				{
					return namedInstance.Instance;
				}
			}
		}

		private object GiveObjectByType(Type type, string name)
		{
			object result = typedInstances[type];
			if (result == null)
			{
				result = Instantiate(type, name);
			}
			return result;
		}

		private object Instantiate(ObjectionType ObjectionType, string id, params object[] args)
		{
			object baseObject = Instantiate(ObjectionType.Type, id, args);

			ObjectionType decorator = ObjectionType.Decorator;
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
				implementationTypes[type] = implementationResolver.ResolveImplementation(type);
				return Instantiate((Type) implementationTypes[type], identifier, args);
			}

			ArrayList arguments = new ArrayList();
			ConstructorInfo constructor = constructorSelectionStrategy.GetConstructor(type);

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
						Type dependencyType = OverrideWithSpecifiedDependencyImplementationIfNecessary(type, parameter.ParameterType, identifier);
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

		private Type OverrideWithSpecifiedDependencyImplementationIfNecessary(Type typeBeingInstantiated, Type staticDependencyType, string identifier)
		{
			Hashtable dependencyTypes;
			if (identifier != null)
			{
				dependencyTypes = (Hashtable) dependencyImplementationsForNames[identifier];
				if (dependencyTypes != null && dependencyTypes[staticDependencyType] != null)
				{
					return (Type) dependencyTypes[staticDependencyType];
				}			
			}
			dependencyTypes = (Hashtable) dependencyImplementations[typeBeingInstantiated];
			if (dependencyTypes != null && dependencyTypes[staticDependencyType] != null)
			{
				return (Type) dependencyTypes[staticDependencyType];
			}
			return staticDependencyType;
		}
	}
}
