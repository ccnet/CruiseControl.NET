using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class PlasticSCMHistoryParser : IHistoryParser
	{
		public Modification[] Parse(TextReader input, DateTime from, DateTime to)
		{
            var mods = new List<Modification>();
            var filemods = new List<string>();

			string line;

			while( (line = input.ReadLine()) != null )
			{
				if( !line.StartsWith(PlasticSCM.DELIMITER.ToString()))
					continue;
				// path date user changeset
				string[] data = line.Split(PlasticSCM.DELIMITER);
				Modification mod = new Modification();
				string path = data[1];
				mod.FileName = Path.GetFileName(path);
				mod.FolderName = Path.GetDirectoryName(path);
				mod.UserName = data[2];
				mod.ModifiedTime = DateTime.ParseExact (data[3], PlasticSCM.DATEFORMAT, CultureInfo.InvariantCulture);
				mod.ChangeNumber =  data[4];
				if (!filemods.Contains(path)) 
				{
					filemods.Add(path);
					mods.Add(mod);
				}
			}
			return mods.ToArray();
			
		}
	}
}