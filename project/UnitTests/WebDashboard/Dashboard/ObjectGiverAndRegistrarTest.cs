using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	// Note - throughout , 'CD' means 'Constructor Dependency'
	[TestFixture]
	public class ObjectGiverAndRegistrarTest
	{
		private ObjectGiverAndRegistrar giverAndRegistrar;
		private object testObject;

		[SetUp]
		public void Setup()
		{
			giverAndRegistrar = new ObjectGiverAndRegistrar();
			testObject = new TestClass();
		}

		[Test]
		public void ShouldReturnObjectRegisteredByType()
		{
			giverAndRegistrar.AddTypedObject(typeof(TestInterface), testObject);
			Assert.AreSame(testObject, giverAndRegistrar.GiveObjectByType(typeof(TestInterface)));
		}

		[Test]
		public void ShouldReturnObjectRegisteredByTypeUsingImplementationTypeOfRegisteredObjectIfRegistrationTypeNotSpecified()
		{
			giverAndRegistrar.AddTypedObjects(testObject);
			Assert.AreSame(testObject, giverAndRegistrar.GiveObjectByType(typeof(TestClass)));
		}

		[Test]
		public void ShouldConstructAnObjectThatHasNoCDs()
		{
			object constructed = giverAndRegistrar.GiveObjectByType(typeof(TestClass));

			Assert.IsNotNull(constructed);
			Assert.IsTrue(constructed is TestClass);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasNoCDsWhenReferencedById()
		{
			giverAndRegistrar.CreateImplementationMapping("foo", typeof(TestClass));
			object constructed = giverAndRegistrar.GiveObjectById("foo");

			Assert.IsNotNull(constructed);
			Assert.IsTrue(constructed is TestClass);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasClassesForCDsByInstantiatingDependenciesIfTheyAreNotRegistered()
		{
			TestClassWithClassDependencies constructed = (TestClassWithClassDependencies) giverAndRegistrar.GiveObjectByType(typeof(TestClassWithClassDependencies));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasClassesForCDsByInstantiatingDependenciesIfTheyAreNotRegisteredWhenReferencedById()
		{
			giverAndRegistrar.CreateImplementationMapping("foo", typeof(TestClassWithClassDependencies));
			TestClassWithClassDependencies constructed = (TestClassWithClassDependencies) giverAndRegistrar.GiveObjectById("foo");

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
		}


		[Test]
		public void ShouldConstructAnObjectThatHasClassesForCDsByUsingRegisteredInstancesIfTheyAreRegistered()
		{
			TestClass dependency = new TestClass();
			giverAndRegistrar.AddTypedObject(typeof(TestClass), dependency);
			TestClassWithClassDependencies constructed = (TestClassWithClassDependencies) giverAndRegistrar.GiveObjectByType(typeof(TestClassWithClassDependencies));

			Assert.IsNotNull(constructed);
			Assert.AreSame(dependency, constructed.Dependency);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasInterfacesForCDsByInstantiatingDependenciesIfTheyAreNotRegistered()
		{
			TestClassWithInterfaceDependencies constructed = (TestClassWithInterfaceDependencies) giverAndRegistrar.GiveObjectByType(typeof(TestClassWithInterfaceDependencies));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasInterfacesForCDsByUsingRegisteredInstancesIfTheyAreRegistered()
		{
			TestClass dependency = new TestClass();
			giverAndRegistrar.AddTypedObject(typeof(TestInterface), dependency);
			TestClassWithInterfaceDependencies constructed = (TestClassWithInterfaceDependencies) giverAndRegistrar.GiveObjectByType(typeof(TestClassWithInterfaceDependencies));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
		}

		[Test]
		public void ShouldUseRuntimeSpecifiedDependencyTypeWhenMultipleImplementationsAvailable()
		{
			giverAndRegistrar.SetDependencyImplementationForType(typeof(ClassThatDependsOnMultiImplInterface), typeof(InterfaceWithMultipleImplementations), typeof(MultiImplTwo));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) giverAndRegistrar.GiveObjectByType(typeof(ClassThatDependsOnMultiImplInterface));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is MultiImplTwo);
		}

		[Test]
		public void ShouldUseRuntimeImplementationTypeWhenMultipleImplementationsAvailable()
		{
			giverAndRegistrar.SetImplementationType(typeof(InterfaceWithMultipleImplementations), typeof(MultiImplOne));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) giverAndRegistrar.GiveObjectByType(typeof(ClassThatDependsOnMultiImplInterface));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is MultiImplOne);
		}

		[Test]
		public void ShouldUseRuntimeSpecifiedDependencyTypeOverImplementationTypeWhenMultipleImplementationsAvailable()
		{
			giverAndRegistrar.SetDependencyImplementationForType(typeof(ClassThatDependsOnMultiImplInterface), typeof(InterfaceWithMultipleImplementations), typeof(MultiImplTwo));
			giverAndRegistrar.SetImplementationType(typeof(InterfaceWithMultipleImplementations), typeof(MultiImplOne));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) giverAndRegistrar.GiveObjectByType(typeof(ClassThatDependsOnMultiImplInterface));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is MultiImplTwo);
		}

		[Test]
		public void ShouldBeAbleToAddDecoratorsForAGivenIdentifier()
		{
			giverAndRegistrar.CreateImplementationMapping("foo", typeof(MultiImplOne)).Decorate(typeof(DecoratingMultiImpl)).Decorate(typeof(ClassThatDependsOnMultiImplInterface));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) giverAndRegistrar.GiveObjectById("foo");

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is DecoratingMultiImpl);
			Assert.IsNotNull(((DecoratingMultiImpl) constructed.Dependency).Dependency);
			Assert.IsTrue(((DecoratingMultiImpl) constructed.Dependency).Dependency is MultiImplOne);
		}

		[Test]
		public void ShouldBeAbleToMarkNMockClassesAsIgnoredForImplementationResolution()
		{
			giverAndRegistrar.IgnoreNMockImplementations = true;
			Assert.IsTrue(giverAndRegistrar.GiveObjectByType(typeof(InterfaceForIgnoring)) is InterfaceForIgnoringImpl);
		}

		[Test]
		public void ShouldBeAbleToSetupDependencyImplementationsForIdentifiers()
		{
			giverAndRegistrar.CreateImplementationMapping("foo", typeof(ClassThatDependsOnMultiImplInterface));
			giverAndRegistrar.SetDependencyImplementationForIdentifer("foo", typeof(InterfaceWithMultipleImplementations), typeof(MultiImplTwo));
			
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) giverAndRegistrar.GiveObjectById("foo");

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is MultiImplTwo);
		}
	}

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
