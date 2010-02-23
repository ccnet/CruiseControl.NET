namespace ThoughtWorks.CruiseControl.WebDashboard.Resources
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Web;
    using System.Configuration;

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
        private Translations()
        {
            var context = HttpContext.Current;
            try
            {
                // Check if there is a override language
                var systemLanguage = ConfigurationManager.AppSettings["language"];

                if (string.IsNullOrEmpty(systemLanguage))
                {
                    // Get the user's preferred language - this comes from the browser so we are assuming that the user has choosen their preferred languages
                    if ((context != null) &&
                        (context.Request != null) &&
                        (context.Request.UserLanguages.Length > 0))
                    {
                        foreach (var language in context.Request.UserLanguages)
                        {
                            try
                            {
                                this.culture = new CultureInfo(language);

                                // Stop on the first non-neutral language
                                if (!this.culture.IsNeutralCulture)
                                {
                                    break;
                                }
                            }
                            catch (ArgumentException)
                            {
                                // This means that the language has invalid characters in it - normally because .NET has incorrectly parsed the user agent string
                            }

                            if (this.culture.IsNeutralCulture)
                            {
                                // Revert to the first culture since there are no non-neutral cultures defined
                                this.culture = new CultureInfo(context.Request.UserLanguages[0]);
                            }
                        }
                    }
                    else
                    {
                        // Use the default culture from the server
                        this.culture = CultureInfo.CurrentUICulture;
                    }
                }
                else
                {
                    this.culture = new CultureInfo(systemLanguage);
                }
            }
            catch
            {
                // If all else fails, use the current UI culture
                this.culture = CultureInfo.CurrentUICulture;
            }

            this.resourceManager.IgnoreCase = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Translations"/> class.
        /// </summary>
        /// <param name="culture">The name of the culture to use.</param>
        public Translations(string culture)
        {
            this.culture = new CultureInfo(culture);
            this.resourceManager.IgnoreCase = true;
        }
        #endregion

        #region Public properties
        #region Culture
        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <value>The user's culture.</value>
        public CultureInfo Culture
        {
            get { return this.culture; }
        }
        #endregion

        #region UICulture
        /// <summary>
        /// Gets the UI culture to use.
        /// </summary>
        /// <value>The UI culture.</value>
        /// <remarks>
        /// This will return a <see cref="CultureInfo"/> that is safe to use in formatting. If the current culture is a neutral culture
        /// it will fall back to the current UI culture on the server.
        /// </remarks>
        public CultureInfo UICulture
        {
            get
            {
                if (this.culture.IsNeutralCulture)
                {
                    // TODO: Figure out how to get from a neutral culture to a more specific UI culture
                    // .NET does not using neutral languages for formatting, therefore fall back to the OS language
                    return CultureInfo.CurrentUICulture;
                }
                else
                {
                    return this.culture;
                }
            }
        }
        #endregion
        #endregion

        #region Public methods
        #region RetrieveCurrent()
        /// <summary>
        /// Retrieves the current instance of the translations for a request.
        /// </summary>
        /// <returns>The <see cref="Translations"/> for the current request.</returns>
        public static Translations RetrieveCurrent()
        {
            var context = HttpContext.Current;
            if ((context == null) || (context.Items == null))
            {
                // Bypass for unit testing
                return new Translations();
            }
            else
            {
                if (context.Items.Contains("translations"))
                {
                    // Return the existing translations instance
                    return context.Items["translations"] as Translations;
                }
                else
                {
                    // Start a new instance
                    var value = new Translations();
                    context.Items.Add("translations", value);
                    return value;
                }
            }
        }
        #endregion

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
