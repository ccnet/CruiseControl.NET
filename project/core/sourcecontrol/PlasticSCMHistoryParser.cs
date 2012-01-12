using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// 
    /// </summary>
    public class PlasticSCMHistoryParser : IHistoryParser
    {
        /// <summary>
        /// Parses the specified input.	
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Modification[] Parse(TextReader input, DateTime from, DateTime to)
        {
            var mods = new List<Modification>();

            string line;

            while ((line = input.ReadLine()) != null)
            {
                if (!line.StartsWith(PlasticSCM.DELIMITER.ToString(CultureInfo.CurrentCulture)))
                    continue;
                string[] data = line.Split(PlasticSCM.DELIMITER);
                Modification mod = new Modification();
                mod.Comment = data[1];
                mod.UserName = data[2];
                mod.ModifiedTime = DateTime.ParseExact(data[3], PlasticSCM.DATEFORMAT, CultureInfo.InvariantCulture);
                mod.ChangeNumber = data[4];
                mods.Add(mod);
            }

            return mods.ToArray();
        }
    }
}