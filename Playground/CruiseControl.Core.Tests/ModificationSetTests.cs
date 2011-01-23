namespace CruiseControl.Core.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class ModificationSetTests
    {
        #region Tests
        [Test]
        public void ConstructorAddsModifications()
        {
            var modification = new Modification();
            var set = new ModificationSet(modification);
            CollectionAssert.AreEqual(new[] { modification }, set.Modifications);
        }
        #endregion
    }
}
