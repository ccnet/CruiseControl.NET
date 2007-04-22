using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	/// <summary>
	/// Contains metadata stored in the .Tag property of a TreeView node for the Integration Queue tree.
	/// </summary>
	public class IntegrationQueueTreeNodeTag
	{
		public readonly IntegrationQueueTreeNodeAdaptor Adaptor;
		public readonly QueueSnapshot QueueSnapshot;
		public readonly QueuedRequestSnapshot QueuedRequestSnapshot;
		public readonly int QueueIndex;

		public IntegrationQueueTreeNodeTag(IntegrationQueueTreeNodeAdaptor adaptor)
			: this(adaptor, null, null, -1)
		{
		}

		public IntegrationQueueTreeNodeTag(IntegrationQueueTreeNodeAdaptor adaptor, QueueSnapshot queueSnapshot)
			: this(adaptor, queueSnapshot, null, -1)
		{
		}

		public IntegrationQueueTreeNodeTag(IntegrationQueueTreeNodeAdaptor adaptor, QueueSnapshot queueSnapshot, 
			QueuedRequestSnapshot queuedRequestSnapshot, int queueIndex)
		{
			this.Adaptor = adaptor;
			this.QueueSnapshot = queueSnapshot;
			this.QueuedRequestSnapshot = queuedRequestSnapshot;
			this.QueueIndex = queueIndex;
		}

		public bool IsServerNode
		{
			get { return QueueSnapshot == null; }
		}

		public bool IsQueueNode
		{
			get { return QueueSnapshot != null && QueuedRequestSnapshot == null; }
		}

		public bool IsQueuedItemNode
		{
			get { return QueuedRequestSnapshot != null; }
		}

		public bool IsFirstItemOnQueue
		{
			get { return IsQueuedItemNode && QueueIndex == 0; }
		}
	}
}
