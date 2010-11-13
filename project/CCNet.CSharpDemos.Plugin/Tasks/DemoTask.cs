using System;
using System.Linq;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;

namespace CCNet.CSharpDemos.Plugin.Tasks
{
    [ReflectorType("demoTask")]
    public class DemoTask
        : ITask
    {
        [ReflectorProperty("name")]
        public string Name { get; set; }

        [ReflectorProperty("author", Required = false)]
        public string Author { get; set; }

        [ReflectorProperty("age", Required = false)]
        public int Age { get; set; }

        [ReflectorProperty("isNonsense", Required = false)]
        public bool IsNonsense { get; set; }

        [ReflectorProperty("items", Required = false)]
        public DemoTask[] InnerItems { get; set; }

        [ReflectorProperty("child", Required = false, InstanceType = typeof(DemoTask))]
        public DemoTask Child { get; set; }

        [ReflectorProperty("typedChild", Required = false, InstanceTypeKey = "type")]
        public DemoTask TypedChild { get; set; }

        [ReflectionPreprocessor]
        public XmlNode PreprocessParameters(NetReflectorTypeTable typeTable, XmlNode inputNode)
        {
            var dobNode = (from node in inputNode.ChildNodes
                               .OfType<XmlNode>()
                           where node.Name == "dob"
                           select node).SingleOrDefault();
            if (dobNode != null)
            {
                var dob = DateTime.Parse(dobNode.InnerText);
                inputNode.RemoveChild(dobNode);
                var ageNode = inputNode.OwnerDocument.CreateElement("age");
                ageNode.InnerText = Convert.ToInt32(
                    (DateTime.Now - dob).TotalDays / 365)
                    .ToString();
                inputNode.AppendChild(ageNode);
            }

            return inputNode;
        }

        public void Run(IIntegrationResult result)
        {
        }
    }
}
