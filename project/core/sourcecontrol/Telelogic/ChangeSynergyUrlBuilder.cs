using System;
using System.Globalization;
using System.Text;
using System.Web;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Telelogic
{
	/// <summary>
	///     A URL builder to link each modification to the ChangeSynergy task details form.
	/// </summary>
	/// <remarks>
	///     <note type="caution">
	///         If you specify values for the optional properties <see cref="Username"/> and
	///         <see cref="Password"/>, make sure that the user has read-only permissions within
	///         your ChangeSynergy lifecycle definition.  This is necessary, since the Uri for 
	///         each modification will allow anonymous access to ChangeSynergy, possibily exposing
	///         vulnerabilities for spoofing, tampering, repudiation, information disclosure,
	///         and/or escalation of priveledge.
	///         <para />
	///         That is, a STRIDE classification of "STRDE", with a possible DREAD rating as 
	///         high as 10 if permissions are inappropriatedly assigned to the anonymous account.
	///     </note>
	///     If you do not specify a <see cref="Username"/>/<see cref="Password"/>, the end-user
	///     will be prompted to ChangeSynergy to login.  However, the login screen will not correctly
	///     populate the <c>database</c> and <c>role</c> select inputs.  This is due to a documented
	///     bug in ChangeSynergy 4.3 SP4.  The bug case tracking number is 2067637;
	///     the change request is R21683.
	/// </remarks>
	/// <include file="example.xml" path="/example" />
	[ReflectorType("changeSynergy")]
	public class ChangeSynergyUrlBuilder : IModificationUrlBuilder
	{
		private string username;
		private string obfuscatedPassword;

		/// <summary>
		///     Network path to the Synergy database instance
		/// </summary>
		[ReflectorProperty("database", Required=false)]
		public string Database;

		/// <summary>
		///     The username to use for ChangeSynergy access.
		///     Can include environmental variables to be replaced.
		/// </summary>
		/// <remarks>
		///     The ChangeSynergy username should be different from the one specified for the
		///     CM Synergy server.  Ideally, you should specify a user with read-only permissions
		///     for ChangeSynergy.  This will prevent someone from modifying objects through
		///     ChangeSynergy.  If you specify an impersonation account with write permissions,
		///     a malicious user could bypass auditing in ChangeSynergy.
		/// </remarks>
		/// <value>
		///     Defaults to <see langword="null" />, which implies that the end-user will
		///     be prompted for thier ChangeSynergy logon credentials.
		/// </value>
		[ReflectorProperty("username", Required=false)]
		public string Username
		{
			get { return username; }
			set
			{
				if (null == value)
				{
					username = null;
				}
				else
				{
					username = Environment.ExpandEnvironmentVariables(value);
				}
			}
		}

		/// <summary>
		///     The Synergy password for the associate <see cref="Username"/> value.
		/// </summary>
		/// <remarks>
		///     Support environment variable expansion.
		/// </remarks>
		/// <value>
		///     Defaults to <see langword="null" />, which implies that the end-user will
		///     be prompted for theIr ChangeSynergy logon credentials.
		/// </value>
		[ReflectorProperty("password", Required=false)]
		public string Password
		{
			get { return obfuscatedPassword; }
			set
			{
				if (null == value)
				{
					obfuscatedPassword = null;
				}
				else
				{
					obfuscatedPassword = Environment.ExpandEnvironmentVariables(value);
					obfuscatedPassword = ObfuscatePassword(obfuscatedPassword);
				}
			}
		}

		/// <summary>
		///     The role to use for the Synergy session.
		/// </summary>
		/// <remarks>
		///     <note type="caution">
		///         If <see cref="Username"/> is specified to allow anonymous access to ChangeSynergy,
		///         you should specify a role with minimum read-only permissions.
		///     </note>
		/// </remarks>
		/// <value>
		///     Defaults to <c>User</c>.
		/// </value>
		[ReflectorProperty("role", Required=false)]
		public string Role;

		/// <summary>
		///     The root path to the ChangeSynergy installation.
		/// </summary>
		/// <example>
		///     <c>http://myserver:8600</c>
		/// </example>
		/// <value>
		///     This should be the protocol scheme, server hostname, and optionally any
		///     port number and root directory information.
		/// </value>
		[ReflectorProperty("url")]
		public string Url;

		/// <summary>
		///     Copies the database path from the CM Synergy session for use with the
		///     ChangeSynergy URLs.
		/// </summary>
		/// <param name="connection">
		///     The CM Synergy database to use as for the ChangeSynergy logon information.
		/// </param>
		public void SetCredentials(SynergyConnectionInfo connection)
		{
			if (null == Database || 0 == Database.Length)
				Database = connection.Database;
		}

		/// <summary>
		///     Returns a formatted URL to access the TaskDetails form for the task
		///     associated with the modification in ChangeSynergy.
		/// </summary>
		/// <param name="modifications">The array of modified files for this integration.</param>
		public void SetupModification(Modification[] modifications)
		{
			const string trustedUrl = @"{0}/servlet/com.continuus.webpt.servlet.PTweb?ACTION_FLAG=frameset_form&TEMPLATE_FLAG=TaskDetailsView" + @"&task_number={{0}}&role={1}&database={2}&user={3}&generic_data={4}";
			const string loginUrl = @"{0}/servlet/com.continuus.webpt.servlet.PTweb?" + @"ACTION_FLAG=tokenless_form&TEMPLATE_FLAG=ConfirmLogin" + @"&role={1}&database={2}&context=";
			const string loginQuery = @"/servlet/com.continuus.webpt.servlet.PTweb?ACTION_FLAG=frameset_form&TEMPLATE_FLAG=TaskDetailsView" + @"&role={0}&database={1}&task_number=";

			string taskUrl;
			string taskQuery;

			if (null != username && 0 != username.Length)
			{
				// cache the information that doesn't change between tasks;
				// the format item "{{0}}" will be escaped to "{0}"
				taskUrl = String.Format(trustedUrl, Url, HttpUtility.UrlEncode(Role), HttpUtility.UrlEncode(Database), HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(obfuscatedPassword));
			}
			else
			{
				// we have to first UrlEncode any of the redirection url querystring parameters
				taskQuery = String.Format(loginQuery, HttpUtility.UrlEncode(Role), HttpUtility.UrlEncode(Database));

				// then UrlEncode the resulting redirection querystring
				taskQuery = HttpUtility.UrlEncode(taskQuery);

				// create the login url
				taskUrl = String.Format(loginUrl, Url, HttpUtility.UrlEncode(Role), HttpUtility.UrlEncode(Database));

				// Concatenate the login url and redirection url querystring.
				// Append the "{0}", as UrlEncode would encode it as "%7b0%7d"
				taskUrl = String.Concat(taskUrl, taskQuery, "{0}");
			}

			foreach (Modification modification in modifications)
			{
				modification.Url = String.Format(taskUrl, modification.ChangeNumber);
			}
		}

		/// <overloads>
		///     <summary>
		///         Obfuscates the password as used by the ChangeSynergy servlets
		///     </summary>
		///     <remarks>
		///         This implementation is a reverse engineering product of the ChangeSynergy
		///         Javascript functions <c>encodePassword</c> and <c>getSeed</c>.
		///         <para />
		///         It is clearly from an *undocumented* Telelogic API.
		///         The source for this API is easily available in clear text
		///         to anyone with a web browser and access to ChangeSynergy.
		///     </remarks>
		///     <param name="password">The plaintext password to obfuscate.</param>
		///     <returns>
		///         An obfuscated string containing the original <paramref name="password" />.
		///     </returns>
		/// </overloads>
		/// <param name="seed">
		///     An integer between 0 and 1000, inclusive.
		///     This can be passed externally for the purpose of unit testing.
		/// </param>
		public string ObfuscatePassword(int seed, string password)
		{
			StringBuilder obfuscatedValue;
			int asciiCode;

			if (null == password)
			{
				return (null);
			}

			obfuscatedValue = new StringBuilder(password.Length*4);

			obfuscatedValue.Append(seed.ToString(CultureInfo.InvariantCulture));
			obfuscatedValue.Append(":");

			/* Create a comma separated list of the product of multiplier seed and the
             * ascii code for each character in the password.  
             * The multipler value (aka seed) is prepended to the list. */
			foreach (char c in password)
			{
				asciiCode = ((int) c)*seed;

				obfuscatedValue.Append(asciiCode.ToString(CultureInfo.InvariantCulture));
				obfuscatedValue.Append(",");
			}

			// strip the trailing comma
			obfuscatedValue.Remove(obfuscatedValue.Length - 1, 1);

			return (obfuscatedValue.ToString());
		}

		// see previous overload for documentation
		private string ObfuscatePassword(string password)
		{
			Random r = new Random();
			int seed = r.Next(0, 1000);

			return (ObfuscatePassword(seed, password));
		}
	}
}