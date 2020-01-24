namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using FluentAssertions;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;

    public class ItemStatusTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsName()
        {
            var item = new ItemStatus("theName");
            Assert.AreEqual("theName", item.Name);
        }

        [Test]
        public void AllPropertiesCanBeSetAndRetrieved()
        {
            var item = new ItemStatus();
            var theName = "myItemName";
            var theDesc = "A description";
            var theError = "any error here";
            var theStatus = ItemBuildStatus.CompletedFailed;
            var startTime = new DateTime(2010, 1, 1, 12, 12, 12);
            var completedTime = new DateTime(2010, 1, 1, 13, 12, 12);
            var estimatedTime = new DateTime(2010, 1, 1, 14, 12, 12);
            var theParent = new ItemStatus();

            item.Name = theName;
            item.Description = theDesc;
            item.Error = theError;
            item.Status = theStatus;
            item.TimeStarted = startTime;
            item.TimeCompleted = completedTime;
            item.TimeOfEstimatedCompletion = estimatedTime;
            item.Parent = theParent;

            Assert.AreEqual(theName, item.Name);
            Assert.AreEqual(theDesc, item.Description);
            Assert.AreEqual(theError, item.Error);
            Assert.AreEqual(theStatus, item.Status);
            Assert.AreEqual(startTime, item.TimeStarted);
            Assert.AreEqual(completedTime, item.TimeCompleted);
            Assert.AreEqual(estimatedTime, item.TimeOfEstimatedCompletion);
            Assert.AreEqual(theParent, item.Parent);
            Assert.AreNotEqual(item.Identifier, item.Parent.Identifier);
        }

        [Test]
        public void CloneGeneratesANewIdenticialInstance()
        {
            var item = new ItemStatus();
            var theName = "myItemName";
            var theDesc = "A description";
            var theError = "any error here";
            var theStatus = ItemBuildStatus.CompletedFailed;
            var startTime = new DateTime(2010, 1, 1, 12, 12, 12);
            var completedTime = new DateTime(2010, 1, 1, 13, 12, 12);
            var estimatedTime = new DateTime(2010, 1, 1, 14, 12, 12);
            var aChild = new ItemStatus("aChild");

            item.Name = theName;
            item.Description = theDesc;
            item.Error = theError;
            item.Status = theStatus;
            item.TimeStarted = startTime;
            item.TimeCompleted = completedTime;
            item.TimeOfEstimatedCompletion = estimatedTime;
            item.ChildItems.Add(aChild);
            var clone = item.Clone();

            Assert.AreEqual(theName, clone.Name);
            Assert.AreEqual(theDesc, clone.Description);
            Assert.AreEqual(theError, clone.Error);
            Assert.AreEqual(theStatus, clone.Status);
            Assert.AreEqual(startTime, clone.TimeStarted);
            Assert.AreEqual(completedTime, clone.TimeCompleted);
            Assert.AreEqual(estimatedTime, clone.TimeOfEstimatedCompletion);
            Assert.AreEqual(item.Identifier, clone.Identifier);
            Assert.AreEqual(1, clone.ChildItems.Count);
            Assert.AreEqual("aChild", clone.ChildItems[0].Name);
            Assert.AreEqual(aChild.Identifier, clone.ChildItems[0].Identifier);
        }

        [Test]
        public void GetHashCodeReturnsHashOfIdentifier()
        {
            var item = new ItemStatus();
            var hash = item.GetHashCode();
            Assert.AreEqual(item.Identifier.GetHashCode(), hash);
        }

        [Test]
        public void EqualsReturnsTrueForSameIdentifier()
        {
            var item1 = new ItemStatus();
            var item2 = item1.Clone();
            Assert.IsTrue(item1.Equals(item2));
        }

        [Test]
        public void EqualsReturnsFalseForDifferentIdentifier()
        {
            var item1 = new ItemStatus();
            var item2 = new ItemStatus();
            Assert.IsFalse(item1.Equals(item2));
        }

        [Test]
        public void EqualsReturnsFalseForNonItemStatus()
        {
            var item1 = new ItemStatus();
            var item2 = "This is a test";
            Assert.IsFalse(item1.Equals(item2));
        }

        [Test]
        public void ToStringGeneratesXml()
        {
            var item = new ItemStatus();
            var theName = "myItemName";
            var theDesc = "A description";
            var theError = "any error here";
            var theStatus = ItemBuildStatus.CompletedFailed;
            var startTime = new DateTime(2010, 1, 1, 12, 12, 12);
            var completedTime = new DateTime(2010, 1, 1, 13, 12, 12);
            var estimatedTime = new DateTime(2010, 1, 1, 14, 12, 12);
            var aChild = new ItemStatus("aChild");

            item.Name = theName;
            item.Description = theDesc;
            item.Error = theError;
            item.Status = theStatus;
            item.TimeStarted = startTime;
            item.TimeCompleted = completedTime;
            item.TimeOfEstimatedCompletion = estimatedTime;
            item.ChildItems.Add(aChild);
            var xml = item.ToString();
            var expected = "<itemStatus xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" name=\"myItemName\" status=\"CompletedFailed\">" +
                "<description>A description</description>" + 
                "<error>any error here</error>" + 
                "<timeStarted>2010-01-01T12:12:12</timeStarted>" + 
                "<timeCompleted>2010-01-01T13:12:12</timeCompleted>" + 
                "<timeOfEstimatedCompletion>2010-01-01T14:12:12</timeOfEstimatedCompletion>" + 
                "<childItems>" + 
                    "<childItem name=\"aChild\" status=\"Unknown\">" + 
                        "<timeStarted xsi:nil=\"true\" />" + 
                        "<timeCompleted xsi:nil=\"true\" />" +
                        "<timeOfEstimatedCompletion xsi:nil=\"true\" />" + 
                        "<childItems />" + 
                    "</childItem>" + 
                "</childItems>" + 
                "</itemStatus>";
            //Assert.AreEqual(expected, xml);

            XDocument.Parse(xml).Should().BeEquivalentTo(XDocument.Parse(expected));
        }

        [Test]
        public void PassThroughSerialisation()
        {
            TestHelpers.EnsureLanguageIsValid();
            var item = new ItemStatus();
            var theName = "myItemName";
            var theDesc = "A description";
            var theError = "any error here";
            var theStatus = ItemBuildStatus.CompletedFailed;
            var startTime = new DateTime(2010, 1, 1, 12, 12, 12);
            var completedTime = new DateTime(2010, 1, 1, 13, 12, 12);
            var estimatedTime = new DateTime(2010, 1, 1, 14, 12, 12);
            var aChild = new ItemStatus("aChild");

            item.Name = theName;
            item.Description = theDesc;
            item.Error = theError;
            item.Status = theStatus;
            item.TimeStarted = startTime;
            item.TimeCompleted = completedTime;
            item.TimeOfEstimatedCompletion = estimatedTime;
            item.ChildItems.Add(aChild);
            var result = TestHelpers.RunSerialisationTest(item);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ItemStatus>(result);
            var actualStatus = result as ItemStatus;
            Assert.AreEqual(theName, actualStatus.Name);
            Assert.AreEqual(theDesc, actualStatus.Description);
            Assert.AreEqual(theError, actualStatus.Error);
            Assert.AreEqual(theStatus, actualStatus.Status);
            Assert.AreEqual(startTime, actualStatus.TimeStarted);
            Assert.AreEqual(completedTime, actualStatus.TimeCompleted);
            Assert.AreEqual(estimatedTime, actualStatus.TimeOfEstimatedCompletion);
            Assert.AreEqual(item.Identifier, actualStatus.Identifier);
            Assert.AreEqual(1, actualStatus.ChildItems.Count);
            Assert.AreEqual("aChild", actualStatus.ChildItems[0].Name);
            Assert.AreEqual(aChild.Identifier, actualStatus.ChildItems[0].Identifier);
        }
        #endregion
    }
}
