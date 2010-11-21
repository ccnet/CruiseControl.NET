using System;
using System.Collections.Generic;
using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
    /// <summary>
    /// 	
    /// </summary>
    public class MercurialHistoryParser : IHistoryParser
    {
        /// <summary>
        /// Parses the specified history.	
        /// </summary>
        /// <param name="history">The history.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Modification[] Parse(TextReader history, DateTime from, DateTime to)
        {
            string mods = "<modifications><array>" + history.ReadToEnd() + "</array></modifications>";
            try
            {
                MercurialModificationCollection hgModifications = (MercurialModificationCollection) NetReflector.Read(mods);
                List<Modification> list = new List<Modification>(hgModifications.modifications.Length);
                foreach (MercurialModification modification in hgModifications.modifications)
                {
                    list.Add(modification);
                }
                return list.ToArray();
            } catch (NetReflectorException ex)
            {
                throw new CruiseControlException("History Parsing Failed", ex);
            }
        }
    }
}
