using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Ysm.Controls
{
    public class TextEditor : ICSharpCode.AvalonEdit.TextEditor
    {
        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.Register("SelectionBrush", typeof(SolidColorBrush), typeof(TextEditor), new PropertyMetadata(default(SolidColorBrush)));

        [Category("Brush")]
        public SolidColorBrush SelectionBrush
        {
            get => (SolidColorBrush)GetValue(SelectionBrushProperty);
            set => SetValue(SelectionBrushProperty, value);
        }

        public static readonly DependencyProperty CaretBrushProperty =
            DependencyProperty.Register("CaretBrush", typeof(SolidColorBrush), typeof(TextEditor), new PropertyMetadata(default(SolidColorBrush)));

        [Category("Brush")]
        public SolidColorBrush CaretBrush
        {
            get => (SolidColorBrush)GetValue(CaretBrushProperty);
            set => SetValue(CaretBrushProperty, value);
        }

        public static readonly DependencyProperty SelectionForegroundProperty =
            DependencyProperty.Register("SelectionForeground", typeof (SolidColorBrush), typeof (TextEditor), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush SelectionForeground
        {
            get => (SolidColorBrush) GetValue(SelectionForegroundProperty);
            set => SetValue(SelectionForegroundProperty, value);
        }

        static TextEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextEditor), new FrameworkPropertyMetadata(typeof(TextEditor)));
        }

        private bool _isLoaded;

        public TextEditor()
        {
            Loaded += TextEditor_Loaded;
        }

        private void TextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                _isLoaded = true;

                Options.ShowBoxForControlCharacters = false;
                Options.EnableHyperlinks = true;
                Options.EnableRectangularSelection = true;
                Options.RequireControlModifierForHyperlinkClick = false;
                Options.ShowColumnRuler = false;
                Options.EnableEmailHyperlinks = true;
                

                TextArea.Caret.CaretBrush = CaretBrush;
                TextArea.SelectionBrush = SelectionBrush;
                TextArea.SelectionForeground = SelectionForeground;
                TextArea.SelectionBorder = null;
                TextArea.SelectionCornerRadius = 0;
                TextArea.TextView.LinkTextForegroundBrush =
                    new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF3A7BDA"));
            }
        }
       
    }
}
