using System;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A date/time that can be serialised.
    /// </summary>
	[Serializable]
	public class SerializableDateTime
	{
		private long ticks;

        /// <summary>
        /// Initialise a new <see cref="SerializableDateTime"/>.
        /// </summary>
        /// <param name="dateTime"></param>
		public SerializableDateTime(DateTime dateTime)
		{
			ticks = dateTime.Ticks;
		}

        /// <summary>
        /// The serialised date/time.
        /// </summary>
		public DateTime DateTime
		{
			get { return new DateTime(ticks); }
		}

        /// <summary>
        /// The default date/time.
        /// </summary>
		public static SerializableDateTime Default = new SerializableDateTime(DateTime.MinValue);
	}
}