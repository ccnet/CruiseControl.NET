using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    public partial class WizardPageBase 
        : UserControl
    {
        #region Private fields
        private WizardPageBase previousPage;
        private WizardPageBase nextPage;
        private bool isValid;
        private bool headerVisible;
        private int headerHeight;
        private Color headerBackgroundColour;
        private Color headerBorderColour;
        private Color headerTitleColour;
        private string headerTitle;
        private Padding headerTitlePadding;
        private Font headerTitleFont;
        private string headerText;
        private Color headerTextColour;
        private Padding headerTextPadding;
        private Font headerTextFont;
        private Image headerImage;
        private Padding headerImagePadding;
        #endregion

        #region Constructors
        public WizardPageBase()
        {
            InitializeComponent();

            // Default values
            CanCancel = true;
            ConfirmCancel = true;
            IsValid = true;
            HeaderVisible = true;
            UseFormIcon = true;
            HeaderHeight = 48;
            ResetHeaderBackgroundColour();
            ResetHeaderBorderColour();
            ResetHeaderTitleColour();
            ResetHeaderTitlePadding();
            ResetHeaderTitleFont();
            ResetHeaderTextColour();
            ResetHeaderTextPadding();
            ResetHeaderTextFont();
            ResetHeaderImagePadding();
        }
        #endregion

        #region Public properties
        #region PreviousPage
        /// <summary>
        /// The previous page (if any).
        /// </summary>
        [Browsable(false)]
        public WizardPageBase PreviousPage
        {
            get { return previousPage; }
            set
            {
                if (!object.ReferenceEquals(previousPage, value))
                {
                    previousPage = value;
                    if (PreviousPageChanged != null) PreviousPageChanged(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region NextPage
        /// <summary>
        /// The next page (if any).
        /// </summary>
        [Browsable(false)]
        public WizardPageBase NextPage
        {
            get { return nextPage; }
            set
            {
                if (!object.ReferenceEquals(previousPage, value))
                {
                    nextPage = value;
                    if (NextPageChanged != null) NextPageChanged(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region CanCancel
        /// <summary>
        /// Can the wizard be cancelled from this page.
        /// </summary>
        [DisplayName("Can Cancel Wizard")]
        [Description("Can the wizard be cancelled from this page. This will control whether the cancel button is enabled or not.")]
        [Category("Wizard Control")]
        [DefaultValue(true)]
        public bool CanCancel { get; set; }
        #endregion

        #region ConfirmCancel
        /// <summary>
        /// Should the user be prompted if they want to cancel.
        /// </summary>
        [DisplayName("Comfirm Cancel")]
        [Description("Should the user be prompted if they want to cancel. This allows for the user accidently clicking on the cancel button.")]
        [Category("Wizard Control")]
        [DefaultValue(true)]
        public bool ConfirmCancel { get; set; }
        #endregion

        #region CanFinish
        /// <summary>
        /// Can the wizard be finished from this page.
        /// </summary>
        [DisplayName("Can Finish Wizard")]
        [Description("Can the wizard be finished from this page. This will control whether the finish button is enabled or not.")]
        [Category("Wizard Control")]
        [DefaultValue(false)]
        public bool CanFinish { get; set; }
        #endregion

        #region IsValid
        /// <summary>
        /// Is the page currently valid (i.e. can move to the next page.)
        /// </summary>
        [Browsable(false)]
        public bool IsValid
        {
            get { return isValid; }
            set
            {
                isValid = value;
                if (NextPageChanged != null) NextPageChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        #region HeaderVisible
        /// <summary>
        /// Is the header visible.
        /// </summary>
        [DisplayName("Header Visible")]
        [Description("Is the wizard header visible. This will be displayed as a band across the top of the page.")]
        [Category("Wizard Display")]
        [DefaultValue(true)]
        public bool HeaderVisible
        {
            get { return headerVisible; }
            set
            {
                headerVisible = value;
                Invalidate();
            }
        }
        #endregion

        #region HeaderHeight
        /// <summary>
        /// The height of the header band.
        /// </summary>
        [DisplayName("Header Height")]
        [Description("The height of the header band.")]
        [Category("Wizard Display")]
        [DefaultValue(48)]
        public int HeaderHeight
        {
            get { return headerHeight; }
            set
            {
                headerHeight = value;
                Invalidate();
            }
        }
        #endregion

        #region HeaderBackgroundColour
        /// <summary>
        /// The background colour of the header band.
        /// </summary>
        [DisplayName("Header Background Colour")]
        [Description("The background colour of the header band.")]
        [Category("Wizard Display")]
        public Color HeaderBackgroundColour
        {
            get { return headerBackgroundColour; }
            set
            {
                headerBackgroundColour = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Resets the header background colour.
        /// </summary>
        public void ResetHeaderBackgroundColour()
        {
            HeaderBackgroundColour = SystemColors.Window;
        }

        /// <summary>
        /// Checks whether the header background colour should be serialised.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeHeaderBackgroundColour()
        {
            return (HeaderBackgroundColour != SystemColors.Window);
        }
        #endregion

        #region HeaderBorderColour
        /// <summary>
        /// The border colour of the header band.
        /// </summary>
        [DisplayName("Header Border Colour")]
        [Description("The border colour of the header band.")]
        [Category("Wizard Display")]
        public Color HeaderBorderColour
        {
            get { return headerBorderColour; }
            set
            {
                headerBorderColour = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Resets the header border colour.
        /// </summary>
        public void ResetHeaderBorderColour()
        {
            HeaderBorderColour = SystemColors.WindowFrame;
        }

        /// <summary>
        /// Checks whether the header border colour should be serialised.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeHeaderBorderColour()
        {
            return (HeaderBorderColour != SystemColors.WindowFrame);
        }
        #endregion

        #region HeaderTitle
        /// <summary>
        /// The title for the header band.
        /// </summary>
        [DisplayName("Header Title")]
        [Description("The title for the header band.")]
        [Category("Wizard Display")]
        [DefaultValue(null)]
        public string HeaderTitle
        {
            get { return headerTitle; }
            set
            {
                headerTitle = value;
                Invalidate();
            }
        }
        #endregion

        #region HeaderTitleColour
        /// <summary>
        /// The colour of the header title.
        /// </summary>
        [DisplayName("Header Title Colour")]
        [Description("The colour of the header title.")]
        [Category("Wizard Display")]
        public Color HeaderTitleColour
        {
            get { return headerTitleColour; }
            set
            {
                headerTitleColour = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Resets the colour of the header title.
        /// </summary>
        public void ResetHeaderTitleColour()
        {
            HeaderTitleColour = SystemColors.WindowText;
        }

        /// <summary>
        /// Checks whether the colour of the header title should be serialised.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeHeaderTitleColour()
        {
            return (HeaderTitleColour != SystemColors.WindowText);
        }
        #endregion

        #region HeaderTitlePadding
        /// <summary>
        /// The padding around the header title.
        /// </summary>
        [DisplayName("Header Title Padding")]
        [Description("The padding around the header title.")]
        [Category("Wizard Display")]
        public Padding HeaderTitlePadding
        {
            get { return headerTitlePadding; }
            set
            {
                headerTitlePadding = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Resets the colour of the header text.
        /// </summary>
        public void ResetHeaderTitlePadding()
        {
            HeaderTitlePadding = new Padding(5);
        }

        /// <summary>
        /// Checks whether the colour of the header text should be serialised.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeHeaderTitlePadding()
        {
            return (HeaderTitlePadding.Left != 5) ||
                (HeaderTitlePadding.Top != 5) ||
                (HeaderTitlePadding.Right != 5) ||
                (HeaderTitlePadding.Bottom != 5);
        }
        #endregion

        #region HeaderTitleFont
        /// <summary>
        /// The font for the header title.
        /// </summary>
        [DisplayName("Header Title Font")]
        [Description("The font for the header title.")]
        [Category("Wizard Display")]
        public Font HeaderTitleFont
        {
            get { return headerTitleFont; }
            set
            {
                headerTitleFont = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Resets the colour of the header text.
        /// </summary>
        public void ResetHeaderTitleFont()
        {
            HeaderTitleFont = new Font(Font, FontStyle.Bold);
        }

        /// <summary>
        /// Checks whether the colour of the header text should be serialised.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeHeaderTitleFont()
        {
            return (HeaderTitleFont.Style != FontStyle.Bold) ||
                (HeaderTitleFont.Size != Font.Size) ||
                (HeaderTitleFont.Name != Font.Name) ||
                (HeaderTitleFont.FontFamily != Font.FontFamily);
        }
        #endregion

        #region HeaderText
        /// <summary>
        /// The text for the header band.
        /// </summary>
        [DisplayName("Header Text")]
        [Description("The text for the header band.")]
        [Category("Wizard Display")]
        [DefaultValue(null)]
        public string HeaderText
        {
            get { return headerText; }
            set
            {
                headerText = value;
                Invalidate();
            }
        }
        #endregion

        #region HeaderTextColour
        /// <summary>
        /// The colour of the header text.
        /// </summary>
        [DisplayName("Header Text Colour")]
        [Description("The colour of the header text.")]
        [Category("Wizard Display")]
        public Color HeaderTextColour
        {
            get { return headerTextColour; }
            set
            {
                headerTextColour = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Resets the colour of the header text.
        /// </summary>
        public void ResetHeaderTextColour()
        {
            HeaderTextColour = SystemColors.WindowText;
        }

        /// <summary>
        /// Checks whether the colour of the header text should be serialised.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeHeaderTextColour()
        {
            return (HeaderTextColour != SystemColors.WindowText);
        }
        #endregion

        #region HeaderTextPadding
        /// <summary>
        /// The padding around the header text.
        /// </summary>
        [DisplayName("Header Text Padding")]
        [Description("The padding around the header text.")]
        [Category("Wizard Display")]
        public Padding HeaderTextPadding
        {
            get { return headerTextPadding; }
            set
            {
                headerTextPadding = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Resets the colour of the header text.
        /// </summary>
        public void ResetHeaderTextPadding()
        {
            HeaderTextPadding = new Padding(5, 0, 5, 5);
        }

        /// <summary>
        /// Checks whether the colour of the header text should be serialised.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeHeaderTextPadding()
        {
            return (HeaderTextPadding.Left != 5) ||
                (HeaderTextPadding.Top != 0) ||
                (HeaderTextPadding.Right != 5) ||
                (HeaderTextPadding.Bottom != 5);
        }
        #endregion

        #region HeaderTextFont
        /// <summary>
        /// The font for the header text.
        /// </summary>
        [DisplayName("Header Text Font")]
        [Description("The font for the header text.")]
        [Category("Wizard Display")]
        public Font HeaderTextFont
        {
            get { return headerTextFont; }
            set
            {
                headerTextFont = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Resets the colour of the header text.
        /// </summary>
        public void ResetHeaderTextFont()
        {
            HeaderTextFont = new Font(Font, FontStyle.Regular);
        }

        /// <summary>
        /// Checks whether the colour of the header text should be serialised.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeHeaderTextFont()
        {
            return (HeaderTextFont.Style != FontStyle.Regular) ||
                (HeaderTextFont.Size != Font.Size) ||
                (HeaderTextFont.Name != Font.Name) ||
                (HeaderTextFont.FontFamily != Font.FontFamily);
        }
        #endregion

        #region UseFormIcon
        /// <summary>
        /// Should the form icon be used if no image is set.
        /// </summary>
        [DisplayName("Use Form Icon")]
        [Description("Should the form icon be used if no image is set. If this is off and no images is selected, then no image will be displayed.")]
        [Category("Wizard UI")]
        [DefaultValue(true)]
        public bool UseFormIcon { get; set; }
        #endregion

        #region HeaderImage
        /// <summary>
        /// The image to display in the header band.
        /// </summary>
        [DisplayName("Header Image")]
        [Description("The image to display in the header band.")]
        [Category("Wizard Display")]
        [DefaultValue(null)]
        public Image HeaderImage
        {
            get { return headerImage; }
            set
            {
                headerImage = value;
                Invalidate();
            }
        }
        #endregion

        #region HeaderImagePadding
        /// <summary>
        /// The padding around the header image.
        /// </summary>
        [DisplayName("Header Text Padding")]
        [Description("The padding around the header image.")]
        [Category("Wizard Display")]
        public Padding HeaderImagePadding
        {
            get { return headerImagePadding; }
            set
            {
                headerImagePadding = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Resets the padding around the header image.
        /// </summary>
        public void ResetHeaderImagePadding()
        {
            HeaderImagePadding = new Padding(5);
        }

        /// <summary>
        /// Checks whether the padding around the header image should be serialised.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeHeaderImagePadding()
        {
            return (HeaderImagePadding.Left != 5) ||
                (HeaderImagePadding.Top != 5) ||
                (HeaderImagePadding.Right != 5) ||
                (HeaderImagePadding.Bottom != 5);
        }
        #endregion

        #region MigrationOptions
        /// <summary>
        /// The current migration options.
        /// </summary>
        [Browsable(false)]
        public MigrationOptions MigrationOptions { get; set; }
        #endregion

        #region Controller
        /// <summary>
        /// The controller.
        /// </summary>
        [Browsable(false)]
        public MainFormController Controller { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region LinkNextPage()
        /// <summary>
        /// Links to the next page.
        /// </summary>
        /// <param name="nextPage"></param>
        public void LinkNextPage(WizardPageBase nextPage)
        {
            NextPage = nextPage;
            nextPage.previousPage = this;
        }
        #endregion

        #region RunPage()
        /// <summary>
        /// Performs any logic to be executed when the page is displayed.
        /// </summary>
        public virtual void RunPage()
        {
        }
        #endregion

        #region CompletePage()
        /// <summary>
        /// Performs any logic to be before the next page is selected.
        /// </summary>
        public virtual void CompletePage()
        {
        }
        #endregion
        #endregion

        #region Public events
        #region PreviousPageChanged
        /// <summary>
        /// The previous page has been changed.
        /// </summary>
        public event EventHandler PreviousPageChanged;
        #endregion

        #region NextPageChanged
        /// <summary>
        /// The next page has been changed.
        /// </summary>
        public event EventHandler NextPageChanged;
        #endregion

        #region PageCompeleted
        /// <summary>
        /// The page has been completed and the host can move onto the next page.
        /// </summary>
        public event EventHandler PageCompeleted;
        #endregion
        #endregion

        #region Protected methods
        #region OnPaint()
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (HeaderVisible)
            {
                // Draw the background
                using (var brush = new SolidBrush(HeaderBackgroundColour))
                {
                    e.Graphics.FillRectangle(brush, new Rectangle(0, 0, ClientSize.Width, HeaderHeight));
                }

                // Draw the border
                using (var pen = new Pen(HeaderBorderColour))
                {
                    e.Graphics.DrawLine(pen, new Point(0, HeaderHeight), new Point(ClientSize.Width, HeaderHeight));
                }

                // Draw the image
                var imageWidth = 0;
                if ((HeaderImage != null) || (UseFormIcon && (FindForm() != null)))
                {
                    var imageToDisplay = HeaderImage;
                    if (imageToDisplay == null)
                    {
                        var form = FindForm();
                        imageToDisplay = form.Icon.ToBitmap();
                    }
                    var srcRect = new Rectangle(0, 0, imageToDisplay.Width, imageToDisplay.Height);
                    var maxHeight = HeaderHeight - HeaderImagePadding.Top - HeaderImagePadding.Bottom;
                    var destRect = new Rectangle(ClientSize.Width - HeaderImagePadding.Right - imageToDisplay.Width,
                        HeaderImagePadding.Top,
                        imageToDisplay.Width,
                        imageToDisplay.Height > maxHeight ? maxHeight : imageToDisplay.Height);
                    e.Graphics.DrawImage(imageToDisplay, destRect, srcRect, GraphicsUnit.Pixel);
                    imageWidth = HeaderImagePadding.Right + imageToDisplay.Width + HeaderImagePadding.Left;
                }

                // Draw the title
                var titleHeight = 0;
                if (!string.IsNullOrEmpty(HeaderTitle))
                {
                    using (var brush = new SolidBrush(HeaderTitleColour))
                    {
                        var rect = new RectangleF(HeaderTitlePadding.Left,
                            HeaderTitlePadding.Top,
                            ClientSize.Width - HeaderTitlePadding.Right - imageWidth,
                            HeaderHeight - HeaderTitlePadding.Bottom);
                        var format = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoWrap);
                        format.Trimming = StringTrimming.EllipsisWord;
                        e.Graphics.DrawString(HeaderTitle, HeaderTitleFont, brush, rect, format);
                        titleHeight = Convert.ToInt32(
                            e.Graphics.MeasureString(HeaderTitle,
                                HeaderTitleFont, 
                                new SizeF(rect.Width, rect.Height), 
                                format).Height) +
                            HeaderTitlePadding.Top +
                            HeaderTitlePadding.Bottom;
                    }
                }

                // Draw the text
                if (!string.IsNullOrEmpty(HeaderText))
                {
                    using (var brush = new SolidBrush(HeaderTextColour))
                    {
                        var rect = new RectangleF(HeaderTextPadding.Left,
                            titleHeight + HeaderTextPadding.Top,
                            ClientSize.Width - HeaderTextPadding.Right - imageWidth,
                            HeaderHeight - titleHeight - HeaderTextPadding.Bottom);
                        var format = new StringFormat(StringFormatFlags.FitBlackBox);
                        format.Trimming = StringTrimming.EllipsisWord;
                        e.Graphics.DrawString(HeaderText, HeaderTextFont, brush, rect, format);
                    }
                }
            }
        }
        #endregion

        #region FirePageCompeleted()
        /// <summary>
        /// Fires the <see cref="PageCompeleted"/> event.
        /// </summary>
        protected void FirePageCompeleted()
        {
            if (PageCompeleted != null) PageCompeleted(this, EventArgs.Empty);
        }
        #endregion
        #endregion
    }
}
