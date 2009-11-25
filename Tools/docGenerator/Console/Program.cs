namespace Console
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.Linq;
    using Exortech.NetReflector;

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                WriteToConsole("No assembly specified", ConsoleColor.Red);
                return;
            }

            var assemblyName = args[0];
            var documentation = new XDocument();
            var documentationPath = Path.ChangeExtension(assemblyName, "xml");
            if (File.Exists(documentationPath))
            {
                documentation = XDocument.Load(documentationPath);
            }

            try
            {
                var baseFolder = Path.Combine(Environment.CurrentDirectory, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture));
                Directory.CreateDirectory(baseFolder);

                var assembly = Assembly.LoadFrom(assemblyName);
                var types = assembly.GetExportedTypes();
                foreach (var type in types)
                {
                    var attributes = type.GetCustomAttributes(typeof(ReflectorTypeAttribute), true);
                    if (attributes.Length > 0)
                    {
                        // There can be only one!
                        var attribute = attributes[0] as ReflectorTypeAttribute;
                        var fileName = Path.Combine(baseFolder, attribute.Name + ".wiki");
                        WriteToConsole("Generating " + attribute.Name + ".wiki", ConsoleColor.White);

                        var typeElement = (from element in documentation.Descendants("member")
                                           where element.Attribute("name").Value == "T:" + type.FullName
                                           select element).SingleOrDefault();

                        using (var output = new StreamWriter(fileName))
                        {
                            var elementName = attribute.Name;
                            if (HasTag(typeElement, "title"))
                            {
                                elementName = typeElement.Element("title").Value;
                            }

                            output.WriteLine("h1. " + elementName);
                            output.WriteLine();
                            if (HasTag(typeElement, "summary"))
                            {
                                WriteDocumentation(typeElement, "summary", output, documentation);
                                output.WriteLine();
                            }

                            if (HasTag(typeElement, "version"))
                            {
                                output.WriteLine("h2. Version");
                                output.WriteLine();
                                output.Write("Available from version ");
                                WriteDocumentation(typeElement, "version", output, documentation);
                                output.WriteLine();
                            }

                            if (HasTag(typeElement, "example"))
                            {
                                output.WriteLine("h2. Examples");
                                output.WriteLine();
                                WriteDocumentation(typeElement, "example", output, documentation);
                                output.WriteLine();
                            }

                            output.WriteLine("h2. Configuration Elements");
                            output.WriteLine();
                            output.WriteLine("|| Element || Description || Type || Required || Default || Version ||");
                            WriteElements(type, output, documentation);
                            output.WriteLine();

                            if (HasTag(typeElement, "remarks"))
                            {
                                output.WriteLine("h2. Notes");
                                output.WriteLine();
                                WriteDocumentation(typeElement, "remarks", output, documentation);
                                output.WriteLine();
                            }
                            output.WriteLine("{info:title=Automatically Generated}");
                            output.WriteLine(
                                "Documentation generated on " +
                                DateTime.Now.ToUniversalTime().ToString("dddd, d MMM yyyy", CultureInfo.InvariantCulture) + 
                                " at " +
                                DateTime.Now.ToUniversalTime().ToString("h:mm:ss tt", CultureInfo.InvariantCulture));
                            output.WriteLine("{info}");
                            output.Flush();
                        }
                    }
                }
            }
            catch (Exception error)
            {
                WriteToConsole(error.Message, ConsoleColor.Red);
            }
        }

        private static void WriteToConsole(string message, ConsoleColor colour)
        {
            var current = System.Console.ForegroundColor;
            System.Console.ForegroundColor = colour;
            System.Console.WriteLine(message);
            System.Console.ForegroundColor = current;
        }

        private static bool HasTag(XElement typeElement, string tagName)
        {
            if (typeElement == null)
            {
                return false;
            }
            else
            {
                return typeElement.Elements(tagName).Count() > 0;
            }
        }

        private static void WriteDocumentation(XElement typeElement, string tagName, StreamWriter output, XDocument documentation)
        {
            foreach (var tagElement in typeElement.Elements(tagName))
            {
                if (tagElement.Elements().Count() == 0)
                {
                    output.WriteLine(tagElement.Value.Trim());
                }
                else
                {
                    foreach (var subElement in tagElement.Elements())
                    {
                        switch (subElement.Name.LocalName)
                        {
                            case "code":
                                var title = subElement.Attribute("title");
                                var options = "borderStyle=solid" +
                                    (title == null ? string.Empty : "|titleBGColor=#ADD6FF|title=" + title.Value);
                                try
                                {
                                    var xmlCode = XDocument.Parse(subElement.Value);
                                    output.WriteLine("{code:xml|" + options + "}");
                                    output.WriteLine(xmlCode.ToString(SaveOptions.None));
                                }
                                catch
                                {
                                    output.WriteLine("{code:" + options + "}");
                                    output.WriteLine(subElement.Value.Trim());
                                }

                                output.WriteLine("{code}");
                                break;

                            case "heading":
                                output.WriteLine("h4. " + subElement.Value.Trim());
                                output.WriteLine();
                                break;

                            default:
                                output.WriteLine(subElement.Value.Trim());
                                break;
                        }
                    }
                }
            }
        }

        private static void WriteElements(Type type, StreamWriter output, XDocument documentation)
        {
            // Put all the elements into a dictionary since they can be either fields or properties :-(
            var elements = new Dictionary<MemberInfo, ReflectorPropertyAttribute>();
            foreach (var field in type.GetFields().OfType<MemberInfo>().Union(type.GetProperties()))
            {
                var attributes = field.GetCustomAttributes(typeof(ReflectorPropertyAttribute), true);
                if (attributes.Length > 0)
                {
                    elements.Add(field, attributes[0] as ReflectorPropertyAttribute);
                }
            }

            // Document all the elements
            foreach (var element in from item in elements
                                    orderby item.Key.Name
                                    select item)
            {
                var memberName = (element.Key is FieldInfo ? "F:" : "P:") + type.FullName + "." + element.Key.Name;
                var description = string.Empty;
                var dataType = element.Key is FieldInfo ? (element.Key as FieldInfo).FieldType : (element.Key as PropertyInfo).PropertyType;
                var dataTypeName = dataType.Name;
                if (dataType.IsEnum)
                {
                    var names = Enum.GetNames(dataType);
                    var builder = new StringBuilder();
                    builder.Append("String - one of:");
                    foreach (var name in names)
                    {
                        builder.Append(Environment.NewLine + "* " + name);
                    }

                    dataTypeName = builder.ToString();
                }

                var defaultValue = string.Empty;
                var version = string.Empty;
                var memberElement = (from xmlElement in documentation.Descendants("member")
                                     where xmlElement.Attribute("name").Value == memberName
                                     select xmlElement).SingleOrDefault();
                if (memberElement != null)
                {
                    description = RetrieveXmlData(memberElement, "summary", "remarks");
                    defaultValue = RetrieveXmlData(memberElement, "default");
                    version = RetrieveXmlData(memberElement, "version");
                }

                // Handle any special keywords
                switch (defaultValue)
                {
                    case "n/a":
                    case "none":
                        defaultValue = "_" + defaultValue + "_";
                        break;
                }

                output.WriteLine(
                    "| " + element.Value.Name +
                    " | " + description +
                    " | " + dataTypeName +
                    " | " + (element.Value.Required ? "Yes" : "No") +
                    " | " + defaultValue +
                    " | " + version +
                    " |");
            }
        }

        private static string RetrieveXmlData(XElement memberElement, params string[] tags)
        {
            var builder = new StringBuilder();

            var count = 0;
            foreach (var tag in tags)
            {
                foreach (var element in memberElement.Elements(tag))
                {
                    var value = element.Value.Trim();
                    if (value.Length > 0)
                    {
                        if (count++ > 0)
                        {
                            builder.Append("\\\\");
                        }

                        builder.Append(value);
                    }
                }
            }

            return builder.ToString();
        }
    }
}
