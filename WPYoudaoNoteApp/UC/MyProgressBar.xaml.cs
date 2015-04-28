

namespace WPYoudaoNoteApp.UC
{
    using System.Windows;
    using System.Windows.Media;

    public partial class MyProgressBar
    {
        public MyProgressBar()
        {
            InitializeComponent();
        }

        public Color DotColor
        {
            get
            {
                return ((SolidColorBrush)bar.Foreground).Color;
            }
            set
            {
                bar.Foreground = new SolidColorBrush(value);
            }
        }

        public string Tip
        {
            get
            {
                return txtTip.Text;
            }
            set
            {
                txtTip.Text = value;
            }
        }

        public Color TipColor
        {
            get
            {
                return ((SolidColorBrush)txtTip.Foreground).Color;
            }
            set
            {
                txtTip.Foreground = new SolidColorBrush(value);
            }
        }

        public Color BackgroundColor
        {
            get { return ((SolidColorBrush) LayoutRoot.Background).Color; }
            set { LayoutRoot.Background = new SolidColorBrush(value);}
        }

        public static readonly DependencyProperty BackgroundColorProperty =
                DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(MyProgressBar),
                new PropertyMetadata(Colors.White, OnColorChanged));
        public static readonly DependencyProperty DotColorProperty =
                DependencyProperty.Register("DotColor", typeof(Color), typeof(MyProgressBar),
                new PropertyMetadata(Colors.White, OnColorChanged));
        public static readonly DependencyProperty TipProperty =
                DependencyProperty.Register("Tip", typeof(string), typeof(MyProgressBar),
                new PropertyMetadata("", OnTipChanged));
        public static readonly DependencyProperty TipColorProperty =
        DependencyProperty.Register("TipColor", typeof(Color), typeof(MyProgressBar),
        new PropertyMetadata(Colors.White, OnTipColorChanged));

        private static void OnColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var bar = obj as MyProgressBar;
            if (null != bar) bar.DotColor = (Color)e.NewValue;
        }

        private static void OnTipChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var bar = obj as MyProgressBar;
            if (null != bar) bar.Tip = e.NewValue.ToString();
        }

        private static void OnTipColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var bar = obj as MyProgressBar;
            if (null != bar) bar.TipColor = (Color)e.NewValue;
        }
    }
}
