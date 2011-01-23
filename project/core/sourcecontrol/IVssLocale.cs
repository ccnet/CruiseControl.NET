using System;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IVssLocale
	{
        /// <summary>
        /// Gets the comment keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		string CommentKeyword { get; }
        /// <summary>
        /// Gets the checked in keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		string CheckedInKeyword { get; }
        /// <summary>
        /// Gets the added keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		string AddedKeyword { get; }
        /// <summary>
        /// Gets the deleted keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		string DeletedKeyword { get; }
        /// <summary>
        /// Gets the destroyed keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		string DestroyedKeyword { get; }
        /// <summary>
        /// Gets the user keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		string UserKeyword { get; }
        /// <summary>
        /// Gets the date keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		string DateKeyword { get; }
        /// <summary>
        /// Gets the time keyword.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		string TimeKeyword { get; }

        /// <summary>
        /// Parses the date time.	
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		DateTime ParseDateTime(string date, string time);
        /// <summary>
        /// Formats the command date.	
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string FormatCommandDate(DateTime date);
        /// <summary>
        /// Gets or sets the server culture.	
        /// </summary>
        /// <value>The server culture.</value>
        /// <remarks></remarks>
		string ServerCulture { get; set; }
	}
}
