
using System;
using System.Runtime.Serialization;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Remote
{
 /// <summary>
 /// Specified project does not exist on the buildserver
 /// </summary>
    [Serializable]
	public class NoSuchProjectException : CruiseControlException
	{
        private const string requestedProjectData = "REQUESTEDPROJECT_NAME";
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
		
		public NoSuchProjectException(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            this.requestedProject = info.GetString(requestedProjectData);
        }

		public string RequestedProject
		{
			get { return requestedProject; }
		}

		private static string ExceptionMessage(string project)
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture,"The project '{0}' does not exist on the CCNet server.", project);
		}

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(requestedProjectData, requestedProject);
        }
	}
}