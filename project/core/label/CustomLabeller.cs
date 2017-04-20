using System;
using System.Linq;
using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;
using System.Globalization;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace ThoughtWorks.CruiseControl.Core.Label
{
    /// <summary>
    /// <para>
    /// Allows CCNet create custom code-generated labels
    /// </para>
    /// <para>
    /// You can do this by specifying your own configuration of the default labeller in your project.
    /// </para>
    /// </summary>
    /// <title>Custom Labeller</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;labeller type="customlabeller"&gt;
    /// &lt;cscode&gt;1&lt;/cscode&gt;
    /// &lt;/labeller&gt;
    /// </code>
    /// </example>
    [ReflectorType("customlabeller")]
    public class CustomLabeller
        : LabellerBase
    {
        /// <summary>
        /// Generates the specified integration result.	
        /// </summary>
        /// <param name="integrationResult">The integration result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string Generate(IIntegrationResult integrationResult)
        {
            MethodInfo method = this.CreateFunction(this.CsCode);
            string ret = (string)method.Invoke(null, new object[1] { integrationResult });
            return ret;
        }

        [ReflectorProperty("usings", Required = false)]
        public string Usings { get; set; }

        [ReflectorProperty("referencedassemblies", Required = false)]
        public string ReferencedAssemblies { get; set; }

        [ReflectorProperty("cscode", Required = true)]
        public string CsCode { get; set; }

        private string CSCodeWrapper
        {
            get
            {
                return @"

using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
            
namespace CustomLabelerGeneratorUserFunctions
{                
    public class CustomLabelerGenerator
    {                
        public static string Generate(ThoughtWorks.CruiseControl.Core.IIntegrationResult integrationResult)
        {
            string ret = ""0.0.0.0"";
            <customCodeForReplace>
            return ret;
        }
    }
}
";
            }
        }

        public MethodInfo CreateFunction(string function)
        {
            System.Text.StringBuilder usings = new System.Text.StringBuilder();
            if (!string.IsNullOrWhiteSpace(this.Usings))
            {
                foreach (var each in this.Usings.Split(';'))
                {
                    if (!string.IsNullOrWhiteSpace(each))
                    {
                        usings.AppendFormat("using {0};", each);
                        usings.AppendLine();
                    }
                }
            }

            string finalCode = usings.ToString() + CSCodeWrapper.Replace("<customCodeForReplace>", function);

            CSharpCodeProvider provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            parameters.ReferencedAssemblies.Add(typeof(ThoughtWorks.CruiseControl.Remote.IntegrationStatus).Assembly.Location);
            if (!string.IsNullOrWhiteSpace(this.ReferencedAssemblies))
            {
                foreach (var each in this.ReferencedAssemblies.Split(';'))
                {
                    if (!string.IsNullOrWhiteSpace(each))
                    {
                        parameters.ReferencedAssemblies.Add(each);
                    }
                }
            }

            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;
            parameters.IncludeDebugInformation = false;
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, finalCode);
            if (results.Errors != null && results.Errors.Count > 0)
            {
                var path = System.IO.Path.GetTempFileName();
                System.IO.File.WriteAllText(path, finalCode);
                foreach (var each in results.Errors)
                {
                    Console.WriteLine("ERROR in {0}: {1}", path, each);
                }

                throw new ApplicationException("There are compilation errors. Please see " + path);
            }


            Type binaryFunction = results.CompiledAssembly.GetType("CustomLabelerGeneratorUserFunctions.CustomLabelerGenerator");
            return binaryFunction.GetMethod("Generate");
        }
    }
}
