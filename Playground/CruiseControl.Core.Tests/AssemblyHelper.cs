namespace CruiseControl.Core.Tests
{
    using System.Diagnostics;
    using System.IO;
    using CruiseControl.Core.Tests.Configuration;

    public static class AssemblyHelper
    {
        public static Stream RetrieveExampleFile(string exampleName)
        {
            var frame = new StackFrame(1);
            var assembly = typeof(ExampleTests).Assembly;
            var typeName = frame.GetMethod().DeclaringType;
            var streamName = typeName.Namespace +
                "." +
                exampleName +
                ".xaml";
            var stream = assembly.GetManifestResourceStream(streamName);
            return stream;
        }
    }
}
