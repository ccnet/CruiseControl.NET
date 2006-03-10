using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	[Serializable]
	public class SerializableDateTime
	{
		private long ticks;

		public SerializableDateTime(DateTime dateTime)
		{
			ticks = dateTime.Ticks;
		}

		public DateTime DateTime
		{
			get { return new DateTime(ticks); }
		}

		public readonly static SerializableDateTime Default = new SerializableDateTime(DateTime.MinValue);
	}
}