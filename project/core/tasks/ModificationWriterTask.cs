using System.IO;
using System.Xml.Serialization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>
    /// This task writes the detected modifications for the current integration to a file as XML. This enables the modifications to be used
    /// by external programs, such as within a NAnt build script.
    /// </para>
    /// </summary>
    /// <title>Modification Writer Task</title>
    /// <version>1.0</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;modificationWriter /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;modificationWriter&gt;
    /// &lt;filename&gt;mods.xml&lt;/filename&gt;
    /// &lt;path&gt;&lt;/path&gt;
    /// &lt;appendTimeStamp&gt;False&lt;/appendTimeStamp&gt;
    /// &lt;/modificationWriter&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>Output format</heading>
    /// <para>
    /// The modifications are written as follows:
    /// </para>
    /// <code>
    /// &lt;!-- Start of the group of modifications (even if just one). --&gt;
    /// &lt;ArrayOfModification xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"&gt;
    /// &lt;!-- Start of one modification. --&gt;
    /// &lt;Modification&gt;
    /// &lt;!-- The change number. --&gt;
    /// &lt;ChangeNumber&gt;... value ...&lt;/ChangeNumber&gt;
    /// &lt;!-- The comment. --&gt;
    /// &lt;Comment&gt;... value ...&lt;/Comment&gt;
    /// &lt;!-- The user's email address. --&gt;
    /// &lt;EmailAddress&gt;... value ...&lt;/EmailAddress&gt;
    /// &lt;!-- The affected file name. --&gt;
    /// &lt;FileName&gt;... value ...&lt;/FileName&gt;
    /// &lt;!-- The affect file's folder name. --&gt;
    /// &lt;FolderName&gt;... value ...&lt;/FolderName&gt;
    /// &lt;!-- The change timestamp, in yyyy-mm-ddThh:mm:ss.nnnn-hhmm format --&gt;
    /// &lt;ModifiedTime&gt;... value ...&lt;/ModifiedTime&gt;
    /// &lt;!-- The operation type. --&gt;
    /// &lt;Type&gt;... value ...&lt;/Type&gt;
    /// &lt;!-- The user name. --&gt;
    /// &lt;UserName&gt;... value ...&lt;/UserName&gt;
    /// &lt;!-- The related URL. --&gt;
    /// &lt;Url&gt;... value ...&lt;/Url&gt;
    /// &lt;!-- The file version. --&gt;
    /// &lt;Version&gt;... value ...&lt;/Version&gt;
    /// &lt;!-- End of modification. --&gt;
    /// &lt;/Modification&gt;
    /// &lt;!-- End of the group of modifications. --&gt;
    /// &lt;/ArrayOfModification&gt;
    /// </code>
    /// <heading>Sample output</heading>
    /// <code>
    /// &lt;ArrayOfModification xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"&gt;
    /// &lt;Modification&gt;
    /// &lt;ChangeNumber&gt;12245&lt;/ChangeNumber&gt;
    /// &lt;Comment&gt;New Project for testing stuff&lt;/Comment&gt;
    /// &lt;EmailAddress&gt;JUser@Example.Com&lt;/EmailAddress&gt;
    /// &lt;FileName&gt;AssemblyInfo.cs&lt;/FileName&gt;
    /// &lt;FolderName&gt;Dev\Server\Interface\Properties\&lt;/FolderName&gt;
    /// &lt;ModifiedTime&gt;2006-11-22T11:11:00-0500&lt;/ModifiedTime&gt;
    /// &lt;Type&gt;add&lt;/Type&gt;
    /// &lt;UserName&gt;joe_user&lt;/UserName&gt;
    /// &lt;Url&gt;http://www.example.com/index.html&lt;/Url&gt;
    /// &lt;Version&gt;5&lt;/Version&gt;
    /// &lt;/Modification&gt;
    /// &lt;Modification&gt;
    /// &lt;ChangeNumber&gt;12244&lt;/ChangeNumber&gt;
    /// &lt;Comment&gt;New Project for accessing web services&lt;/Comment&gt;
    /// &lt;EmailAddress&gt;SSpade@Example.Com&lt;/EmailAddress&gt;
    /// &lt;FileName&gt;Interface&lt;/FileName&gt;
    /// &lt;FolderName&gt;Dev\Server\&lt;/FolderName&gt;
    /// &lt;ModifiedTime&gt;2006-11-22T11:10:44-0500&lt;/ModifiedTime&gt;
    /// &lt;Type&gt;add&lt;/Type&gt;
    /// &lt;UserName&gt;sam_spade&lt;/UserName&gt;
    /// &lt;Url&gt;http://www.example.com/index.html&lt;/Url&gt;
    /// &lt;Version&gt;4&lt;/Version&gt;
    /// &lt;/Modification&gt;
    /// &lt;/ArrayOfModification&gt;    
    /// </code>
    /// </remarks>
    [ReflectorType("modificationWriter")]
    public class ModificationWriterTask 
        : TaskBase
    {
        private readonly IFileSystem fileSystem;

        public ModificationWriterTask()
            : this(new SystemIoFileSystem())
        { }

        public ModificationWriterTask(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Writing Modifications");                

            XmlSerializer serializer = new XmlSerializer(typeof(Modification[]));
            StringWriter writer = new Utf8StringWriter();
            serializer.Serialize(writer, result.Modifications);
            string filename = ModificationFile(result);
            fileSystem.EnsureFolderExists(filename);
            fileSystem.Save(filename, writer.ToString());

            return true;
        }

        private string ModificationFile(IIntegrationResult result)
        {
        	if (!AppendTimeStamp)
        		return Path.Combine(result.BaseFromArtifactsDirectory(OutputPath), Filename);

        	FileInfo fi = new FileInfo(Filename);
        	string dummy = Filename.Remove(Filename.Length - fi.Extension.Length, fi.Extension.Length);
        	string newFileName = string.Format("{0}_{1}{2}", dummy, result.StartTime.ToString("yyyyMMddHHmmssfff"),
        	                                   fi.Extension);

        	return Path.Combine(result.BaseFromArtifactsDirectory(OutputPath), newFileName);
        }

    	/// <summary>
        /// The filename for the file containing the modifications.
        /// </summary>
        /// <version>1.0</version>
        /// <default>modifications.xml</default>
        [ReflectorProperty("filename", Required = false)]
        public string Filename = "modifications.xml";

        /// <summary>
        /// The directory to write the xml file to. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>Project Artefact Directory</default>
        [ReflectorProperty("path", Required = false)]
        public string OutputPath;

        /// <summary>
        /// Appends the integration start time to the filename, just before the extention. Making it possible to create a modification file
        /// per integration, without overwriting existing ones. Intended to be used with the <link>Modification Reader Task</link>.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("appendTimeStamp", Required = false)]
        public bool AppendTimeStamp;
    }
}
