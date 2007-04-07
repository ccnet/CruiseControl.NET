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
		public readonly NamedQueueSnapshot NamedQueueSnapshot;
		public readonly QueuedItemSnapshot QueuedItemSnapshot;
		public readonly int QueueIndex;

		public IntegrationQueueTreeNodeTag(IntegrationQueueTreeNodeAdaptor adaptor)
			: this(adaptor, null, null, -1)
		{
		}

		public IntegrationQueueTreeNodeTag(IntegrationQueueTreeNodeAdaptor adaptor, NamedQueueSnapshot namedQueueSnapshot)
			: this(adaptor, namedQueueSnapshot, null, -1)
		{
		}

		public IntegrationQueueTreeNodeTag(IntegrationQueueTreeNodeAdaptor adaptor, NamedQueueSnapshot namedQueueSnapshot, 
			QueuedItemSnapshot queuedItemSnapshot, int queueIndex)
		{
			this.Adaptor = adaptor;
			this.NamedQueueSnapshot = namedQueueSnapshot;
			this.QueuedItemSnapshot = queuedItemSnapshot;
			this.QueueIndex = queueIndex;
		}

		public bool IsServerNode
		{
			get { return NamedQueueSnapshot == null; }
		}

		public bool IsNamedQueueNode
		{
			get { return NamedQueueSnapshot != null && QueuedItemSnapshot == null; }
		}

		public bool IsQueuedItemNode
		{
			get { return QueuedItemSnapshot != null; }
		}

		public bool IsFirstItemOnQueue
		{
			get { return IsQueuedItemNode && QueueIndex == 0; }
		}
	}
}
