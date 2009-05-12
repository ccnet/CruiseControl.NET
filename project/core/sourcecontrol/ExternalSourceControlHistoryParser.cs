using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// Parser for ExternalSourceControl modification output.
	/// </summary>
	public class ExternalSourceControlHistoryParser : IHistoryParser
	{
        /// This method creates and runs a command to list all the modifications in the specified 
        /// timespan, and expects the modifications to be returned in the serialized form of the
        /// ThoughtWorks.CruiseControl.Core.Modification class <i>ala</i> 
        /// <see cref="System.Xml.Serialization.XmlSerializer.Serialize(Stream, object)"/>.

        #region Fields
        // None yet.
        #endregion

        #region Constructors

        public ExternalSourceControlHistoryParser()
		{
        }

        #endregion

        #region Interface methods

        /// <summary>
		/// Construct and return an array of Modifications describing the changes in
		/// the AccuRev workspace, based on the output of the "accurev hist" command.
		/// </summary>
        /// <param name="history">the stream of <code>&lt;modifications&gt;</code> input</param>
		/// <param name="from">the starting date and time for the range of modifications we want.</param>
		/// <param name="to">the ending date and time for the range of modifications we want.</param>
		/// <returns>the changes in the specified time range.</returns>
		public Modification[] Parse(TextReader history, DateTime from, DateTime to)
        {
        	XmlSerializer serializer = new XmlSerializer(typeof (Modification[]));
			Modification[] mods = new Modification[0];
        	try
        	{
        		// return 0 modifications if "history" is empty.
        		if (history.Peek() == -1)
					return mods;

        		mods = (Modification[]) serializer.Deserialize(history);
        	}
        	catch (InvalidOperationException e)
        	{
				Log.Error(e);

        		if (e.InnerException is XmlException)
					return mods;

        		throw;
        	}
        	ArrayList results = new ArrayList();

        	foreach (Modification mod in mods)
        	{
        		if ((mod.ModifiedTime >= from) & (mod.ModifiedTime <= to))
        			results.Add(mod);
        	}
        	return (Modification[]) results.ToArray(typeof (Modification));
        }

		#endregion

        #region Internal methods
        // None yet.
        #endregion
    }
}
