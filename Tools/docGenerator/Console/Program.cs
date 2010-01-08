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
    using System.Text.RegularExpressions;
    using System.Diagnostics;
    using System.ServiceModel;
    using Console.ConfluenceApi;

    public class Program
    {
        private static Regex specialChars;
        private static List<string> problemList = new List<string>();

        public static void Main(string[] args)
        {
            var cmdArgs = new List<string>(args);

            if (args.Length == 0)
            {
                WriteToConsole("No command specified", ConsoleColor.Red);
            }
            else
            {
                var cmd = cmdArgs[0];
                cmdArgs.RemoveAt(0);
                switch (cmd.ToLowerInvariant())
                {
                    case "generate":
                        GenerateDocumentation(cmdArgs);
                        break;

                    case "list":
                        ListConfluenceItems(cmdArgs);
                        break;

                    case "publish":
                        PublishConfluenceItems(cmdArgs);
                        break;

                    default:
                        WriteToConsole("Unknown command: " + cmd, ConsoleColor.Red);
                        break;
                }
            }

            // Pause when running within Visual Studio
            if (Debugger.IsAttached)
            {
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Lists the confluence items.
        /// </summary>
        /// <param name="cmdArgs">The command-line args.</param>
        private static void ListConfluenceItems(List<String> cmdArgs)
        {
            var isValid = false;
            var url = string.Empty;
            var user = string.Empty;
            var pwd = string.Empty;
            var output = string.Empty;
            if (cmdArgs.Count == 0)
            {
                WriteToConsole("Confluence URL not specified", ConsoleColor.Red);
            }
            else if (cmdArgs.Count == 1)
            {
                WriteToConsole("Username not specified", ConsoleColor.Red);
            }
            else if (cmdArgs.Count == 2)
            {
                WriteToConsole("Password not specified", ConsoleColor.Red);
            }
            else if (cmdArgs.Count == 3)
            {
                WriteToConsole("Output path not specified", ConsoleColor.Red);
            }
            else
            {
                isValid = true;
                url = cmdArgs[0];
                user = cmdArgs[1];
                pwd = cmdArgs[2];
                output = cmdArgs[3];
                if (!Path.IsPathRooted(output))
                {
                    output = Path.Combine(Environment.CurrentDirectory, output);
                }
            }

            if (isValid)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                WriteToConsole("Retrieving Confluence items from " + url, ConsoleColor.Gray);
                var count = 0;

                try
                {
                    var binding = new BasicHttpBinding();
                    binding.MaxReceivedMessageSize = int.MaxValue;
                    var client = new ConfluenceSoapServiceClient(
                        binding, 
                        new EndpointAddress(url));

                    try
                    {
                        var session = client.login(user, pwd);
                        try
                        {
                            var pages = client.getPages(session, "CCNET");
                            var document = new XDocument();
                            if (File.Exists(output))
                            {
                                document = XDocument.Load(output);
                            }

                            // Make sure there is a root element
                            var rootEl = document.Root;
                            if (rootEl == null)
                            {
                                rootEl = new XElement(
                                    "confluence",
                                    new XAttribute("space", "CCNET"));
                                document.Add(rootEl);
                            }

                            foreach (var page in pages)
                            {
                                // Attempt to find each page
                                var pageId = page.id.ToString();
                                var pageEl = (from element in rootEl.Elements("page")
                                              where element.Attribute("id").Value == pageId
                                              select element).SingleOrDefault();
                                if (pageEl == null)
                                {
                                    // If the page does not exist, add it
                                    pageEl = new XElement(
                                        "page",
                                        new XAttribute("title", page.title),
                                        new XAttribute("id", pageId),
                                        new XAttribute("url", page.url),
                                        new XAttribute("parentId", page.parentId.ToString()));
                                    rootEl.Add(pageEl);
                                    count++;
                                }
                            }

                            if (count > 0)
                            {
                                // Order all the elements
                                var newDoc = new XDocument(
                                    new XElement("confluence",
                                        new XAttribute("space", "CCNET")));
                                newDoc.Root.Add(from element in document.Root.Elements()
                                                orderby element.Attribute("title").Value
                                                select element);
                                newDoc.Save(output);
                                WriteToConsole("Document updated", ConsoleColor.White);
                            }
                        }
                        finally
                        {
                            client.logout(session);
                        }
                    }
                    catch (Exception error)
                    {
                        WriteToConsole("ERROR: " + error.Message, ConsoleColor.Red);
                    }
                }
                catch (Exception error)
                {
                    WriteToConsole("ERROR: " + error.Message, ConsoleColor.Red);
                }

                stopwatch.Stop();
                WriteToConsole(
                    count.ToString() + " new confluence items retrieved in " + stopwatch.Elapsed.TotalSeconds.ToString("#,##0.00") + "s",
                    ConsoleColor.Gray);
            }
        }

        /// <summary>
        /// Publishes items to Confluence.
        /// </summary>
        /// <param name="cmdArgs">The command-line args.</param>
        private static void PublishConfluenceItems(List<String> cmdArgs)
        {
            var isValid = false;
            var url = string.Empty;
            var user = string.Empty;
            var pwd = string.Empty;
            var input = string.Empty;
            if (cmdArgs.Count == 0)
            {
                WriteToConsole("Confluence URL not specified", ConsoleColor.Red);
            }
            else if (cmdArgs.Count == 1)
            {
                WriteToConsole("Username not specified", ConsoleColor.Red);
            }
            else if (cmdArgs.Count == 2)
            {
                WriteToConsole("Password not specified", ConsoleColor.Red);
            }
            else if (cmdArgs.Count == 3)
            {
                WriteToConsole("Input path not specified", ConsoleColor.Red);
            }
            else
            {
                isValid = true;
                url = cmdArgs[0];
                user = cmdArgs[1];
                pwd = cmdArgs[2];
                input = cmdArgs[3];
                if (!Path.IsPathRooted(input))
                {
                    input = Path.Combine(Environment.CurrentDirectory, input);
                }
            }

            if (isValid)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                WriteToConsole("Publishing Confluence items from " + url, ConsoleColor.Gray);
                var count = 0;

                try
                {
                    var binding = new BasicHttpBinding();
                    binding.MaxReceivedMessageSize = int.MaxValue;
                    binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                    var client = new ConfluenceSoapServiceClient(
                        binding,
                        new EndpointAddress(url));

                    try
                    {
                        var session = client.login(user, pwd);
                        try
                        {
                            var inputDir = Path.GetDirectoryName(input);
                            var inputDoc = XDocument.Load(input);
                            foreach (var pageEl in inputDoc.Root.Elements())
                            {
                                var sourceAttr = pageEl.Attribute("source");
                                var titleAttr = pageEl.Attribute("title");
                                var idAttr = pageEl.Attribute("id");
                                var parentAttr = pageEl.Attribute("parentId");
                                if (sourceAttr != null)
                                {
                                    WriteToConsole(
                                        "Publishing " + sourceAttr.Value + " to " + titleAttr.Value + "[" + idAttr.Value + "]",
                                        ConsoleColor.White);

                                    var sourcePath = sourceAttr.Value;
                                    if (!Path.IsPathRooted(sourcePath))
                                    {
                                        sourcePath = Path.Combine(inputDir, sourcePath);
                                    }

                                    if (!File.Exists(sourcePath))
                                    {
                                        WriteToConsole("Unable to find file " + sourcePath, ConsoleColor.Yellow);
                                    }
                                    else
                                    {
                                        var page = client.getPage(session, "CCNET", titleAttr.Value);
                                        var newContent = File.ReadAllText(sourcePath).Replace("\r\n", "\n");
                                        var autoPos = newContent.LastIndexOf("{info:title=Automatically Generated}");
                                        if (string.Compare(page.content, 0, newContent, 0, autoPos == -1 ? newContent.Length : autoPos) != 0)
                                        {
                                            page.content = newContent;
                                            client.storePage(session, page);
                                            WriteToConsole(titleAttr.Value + " updated", ConsoleColor.DarkYellow);
                                            count++;
                                        }
                                        else
                                        {
                                            WriteToConsole(titleAttr.Value + " unchanged, publish skipped", ConsoleColor.Gray);
                                        }
                                    }
                                }
                            }
                        }
                        finally
                        {
                            client.logout(session);
                        }
                    }
                    catch (Exception error)
                    {
                        WriteToConsole("ERROR: " + error.Message, ConsoleColor.Red);
                    }
                }
                catch (Exception error)
                {
                    WriteToConsole("ERROR: " + error.Message, ConsoleColor.Red);
                }

                stopwatch.Stop();
                WriteToConsole(
                    count.ToString() + " confluence items updated in " + stopwatch.Elapsed.TotalSeconds.ToString("#,##0.00") + "s",
                    ConsoleColor.Gray);
            }
        }

        public static void GenerateDocumentation(List<string> args)
        {
            specialChars = new Regex(@"[\|\[\]\*_+-]", RegexOptions.Compiled);
            if (args.Count == 0)
            {
                WriteToConsole("No assembly specified", ConsoleColor.Red);
                return;
            }

            var assemblyName = args[0];
            if (!File.Exists(assemblyName))
            {
                WriteToConsole("Cannot find assembly: " + assemblyName, ConsoleColor.Red);
                return;
            }

            var documentation = new XDocument();
            var documentationPath = Path.ChangeExtension(assemblyName, "xml");
            if (File.Exists(documentationPath))
            {
                documentation = XDocument.Load(documentationPath);
            }
            else
            {
                WriteToConsole("No XML file found for assembly: " + assemblyName, ConsoleColor.Red);
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            WriteToConsole("Starting documentation generation for " + Path.GetFileName(assemblyName), ConsoleColor.Gray);
            problemList.Clear();

            try
            {
                var baseFolder = Path.Combine(Environment.CurrentDirectory, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture));
                if (args.Count > 1)
                {
                    baseFolder = args[1];
                    if (!Path.IsPathRooted(baseFolder))
                    {
                        baseFolder = Path.Combine(Environment.CurrentDirectory, baseFolder);
                    }
                }
                Debug.WriteLine("BaseFolder : " + baseFolder);

                Directory.CreateDirectory(baseFolder);
                var assembly = Assembly.LoadFrom(assemblyName);

                // Load the documentation for any dependencies
                LoadDependencyDocumentation(Path.GetDirectoryName(assemblyName), assembly, documentation);

                var publicTypesInAssembly = assembly.GetExportedTypes();
                foreach (var publicType in publicTypesInAssembly)
                {
                    var attributes = publicType.GetCustomAttributes(typeof(ReflectorTypeAttribute), true);
                    if (attributes.Length > 0)
                    {
                        Debug.WriteLine("Found reflector attributes in " + publicType.FullName);

                        // There can be only one!
                        var attribute = attributes[0] as ReflectorTypeAttribute;
                        var fileName = Path.Combine(baseFolder, attribute.Name + ".wiki");
                        WriteToConsole("Generating " + attribute.Name + ".wiki", ConsoleColor.White);
                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }

                        var typeElement = (from element in documentation.Descendants("member")
                                           where element.Attribute("name").Value == "T:" + publicType.FullName
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
                            else
                            {
                                problemList.Add("No Summary tag for " + publicType.FullName + " file " + fileName);
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
                            else
                            {
                                if (publicType.IsClass)
                                {
                                    problemList.Add("No example tag for " + publicType.FullName + " file " + fileName);
                                }

                            }

                            output.WriteLine("h2. Configuration Elements");
                            output.WriteLine();
                            var elements = ListElements(publicType);
                            var keyElement = typeElement != null ? typeElement.Element("key") : null;
                            if ((elements.Count > 0) || (keyElement != null))
                            {
                                output.WriteLine("|| Element || Description || Type || Required || Default || Version ||");
                                WriteElements(elements, output, documentation, typeElement);
                            }
                            else
                            {
                                output.WriteLine("There is no configuration for this plugin.");
                            }
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

                        var xsdFile = Path.Combine(baseFolder, attribute.Name + ".xsd");
                        if (File.Exists(xsdFile))
                        {
                            File.Delete(xsdFile);
                        }

                        WriteXsdFile(xsdFile, attribute, publicType, documentation, typeElement);
                    }
                }
            }
            catch (Exception error)
            {
                WriteToConsole(error.Message, ConsoleColor.Red);
                throw;
            }

            stopwatch.Stop();
            WriteToConsole(
                "Documentation generation finished in " + stopwatch.Elapsed.TotalSeconds.ToString("#,##0.00") + "s", 
                ConsoleColor.Gray);

            if (problemList.Count > 0)
            {
                WriteToConsole("Problems encountered : " + problemList.Count.ToString(), ConsoleColor.Yellow);

                foreach (string s in problemList)
                {
                    WriteToConsole(s, ConsoleColor.Yellow);
                }
            }
        }

        private static void WriteXsdFile(string xsdFile, ReflectorTypeAttribute attribute, Type type, XDocument documentation, XElement typeElement)
        {
            using (var output = new StreamWriter(xsdFile))
            {
                WriteToConsole("Generating " + attribute.Name + ".xsd", ConsoleColor.White);
                output.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                output.WriteLine("<xs:schema targetNamespace=\"http://thoughtworks.org/ccnet/1/5\" elementFormDefault=\"qualified\" xmlns=\"http://thoughtworks.org/ccnet/1/5\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">");
                output.WriteLine("<xs:element name=\"" + attribute.Name + "\" type=\"" + attribute.Name + "\">");
                if (typeElement != null)
                {
                    var summary = RetrieveXmlData(typeElement, "summary");
                    if (summary != null)
                    {
                        output.WriteLine("<xs:annotation>");
                        output.WriteLine("<xs:documentation>");
                        output.WriteLine(MakeXmlSafe(summary));
                        output.WriteLine("</xs:documentation>");
                        output.WriteLine("</xs:annotation>");
                    }
                }

                output.WriteLine("</xs:element>");
                output.WriteLine("<xs:complexType name=\"" + attribute.Name + "\">");

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

                var enums = new List<Type>();
                if (elements.Count > 0)
                {
                    output.WriteLine("<xs:all>");
                    foreach (var element in elements)
                    {
                        var dataType = (element.Key is FieldInfo) ?
                            (element.Key as FieldInfo).FieldType :
                            (element.Key as PropertyInfo).PropertyType;
                        output.WriteLine(
                            "<xs:element name=\"" + element.Value.Name + 
                            "\" type=\"" + dataType.Name + 
                            "\" minOccurs=\"" + (element.Value.Required ? "1" : "0") +
                            "\" maxOccurs=\"1\">");

                        var memberName = (element.Key is FieldInfo ? "F:" : "P:") + element.Key.DeclaringType.FullName + "." + element.Key.Name;
                        var memberElement = (from xmlElement in documentation.Descendants("member")
                                             where xmlElement.Attribute("name").Value == memberName
                                             select xmlElement).SingleOrDefault();

                        if (memberElement != null)
                        {
                            var description = RetrieveXmlData(memberElement, "summary");
                            if (description != null)
                            {
                                output.WriteLine("<xs:annotation>");
                                output.WriteLine("<xs:documentation>");
                                output.WriteLine(MakeXmlSafe(description));
                                output.WriteLine("</xs:documentation>");
                                output.WriteLine("</xs:annotation>");
                            }
                        }
                        output.WriteLine("</xs:element>");
                        if (dataType.IsEnum)
                        {
                            enums.Add(dataType);
                        }
                    }

                    output.WriteLine("</xs:all>");
                }

                output.WriteLine("</xs:complexType>");

                foreach (var enumType in enums)
                {
                    output.WriteLine("<xs:simpleType name=\"" + enumType.Name + "\">");
                    output.WriteLine("<xs:restriction base=\"xs:string\">");
                    foreach (var value in Enum.GetNames(enumType)){
                        output.WriteLine("<xs:enumeration value=\"" + value + "\"/>");
                    }

                    output.WriteLine("</xs:restriction>");
                    output.WriteLine("</xs:simpleType>");
                }

                output.WriteLine("</xs:schema>");
                output.Flush();
            }
        }

        private static string MakeXmlSafe(string value)
        {
            return value.Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        private static void LoadDependencyDocumentation(string baseFolder, Assembly assembly, XDocument documentation)
        {
            Debug.WriteLine("Loading Dependency Documentation for " + assembly.FullName);

            foreach (var dependency in assembly.GetReferencedAssemblies())
            {
                var dependencyData = Path.Combine(baseFolder, dependency.Name + ".xml");
                if (File.Exists(dependencyData))
                {
                    var dependencyXml = XDocument.Load(dependencyData);
                    if (documentation.Root != null)
                    {
                        documentation.Root.Add(dependencyXml.Root.Elements());
                    }
                    else
                    {
                        documentation.Add(dependencyXml.Root);
                    }
                }
                else
                {
                    if (dependency.Name.StartsWith("ThoughtWorks"))
                    {
                        problemList.Add("!!! No xml documentation found for " + dependency.Name + " expected  " + dependencyData);
                    }                   
                }
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
                output.WriteLine(ParseElement(tagElement));
            }
        }

        private static string ParseElement(XElement element)
        {
            var builder = new StringBuilder();

            foreach (var node in element.Nodes())
            {
                if (node is XText)
                {
                    builder.Append(TrimValue((node as XText).Value));
                }
                else if (node is XElement)
                {
                    var childElement = node as XElement;
                    switch (childElement.Name.LocalName)
                    {
                        case "code":
                            var isXmlAttribute = childElement.Attribute("type");
                            var isXml = (isXmlAttribute == null) || string.Equals("xml", isXmlAttribute.Value, StringComparison.InvariantCultureIgnoreCase);
                            var codeTitle = childElement.Attribute("title");
                            var options = "borderStyle=solid" +
                                (codeTitle == null ? string.Empty : "|titleBGColor=#ADD6FF|title=" + codeTitle.Value);

                            if (isXml)
                            {
                                try
                                {
                                    var xmlCode = XDocument.Parse(childElement.Value);
                                    builder.AppendLine("{code:xml|" + options + "}");
                                    builder.AppendLine(xmlCode.ToString(SaveOptions.None));
                                }
                                catch
                                {
                                    isXml = false;
                                }
                            }
                            
                            if (!isXml)
                            {
                                if ((isXmlAttribute != null) && (isXmlAttribute.Value.Length > 0))
                                {
                                    builder.AppendLine("{code:" + isXmlAttribute.Value + "|" + options + "}");
                                }
                                else
                                {
                                    builder.AppendLine("{code:" + options + "}");
                                }
                                builder.AppendLine(childElement.Value);
                            }

                            builder.AppendLine("{code}");
                            break;

                        case "heading":
                            builder.AppendLine("h4. " + TrimValue(childElement.Value));
                            break;

                        case "list":
                            foreach (XElement itemElement in childElement.Elements("item"))
                            {
                                if (!builder.ToString().EndsWith(Environment.NewLine))
                                {
                                    builder.AppendLine();
                                }

                                builder.Append("* " + TrimValue(itemElement.Value));
                            }
                            break;

                        case "b":
                            var boldValue = TrimValue(childElement.Value);
                            builder.Append(" *" + boldValue.Trim() + (boldValue.EndsWith(" ") ? "* " : "*"));
                            break;

                        case "para":
                            var paraType = childElement.Attribute("type");
                            var paraChild = new XElement(childElement);
                            if (paraType != null)
                            {
                                builder.Append("{" + paraType.Value);
                                var paraTitle = paraChild.Element("title");
                                if (paraTitle != null)
                                {
                                    builder.Append(":title=" + TrimValue(paraTitle.Value));
                                    paraTitle.Remove();
                                }

                                builder.AppendLine("}");
                            }

                            builder.AppendLine(ParseElement(paraChild));
                            if (paraType != null)
                            {
                                builder.AppendLine("{" + paraType.Value + "}");
                            }
                            break;

                        case "link":
                            builder.Append("[" + TrimValue(childElement.Value) + "]");
                            break;

                        case "include":
                            builder.Append("{include:" + TrimValue(childElement.Value) + "}");
                            break;

                        default:
                            builder.Append(ParseElement(childElement));
                            break;
                    }
                }
            }

            return builder.ToString();
        }

        private static string TrimValue(string value)
        {
            var builder = new StringBuilder();

            var lines = value.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (var loop = 0; loop < lines.Length; loop++)
            {
                if (loop > 0)
                {
                    builder.Append(" ");
                }

                builder.Append(specialChars.Replace(lines[loop].Trim(), r => "\\" + r.Value));
            }

            // Add start and end spaces if the value has these - saves having to check if a space needs to be added later
            var output = (value.StartsWith(" ") ? " " : string.Empty) + builder.ToString() + (value.EndsWith(" ") ? " " : string.Empty);
            return output;
        }

        private static Dictionary<MemberInfo, ReflectorPropertyAttribute> ListElements(Type type)
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
            return elements;
        }

        private static void WriteElements(Dictionary<MemberInfo, ReflectorPropertyAttribute> elements, StreamWriter output, XDocument documentation, XElement typeElement)
        {
            if (typeElement != null)
            {
                // Check if this item has a required key
                var keyElement = typeElement.Element("key");
                if (keyElement != null)
                {
                    output.Write("| " + (keyElement.Attribute("name") == null ? string.Empty : TrimValue(keyElement.Attribute("name").Value)));
                    output.Write(" | " + (keyElement.Element("description") == null ? string.Empty : TrimValue(keyElement.Element("description").Value)));
                    output.Write(" | String - must be " + (keyElement.Element("value") == null ? string.Empty : TrimValue(keyElement.Element("value").Value)));
                    output.Write(" | Yes | _n/a_ | " + (typeElement.Element("version") == null ? string.Empty : TrimValue(typeElement.Element("version").Value)));
                    output.WriteLine(" |");
                }
            }

            // Document all the elements
            foreach (var element in from item in elements
                                    orderby item.Key.Name
                                    select item)
            {
                var memberName = (element.Key is FieldInfo ? "F:" : "P:") + element.Key.DeclaringType.FullName + "." + element.Key.Name;
                var description = string.Empty;
                var dataType = element.Key is FieldInfo ? (element.Key as FieldInfo).FieldType : (element.Key as PropertyInfo).PropertyType;
                var dataTypeName = dataType.Name;
                var memberElement = (from xmlElement in documentation.Descendants("member")
                                     where xmlElement.Attribute("name").Value == memberName
                                     select xmlElement).SingleOrDefault();
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
                else if (dataType.IsArray)
                {
                    var itemType = dataType.GetElementType();
                    if (itemType.IsEnum)
                    {
                        var names = Enum.GetNames(itemType);
                        var builder = new StringBuilder();
                        builder.Append("String array\\\\The following values are valid:");
                        foreach (var name in names)
                        {
                            builder.Append(Environment.NewLine + "* " + name);
                        }

                        dataTypeName = builder.ToString();
                    }
                    else
                    {
                        dataTypeName = RetrieveTypeName(memberElement, itemType, documentation) + " array";
                    }
                }
                else
                {
                    dataTypeName = RetrieveTypeName(memberElement, dataType, documentation);
                }

                var defaultValue = string.Empty;
                var version = string.Empty;
                if (memberElement != null)
                {
                    description = RetrieveXmlData(memberElement, "summary", "remarks");
                    defaultValue = RetrieveXmlData(memberElement, "default");
                    version = RetrieveXmlData(memberElement, "version");
                    var values = memberElement.Element("values");
                    if (values != null)
                    {
                        // If values are defined, then this must be a string value
                        if (dataType.IsArray)
                        {
                            dataTypeName = "String array\\\\The following values are valid:";
                        }
                        else
                        {
                            dataTypeName = "String - one of:";
                        }

                        foreach (var valueElement in values.Elements("value"))
                        {
                            dataTypeName += Environment.NewLine + "* " + TrimValue(valueElement.Value);
                        }
                    }
                }

                // Handle any special keywords
                switch (defaultValue)
                {
                    case "n/a":
                    case "none":
                    case "None":
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
                    var value = ParseElement(element);
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
        private static string RetrieveTypeName(XElement memberElement, Type dataType, XDocument documentation)
        {
            var dataTypeName = dataType.FullName;
            var dataTypeElement = memberElement != null ? memberElement.Element("dataType") : null;
            if (dataTypeElement != null)
            {
                dataTypeName = dataTypeElement.Value ?? dataTypeName;
            }
            else if (dataType.IsGenericType)
            {
                if (dataType.GetGenericTypeDefinition().Name == "Nullable`1")
                {
                    dataTypeName = dataType.GetGenericArguments()[0].FullName;
                }
            }

            var typeElement = (from element in documentation.Descendants("member")
                               where element.Attribute("name").Value == "T:" + dataTypeName
                               select element).SingleOrDefault();
            var titleAttribute = typeElement != null ? typeElement.Element("title") : null;
            if (titleAttribute != null)
            {
                return "[" + titleAttribute.Value.Trim() + "]";
            }
            else
            {
                return dataType.Name;
            }
        }
    }
}
