using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Telelogic
{
	/// <summary>
	/// A CM Synergy project (which is similar to a "view" in other SCM packages).
	/// </summary>
    /// <title>Synergy Project</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;project&gt;
    /// &lt;release&gt;Product/1.0&lt;/release&gt;
    /// &lt;projectSpecification&gt;Product-1&lt;/projectSpecification&gt;
    /// &lt;taskFolder&gt;1234&lt;/taskFolder&gt;
    /// &lt;baseline&gt;false&lt;/baseline&gt;
    /// &lt;purpose&gt;Integration Testing&lt;/purpose&gt;
    /// &lt;template&gt;true&lt;/template&gt;
    /// &lt;/project&gt;
    /// </code>
    /// </example>
    [ReflectorType("synergyProject")]
	public class SynergyProjectInfo
	{
		/// <summary>The default value for <see cref="TaskFolder"/></summary>
		public const int DefaultTaskFolder = 0;

		/// <summary>The default value for <see cref="Purpose"/></summary>
		public const string DefaultPurpose = "Integration Testing";

		/// <summary>
		/// The configured Synergy release value for the given project.
		/// </summary>
		/// <remarks>
		/// The component + version specification.
		/// </remarks>
		/// <value>
		/// Defaults to <see langword="null" />.
		/// </value>
        /// <version>1.0</version>
        /// <default>n/a</default>
		[ReflectorProperty("release")]
		public string Release;

		/// <summary>
		/// The configured Synergy project specification for all source control operations.
		/// </summary>
		/// <value>
		/// Defaults to <see langword="null" />.
		/// </value>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("projectSpecification")]
		public string ProjectSpecification;

		/// <summary>
		/// The 4 part object identifier for this project.
		/// </summary>
		/// <value>
		/// Defaults to <see langword="null" />.
		/// </value>
		public string ObjectName;

		/// <summary>
		/// The project's work area path.
		/// </summary>
		/// <value>
		/// Defaults to <see langword="null" />.
		/// </value>
		public string WorkAreaPath;

		/// <summary>
		/// The folder specification for the shared folder which will be used to "manually" add successfully integrated tasks added to.
		/// </summary>
		/// <remarks>
		/// This should be a shared folder added to the reconfigure template/properties for all developer's projects (i.e., projects for purpose "Insulated Development"
        /// or "Colloborative Development").  The easiset way to do this is by creating a "folder template", which is then added to the "Default Release" reconfigure
        /// templates "Default Release:Insulated Development" or "Default Release:Colloborative Development".
		/// Adding integrated tasks to a shared task folder is an alternative to creating a new baseline for every successful integration build.  Most source control
        /// providers implemented by CruiseControl.NET use labels for grouping change sets.  However, Synergy is an activity based SCM tool that groups changes by ...
		/// </remarks>
		/// <value>
		/// Defaults to <c>0</c>.
		/// </value>
        /// <version>1.0</version>
        /// <default>0</default>
        [ReflectorProperty("taskFolder", Required = false)]
		public int TaskFolder = DefaultTaskFolder;

		/// <summary>
		/// If true, creates a new baseline for the project configuration after a successful integration.
		/// </summary>
		/// <remarks>
		/// Realistically, a successful continuous integration does not justify creation of a baseline.  Baselines should be used to create a snapshot of a configuration,
        /// so that anyone can  usually based on a project mileston
		/// </remarks>
		/// <value>
		///     <see langword="false"/> by default
		/// </value>
        /// <version>1.0</version>
        /// <default>0</default>
        [ReflectorProperty("baseline", Required = false)]
		public bool BaseliningEnabled = false;

		/// <summary>
		/// If true, resets the reconfigure properties for this project and all subprojects to use the reconfigure template.
		/// </summary>
		/// <remarks>
		/// Do not set to true if you have manually set the template to reconfigure with specific settings (e.g., reconfigure by object status, with a manually added set
        /// of tasks, etc).
		/// </remarks>
		/// <value>
		///     <see langword="false"/> by default
		/// </value>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("template", Required = false)]
		public bool TemplateEnabled = false;

		/// <summary>
		/// If enabled, updates the work area from the database, discarding all uncontrolled files in the work area and changes to static objects.
		/// </summary>
		/// <remarks>
		/// Useful if your build process adds or modifies files in the source tree.
		/// </remarks>
		/// <value>
		///     <see langword="false"/> by default
		/// </value>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("reconcile", Required = false)]
		public string[] ReconcilePaths;

		/// <summary>
		/// Synergy purpose specification for the project and any created baselines.
		/// </summary>
		/// <value>
		/// Defaults to <c>Integration Testing</c>.
		/// </value>
        /// <version>1.0</version>
        /// <default>Integration Testing</default>
        [ReflectorProperty("purpose", Required = false)]
		public string Purpose = DefaultPurpose;

		/// <summary>
		///     The timestamp of when the integration project was last reconfigured.
		///     Tracks the last time the project was reconfigured, to ensure that 
		///     an out-of-process reconfigure was not executed.
		/// </summary>
		/// <remarks>
		///     This is important to guarantee that an external user/process has not reconfigured
		///     the project during the current CCNET integration run.
		///     
		///     <note type="implementnotes">
		///         There exists potential more a more robust implementation, via calls to
		///         <c>ccm accent base_asm_lock [compver] [reason string, "Locked by CCNET"]</c>
		///         and 
		///         <c>ccm accent base_asm_unlock [compver]</c> in a try/finally block.
		///         <para />
		///         This would leverage CM Synergy's provision for queuing access to a project
		///         across multiple processes using the same database.  However, this could be 
		///         dangerous if the CCNET process were killed before a project was unlocked.
		///         Perhaps that could be handled by an AppDomain.Unload delegate.
		///         <para />
		///         For more information, see the CM Synergy Advanced Customization Guide.
		///     </note>
		/// </remarks>
		/// <value>
		///     Defaults to <see cref="DateTime.MinValue" />.
		/// </value>
		public DateTime LastReconfigureTime = DateTime.MinValue;
	}
}