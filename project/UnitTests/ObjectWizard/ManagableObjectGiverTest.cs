using NUnit.Framework;
using ObjectWizard;
using ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.ObjectWizard
{
	[TestFixture]
	public class ManagableObjectGiverTest
	{
		private ManagableObjectGiver giverRegistrar;
		private object testObject;

		[SetUp]
		public void Setup()
		{
			giverRegistrar = new ManagableObjectGiver();
			testObject = new TestClass();
		}

		[Test]
		public void ShouldReturnInstanceRegisteredByType()
		{
			giverRegistrar.AddTypedInstance(typeof(TestInterface), testObject);
			Assert.AreSame(testObject, giverRegistrar.GiveObjectByType(typeof(TestInterface)));
		}

		[Test]
		public void ShouldReturnInstanceRegisteredById()
		{
			giverRegistrar.CreateInstanceMapping("myObject", testObject);
			Assert.AreSame(testObject, giverRegistrar.GiveObjectById("myObject"));
		}

		[Test]
		public void ShouldReturnObjectRegisteredByTypeUsingImplementationTypeOfRegisteredObjectIfRegistrationTypeNotSpecified()
		{
			giverRegistrar.AddInstances(testObject);
			Assert.AreSame(testObject, giverRegistrar.GiveObjectByType(typeof(TestClass)));
		}

		[Test]
		public void ShouldConstructAnObjectThatHasNoCDs()
		{
			object constructed = giverRegistrar.GiveObjectByType(typeof(TestClass));

			Assert.IsNotNull(constructed);
			Assert.IsTrue(constructed is TestClass);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasNoCDsWhenReferencedById()
		{
			giverRegistrar.CreateImplementationMapping("foo", typeof(TestClass));
			object constructed = giverRegistrar.GiveObjectById("foo");

			Assert.IsNotNull(constructed);
			Assert.IsTrue(constructed is TestClass);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasClassesForCDsByInstantiatingDependenciesIfTheyAreNotRegistered()
		{
			TestClassWithClassDependencies constructed = (TestClassWithClassDependencies) giverRegistrar.GiveObjectByType(typeof(TestClassWithClassDependencies));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasClassesForCDsByInstantiatingDependenciesIfTheyAreNotRegisteredWhenReferencedById()
		{
			giverRegistrar.CreateImplementationMapping("foo", typeof(TestClassWithClassDependencies));
			TestClassWithClassDependencies constructed = (TestClassWithClassDependencies) giverRegistrar.GiveObjectById("foo");

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
		}


		[Test]
		public void ShouldConstructAnObjectThatHasClassesForCDsByUsingRegisteredInstancesIfTheyAreRegistered()
		{
			TestClass dependency = new TestClass();
			giverRegistrar.AddTypedInstance(typeof(TestClass), dependency);
			TestClassWithClassDependencies constructed = (TestClassWithClassDependencies) giverRegistrar.GiveObjectByType(typeof(TestClassWithClassDependencies));

			Assert.IsNotNull(constructed);
			Assert.AreSame(dependency, constructed.Dependency);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasInterfacesForCDsByInstantiatingDependenciesIfTheyAreNotRegistered()
		{
			TestClassWithInterfaceDependencies constructed = (TestClassWithInterfaceDependencies) giverRegistrar.GiveObjectByType(typeof(TestClassWithInterfaceDependencies));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasInterfacesForCDsByUsingRegisteredInstancesIfTheyAreRegistered()
		{
			TestClass dependency = new TestClass();
			giverRegistrar.AddTypedInstance(typeof(TestInterface), dependency);
			TestClassWithInterfaceDependencies constructed = (TestClassWithInterfaceDependencies) giverRegistrar.GiveObjectByType(typeof(TestClassWithInterfaceDependencies));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
		}

		[Test]
		public void ShouldUseRuntimeSpecifiedDependencyTypeWhenMultipleImplementationsAvailable()
		{
			giverRegistrar.SetDependencyImplementationForType(typeof(ClassThatDependsOnMultiImplInterface), typeof(InterfaceWithMultipleImplementations), typeof(MultiImplTwo));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) giverRegistrar.GiveObjectByType(typeof(ClassThatDependsOnMultiImplInterface));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is MultiImplTwo);
		}

		[Test]
		public void ShouldUseRuntimeImplementationTypeWhenMultipleImplementationsAvailable()
		{
			giverRegistrar.SetImplementationType(typeof(InterfaceWithMultipleImplementations), typeof(MultiImplOne));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) giverRegistrar.GiveObjectByType(typeof(ClassThatDependsOnMultiImplInterface));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is MultiImplOne);
		}

		[Test]
		public void ShouldUseRuntimeSpecifiedDependencyTypeOverImplementationTypeWhenMultipleImplementationsAvailable()
		{
			giverRegistrar.SetDependencyImplementationForType(typeof(ClassThatDependsOnMultiImplInterface), typeof(InterfaceWithMultipleImplementations), typeof(MultiImplTwo));
			giverRegistrar.SetImplementationType(typeof(InterfaceWithMultipleImplementations), typeof(MultiImplOne));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) giverRegistrar.GiveObjectByType(typeof(ClassThatDependsOnMultiImplInterface));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is MultiImplTwo);
		}

		[Test]
		public void ShouldBeAbleToAddDecoratorsForAGivenIdentifiedImplementation()
		{
			giverRegistrar.CreateImplementationMapping("foo", typeof(MultiImplOne)).Decorate(typeof(DecoratingMultiImpl)).Decorate(typeof(ClassThatDependsOnMultiImplInterface));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) giverRegistrar.GiveObjectById("foo");

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is DecoratingMultiImpl);
			Assert.IsNotNull(((DecoratingMultiImpl) constructed.Dependency).Dependency);
			Assert.IsTrue(((DecoratingMultiImpl) constructed.Dependency).Dependency is MultiImplOne);
		}

		[Test]
		public void ShouldBeAbleToAddDecoratorsForAGivenIdentifiedInstance()
		{
			MultiImplOne instance = new MultiImplOne();
			giverRegistrar.CreateInstanceMapping("foo", instance).Decorate(typeof(DecoratingMultiImpl)).Decorate(typeof(ClassThatDependsOnMultiImplInterface));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) giverRegistrar.GiveObjectById("foo");

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is DecoratingMultiImpl);
			Assert.IsNotNull(((DecoratingMultiImpl) constructed.Dependency).Dependency);
			Assert.AreSame(instance, ((DecoratingMultiImpl) constructed.Dependency).Dependency);
		}

		[Test]
		public void ShouldBeAbleToMarkNMockClassesAsIgnoredForImplementationResolution()
		{
			giverRegistrar.IgnoreNMockImplementations = true;
			Assert.IsTrue(giverRegistrar.GiveObjectByType(typeof(InterfaceForIgnoring)) is InterfaceForIgnoringImpl);
		}

		[Test]
		public void ShouldBeAbleToSetupDependencyImplementationsForIdentifiers()
		{
			giverRegistrar.CreateImplementationMapping("foo", typeof(ClassThatDependsOnMultiImplInterface));
			giverRegistrar.SetDependencyImplementationForIdentifer("foo", typeof(InterfaceWithMultipleImplementations), typeof(MultiImplTwo));
			
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) giverRegistrar.GiveObjectById("foo");

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is MultiImplTwo);
		}
	}
}

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	// Note - throughout , 'CD' means 'Constructor Dependency'

	public interface TestInterface { }

	public class TestClass : TestInterface { }

	public class TestClassWithClassDependencies
	{
		private readonly TestClass dependency;

		public TestClassWithClassDependencies(TestClass dependency)
		{
			this.dependency = dependency;
		}

		public TestClass Dependency
		{
			get { return dependency; }
		}
	}

	public class TestClassWithInterfaceDependencies
	{
		private readonly TestInterface dependency;

		public TestClassWithInterfaceDependencies(TestInterface dependency)
		{
			this.dependency = dependency;
		}

		public TestInterface Dependency
		{
			get { return dependency; }
		}
	}

	public class ClassThatDependsOnMultiImplInterface
	{
		private readonly InterfaceWithMultipleImplementations dependency;

		public InterfaceWithMultipleImplementations Dependency
		{
			get { return dependency; }
		}

		public ClassThatDependsOnMultiImplInterface(InterfaceWithMultipleImplementations dependency)
		{
			this.dependency = dependency;
		}
	}

	public interface InterfaceWithMultipleImplementations { }

	public class MultiImplOne : InterfaceWithMultipleImplementations { }

	public class MultiImplTwo : InterfaceWithMultipleImplementations { }

	public class DecoratingMultiImpl : InterfaceWithMultipleImplementations
	{
		private readonly InterfaceWithMultipleImplementations dependency;

		public InterfaceWithMultipleImplementations Dependency
		{
			get { return dependency; }
		}

		public DecoratingMultiImpl(InterfaceWithMultipleImplementations dependency)
		{
			this.dependency = dependency;
		}
	}

	interface InterfaceForIgnoring { }
	class InterfaceForIgnoringImpl : InterfaceForIgnoring { }
	class ProxyInterfaceForIgnoring_4 : InterfaceForIgnoring { }
}
