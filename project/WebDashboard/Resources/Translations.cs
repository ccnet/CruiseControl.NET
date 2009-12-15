using System.Globalization;
using System.Resources;
using System.Web;
namespace ThoughtWorks.CruiseControl.WebDashboard.Resources
{
    /// <summary>
    /// Provides translations for different resources.
    /// </summary>
    public class Translations
    {
        #region Private fields
        private CultureInfo culture;
        private ResourceManager resourceManager = new ResourceManager("ThoughtWorks.CruiseControl.WebDashboard.Resources.Languages",
            typeof(Translations).Assembly);
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Translations"/> class.
        /// </summary>
        public Translations()
        {
            var culture = "en-US";
            if ((HttpContext.Current != null) &&
                (HttpContext.Current.Request != null) &&
                (HttpContext.Current.Request.UserLanguages.Length > 0))
            {
                culture = HttpContext.Current.Request.UserLanguages[0];
            }

            this.culture = new CultureInfo(culture);
            this.resourceManager.IgnoreCase = true;
        }
        #endregion

        #region Public methods
        #region Translate()
        /// <summary>
        /// Translates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="args">The args to pass.</param>
        /// <returns>
        /// The translated value if found, the value otherwise.
        /// </returns>
        public string Translate(string value, params object[] args)
        {
            var translation = this.resourceManager.GetString(value, this.culture) ?? value;
            if (args.Length > 0)
            {
                translation = string.Format(this.culture, translation, args);
            }

            return translation;
        }
        #endregion

        #region TranslateJson()
        /// <summary>
        /// Translates the specified value into JSON.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="args">The args to pass.</param>
        /// <returns>
        /// The translated value if found, the value otherwise.
        /// </returns>
        /// <remarks>
        /// This will ensure all apostrophes and quotes are correctly encoded.
        /// </remarks>
        public string TranslateJson(string value, params object[] args)
        {
            var translation = this.resourceManager.GetString(value, this.culture) ?? value;
            if (args.Length > 0)
            {
                translation = string.Format(this.culture, translation, args);
            }

            return translation.Replace("'", "\\'").Replace("\"", "\\\"");
        }
        #endregion
        #endregion
    }
}
