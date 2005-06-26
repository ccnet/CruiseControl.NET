using System;
using System.IO;
using System.Text.RegularExpressions;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("pathFilter")]
	public class PathFilter : IModificationFilter
	{
		private string pathPattern;
		private Regex exFileName;
		private Regex exFolder;

		[ReflectorProperty("pattern", Required=true)]
		public string Pattern
		{
			get { return pathPattern; }
			set
			{
				pathPattern = value;
				PrepareFileNameRegex();
				PrepareFolderRegex();
			}
		}

		public bool Accept(Modification modification)
		{
			return MatchesFolder(modification) && MatchesFilename(modification);
		}

		private bool MatchesFilename(Modification modification)
		{
			if (modification.FileName == null) return false;
			return exFileName.IsMatch(modification.FileName);
		}

		private bool MatchesFolder(Modification modification)
		{
			if (modification.FolderName == null) return false;
			return exFolder.IsMatch(modification.FolderName);
		}

		private void PrepareFileNameRegex()
		{
			string fileNamePattern = Path.GetFileNameWithoutExtension(pathPattern);
			string extensionPattern = Path.GetExtension(pathPattern);

			if (fileNamePattern.Equals(string.Empty))
				fileNamePattern = "*";

			fileNamePattern = EscapeAndReplaceAsterisk(fileNamePattern);

			if (! extensionPattern.Equals(string.Empty))
			{
				bool optionalExtension = extensionPattern.Equals(".*");

				extensionPattern = EscapeAndReplaceAsterisk(extensionPattern);

				if (optionalExtension)
					extensionPattern = "(" + extensionPattern + ")?";
			}

			exFileName = new Regex("^" + fileNamePattern + extensionPattern + "$");
		}

		private void PrepareFolderRegex()
		{
			string folderPattern = Path.GetDirectoryName(pathPattern);
			string altFolderPattern = folderPattern.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			string strippedFolderPattern;
			string strippedAltFolderPattern;

			ConstructFolderRegex(folderPattern, Path.DirectorySeparatorChar,
			                     out strippedFolderPattern, out folderPattern);

			ConstructFolderRegex(altFolderPattern, Path.AltDirectorySeparatorChar,
			                     out strippedAltFolderPattern, out altFolderPattern);

			exFolder = new Regex("(^" + folderPattern +
				"$)|(^" + strippedFolderPattern +
				"$)|(^" + altFolderPattern +
				"$)|(^" + strippedAltFolderPattern + "$)");
		}

		private void ConstructFolderRegex(
			string sourcePattern,
			char separator,
			out string simplePattern,
			out string flexiblePattern)
		{
			string dirSeparator = new String(separator, 1);
			string doubleDirSeparator = new String(separator, 2);

			simplePattern = sourcePattern.Replace("*", "");
			simplePattern = simplePattern.Replace(doubleDirSeparator, dirSeparator);
			if (simplePattern.EndsWith(dirSeparator))
				simplePattern = simplePattern.Substring(0, simplePattern.Length - 1);
			simplePattern = Regex.Escape(simplePattern);

			flexiblePattern = Regex.Escape(sourcePattern);
			flexiblePattern = flexiblePattern.Replace("\\*\\*", GetWildCardRegexPatternForPath());
			flexiblePattern = flexiblePattern.Replace("\\*", GetWildCardRegexPatternForFolderName());
		}

		private string EscapeAndReplaceAsterisk(string s)
		{
			s = Regex.Escape(s);
			return s.Replace("\\*", GetAcceptableCharacterRange());
		}

		/// <summary>
		/// Gets regex fragment matching acceptable file names.
		/// </summary>
		/// <remarks>
		/// At least one character is required to successfully
		/// match the pattern.
		/// </remarks>
		private string GetAcceptableCharacterRange()
		{
			return string.Concat("[^", Regex.Escape(InvalidPathCharacters), "]+");
		}

		/// <summary>
		/// Gets regex fragment matching path fragments.
		/// </summary>
		/// <remarks>
		/// Zero or more acceptable characters will match the
		/// pattern. This pattern will permit directory separators.
		/// </remarks>
		private string GetWildCardRegexPatternForPath()
		{
			return string.Concat("[^", Regex.Escape(InvalidPathCharacters), "]*");
		}

		/// <summary>
		/// Gets regex fragment matching single folder name.
		/// </summary>
		/// <remarks>
		/// Zero or more acceptable characters will match the
		/// pattern. This pattern will not permit directory 
		/// separators.
		/// </remarks>
		private string GetWildCardRegexPatternForFolderName()
		{
			string invalidCharacters = InvalidPathCharacters + DirectorySeparators;
			return string.Concat("[^", Regex.Escape(invalidCharacters), "]*");
		}

		private static readonly string InvalidPathCharacters =
			new String(Path.InvalidPathChars);

		private static readonly string DirectorySeparators =
			new string(Path.DirectorySeparatorChar, 1) +
				new string(Path.AltDirectorySeparatorChar, 1);
	}
}