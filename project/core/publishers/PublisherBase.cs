using System;
using System.IO;
using System.Xml;

namespace tw.ccnet.core.publishers 
{
	public abstract class PublisherBase : IIntegrationEventHandler
	{
		private IntegrationEventHandler _publish; 

		public PublisherBase()
		{
			_publish = new IntegrationEventHandler(Publish);
		}
		
		public IntegrationEventHandler IntegrationEventHandler 
		{
			get { return _publish;}
		}
		
		public abstract void Publish(object source, IntegrationResult result);
	}
}


