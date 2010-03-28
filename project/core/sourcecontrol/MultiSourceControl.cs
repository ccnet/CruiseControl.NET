using System.Collections.Generic;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// You can use the 'Multi' Source Control plugin to check for modifications from any number of source control repositories. You may want
    /// to do this if (for example) you want to build if the source for your project changes, or if the binaries your project depends on change
    /// (which may be stored on a file server).
    /// </summary>
    /// <title>Multi Source Control Block</title>
    /// <version>1.0</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>multi</value>
    /// </key>
    /// <example>
    /// <code>
    /// &lt;sourcecontrol type="multi"&gt;
    /// &lt;sourceControls&gt;
    /// &lt;filesystem&gt;
    /// &lt;!-- Check for changes in the latest 1.2 version of the server... --&gt;
    /// &lt;repositoryRoot&gt;\\DistributionFileServer\Server\1.2.latest&lt;/repositoryRoot&gt;
    /// &lt;/filesystem&gt;
    /// &lt;cvs&gt;
    /// &lt;!-- ...or in the source of the client project --&gt;
    /// &lt;executable&gt;c:\tools\cvs-exe\cvswithplinkrsh.bat&lt;/executable&gt;
    /// &lt;workingDirectory&gt;c:\localcvs\myproject\client&lt;/workingDirectory&gt;
    /// &lt;/cvs&gt;
    /// &lt;/sourceControls&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// Note that, due to the way the configuration gets parsed, if you are using a "multi" block, then the items within the 
    /// &lt;sourceControls&gt; element should not be &lt;sourcecontrol&gt; elements (as you may expect). Instead, the name of the element 
    /// should be the same as you would put in the "type" attribute when using a &lt;sourcecontrol&gt; element.
    /// </para>
    /// <para>
    /// For example, normally you would point to a cvs repository like this:
    /// </para>
    /// <code>
    /// &lt;sourcecontrol type="cvs"&gt;
    /// &lt;executable&gt;c:\tools\cvs-exe\cvswithplinkrsh.bat&lt;/executable&gt;
    /// &lt;workingDirectory&gt;c:\localcvs\myproject\client&lt;/workingDirectory&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// <para>
    /// But inside a &lt;sourcecontrol type="multi"&lt; element, this becomes:
    /// </para>
    /// <code>
    /// &lt;sourcecontrol type="multi"&gt;
    /// &lt;sourceControls&gt;
    /// &lt;cvs&gt;
    /// &lt;executable&gt;c:\tools\cvs-exe\cvswithplinkrsh.bat&lt;/executable&gt;
    /// &lt;workingDirectory&gt;c:\localcvs\myproject\client&lt;/workingDirectory&gt;
    /// &lt;/cvs&gt;
    /// &lt;/sourceControls&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </remarks>
	[ReflectorType("multi")]
	public class MultiSourceControl 
        : SourceControlBase
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSourceControl"/> class.
        /// </summary>
        public MultiSourceControl()
        {
            this.RequireChangesFromAll = false;
        }

		private ISourceControl[] _sourceControls;

        /// <summary>
        /// If true, only return a list of modifications if all sourceControl sections return a non-empty list. Note that this is
        /// short-circuiting, i.e. if the first sourceControl returns an empty list, the next won't be called (this can be useful for
        /// situations where you have a slow source control server and you want to check a specific file first as a trigger).
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("requireChangesFromAll", Required = false)]
        public bool RequireChangesFromAll { get; set; }

        /// <summary>
        /// The list of other Source Control Blocks to include.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("sourceControls", Required = true)]
		public ISourceControl[] SourceControls 
		{
			get 
			{
				if (_sourceControls == null)
					_sourceControls = new ISourceControl[0];

				return _sourceControls;
			}

			set { _sourceControls = value; }
		}

        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
            var modificationSet = new Dictionary<Modification, bool>();
            foreach (ISourceControl sourceControl in SourceControls)
            {
                Modification[] mods = sourceControl.GetModifications(from, to);
                if (mods != null && mods.Length > 0)
                {
                    foreach (var mod in mods)
                    {
                        modificationSet[mod] = true;
                    }
                }
                else if (RequireChangesFromAll)
                {
                    modificationSet.Clear();
                    break;
                }
            }

            var modArray = new Modification[modificationSet.Count];
            modificationSet.Keys.CopyTo(modArray, 0);
            return modArray; 
		}

        public override void LabelSourceControl(IIntegrationResult result)
		{
			foreach (ISourceControl sourceControl in SourceControls)
			{
				sourceControl.LabelSourceControl(result);
			}
		}

        public override void GetSource(IIntegrationResult result) 
		{
			foreach (ISourceControl sourceControl in SourceControls)
			{
				sourceControl.GetSource(result);
			}
		}

        public override void Initialize(IProject project)
		{
		}

        public override void Purge(IProject project)
		{
		}

        #region ApplyParameters()
        /// <summary>
        /// Applies the input parameters to the task.
        /// </summary>
        /// <param name="parameters">The parameters to apply.</param>
        /// <param name="parameterDefinitions">The original parameter definitions.</param>
        public override void ApplyParameters(Dictionary<string, string> parameters, IEnumerable<ParameterBase> parameterDefinitions)
        {
            base.ApplyParameters(parameters, parameterDefinitions);
            foreach (var child in SourceControls)
            {
                var dynamicChild = child as IParamatisedItem;
                if (dynamicChild != null) dynamicChild.ApplyParameters(parameters, parameterDefinitions);
            }
        }
        #endregion

        #region PreprocessParameters()
        /// <summary>
        /// Preprocesses a node prior to loading it via NetReflector.
        /// </summary>
        /// <param name="typeTable">The type table.</param>
        /// <param name="inputNode">The input node.</param>
        /// <returns></returns>
        [ReflectionPreprocessor]
        public override XmlNode PreprocessParameters(NetReflectorTypeTable typeTable, XmlNode inputNode)
        {
            return DynamicValueUtility.ConvertXmlToDynamicValues(typeTable, inputNode, "sourceControls");
        }
        #endregion
    }
}