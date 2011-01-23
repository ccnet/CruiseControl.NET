
namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// A summary of the integration queue node type as interesting to cctray
	/// </summary>
	public class IntegrationQueueNodeType
	{
		/// <summary>
		///  This is a Remoting server
		/// </summary>
		public static readonly IntegrationQueueNodeType RemotingServer = new IntegrationQueueNodeType("RemotingServer", 0);

		/// <summary>
		///  This is an HTTP server
		/// </summary>
		public static readonly IntegrationQueueNodeType HttpServer = new IntegrationQueueNodeType("HttpServer", 1);

		/// <summary>
		/// This is a queue with no requests on it
		/// </summary>
		public static readonly IntegrationQueueNodeType QueueEmpty = new IntegrationQueueNodeType("QueueEmpty", 2);

        /// <summary>
        /// This is a queue with some requests on it
        /// </summary>
        public static readonly IntegrationQueueNodeType QueuePopulated = new IntegrationQueueNodeType("QueuePopulated", 3);

        /// <summary>
        /// This is first item in the queue but is just checking for modifications
        /// </summary>
        public static readonly IntegrationQueueNodeType CheckingModifications = new IntegrationQueueNodeType("CheckingModifications", 4);

		/// <summary>
		/// This is first item in the queue and is currently building
		/// </summary>
		public static readonly IntegrationQueueNodeType Building = new IntegrationQueueNodeType("Building", 5);

		/// <summary>
		/// This is pending integration
		/// </summary>
		public static readonly IntegrationQueueNodeType PendingInQueue = new IntegrationQueueNodeType("PendingInQueue", 6);

		public readonly string Name;
		public readonly int ImageIndex;

		private IntegrationQueueNodeType( string name, int imageIndex )
		{
			Name = name;
			ImageIndex = imageIndex;
		}
	}
}
