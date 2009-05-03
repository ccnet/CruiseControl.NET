using Exortech.NetReflector;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class DynamicValueUtilityTests
    {
        [Test]
        public void SplitPropertyIntoPartsSingleValue()
        {
            DynamicValueUtility.PropertyPart[] parts = DynamicValueUtility.SplitPropertyName("Test");
            Assert.AreEqual(1, parts.Length, "Length differs from expected");
            CheckPart(parts[0], 0, "Test", null, null);
        }

        [Test]
        public void SplitPropertyIntoPartsSingleValueWithKey()
        {
            DynamicValueUtility.PropertyPart[] parts = DynamicValueUtility.SplitPropertyName("Test[Key=Value]");
            Assert.AreEqual(1, parts.Length, "Length differs from expected");
            CheckPart(parts[0], 0, "Test", "Key", "Value");
        }

        [Test]
        public void SplitPropertyIntoPartsMultipleValues()
        {
            DynamicValueUtility.PropertyPart[] parts = DynamicValueUtility.SplitPropertyName("Test.Part2.Part3");
            Assert.AreEqual(3, parts.Length, "Length differs from expected");
            CheckPart(parts[0], 0, "Test", null, null);
            CheckPart(parts[1], 1, "Part2", null, null);
            CheckPart(parts[2], 2, "Part3", null, null);
        }

        [Test]
        public void SplitPropertyIntoPartsMultipleValuesWithKey()
        {
            DynamicValueUtility.PropertyPart[] parts = DynamicValueUtility.SplitPropertyName("Test.Part2[Key=Value].Part3");
            Assert.AreEqual(3, parts.Length, "Length differs from expected");
            CheckPart(parts[0], 0, "Test", null, null);
            CheckPart(parts[1], 1, "Part2", "Key", "Value");
            CheckPart(parts[2], 2, "Part3", null, null);
        }

        private void CheckPart(DynamicValueUtility.PropertyPart part, int position, string name, string keyName, string keyValue)
        {
            Assert.AreEqual(name, part.Name, string.Format("Part name does not match [{0}]", position));
            Assert.AreEqual(keyName, part.KeyName, string.Format("Part key name does not match [{0}]", position));
            Assert.AreEqual(keyValue, part.KeyValue, string.Format("Part key value does not match [{0}]", position));
        }

        [Test]
        public void FindActualPropertyWithValidProperty()
        {
            TestClass testValue = new TestClass();
            testValue.Name = "My name";
            MemberInfo property = DynamicValueUtility.FindActualProperty(testValue, "someName");
            Assert.IsNotNull(property, "Property not found");
            Assert.AreEqual("Name", property.Name, "Property names do not match");
        }

        [Test]
        public void FindActualPropertyWithInvalidProperty()
        {
            TestClass testValue = new TestClass();
            testValue.Name = "My name";
            object property = DynamicValueUtility.FindActualProperty(testValue, "Name");
            Assert.IsNull(property, "Property found");
        }

        [Test]
        public void FindKeyedValueWithActualValue()
        {
            TestClass testValue = new TestClass();
            TestClass subValue = new TestClass();
            subValue.Name = "A value";
            testValue.SubValues = new TestClass[]{
                subValue
            };
            object result = DynamicValueUtility.FindKeyedValue(testValue.SubValues, "someName", "A value");
            Assert.IsNotNull(result, "Keyed value not found");
            Assert.AreSame(subValue, result, "Found value does not match");
        }

        [Test]
        public void FindTypedValueWithActualValue()
        {
            TestClass testValue = new TestClass();
            TestClass subValue = new TestClass();
            testValue.SubValues = new TestClass[]{
                subValue
            };
            object result = DynamicValueUtility.FindTypedValue(testValue.SubValues, "testInstance");
            Assert.IsNotNull(result, "Typed value not found");
            Assert.AreSame(subValue, result, "Found value does not match");
        }

        [Test]
        public void FindPropertySingle()
        {
            TestClass rootValue = new TestClass("root");
            DynamicValueUtility.PropertyValue result = DynamicValueUtility.FindProperty(rootValue, "someName");
            Assert.IsNotNull(result, "Property not found");
            Assert.AreEqual("Name", result.Property.Name, "Property names do not match");
            Assert.AreEqual("root", result.Value, "Property values do not match");
        }

        [Test]
        public void FindPropertyMultiple()
        {
            TestClass rootValue = new TestClass("root");
            rootValue.SubValues = new TestClass[] {
                new TestClass("child1"),
                new TestClass("child2")
            };
            DynamicValueUtility.PropertyValue result = DynamicValueUtility.FindProperty(rootValue, "sub.testInstance.someName");
            Assert.IsNotNull(result, "Property not found");
            Assert.AreEqual("Name", result.Property.Name, "Property names do not match");
            Assert.AreEqual("child2", result.Value, "Property values do not match");
        }

        [Test]
        public void FindPropertyMultipleWithKey()
        {
            TestClass rootValue = new TestClass("root");
            rootValue.SubValues = new TestClass[] {
                new TestClass("child1"),
                new TestClass("child2")
            };
            DynamicValueUtility.PropertyValue result = DynamicValueUtility.FindProperty(rootValue, "sub[someName=child1].someName");
            Assert.IsNotNull(result, "Property not found");
            Assert.AreEqual("Name", result.Property.Name, "Property names do not match");
            Assert.AreEqual("child1", result.Value, "Property values do not match");
        }

        [Test]
        public void FindPropertyMultipleWithIndex()
        {
            TestClass rootValue = new TestClass("root");
            rootValue.SubValues = new TestClass[] {
                new TestClass("child1"),
                new TestClass("child2")
            };
            DynamicValueUtility.PropertyValue result = DynamicValueUtility.FindProperty(rootValue, "sub[0].someName");
            Assert.IsNotNull(result, "Property not found");
            Assert.AreEqual("Name", result.Property.Name, "Property names do not match");
            Assert.AreEqual("child1", result.Value, "Property values do not match");
        }

        [Test]
        public void ChangePropertySameType()
        {
            TestClass rootValue = new TestClass("root");
            DynamicValueUtility.PropertyValue result = DynamicValueUtility.FindProperty(rootValue, "someName");
            result.ChangeProperty("nonRoot");
            Assert.AreEqual("nonRoot", rootValue.Name, "Property not changed");
        }

        [Test]
        public void ChangePropertyDifferentType()
        {
            TestClass rootValue = new TestClass("root");
            rootValue.Value = 100;
            DynamicValueUtility.PropertyValue result = DynamicValueUtility.FindProperty(rootValue, "aValue");
            result.ChangeProperty("20");
            Assert.AreEqual(20, rootValue.Value, "Property not changed");
        }

        [ReflectorType("testInstance")]
        public class TestClass
        {
            private string myName;
            private int myValue;
            private TestClass[] mySubValues;

            public TestClass() { }
            public TestClass(string name) { this.myName = name; }

            [ReflectorProperty("someName")]
            public string Name
            {
                get { return myName; }
                set { myName = value; }
            }

            [ReflectorProperty("aValue")]
            public int Value
            {
                get { return myValue; }
                set { myValue = value; }
            }

            [ReflectorProperty("sub")]
            public TestClass[] SubValues
            {
                get { return mySubValues; }
                set { mySubValues = value; }
            }
        }
    }
}
