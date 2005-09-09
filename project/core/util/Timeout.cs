using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class Timeout
	{

		public static Timeout DefaultTimeout= new Timeout(600000);

		private int timeout;
		private TimeUnits unit = TimeUnits.MILLIS;

		public Timeout(int periodInMillis) :  this(periodInMillis, TimeUnits.MILLIS)
		{
		}

		public Timeout(int period, TimeUnits unit)
		{
			this.timeout = period;
			if (unit!=null) this.unit = unit;
		}

		public TimeUnits TimeUnits
		{
			get { return unit; }
		}

		public int Millis
		{
			get { return unit.ToMillis(timeout); }
		}

		public override bool Equals(object obj)
		{
			if (obj==this) return true;
			
			Timeout other = obj as Timeout;
			
			if (obj==null) return false;
			
			return this.Millis == other.Millis;
		}
		
		public override int GetHashCode()
		{
			return Millis;
		}

		public void write(XmlWriter writer)
		{
			writer.WriteStartElement("timeout");
			if ( !unit.Equals(TimeUnits.MILLIS) )
			{
				writer.WriteAttributeString("units", unit.ToString());
			}
			writer.WriteString(timeout.ToString());
			writer.WriteEndElement();
		}


		public override string ToString()
		{
			return timeout + " " + unit.ToString();
		}
	}


}
