using NUnit.Framework;
using Objection;

namespace Objection.UnitTests.AcceptanceTests
{
	// Note - throughout , 'CD' means 'Constructor Dependency'
	[TestFixture]
	public class TypicalUsageTests
	{
		private ObjectionStore store;
		private object testObject;

		[SetUp]
		public void Setup()
		{
			store = new ObjectionStore();
			testObject = new TestClass();
		}

		[Test]
		public void ShouldReturnInstanceRegisteredByType()
		{
			store.AddInstanceForType(typeof(TestInterface), testObject);
			Assert.AreSame(testObject, store.GetByType(typeof(TestInterface)));
		}

		[Test]
		public void ShouldReturnInstanceRegisteredById()
		{
			store.AddInstanceForName("myObject", testObject);
			Assert.AreSame(testObject, store.GetByName("myObject"));
		}

		[Test]
		public void ShouldReturnObjectRegisteredByTypeUsingImplementationTypeOfRegisteredObjectIfRegistrationTypeNotSpecified()
		{
			store.AddInstance(testObject);
			Assert.AreSame(testObject, store.GetByType(typeof(TestClass)));
		}

		[Test]
		public void ShouldConstructAnObjectThatHasNoCDs()
		{
			object constructed = store.GetByType(typeof(TestClass));

			Assert.IsNotNull(constructed);
			Assert.IsTrue(constructed is TestClass);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasNoCDsWhenReferencedById()
		{
			store.AddTypeForName("foo", typeof(TestClass));
			object constructed = store.GetByName("foo");

			Assert.IsNotNull(constructed);
			Assert.IsTrue(constructed is TestClass);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasClassesForCDsByInstantiatingDependenciesIfTheyAreNotRegistered()
		{
			TestClassWithClassDependencies constructed = (TestClassWithClassDependencies) store.GetByType(typeof(TestClassWithClassDependencies));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasClassesForCDsByInstantiatingDependenciesIfTheyAreNotRegisteredWhenReferencedById()
		{
			store.AddTypeForName("foo", typeof(TestClassWithClassDependencies));
			TestClassWithClassDependencies constructed = (TestClassWithClassDependencies) store.GetByName("foo");

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
		}


		[Test]
		public void ShouldConstructAnObjectThatHasClassesForCDsByUsingRegisteredInstancesIfTheyAreRegistered()
		{
			TestClass dependency = new TestClass();
			store.AddInstanceForType(typeof(TestClass), dependency);
			TestClassWithClassDependencies constructed = (TestClassWithClassDependencies) store.GetByType(typeof(TestClassWithClassDependencies));

			Assert.IsNotNull(constructed);
			Assert.AreSame(dependency, constructed.Dependency);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasInterfacesForCDsByInstantiatingDependenciesIfTheyAreNotRegistered()
		{
			TestClassWithInterfaceDependencies constructed = (TestClassWithInterfaceDependencies) store.GetByType(typeof(TestClassWithInterfaceDependencies));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
		}

		[Test]
		public void ShouldConstructAnObjectThatHasInterfacesForCDsByUsingRegisteredInstancesIfTheyAreRegistered()
		{
			TestClass dependency = new TestClass();
			store.AddInstanceForType(typeof(TestInterface), dependency);
			TestClassWithInterfaceDependencies constructed = (TestClassWithInterfaceDependencies) store.GetByType(typeof(TestClassWithInterfaceDependencies));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
		}

		[Test]
		public void ShouldUseRuntimeSpecifiedDependencyTypeWhenMultipleImplementationsAvailable()
		{
			store.SetDependencyImplementationForType(typeof(ClassThatDependsOnMultiImplInterface), typeof(InterfaceWithMultipleImplementations), typeof(MultiImplTwo));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) store.GetByType(typeof(ClassThatDependsOnMultiImplInterface));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is MultiImplTwo);
		}

		[Test]
		public void ShouldUseRuntimeImplementationTypeWhenMultipleImplementationsAvailable()
		{
			store.SetImplementationType(typeof(InterfaceWithMultipleImplementations), typeof(MultiImplOne));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) store.GetByType(typeof(ClassThatDependsOnMultiImplInterface));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is MultiImplOne);
		}

		[Test]
		public void ShouldUseRuntimeSpecifiedDependencyTypeOverImplementationTypeWhenMultipleImplementationsAvailable()
		{
			store.SetDependencyImplementationForType(typeof(ClassThatDependsOnMultiImplInterface), typeof(InterfaceWithMultipleImplementations), typeof(MultiImplTwo));
			store.SetImplementationType(typeof(InterfaceWithMultipleImplementations), typeof(MultiImplOne));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) store.GetByType(typeof(ClassThatDependsOnMultiImplInterface));

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is MultiImplTwo);
		}

		[Test]
		public void ShouldBeAbleToAddDecoratorsForAGivenIdentifiedImplementation()
		{
			store.AddTypeForName("foo", typeof(MultiImplOne)).Decorate(typeof(DecoratingMultiImpl)).Decorate(typeof(ClassThatDependsOnMultiImplInterface));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) store.GetByName("foo");

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
			store.AddInstanceForName("foo", instance).Decorate(typeof(DecoratingMultiImpl)).Decorate(typeof(ClassThatDependsOnMultiImplInterface));
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) store.GetByName("foo");

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is DecoratingMultiImpl);
			Assert.IsNotNull(((DecoratingMultiImpl) constructed.Dependency).Dependency);
			Assert.AreSame(instance, ((DecoratingMultiImpl) constructed.Dependency).Dependency);
		}

		[Test]
		public void ShouldBeAbleToMarkNMockClassesAsIgnoredForImplementationResolution()
		{
			NMockAwareImplementationResolver resolver = new NMockAwareImplementationResolver();
			resolver.IgnoreNMockImplementations = true;
			store = new ObjectionStore(resolver, new MaxLengthConstructorSelectionStrategy());
			Assert.IsTrue(store.GetByType(typeof(InterfaceForIgnoring)) is InterfaceForIgnoringImpl);

		}

		[Test]
		public void ShouldBeAbleToSetupDependencyImplementationsForIdentifiers()
		{
			store.AddTypeForName("foo", typeof(ClassThatDependsOnMultiImplInterface));
			store.SetDependencyImplementationForName("foo", typeof(InterfaceWithMultipleImplementations), typeof(MultiImplTwo));
			
			ClassThatDependsOnMultiImplInterface constructed = (ClassThatDependsOnMultiImplInterface) store.GetByName("foo");

			Assert.IsNotNull(constructed);
			Assert.IsNotNull(constructed.Dependency);
			Assert.IsTrue(constructed.Dependency is MultiImplTwo);
		}
	}
}