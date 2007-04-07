
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
		/// This is a named queue
		/// </summary>
		public static readonly IntegrationQueueNodeType Queue = new IntegrationQueueNodeType("Queue", 2);

		/// <summary>
		/// This is first item in the queue and likely currently integrating
		/// </summary>
		public static readonly IntegrationQueueNodeType FirstInQueue = new IntegrationQueueNodeType("FirstInQueue", 3);

		/// <summary>
		/// This is pending integration
		/// </summary>
		public static readonly IntegrationQueueNodeType PendingInQueue = new IntegrationQueueNodeType("PendingInQueue", 4);

		public readonly string Name;
		public readonly int ImageIndex;

		private IntegrationQueueNodeType( string name, int imageIndex )
		{
			Name = name;
			ImageIndex = imageIndex;
		}
	}
}
