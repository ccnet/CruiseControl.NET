namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Xml;

	/// <summary>
	/// Parses <see cref="MercurialModification"/> objects from a <see cref="Mercurial"/> change log.
	/// </summary>
	public class MercurialHistoryParser : IHistoryParser
	{
		public Modification[] Parse(TextReader history, DateTime from, DateTime to)
		{
			var mods = new List<Modification>();

			var doc = new XmlDocument();
			try
			{
				doc.LoadXml(history.ReadToEnd());
			}
			catch( Exception ex )
			{
				throw new CruiseControlException("Unable to parse Mercurial history. Expected to get XML from the `hg log --style xml -v` command.", ex);
			}

			var logEntries = doc.SelectNodes("/log/logentry");
			if( logEntries == null )
			{
				return mods.ToArray();
			}

			foreach (XmlElement logEntry in logEntries)
			{
				var revision = 0;
				var revisionAttr = logEntry.Attributes["revision"];
				if( revisionAttr != null )
				{
					Int32.TryParse(revisionAttr.Value, out revision);
				}

				var nodeAttr = logEntry.Attributes["node"];
				var node = "";
				if( nodeAttr != null )
				{
					node = nodeAttr.Value;
				}

				var authorNode = logEntry["author"];
				string email = "", username = "";
				if (authorNode != null)
				{
					email = authorNode.Attributes["email"].Value;
					if (!string.IsNullOrEmpty(authorNode.InnerText))
					{
						username = authorNode.InnerText;
					}
					else
					{
						username = (email.Contains("@"))
							? email.Substring(0, email.IndexOf('@'))
							: email;
					}
				}

				var dateNode = logEntry["date"];
				DateTime modifiedAt = DateTime.MinValue;
				if (dateNode != null)
				{
					DateTime.TryParse(dateNode.InnerText, out modifiedAt);
				}

				var commentNode = logEntry["msg"];
				var comment = "";
				if( commentNode != null )
				{
					comment = commentNode.InnerText;
				}

				var pathsNode = logEntry["paths"];
				if (pathsNode == null || pathsNode.ChildNodes.Count == 0)
				{
					continue;
				}

				foreach (XmlElement fileNode in pathsNode.ChildNodes)
				{
					var path = fileNode.InnerText;

					var mod = new MercurialModification
					{
						ChangeNumber = revision,
						Comment = comment,
						EmailAddress = email,
						FileName = Path.GetFileName(path),
						FolderName = Path.GetDirectoryName(path),
						ModifiedTime = modifiedAt,
						UserName = username,
						Version = node
					};
					mods.Add(mod);
				}
			}
			return mods.ToArray();
		}
	}
}
