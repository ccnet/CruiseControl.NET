using System;
using System.Xml;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    public class TimeoutSerializer : XmlMemberSerialiser
    {
        public TimeoutSerializer(ReflectorMember info, ReflectorPropertyAttribute attribute)
            : base(info, attribute)
        { }

        public override void Write(XmlWriter writer, object target)
        {
            if (target == null) return;
            if (!(target is Timeout)) target = ReflectorMember.GetValue(target);

            Timeout to = target as Timeout;
            if (to != null)
            {
                to.Write(writer);
            }
        }

        public override object Read(XmlNode node, NetReflectorTypeTable types)
        {
            if (node == null)
            {
                // NetReflector should do this check, but doesn't
                if (this.Attribute.Required)
                {
                    throw new NetReflectorItemRequiredException(Attribute.Name + " is required");
                }
                else
                {
                    return null;
                }
            }

            Timeout timeout = Timeout.DefaultTimeout;
            if (node is XmlAttribute)
            {
                XmlAttribute a = (XmlAttribute)node;
                try
                {
                    timeout = new Timeout(Int32.Parse(a.Value));
                }
                catch (Exception)
                {
                    Log.Warning("Could not parse timeout string. Using default timeout.");
                }
            }
            else if (node is XmlElement)
            {
                XmlElement e = (XmlElement)node;
                try
                {
                    TimeUnits units = TimeUnits.MILLIS;
                    string unitsString = e.GetAttribute("units");
                    if (unitsString != null && unitsString != string.Empty)
                    {
                        units = TimeUnits.Parse(unitsString);
                    }
                    timeout = new Timeout(Int32.Parse(e.InnerText), units);
                }
                catch (Exception)
                {
                    Log.Warning("Could not parse timeout string. Using default timeout.");
                }
            }
            return timeout;
        }
    }
}