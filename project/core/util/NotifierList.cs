using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public delegate void NotifierDelegate(object value);

	public class NotifierList : ArrayList
	{
		private event NotifierDelegate _addEvent;
		private event NotifierDelegate _removeEvent;

		public virtual void AddDelegateForAddEvent(NotifierDelegate handler)
		{
			_addEvent += handler;
		}

		public virtual void AddDelegateForRemoveEvent(NotifierDelegate handler)
		{
			_removeEvent += handler;
		}

		public override int Add(object value)
		{
			int index = base.Add(value);
			SendNotification(_addEvent, value);
			return index;
		}

		public override void Remove(object value)
		{
			base.Remove(value);
			SendNotification(_removeEvent, value);
		}

		private void SendNotification(NotifierDelegate handler, object value)
		{
			if (handler != null) 
			{
				handler(value);
			}
		}

	}
}
