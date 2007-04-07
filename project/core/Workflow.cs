using System;
using System.Collections;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// A generic project contains a collection of tasks.  It will execute them in the specified order.  It is possible to have multiple tasks of the same type.
	/// <code>
	/// <![CDATA[
	/// <workflow name="foo">
	///		<tasks>
	///			<sourcecontrol type="cvs"></sourcecontrol>
	///			<build type="nant"></build>
	///		</tasks>
	///		<state type="state"></state>
	/// </workflow>
	/// ]]>
	/// </code>
	/// </summary>
	[ReflectorType("workflow")]
	public class Workflow : ProjectBase, IProject
	{
		private IList _tasks = new ArrayList();
		private WorkflowResult _currentIntegrationResult;

		[ReflectorCollection("tasks", InstanceType = typeof(ArrayList))]
		public IList Tasks
		{
			get { return _tasks; }
			set { _tasks = value; }
		}

		public IntegrationResult CurrentIntegration
		{
			get { return _currentIntegrationResult; }
		}

		public IIntegrationResult Integrate(IntegrationRequest request)
		{
			_currentIntegrationResult = new WorkflowResult();

			foreach (ITask task in Tasks)
			{
				try 
				{ 
					RunTask(task); 
				}
				catch (CruiseControlException ex) 
				{
					_currentIntegrationResult.ExceptionResult = ex;
				}
			}
			return _currentIntegrationResult;
		}

		public void NotifyPendingState()
		{
			throw new NotImplementedException();
		}

		public void NotifySleepingState()
		{
			throw new NotImplementedException();
		}

		private void RunTask(ITask task)
		{
			task.Run(_currentIntegrationResult);
		}
		
		public IntegrationStatus LatestBuildStatus
		{
			get { return _currentIntegrationResult.Status; }
		}

		public void Purge(bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			return;
		}

		public XmlDocument Statistics
		{
			get { throw new NotImplementedException(); }
		}

		public IIntegrationRepository IntegrationRepository
		{
			get { throw new NotImplementedException(); }
		}

		public string QueueName
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public int QueuePriority
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public void Initialize()
		{
			throw new NotImplementedException();
		}

		public ProjectStatus CreateProjectStatus(IProjectIntegrator integrator)
		{
			throw new NotImplementedException();
		}

		public void AddMessage(Message message)
		{
			throw new NotImplementedException();
		}

		public int MinimumSleepTimeMillis 
		{ 
			get { return 0; }
		}

		public string WebURL 
		{ 
			get { return ""; }
		}
	}
}
