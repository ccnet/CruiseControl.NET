using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Defines a source control block.
    /// </summary>
    /// <title>Source Control Blocks</title>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ISourceControl
	{
        /// <summary>
        /// Gets the modifications from the source control provider
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
		Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to);

        /// <summary>
        /// Labels the source control provider with the current label
        /// </summary>
        /// <param name="result"></param>
		void LabelSourceControl(IIntegrationResult result);
		
        /// <summary>
        /// Gets the source from the source conrol provider
        /// </summary>
        /// <param name="result"></param>
        void GetSource(IIntegrationResult result);

        /// <summary>
        /// Initializes the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <remarks></remarks>
		void Initialize(IProject project);
        /// <summary>
        /// Purges the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <remarks></remarks>
		void Purge(IProject project);
	}
}