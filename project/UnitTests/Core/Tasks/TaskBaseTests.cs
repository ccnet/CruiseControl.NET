using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using Moq;
using NUnit.Framework;
//using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class TaskBaseTests
    {
        #region Private fields
        private MockRepository mocks = new MockRepository(MockBehavior.Default);
        #endregion

        #region Tests
        #region RetrieveDescription()
        [Test]
        public void RetrieveDescriptionOrNameRetrievesTheDescriptionWhenDescriptionIsSet()
        {
            var task = new TestTask();
            task.Description = "Test Task";
            var actual = task.RetrieveDescriptionOrName();
            Assert.AreEqual(task.Description, actual);
        }

        [Test]
        public void RetrieveDescriptionOrNameRetrievesTheNameWhenDescriptionIsNotSet()
        {
            var task = new TestTask();
            task.Description = null;
            var actual = task.RetrieveDescriptionOrName();
            Assert.AreEqual(task.GetType().Name, actual);
        }
        #endregion

        #region Run()
        [Test]
        public void RunWithTrueResultMarksStatusAsSuccess()
        {
            var task = new TestTask
            {
                Result = () => true
            };
            var result = mocks.Create<IIntegrationResult>().Object;

            task.Run(result);
            mocks.VerifyAll();

            Assert.IsTrue(task.Executed);
            Assert.AreEqual(ItemBuildStatus.CompletedSuccess, task.CurrentStatus.Status);
        }

        [Test]
        public void RunWithFalseResultMarksStatusAsFailed()
        {
            var task = new TestTask
            {
                Result = () => false
            };
            var result = mocks.Create<IIntegrationResult>().Object;

            task.Run(result);
            mocks.VerifyAll();

            Assert.IsTrue(task.Executed);
            Assert.AreEqual(ItemBuildStatus.CompletedFailed, task.CurrentStatus.Status);
        }

        [Test]
        public void RunWithExceptionMarksStatusAsFailed()
        {
            var task = new TestTask
            {
                Result = () =>
                {
                    throw new CruiseControlException();
                }
            };
            var result = mocks.Create<IIntegrationResult>().Object;

            // This may look like a weird test, but we don't care that Run() has an exception
            try
            {
                task.Run(result);
            }
            catch (Exception) { }
            mocks.VerifyAll();

            Assert.IsTrue(task.Executed);
            Assert.AreEqual(ItemBuildStatus.CompletedFailed, task.CurrentStatus.Status);
        }
        #endregion

        #region PreprocessParameters()
        [Test]
        public void PreprocessParametersReturnsOriginalNodeWhenNoDynamicValues()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItem>Text</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            Assert.AreEqual(xml, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidAttributeDynamicValue()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"$[value|default]\"><subItem>Text</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"default\"><subItem>Text</subItem>" +
                "<dynamicValues>" +
                    "<directValue>" +
                        "<parameter>value</parameter>" +
                        "<default>default</default>" +
                        "<property>attrib</property>" +
                    "</directValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidNodeDynamicValue()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItem>$[value|default]</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItem>default</subItem>" +
                "<dynamicValues>" +
                    "<directValue>" +
                        "<parameter>value</parameter>" +
                        "<default>default</default>" +
                        "<property>subItem</property>" +
                    "</directValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidAttributeDynamicValueWithoutDefault()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"$[value]\"><subItem>Text</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"\"><subItem>Text</subItem>" +
                "<dynamicValues>" +
                    "<directValue>" +
                        "<parameter>value</parameter>" +
                        "<property>attrib</property>" +
                    "</directValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidNodeDynamicValueWithoutDefault()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItem>$[value]</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItem></subItem>" +
                "<dynamicValues>" +
                    "<directValue>" +
                        "<parameter>value</parameter>" +
                        "<property>subItem</property>" +
                    "</directValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [ReflectorType("item")]
        private class Item
        {
            [ReflectorProperty("subItems", Required = true)]
            public SubItemBase[] SubItems { get; set; }

            [ReflectorProperty("subItemsDV", Required = true)]
            public SubItemDV[] SubItemsDV { get; set; }
        }

        [ReflectorType("subItem")]
        private class SubItem : SubItemBase
        {
            [ReflectorProperty("subSubItems", Required = false)]
            public SubItemBase[] SubSubItems { get; set; }

            [ReflectorProperty("subSubItemsDV", Required = false)]
            public SubItemDV[] SubSubItemsDV { get; set; }
        }

        private class subSubItem : SubItemBase
        {
            public string value { get; set; }
        }

        private class SubItemBase
        {
            public string prop { get; set; }
        }

        private class ItemDV : Item, IWithDynamicValuesItem
        {
            public IDynamicValue[] DynamicValues { get; set; }
        }

        [ReflectorType("subItemDV")]
        private class SubItemDV : SubItem, IWithDynamicValuesItem
        {
            [ReflectorProperty("dynamicValues", Required = false)]
            public IDynamicValue[] DynamicValues { get; set; }
        }

        [ReflectorType("subSubItemDV")]
        private class subSubItemDV : subSubItem, IWithDynamicValuesItem
        {
            [ReflectorProperty("dynamicValues", Required = false)]
            public IDynamicValue[] DynamicValues { get; set; }
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidNestedNodesDynamicValueWithoutDefault()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItems><subItem>$[value]</subItem><subItem>$[value2]</subItem></subItems></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            NetReflectorTypeTable typeTable = new NetReflectorTypeTable();
            typeTable.Add(typeof(Item));
            var actual = task.PreprocessParameters(typeTable, document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItems><subItem></subItem><subItem></subItem></subItems>" +
                "<dynamicValues>" +
                    "<directValue>" +
                        "<parameter>value</parameter>" +
                        "<property>subItems[0]</property>" +
                    "</directValue>" +
                    "<directValue>" +
                        "<parameter>value2</parameter>" +
                        "<property>subItems[1]</property>" +
                    "</directValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidNestedNodesDynamicValueWithoutDefaultWithComment()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItems><subItem>$[value]</subItem><!-- this is a comment --><subItem>$[value2]</subItem></subItems></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            NetReflectorTypeTable typeTable = new NetReflectorTypeTable();
            typeTable.Add(typeof(Item));
            var actual = task.PreprocessParameters(typeTable, document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItems><subItem></subItem><!-- this is a comment --><subItem></subItem></subItems>" +
                "<dynamicValues>" +
                    "<directValue>" +
                        "<parameter>value</parameter>" +
                        "<property>subItems[0]</property>" +
                    "</directValue>" +
                    "<directValue>" +
                        "<parameter>value2</parameter>" +
                        "<property>subItems[1]</property>" +
                    "</directValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidDoublyNestedNodesDynamicValueWithoutDefault()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItems><subItem><subSubItems><subSubItem><value>$[value]</value></subSubItem><subSubItem><value>$[value2]</value></subSubItem></subSubItems></subItem></subItems></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            NetReflectorTypeTable typeTable = new NetReflectorTypeTable();
            typeTable.Add(typeof(Item));
            typeTable.Add(typeof(SubItem));
            typeTable.Add(typeof(subSubItem));
            var actual = task.PreprocessParameters(typeTable, document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItems><subItem><subSubItems><subSubItem><value></value></subSubItem><subSubItem><value></value></subSubItem></subSubItems></subItem></subItems>" +
                "<dynamicValues>" +
                    "<directValue>" +
                        "<parameter>value</parameter>" +
                        "<property>subItems[0].subSubItems[0].value</property>" +
                    "</directValue>" +
                    "<directValue>" +
                        "<parameter>value2</parameter>" +
                        "<property>subItems[0].subSubItems[1].value</property>" +
                    "</directValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidDoublyNestedNodesDynamicValueWithoutDefaultWithComment()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItems><subItem><subSubItems><subSubItem><value>$[value]</value></subSubItem><!-- this is a comment --><subSubItem><value>$[value2]</value></subSubItem></subSubItems></subItem></subItems></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            NetReflectorTypeTable typeTable = new NetReflectorTypeTable();
            typeTable.Add(typeof(Item));
            typeTable.Add(typeof(SubItem));
            typeTable.Add(typeof(subSubItem));
            var actual = task.PreprocessParameters(typeTable, document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItems><subItem><subSubItems><subSubItem><value></value></subSubItem><!-- this is a comment --><subSubItem><value></value></subSubItem></subSubItems></subItem></subItems>" +
                "<dynamicValues>" +
                    "<directValue>" +
                        "<parameter>value</parameter>" +
                        "<property>subItems[0].subSubItems[0].value</property>" +
                    "</directValue>" +
                    "<directValue>" +
                        "<parameter>value2</parameter>" +
                        "<property>subItems[0].subSubItems[1].value</property>" +
                    "</directValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidNestedDynamicValuesNodes()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItemsDV><subItemDV><prop>$[value]</prop></subItemDV><subItemDV><prop>$[value2]</prop></subItemDV></subItemsDV></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            NetReflectorTypeTable typeTable = new NetReflectorTypeTable();
            typeTable.Add(typeof(Item));
            typeTable.Add(typeof(SubItemDV));
            var actual = task.PreprocessParameters(typeTable, document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItemsDV>" +
                "<subItemDV>" +
                    "<prop></prop>" +
                    "<dynamicValues>" +
                        "<directValue>" +
                            "<parameter>value</parameter>" +
                            "<property>prop</property>" +
                        "</directValue>" +
                    "</dynamicValues>" +
                "</subItemDV>" +
                "<subItemDV>" +
                    "<prop></prop>" +
                    "<dynamicValues>" +
                        "<directValue>" +
                            "<parameter>value2</parameter>" +
                            "<property>prop</property>" +
                        "</directValue>" +
                    "</dynamicValues>" +
                "</subItemDV>" +
                "</subItemsDV>" +
                "</item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidNestedDynamicValuesNodesInBaseParent()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItems><subItemDV><prop>$[value]</prop></subItemDV><subItemDV><prop>$[value2]</prop></subItemDV></subItems></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            NetReflectorTypeTable typeTable = new NetReflectorTypeTable();
            typeTable.Add(typeof(Item));
            typeTable.Add(typeof(SubItemDV));
            var actual = task.PreprocessParameters(typeTable, document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItems>" +
                "<subItemDV>" +
                    "<prop></prop>" +
                    "<dynamicValues>" +
                        "<directValue>" +
                            "<parameter>value</parameter>" +
                            "<property>prop</property>" +
                        "</directValue>" +
                    "</dynamicValues>" +
                "</subItemDV>" +
                "<subItemDV>" +
                    "<prop></prop>" +
                    "<dynamicValues>" +
                        "<directValue>" +
                            "<parameter>value2</parameter>" +
                            "<property>prop</property>" +
                        "</directValue>" +
                    "</dynamicValues>" +
                "</subItemDV>" +
                "</subItems>" +
                "</item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidDoublyNestedDynamicValuesNodes()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItems><subItem><subSubItemsDV><subSubItemDV><prop>$[value]</prop></subSubItemDV><subSubItemDV><prop>$[value2]</prop></subSubItemDV></subSubItemsDV></subItem></subItems></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            NetReflectorTypeTable typeTable = new NetReflectorTypeTable();
            typeTable.Add(typeof(Item));
            typeTable.Add(typeof(SubItem));
            typeTable.Add(typeof(SubItemDV));
            typeTable.Add(typeof(subSubItemDV));
            var actual = task.PreprocessParameters(typeTable, document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItems><subItem><subSubItemsDV>" +
                "<subSubItemDV>" +
                    "<prop></prop>" +
                    "<dynamicValues>" +
                        "<directValue>" +
                            "<parameter>value</parameter>" +
                            "<property>prop</property>" +
                        "</directValue>" +
                    "</dynamicValues>" +
                "</subSubItemDV>" +
                "<subSubItemDV>" +
                    "<prop></prop>" +
                    "<dynamicValues>" +
                        "<directValue>" +
                            "<parameter>value2</parameter>" +
                            "<property>prop</property>" +
                        "</directValue>" +
                    "</dynamicValues>" +
                "</subSubItemDV>" +
                "</subSubItemsDV>" +
                "</subItem></subItems></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidDoublyNestedMixedDynamicValuesNodes()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItems><subItem><subSubItemsDV><subSubItemDV><prop>$[value]</prop></subSubItemDV></subSubItemsDV><prop>$[value2]</prop></subItem></subItems></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            NetReflectorTypeTable typeTable = new NetReflectorTypeTable();
            typeTable.Add(typeof(Item));
            typeTable.Add(typeof(SubItem));
            typeTable.Add(typeof(SubItemDV));
            typeTable.Add(typeof(subSubItemDV));
            var actual = task.PreprocessParameters(typeTable, document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItems><subItem><subSubItemsDV>" +
                    "<subSubItemDV>" +
                        "<prop></prop>" +
                        "<dynamicValues>" +
                            "<directValue>" +
                                "<parameter>value</parameter>" +
                                "<property>prop</property>" +
                            "</directValue>" +
                        "</dynamicValues>" +
                    "</subSubItemDV>" +
                "</subSubItemsDV>" +
                "<prop></prop>" +
                "</subItem></subItems>" +
                "<dynamicValues>" +
                    "<directValue>" +
                        "<parameter>value2</parameter>" +
                        "<property>subItems[0].prop</property>" +
                    "</directValue>" +
                "</dynamicValues>" +
                "</item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsSingleReplacementValueForValidAttributeDynamicValue()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"($[value1|default])\"><subItem>Text</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"\"><subItem>Text</subItem>" +
                "<dynamicValues>" +
                    "<replacementValue>" +
                        "<format>({0})</format>" +
                        "<parameters>" +
                            "<namedValue name=\"value1\" value=\"default\" />" +
                        "</parameters>" +
                        "<property>attrib</property>" +
                    "</replacementValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsSingleReplacementValueForValidNodeDynamicValue()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItem>($[value1|default])</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItem></subItem>" +
                "<dynamicValues>" +
                    "<replacementValue>" +
                        "<format>({0})</format>" +
                        "<parameters>" +
                            "<namedValue name=\"value1\" value=\"default\" />" +
                        "</parameters>" +
                        "<property>subItem</property>" +
                    "</replacementValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsSingleReplacementValueForValidAttributeDynamicValueWithoutDefault()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"($[value1])\"><subItem>Text</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"\"><subItem>Text</subItem>" +
                "<dynamicValues>" +
                    "<replacementValue>" +
                        "<format>({0})</format>" +
                        "<parameters>" +
                            "<namedValue name=\"value1\" />" +
                        "</parameters>" +
                        "<property>attrib</property>" +
                    "</replacementValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsSingleReplacementValueForValidNodeDynamicValueWithoutDefault()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItem>($[value1])</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItem></subItem>" +
                "<dynamicValues>" +
                    "<replacementValue>" +
                        "<format>({0})</format>" +
                        "<parameters>" +
                            "<namedValue name=\"value1\" />" +
                        "</parameters>" +
                        "<property>subItem</property>" +
                    "</replacementValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsSingleReplacementValueForValidAttributeDynamicValueWithFormat()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"($[value1|default|00])\"><subItem>Text</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"\"><subItem>Text</subItem>" +
                "<dynamicValues>" +
                    "<replacementValue>" +
                        "<format>({0:00})</format>" +
                        "<parameters>" +
                            "<namedValue name=\"value1\" value=\"default\" />" +
                        "</parameters>" +
                        "<property>attrib</property>" +
                    "</replacementValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsSingleReplacementValueForValidNodeDynamicValueWithFormat()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItem>($[value1|default|00])</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItem></subItem>" +
                "<dynamicValues>" +
                    "<replacementValue>" +
                        "<format>({0:00})</format>" +
                        "<parameters>" +
                            "<namedValue name=\"value1\" value=\"default\" />" +
                        "</parameters>" +
                        "<property>subItem</property>" +
                    "</replacementValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsMultipleReplacementValueForValidAttributeDynamicValue()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"$[value1|default] $[value2|default]\"><subItem>Text</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"\"><subItem>Text</subItem>" +
                "<dynamicValues>" +
                    "<replacementValue>" +
                        "<format>{0} {1}</format>" +
                        "<parameters>" +
                            "<namedValue name=\"value1\" value=\"default\" />" +
                            "<namedValue name=\"value2\" value=\"default\" />" +
                        "</parameters>" +
                        "<property>attrib</property>" +
                    "</replacementValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsMultipleReplacementValueForValidNodeDynamicValue()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItem>$[value1|default] $[value2|default]</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItem></subItem>" +
                "<dynamicValues>" +
                    "<replacementValue>" +
                        "<format>{0} {1}</format>" +
                        "<parameters>" +
                            "<namedValue name=\"value1\" value=\"default\" />" +
                            "<namedValue name=\"value2\" value=\"default\" />" +
                        "</parameters>" +
                        "<property>subItem</property>" +
                    "</replacementValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsMultipleReplacementValueForValidAttributeDynamicValueWithoutDefault()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"$[value1] $[value2]\"><subItem>Text</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"\"><subItem>Text</subItem>" +
                "<dynamicValues>" +
                    "<replacementValue>" +
                        "<format>{0} {1}</format>" +
                        "<parameters>" +
                            "<namedValue name=\"value1\" />" +
                            "<namedValue name=\"value2\" />" +
                        "</parameters>" +
                        "<property>attrib</property>" +
                    "</replacementValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsMultipleReplacementValueForValidNodeDynamicValueWithoutDefault()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItem>$[value1] $[value2]</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItem></subItem>" +
                "<dynamicValues>" +
                    "<replacementValue>" +
                        "<format>{0} {1}</format>" +
                        "<parameters>" +
                            "<namedValue name=\"value1\" />" +
                            "<namedValue name=\"value2\" />" +
                        "</parameters>" +
                        "<property>subItem</property>" +
                    "</replacementValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsMultipleReplacementValueForValidAttributeDynamicValueWithFormat()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"$[value1|default|00] $[value2|default|00]\"><subItem>Text</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"\"><subItem>Text</subItem>" +
                "<dynamicValues>" +
                    "<replacementValue>" +
                        "<format>{0:00} {1:00}</format>" +
                        "<parameters>" +
                            "<namedValue name=\"value1\" value=\"default\" />" +
                            "<namedValue name=\"value2\" value=\"default\" />" +
                        "</parameters>" +
                        "<property>attrib</property>" +
                    "</replacementValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsMultipleReplacementValueForValidNodeDynamicValueWithFormat()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"value\"><subItem>$[value1|default|00] $[value2|default|00]</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"value\"><subItem></subItem>" +
                "<dynamicValues>" +
                    "<replacementValue>" +
                        "<format>{0:00} {1:00}</format>" +
                        "<parameters>" +
                            "<namedValue name=\"value1\" value=\"default\" />" +
                            "<namedValue name=\"value2\" value=\"default\" />" +
                        "</parameters>" +
                        "<property>subItem</property>" +
                    "</replacementValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }

        [Test]
        public void PreprocessParametersAddsDirectValueForValidAttributeDynamicValueWithEscapedCharacter()
        {
            var document = new XmlDocument();
            var xml = "<item attrib=\"$[first\\|second|first\\|second]\"><subItem>Text</subItem></item>";
            document.LoadXml(xml);

            var task = new TestTask();
            var actual = task.PreprocessParameters(new NetReflectorTypeTable(), document.DocumentElement);
            var expected = "<item attrib=\"first|second\"><subItem>Text</subItem>" +
                "<dynamicValues>" +
                    "<directValue>" +
                        "<parameter>first|second</parameter>" +
                        "<default>first|second</default>" +
                        "<property>attrib</property>" +
                    "</directValue>" +
                "</dynamicValues></item>";
            Assert.AreEqual(expected, actual.OuterXml);
        }
        #endregion

        #region ApplyParameters()
        [Test]
        public void ApplyParametersHandlesNull()
        {
            var task = new TestTask
            {
                DynamicValues = null
            };
            var parameters = new Dictionary<string, string>();
            var definitions = new List<ParameterBase>();
            task.ApplyParameters(parameters, definitions);
        }

        [Test]
        public void ApplyParametersHandlesEmpty()
        {
            var task = new TestTask
            {
                DynamicValues = new IDynamicValue[0]
            };
            var parameters = new Dictionary<string, string>();
            var definitions = new List<ParameterBase>();
            task.ApplyParameters(parameters, definitions);
        }

        [Test]
        public void ApplyParametersHandlesParameter()
        {
            var dynamicValue = mocks.Create<IDynamicValue>(MockBehavior.Strict).Object;
            var task = new TestTask
            {
                DynamicValues = new IDynamicValue[]
                {
                    dynamicValue
                }
            };
            var parameters = new Dictionary<string, string>();
            var definitions = new List<ParameterBase>();
            Mock.Get(dynamicValue).Setup(_dynamicValue => _dynamicValue.ApplyTo(task, parameters, definitions)).Verifiable();

            task.ApplyParameters(parameters, definitions);
            mocks.VerifyAll();
        }
        #endregion
        #endregion

        #region Private classes
        private class TestTask
            : TaskBase
        {
            public ThoughtWorks.CruiseControl.CCTrayLib.Presentation.Func<bool> Result { get; set; }
            public bool Executed { get; set; }

            protected override bool Execute(IIntegrationResult result)
            {
                Executed = true;
                return Result();
            }
        }
        #endregion
    }
}

