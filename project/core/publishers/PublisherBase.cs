using System;
using System.IO;
using System.Xml;

using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers 
{
	/// <summary>
	/// Base class for all CruiseControl.NET build result publishers.
	/// </summary>
	public abstract class PublisherBase : IIntegrationCompletedEventHandler
	{
		private IntegrationCompletedEventHandler _publish;

		public PublisherBase()
		{
			_publish = new IntegrationCompletedEventHandler(Project_IntegrationCompleted);
		}

		public IntegrationCompletedEventHandler IntegrationCompletedEventHandler 
		{
			get 
			{
				return _publish;
			}
		}

		/// <summary>
		/// The handler for the <see cref="IntegrationCompleted"/> event on IProject.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void Project_IntegrationCompleted(object source, IntegrationCompletedEventArgs e)
		{
			try 
			{
				PublishIntegrationResults((IProject)source, e.IntegrationResult);
			}
			catch (Exception ex)
			{
				// TODO what do we do with these exceptions (apart from log them)???
				Log.Warning(new CruiseControlException("Exception thrown by publisher", ex));
			}
		}

		/// <summary>
		/// Performs all actions for this publisher implementation.
		/// </summary>
		/// <param name="project">The project from which results originated.</param>
		/// <param name="result">The result of this integration.</param>
		public abstract void PublishIntegrationResults(IProject project, IntegrationResult result);
	}
}
