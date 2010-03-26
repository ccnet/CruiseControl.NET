namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    using Exortech.NetReflector;

    /// <summary>
    /// Use the Null Source Control if you don't want to check a Source Control repository for changes. In this instance you would always want
    /// to either use a 'Force Build' Trigger or always manually start builds, from the <link>Web Dashboard</link> or <link>CCTray</link>.
    /// </summary>
    /// <title>Null Source Control Block</title>
    /// <version>1.0</version>
    /// <remarks>
    /// Strictly speaking, this element isn't required. The build server will behave the same way if there are no Source Control Blocks. Still,
    /// it's useful to include this in configuration files to make it clear.
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;sourcecontrol type="nullSourceControl" /&gt;
    /// </code>
    /// </example>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>nullSourceControl</value>
    /// </key>
    [ReflectorType("nullSourceControl")]
	public class NullSourceControl : ISourceControl
	{
        /// <summary>
        /// Defines wheter or not to fail the checking for modifications.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("failGetModifications", Required = false)]
        public bool FailGetModifications { get; set; }

        /// <summary>
        /// Defines wheter or not to fail the labeling.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("failLabelSourceControl", Required = false)]
        public bool FailLabelSourceControl { get; set; }

        /// <summary>
        /// Defines wheter or not to fail the getting of the source.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("failGetSource", Required = false)]
        public bool FailGetSource { get; set; }

        /// <summary>
        /// Gets the modifications from the source control provider
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
		public Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
            if (FailGetModifications)
            {
                throw new System.Exception("Failing GetModifications");
            }
            else
            {
                return new Modification[0];
            }
		}

		public void LabelSourceControl(IIntegrationResult result) 
		{
            if (FailLabelSourceControl)
            {
                throw new System.Exception("Failing label source control");
            }
		}

		public void GetSource(IIntegrationResult result)
		{
            if (FailGetSource)
            {
                throw new System.Exception("Failing getting the source");
            }
			
		}

		public void Initialize(IProject project)
		{
		}

		public void Purge(IProject project)
		{
		}
	}
}
