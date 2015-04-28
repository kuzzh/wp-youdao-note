

namespace YoudaoNoteEditor
{
    using C1.Phone.RichTextBox;
    using C1.Phone.RichTextBox.Documents;
    using System;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    public partial class RichTextEditor
    {
        public RichTextEditor()
        {
            InitializeComponent();
        }

        public void SetHtml(string html)
        {
            rtbContent.Html = html;
        }

        public void SetFocus()
        {
            rtbContent.Focus();
        }

        public string GetHtml()
        {
            return rtbContent.Html;
        }

        public string GetText()
        {
            // remove zero width space:&#x200b;
            return rtbContent.Text.Replace("￼", "");
        }

        public string InsertImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                throw new ArgumentNullException("imagePath");
            }

            rtbContent.Selection.Start.InsertInline(new C1InlineUIContainer()
            {
                Content = new Image()
                {
                    Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),   //set image source
                }
            });

            return rtbContent.Html;
        } 

        public bool Readonly
        {
            get { return rtbContent.IsReadOnly; }
            set { rtbContent.IsReadOnly = value; }
        }
    }
}
