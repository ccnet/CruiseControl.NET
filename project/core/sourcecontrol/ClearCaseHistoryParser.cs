using System;
using System.Collections;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// Provides for parsing output from a ClearCase cleartool.exe lshist command.
	/// </summary>
	/// <remarks>
	/// Written by Garrett M. Smith (gsmith@thoughtworks.com).
	/// Parsing logic inspired from (but improved upon) CruiseControl for Java.
	/// </remarks>
	public class ClearCaseHistoryParser : IHistoryParser
	{
		
		/// <summary>
		/// Unlikely combination of characters to separate fields
		/// </summary>
		public readonly static string DELIMITER = "#~#";
		
		/// <summary>
		/// Unlikely combination of characters to indicate end of one line in query.
		/// Carriage return (\n) may be used in comments and so is not available to us.
		/// </summary>
		public readonly static string END_OF_LINE_DELIMITER = "@#@#@#@#@#@#@#@#@#@#@#@";
		
		public Modification[] Parse( TextReader history, DateTime from, DateTime to )
		{
			return ParseStream( history );
		}

		public void AssignFileInfo( Modification modification, string file )
		{
			int separatorLocation = file.LastIndexOf( Path.DirectorySeparatorChar.ToString() );
			if ( separatorLocation > - 1 )
			{
				modification.FolderName = file.Substring( 0, separatorLocation );
				modification.FileName = file.Substring( separatorLocation + 1 );
			}
			else
			{
				modification.FolderName = string.Empty;
				modification.FileName = file;
			}
		}

		public void AssignModificationTime( Modification modification, string time )
		{
			try
			{
				modification.ModifiedTime = DateTime.Parse( time );
			}
			catch ( FormatException )
			{
				modification.ModifiedTime = DateTime.MinValue;
			}
		}

		public Modification CreateNewModification(
			string userName,
			string time,
			string elementName,
			string modificationType,
			string comment,
			string change )
		{
			Modification modification = new Modification();
			// ClearCase change number is a string, not an int
			modification.ChangeNumber = ParseChangeNumber( change );
			modification.UserName = userName;
			modification.Type = modificationType;
			modification.Comment = ( comment == string.Empty ? null : comment );
			AssignFileInfo( modification, elementName );
			AssignModificationTime( modification, time );
			return modification;
		}

		public int ParseChangeNumber( string item )
		{
			if ( item == null )
			{
				return -1;
			}
			int index = item.LastIndexOf( "\\" );
			if ( index == -1 )
			{
				return -1;
			}
			try 
			{
				return Int32.Parse( item.Substring( index + 1 ) );
			}
			catch ( FormatException )
			{
				return -1;
			}
		}

		public Modification ParseEntry( string line )
		{
			string[] tokens = TokenizeEntry( line );
			if ( tokens == null
				 || tokens.Length != 8 )
			{
				return null;
			}
			// A branch event shouldn't be considered a modification
			string modificationType = tokens[4].Trim().ToLower();
			if ( modificationType == "mkbranch" || modificationType == "rmbranch" )
			{
				return null;
			}
			return CreateNewModification(
				tokens[0].Trim(),
				tokens[1].Trim(),
				tokens[2].Trim(),
				tokens[4].Trim(),
				tokens[7].Trim(),
				tokens[5].Trim() );
		}

		internal Modification[] ParseStream( TextReader reader )
		{
			ArrayList modifications = new ArrayList();
			string nextLine;
			while ( ( nextLine = reader.ReadLine() ) != null )
			{
				string line = null;

				if (nextLine.IndexOf(END_OF_LINE_DELIMITER) == -1)
				{
					line = AccumulateMultiLineEntry(nextLine, reader);
				}
				else
				{
					line = nextLine;
				}

				int lineIndex = line.IndexOf( END_OF_LINE_DELIMITER );
				Modification modification = ParseEntry( line.Substring( 0, lineIndex ) );	

				if ( modification != null )
				{
					modifications.Add( modification );
				}
			}
			return ( Modification[] ) modifications.ToArray( typeof( Modification ) );
		}

		private string AccumulateMultiLineEntry(string nextLine, TextReader reader)
		{
			string followingLine;
			while (nextLine.IndexOf(END_OF_LINE_DELIMITER) == -1 && (followingLine = reader.ReadLine()) != null)
			{
				nextLine += followingLine;
			}
			return nextLine;
		}


		public string[] TokenizeEntry( string line )
		{
			ArrayList items = new ArrayList();
			int firstDelimiter = -1;
			int entryIndex = 0;
			int secondDelimiter = line.IndexOf( DELIMITER, entryIndex );
			while ( secondDelimiter != -1 )
			{
				items.Add( line.Substring( entryIndex, secondDelimiter - entryIndex ) );

				firstDelimiter = secondDelimiter;
				entryIndex = firstDelimiter + DELIMITER.Length;
				secondDelimiter = line.IndexOf( DELIMITER, entryIndex );
			}
			items.Add( line.Substring( entryIndex, line.Length - entryIndex ) );
			return ( string[] ) items.ToArray( typeof( string ) );
		}
	}
}