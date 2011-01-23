
using System.Xml;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Timeout configuration.
    /// </summary>
    /// <title>Timeout Configuration</title>
	public class Timeout
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public static Timeout DefaultTimeout = new Timeout(600000);

		private int timeout;
		private TimeUnits unit = TimeUnits.MILLIS;

        /// <summary>
        /// Normalizes this instance.	
        /// </summary>
        /// <remarks></remarks>
      public void Normalize()
      {
         int millisecs = this.Millis;
         if (millisecs % 1000 == 0)
         {
            unit = TimeUnits.SECONDS;
            timeout = millisecs / 1000;

            if (timeout % 60 == 0)
            {
               unit = TimeUnits.MINUTES;
               timeout /= 60;

               if (timeout % 60 == 0)
               {
                  unit = TimeUnits.HOURS;
                  timeout /= 60;
               }
            }
         }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="Timeout" /> class.	
      /// </summary>
      /// <param name="periodInMillis">The period in millis.</param>
      /// <remarks></remarks>
		public Timeout(int periodInMillis) : this(periodInMillis, TimeUnits.MILLIS)
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="Timeout" /> class.	
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="unit">The unit.</param>
        /// <remarks></remarks>
		public Timeout(int period, TimeUnits unit)
		{
			this.timeout = period;
			if (unit != null) this.unit = unit;
		}

        /// <summary>
        /// Gets the time units.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public TimeUnits TimeUnits
		{
			get { return unit; }
		}

        /// <summary>
        /// Gets the millis.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public int Millis
		{
			get { return unit.ToMillis(timeout); }
		}

        /// <summary>
        /// Equalses the specified obj.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public override bool Equals(object obj)
		{
			if (obj == this) return true;
			Timeout other = obj as Timeout;
			if (obj == null) return false;
			return this.Millis == other.Millis;
		}

        /// <summary>
        /// Gets the hash code.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override int GetHashCode()
		{
			return Millis;
		}

        /// <summary>
        /// Writes the specified writer.	
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <remarks></remarks>
		public void Write(XmlWriter writer)
		{
			writer.WriteStartElement("timeout");
			if (!unit.Equals(TimeUnits.MILLIS))
			{
				writer.WriteAttributeString("units", unit.ToString());
			}
			writer.WriteString(timeout.ToString(CultureInfo.CurrentCulture));
			writer.WriteEndElement();
		}

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override string ToString()
		{
			return timeout + " " + unit.ToString();
		}
	}
}