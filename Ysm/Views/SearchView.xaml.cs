using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ysm.Core;

namespace Ysm.Views
{
    public partial class SearchView
    {
        #region Text

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(SearchView), new PropertyMetadata(default(string), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SearchView searchView)
            {
                searchView.IsSearch = searchView.Text.NotNull();
                searchView.RaiseTextChangedEvent();
            }
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion

        #region TextChanged

        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
            "TextChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchView));

        public event RoutedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        private void RaiseTextChangedEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(TextChangedEvent);

            RaiseEvent(args);
        }
        

        #endregion

        public bool IsSearch { get; set; }

        public SearchView()
        {
            InitializeComponent();
        }

        private void SearchBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SearchBox.Clear();
            }
        }

        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ClearButton.IsEnabled = !SearchBox.Text.IsNull();
        }

        private void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            SearchBox.Clear();
        }

        public void Clear()
        {
            SearchBox.Text = string.Empty;
        }
    }
}
