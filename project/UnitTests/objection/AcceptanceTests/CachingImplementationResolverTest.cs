using System;
using System.Collections;
using System.Threading;
using NMock;
using NUnit.Framework;

namespace Objection.UnitTests.AcceptanceTests
{
	[TestFixture]
	public class CachingImplementationResolverTest
	{
		[Test]
        [Ignore("Fails randomly")]
		public void ShouldOnlyAllowOneThreadToResolveEachType()
		{
			TypeToTypeMap sharedMap = new HashtableTypeMap(Hashtable.Synchronized(new Hashtable()));
			
			DynamicMock expectedToBeUsed = new DynamicMock(typeof(ImplementationResolver));
			expectedToBeUsed.ExpectAndReturn("ResolveImplementation", typeof(TestClass), typeof(TestInterface));
			DynamicMock notExpectedToBeUsed = new DynamicMock(typeof(ImplementationResolver));
			notExpectedToBeUsed.ExpectNoCall("ResolveImplementation", typeof(Type));

			StallingImplementationResolver stallingResolver = new StallingImplementationResolver((ImplementationResolver) expectedToBeUsed.MockInstance);
			ImplementationResolver resolvingResolver = new CachingImplementationResolver(
				stallingResolver, sharedMap);
			ImplementationResolver moochingResolver = new CachingImplementationResolver((ImplementationResolver) notExpectedToBeUsed.MockInstance, sharedMap);

			ImplementationResolverRunner resolvingRunner = new ImplementationResolverRunner(resolvingResolver, typeof(TestInterface));
			Thread resolvingThread = new Thread(
				new ThreadStart(resolvingRunner.runResolution));
			ImplementationResolverRunner moochingRunner = new ImplementationResolverRunner(moochingResolver, typeof(TestInterface));
			Thread moochingThread = new Thread(
				new ThreadStart(moochingRunner.runResolution));

			resolvingThread.Start();
			moochingThread.Start();
			Thread.Sleep(500); // allow moochingThread to catch up to resolvingThread
			stallingResolver.Resume();
			
			Assert.IsTrue(resolvingThread.Join(200), "Resolving thread did not complete before timeout.");
			Assert.IsTrue(moochingThread.Join(200), "Mooching thread did not complete before timeout.");
			
			expectedToBeUsed.Verify();
			notExpectedToBeUsed.Verify();

			Assert.AreEqual(typeof(TestClass), resolvingRunner.implementationType);
			Assert.AreEqual(typeof(TestClass), moochingRunner.implementationType);
		}
	}
	
	internal class StallingImplementationResolver : ImplementationResolver
	{
		private readonly ImplementationResolver decoratedResolver;
		private readonly ManualResetEvent stallObject;

		public StallingImplementationResolver(ImplementationResolver decoratedResolver)
		{
			stallObject = new ManualResetEvent(false);
			this.decoratedResolver = decoratedResolver;
		}

		public Type ResolveImplementation(Type baseType)
		{
			Type resolvedType = decoratedResolver.ResolveImplementation(baseType);
			stallObject.WaitOne();
			return resolvedType;
		}
		
		public void Resume()
		{
			stallObject.Set();
		}
	}

	internal class HashtableTypeMap : TypeToTypeMap
	{
		private readonly Hashtable hashtable;

		public HashtableTypeMap(Hashtable hashtable)
		{
			this.hashtable = hashtable;
		}

		public Type this[Type baseType]
		{
			get { return (Type) hashtable[baseType]; }
			set { hashtable[baseType] = value; }
		}
	}

	internal class ImplementationResolverRunner
	{
		private readonly ImplementationResolver implementationResolver;
		private readonly Type typeToResolve;
		public Type implementationType;

		public ImplementationResolverRunner(ImplementationResolver implementationResolver, Type typeToResolve)
		{
			this.implementationResolver = implementationResolver;
			this.typeToResolve = typeToResolve;
		}
		
		public void runResolution()
		{
			implementationType = implementationResolver.ResolveImplementation(typeToResolve);
		}
	}
}
