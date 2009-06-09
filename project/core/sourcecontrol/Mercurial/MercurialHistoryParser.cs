using System;
using System.Collections.Generic;
using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
    public class MercurialHistoryParser : IHistoryParser
    {
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
