using System;
using System.IO;
using System.Xml;

namespace tw.ccnet.core.publishers 
{
	public abstract class PublisherBase : IIntegrationCompletedEventHandler
	{
		private IntegrationCompletedEventHandler _publish; 

		public PublisherBase()
		{
			_publish = new IntegrationCompletedEventHandler(Publish);
		}
		
		public IntegrationCompletedEventHandler IntegrationCompletedEventHandler 
		{
			get { return _publish;}
		}
		
		public abstract void Publish(object source, IntegrationResult result);
	}
}


