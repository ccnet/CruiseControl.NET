using System;
using System.Runtime.Serialization;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Remote
{
	[Serializable]
	public class NoSuchProjectException : CruiseControlException
	{
		private readonly string requestedProject;

		public NoSuchProjectException() : base(ExceptionMessage(string.Empty)) {}

		public NoSuchProjectException(string requestedProject) : base(ExceptionMessage(requestedProject))
		{
			this.requestedProject = requestedProject;
		}
		
		public NoSuchProjectException(string requestedProject, Exception e) : base(ExceptionMessage(requestedProject), e)
		{
			this.requestedProject = requestedProject;			
		}
		
		public NoSuchProjectException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		public string RequestedProject
		{
			get { return requestedProject; }
		}

		private static string ExceptionMessage(string project)
		{
			return string.Format("The project: {0} does not exist on the CCNet server.", project);
		}
	}
}