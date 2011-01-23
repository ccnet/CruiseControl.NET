
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

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSuchProjectException" /> class.	
        /// </summary>
        /// <remarks></remarks>
		public NoSuchProjectException() : base(ExceptionMessage(string.Empty)) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSuchProjectException" /> class.	
        /// </summary>
        /// <param name="requestedProject">The requested project.</param>
        /// <remarks></remarks>
		public NoSuchProjectException(string requestedProject) : base(ExceptionMessage(requestedProject))
		{
			this.requestedProject = requestedProject;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSuchProjectException" /> class.	
        /// </summary>
        /// <param name="requestedProject">The requested project.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
		public NoSuchProjectException(string requestedProject, Exception e) : base(ExceptionMessage(requestedProject), e)
		{
			this.requestedProject = requestedProject;			
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSuchProjectException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
		public NoSuchProjectException(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            this.requestedProject = info.GetString(requestedProjectData);
        }

        /// <summary>
        /// Gets the requested project.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public string RequestedProject
		{
			get { return requestedProject; }
		}

		private static string ExceptionMessage(string project)
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture,"The project '{0}' does not exist on the CCNet server.", project);
		}

        /// <summary>
        /// Gets the object data.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(requestedProjectData, requestedProject);
        }
	}
}