using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Ysm.Core;

namespace Ysm.Controls
{
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    public class RenameBox : Control
    {
        private const string PART_TextBox = "PART_TextBox";

        private TextBox _textBox;

        private string _temp;
        // -- Events --

        #region BeforeRename Event

        public static readonly RoutedEvent BeforeRenameEvent = EventManager.RegisterRoutedEvent
            ("BeforeRename", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RenameBox));

        public event RoutedEventHandler BeforeRename
        {
            add => AddHandler(BeforeRenameEvent, value);
            remove => RemoveHandler(BeforeRenameEvent, value);
        }

        private void RaiseBeforeRenameEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(BeforeRenameEvent);

            RaiseEvent(args);
        }

        #endregion

        #region AfterRename Event

        public static readonly RoutedEvent AfterRenameEvent = EventManager.RegisterRoutedEvent
            ("AfterRename", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>),
                typeof(RenameBox));

        public event RoutedPropertyChangedEventHandler<string> AfterRename
        {
            add => AddHandler(AfterRenameEvent, value);
            remove => RemoveHandler(AfterRenameEvent, value);
        }

        private void RaiseAfterRenameEvent(string oldOne, string newOne)
        {
            RoutedPropertyChangedEventArgs<string> args = new RoutedPropertyChangedEventArgs<string>(oldOne, newOne,
                AfterRenameEvent);

            RaiseEvent(args);
        }

        #endregion

        #region TextChanged Event

        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent
            ("TextChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>),
                typeof(RenameBox));

        public event RoutedPropertyChangedEventHandler<string> TextChanged
        {
            add => AddHandler(TextChangedEvent, value);
            remove => RemoveHandler(TextChangedEvent, value);
        }

        private void RaiseTextChangedEvent(string oldOne, string newOne)
        {
            RoutedPropertyChangedEventArgs<string> args = new RoutedPropertyChangedEventArgs<string>(oldOne, newOne,
                TextChangedEvent);
            RaiseEvent(args);
        }

        #endregion

        // -- DependencyProperties --

        #region CanRename

        public static readonly DependencyProperty CanRenameProperty = DependencyProperty.Register
            ("CanRename", typeof(bool), typeof(RenameBox), new PropertyMetadata(true));

        public bool CanRename
        {
            get => (bool)GetValue(CanRenameProperty);
            set => SetValue(CanRenameProperty, value);
        }

        #endregion

        #region Text

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
            ("Text", typeof(string), typeof(RenameBox), new PropertyMetadata(default(string)));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion

        #region MaxLines

        public static readonly DependencyProperty MaxLinesProperty = DependencyProperty.Register
            ("MaxLines", typeof(int), typeof(RenameBox), new PropertyMetadata(1));

        public int MaxLines
        {
            get => (int)GetValue(MaxLinesProperty);
            set => SetValue(MaxLinesProperty, value);
        }

        #endregion

        #region IsRenaming

        public static readonly DependencyProperty IsRenamingProperty = DependencyProperty.Register
            ("IsRenaming", typeof(bool), typeof(RenameBox), new PropertyMetadata(default(bool), IsRenaming_PropertyChanged));

        private static void IsRenaming_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (d is RenameBox renameBox && renameBox.IsRendered)
                {
                    renameBox.StartRename();
                }
            }
        }

        public bool IsRenaming
        {
            get => (bool)GetValue(IsRenamingProperty);
            set => SetValue(IsRenamingProperty, value);
        }

        #endregion

        #region IsRendered

        public static readonly DependencyProperty IsRenderedProperty = DependencyProperty.Register(
            "IsRendered", typeof(bool), typeof(RenameBox), new PropertyMetadata(default(bool)));

        public bool IsRendered
        {
            get => (bool)GetValue(IsRenderedProperty);
            set => SetValue(IsRenderedProperty, value);
        }

        #endregion

        #region RenamingBackground

        public static readonly DependencyProperty RenamingBackgroundProperty = DependencyProperty.Register
            ("RenamingBackground", typeof(SolidColorBrush), typeof(RenameBox),
                new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush RenamingBackground
        {
            get => (SolidColorBrush)GetValue(RenamingBackgroundProperty);
            set => SetValue(RenamingBackgroundProperty, value);
        }

        #endregion

        #region RenamingForeground

        public static readonly DependencyProperty RenamingForegroundProperty = DependencyProperty.Register
            ("RenamingForeground", typeof(SolidColorBrush), typeof(RenameBox),
                new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush RenamingForeground
        {
            get => (SolidColorBrush)GetValue(RenamingForegroundProperty);
            set => SetValue(RenamingForegroundProperty, value);
        }

        #endregion

        // static .ctor
        static RenameBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RenameBox), new FrameworkPropertyMetadata(typeof(RenameBox)));
        }

        public RenameBox()
        {
            Loaded += RenameBox_Loaded;
        }

        private void RenameBox_Loaded(object sender, RoutedEventArgs e)
        {
            IsRendered = true;

            if (IsRenaming)
            {
                StartRename();
            }
        }

        // ApplyTemplate
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBox = GetTemplateChild(PART_TextBox) as TextBox;
            if (_textBox != null)
            {
                _textBox.LostFocus += TextBox_LostFocus;
                _textBox.KeyDown += TextBox_KeyDown;
                _textBox.TextChanged += TextBox_TextChanged;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RaiseTextChangedEvent("", _textBox.Text);
        }

        #region Rename

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        _textBox.Text = _temp;
                        EndRename();
                        e.Handled = true;
                    }
                    break;
                case Key.Enter:
                    {
                        EndRename();
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EndRename();
        }

        public void StartRename()
        {
            if (CanRename)
            {
                // TODO: lehet, hogy ez okozza a TreeView focus hibáját
                _textBox.ForceFocus();

                _textBox.SelectAll();

                _textBox.ScrollToHome();

                _temp = _textBox.Text;

                RaiseBeforeRenameEvent();
            }
        }

        public void EndRename()
        {
            if (IsRenaming)
            {
                if (_textBox.Text.IsNull())
                {
                    _textBox.Text = _temp;

                    RaiseAfterRenameEvent(_temp, _temp);
                }
                else if (_textBox.Text == _temp)
                {
                    RaiseAfterRenameEvent(_temp, _temp);
                }
                else
                {
                    RaiseAfterRenameEvent(_temp, _textBox.Text);
                }

                IsRenaming = false;
            }
        }

        #endregion
    }
}
