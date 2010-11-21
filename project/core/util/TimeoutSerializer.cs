
using System;
using System.Xml;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
    public class TimeoutSerializer : XmlMemberSerialiser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutSerializer" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="attribute">The attribute.</param>
        /// <remarks></remarks>
        public TimeoutSerializer(ReflectorMember info, ReflectorPropertyAttribute attribute)
            : base(info, attribute)
        { }

        /// <summary>
        /// Writes the specified writer.	
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="target">The target.</param>
        /// <remarks></remarks>
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

        /// <summary>
        /// Reads the specified node.	
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="types">The types.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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
            XmlAttribute a = node as XmlAttribute;

            if (a != null)
            {
                try
                {
                    timeout = new Timeout(Int32.Parse(a.Value, CultureInfo.CurrentCulture));
                }
                catch (Exception)
                {
                    Log.Warning("Could not parse timeout string. Using default timeout.");
                }
            }
            else
            {
                var e = node as XmlElement;
                if (e != null)
                {
                    try
                    {
                        TimeUnits units = TimeUnits.MILLIS;
                        string unitsString = e.GetAttribute("units");
                        if (unitsString != null && !(unitsString != null && unitsString.Length == 0))
                        {
                            units = TimeUnits.Parse(unitsString);
                        }
                        timeout = new Timeout(Int32.Parse(e.InnerText, CultureInfo.CurrentCulture), units);
                    }
                    catch (Exception)
                    {
                        Log.Warning("Could not parse timeout string. Using default timeout.");
                    }
                }
            }
            return timeout;
        }
    }
}