using System;
using NMock;
using NUnit.Framework;
using ObjectWizard;
using ObjectWizard.NetReflector;

namespace ThoughtWorks.CruiseControl.UnitTests.ObjectWizard.NetReflector
{
	[TestFixture]
	public class ObjectGiverNetReflectorInstantiatorTest
	{
		[Test]
		public void ShouldUseObjectGiverToInstantiateTypes()
		{
			DynamicMock objectGiverMock = new DynamicMock(typeof(ObjectGiver));
			Type typeToInstantiate = this.GetType();
			object instantiated = "foo";
			objectGiverMock.ExpectAndReturn("GiveObjectByType", instantiated, typeToInstantiate);

			ObjectGiverNetReflectorInstantiator instantiator = new ObjectGiverNetReflectorInstantiator((ObjectGiver) objectGiverMock.MockInstance);
			Assert.AreEqual(instantiated, instantiator.Instantiate(typeToInstantiate));
			objectGiverMock.Verify();
		}
	}
}
