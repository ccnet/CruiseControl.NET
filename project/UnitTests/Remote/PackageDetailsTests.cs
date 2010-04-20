namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;

    [TestFixture]
    public class PackageDetailsTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsName()
        {
            var package = new PackageDetails("The name");
            Assert.AreEqual("The name", package.FileName);
        }

        [Test]
        public void AllPropertiesCanBeSetAndRetrieved()
        {
            var package = new PackageDetails();
            var theName = "the name";
            var theBuild = "buildLabel.x";
            var theDate = new DateTime(2010, 1, 1);
            var theSize = 123456;
            var theCount = 4;
            var theFile = "The Name of Some file";

            package.Name = theName;
            package.BuildLabel = theBuild;
            package.DateTime = theDate;
            package.Size = theSize;
            package.NumberOfFiles = theCount;
            package.FileName = theFile;

            Assert.AreEqual(theName, package.Name);
            Assert.AreEqual(theBuild, package.BuildLabel);
            Assert.AreEqual(theDate, package.DateTime);
            Assert.AreEqual(theSize, package.Size);
            Assert.AreEqual(theCount, package.NumberOfFiles);
            Assert.AreEqual(theFile, package.FileName);
        }

        [Test]
        public void PassThroughSerialisation()
        {
            TestHelpers.EnsureLanguageIsValid();
            var package = new PackageDetails();
            var theName = "the name";
            var theBuild = "buildLabel.x";
            var theDate = new DateTime(2010, 1, 1);
            var theSize = 123456;
            var theCount = 4;
            var theFile = "The Name of Some file";

            package.Name = theName;
            package.BuildLabel = theBuild;
            package.DateTime = theDate;
            package.Size = theSize;
            package.NumberOfFiles = theCount;
            package.FileName = theFile;
            var result = TestHelpers.RunSerialisationTest(package);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PackageDetails>(result);
            var actualPackage = result as PackageDetails;
            Assert.AreEqual(theName, actualPackage.Name);
            Assert.AreEqual(theBuild, actualPackage.BuildLabel);
            Assert.AreEqual(theDate, actualPackage.DateTime);
            Assert.AreEqual(theSize, actualPackage.Size);
            Assert.AreEqual(theCount, actualPackage.NumberOfFiles);
            Assert.AreEqual(theFile, actualPackage.FileName);
        }
        #endregion
    }
}
