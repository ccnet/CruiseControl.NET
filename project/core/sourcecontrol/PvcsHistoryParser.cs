using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol 
{
	public class PvcsHistoryParser : IHistoryParser 
	{
		public const string UNKNOWN = "checked in";
		public Modification[] Parse(TextReader reader)
		{

			PvcsModificationBuilder builder = new PvcsModificationBuilder();
			string line;
			while ( (line=reader.ReadLine()) != null) 
			{
				builder.addLine(line);
			}
			ArrayList mods = builder.getList();
			mods = (mods == null) ? new ArrayList() : mods;
			return (Modification[]) mods.ToArray(typeof(Modification));
		}

	}
	
	public class PvcsModificationBuilder {
		const string FROM_PVCS_DATE_FORMAT = "MMM dd yyyy HH:mm:ss";
		const string ARCHIVE = "Archive:";
		private Modification modification; 
		private ArrayList modifications = null;
		private bool  firstModifiedTime = true;
		private bool  firstUserName     = true;
		private bool  nextLineIsComment = false;
		private bool  waitingForNextValidStart = false;
		
		public ArrayList getList(){
			return modifications;
		}
		
		private void initializeModification(){
			if (modifications == null){
				modifications = new ArrayList();
			}
			modification = new Modification();
			modification.Type = PvcsHistoryParser.UNKNOWN;
			modification.FolderName = PvcsHistoryParser.UNKNOWN;
			firstModifiedTime = true;
			firstUserName = true;
			nextLineIsComment = false;
			waitingForNextValidStart = false;
		}
		
		public void addLine(string line){
			if (line.StartsWith(ARCHIVE)){
				initializeModification();
				modification.FolderName = line.Substring(ARCHIVE.Length).Trim();
			}
			else if (waitingForNextValidStart || line.StartsWith("Branches:")){ 
				// we're in this state after we've got the last useful line
				// from the previous item, but haven't yet started a new one
				// -- we should just skip these lines till we start a new one
				return;
			}
			else if (line.StartsWith("Workfile:")){ 
				modification.FileName = line.Substring(18);
			}           
			else if (line.StartsWith("Checked in:")){
				// if this is the newest revision...
				if (firstModifiedTime) {
					firstModifiedTime = false;
					string lastMod = line.Substring(16);
					modification.ModifiedTime = ParseDateTime(lastMod);
				}
			}
			else if (nextLineIsComment == true){
				// used boolean because don't know what comment will startWith....
				modification.Comment = line;
				// comment is last line we need, so add this mod to list,
				//  then set indicator to ignore future lines till next new item
				modifications.Add(modification);
				waitingForNextValidStart = true;
			}
			else if (line.StartsWith("Author id:")){
				// if this is the newest revision...
				if (firstUserName){
					modification.UserName = ParseUserName(line);
					firstUserName = false;
					nextLineIsComment = true;
				}
			}  // end of Author id
			
		}   // end of addLine
		
		public string ParseUserName(string line) 
		{
			return line.Substring(11).Split()[0].Trim();
		}
		
		private DateTime ParseDateTime(string dateString)
		{
			try 
			{
				return DateTime.ParseExact(
					dateString,
					FROM_PVCS_DATE_FORMAT,
					DateTimeFormatInfo.GetInstance(CultureInfo.InvariantCulture));
			}
			catch (Exception ex) 
			{
				throw new CruiseControlException("Unable to parse: "+dateString, ex);
			}
		}
		
	}  // end of class ModificationBuilder
	
}
