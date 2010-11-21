/********************************************************************************
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 ********************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// Implementation of IHistoryParser to handle Surround SCM output that describes modifications within the version control system.
	/// Format of output is:
	/// total-#
	/// {History Line}
	///
	/// where
	/// {History Line} has the following format:
    /// &lt;repository&gt;&lt;filename&gt;&lt;rev&gt;&lt;action&gt;&lt;timestamp&gt;&lt;comment&gt;&lt;username&gt;&lt;user email&gt;
	/// </summary>
	public class SurroundHistoryParser : IHistoryParser
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string TO_SSCM_DATE_FORMAT = "yyyyMMddHHmmss";

        /// <summary>
        /// Parses the specified SSCM log.	
        /// </summary>
        /// <param name="sscmLog">The SSCM log.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public Modification[] Parse(TextReader sscmLog, DateTime from, DateTime to)
		{
			string line = sscmLog.ReadLine();
			int totalLines = int.Parse(line.Split('-')[1]);

            var modList = new List<Modification>(totalLines);
			for (int i = 0; i < totalLines; i++)
			{
				line = sscmLog.ReadLine();
				modList.Add(ParseModificationLine(line));
			}
			return modList.ToArray();
		}

		private Modification ParseModificationLine(string line)
		{
			Match match = Regex.Match(line, @"^<([^>]*)><([^>]*)><([^>]*)><([^>]*)><([^>]*)><([^>]*)><([^>]*)><([^>]*)>$");
			if (!match.Success)
			{
				throw new ArgumentException("Unable to parse line: " + line);
			}
			Modification modification = new Modification();
			modification.FolderName = match.Groups[1].ToString();
			modification.FileName = match.Groups[2].ToString();
			modification.ChangeNumber = match.Groups[3].ToString();
			modification.Type = match.Groups[4].ToString();
			modification.ModifiedTime = DateTime.ParseExact(match.Groups[5].ToString(), TO_SSCM_DATE_FORMAT, CultureInfo.InvariantCulture);
			modification.Comment = match.Groups[6].ToString();
			modification.UserName = match.Groups[7].ToString();
			modification.EmailAddress = match.Groups[8].ToString();
			return modification;
		}
	}
}