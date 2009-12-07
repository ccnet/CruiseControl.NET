using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// Defines a user who will receive e-mails.
    /// </summary>
    /// <title>Email User</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;user name="BuildGuru" group="buildmaster" address="buildguru@mycompany.com" /&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// Users do not need to belong to a group. If they are not in a group then they may still receive emails when they
    /// have committed changes that are part of the current build, depending on the setting of
    /// "modifierNotificationTypes" and the state of the build.
    /// </para>
    /// <para>
    /// See the section on the &lt;converters&gt; setting for manipulations that can be done to transform a user name
    /// to an address if the address is not specified.
    /// </para>
    /// <para type="warning">
    /// It is essential that the value of the name attribute matches the name for the user in the sourcecontrol system.
    /// This is the only way that CruiseControl.Net can reconcile the user that committed a change with the address to
    /// send the email to.
    /// </para>
    /// </remarks>
	[ReflectorType("user")]
	public class EmailUser
	{
		public EmailUser() { }

		public EmailUser(string name, string group, string address)
		{
			Name = name;
			Address = address;
			Group = group;
		}

        /// <summary>
        /// The user name of a user. This should match the user name in Source Control. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
		[ReflectorProperty("name")]
		public string Name;

        /// <summary>
        /// The Internet-style email address of the user (e.g., "joe@example.com").
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("address")]
		public string Address;

        /// <summary>
        /// The group that the user is in. This needs to match the name of one of the &lt;group&gt; elements.
        /// </summary>
        /// <version>1.3</version>
        /// <default>None</default>
        [ReflectorProperty("group", Required = false)]
		public string Group;

		public override bool Equals(Object obj)
		{
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			EmailUser user = (EmailUser)obj;
			return (user.Name == Name && user.Address == Address && user.Group == Group);
		}

		public override int GetHashCode()
		{
			return String.Concat(Name, Address, Group).GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("Email User: {0} {1} {2}", Name, Address, Group);
		}
	}
}