namespace Objection.UnitTests.AcceptanceTests
{
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