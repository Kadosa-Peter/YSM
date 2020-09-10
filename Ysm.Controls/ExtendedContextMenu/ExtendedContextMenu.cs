using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ysm.Core;

namespace Ysm.Controls
{
    [TemplatePart(Name = PART_SearchBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Clear, Type = typeof(IconButton))]
    public class ExtendedContextMenu : ContextMenu
    {
        private const string PART_SearchBox = "PART_SearchBox";
        private const string PART_Clear = "PART_Clear";
        private TextBox _searchBox;
        private IconButton _clearButton;

        private ExtendedContextMenuItem _item;

        #region ShowSearch

        public static readonly DependencyProperty ShowSearchProperty = DependencyProperty.Register
            ("ShowSearch", typeof(bool), typeof(ExtendedContextMenu), new PropertyMetadata(true));

        public bool ShowSearch
        {
            get => (bool)GetValue(ShowSearchProperty);
            set => SetValue(ShowSearchProperty, value);
        }

        #endregion

        // static .ctro
        static ExtendedContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedContextMenu),
                new FrameworkPropertyMetadata(typeof(ExtendedContextMenu)));
        }

        // ApplyTemplate
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _searchBox = GetTemplateChild(PART_SearchBox) as TextBox;
            if (_searchBox != null)
            {
                _searchBox.TextChanged += SearchBox_TextChanged;
                _searchBox.PreviewKeyDown += SearchBox_PreviewKeyDown;

                if (!ShowSearch)
                    _searchBox.Visibility = Visibility.Collapsed;
            }

            _clearButton = GetTemplateChild(PART_Clear) as IconButton;

            if (_clearButton != null)
                _clearButton.Click += ClearButton_Click;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _searchBox.Clear();
        }

        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _searchBox.Clear();
                _searchBox.ForceFocus();
                e.Handled = true;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = _searchBox.Text.ToLower();

            if (text.NotNull())
            {
                foreach (ExtendedContextMenuItem item in Items)
                {
                    if (item.IsSeparator)
                    {
                        item.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        string title = item.Title.ToLower();

                        if (title.Contains(text))
                        {
                            item.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                    }

                }
            }
            else
            {
                foreach (ExtendedContextMenuItem item in Items)
                {
                    item.Visibility = Visibility.Visible;
                }
            }
        }

        protected override void OnOpened(RoutedEventArgs e)
        {
            base.OnOpened(e);

            SelectSearchBox();

            e.Handled = true;
        }

        protected override void OnClosed(RoutedEventArgs e)
        {
            base.OnClosed(e);

            _searchBox.Clear();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Escape)
            {
                if (_searchBox.Text.NotNull())
                {
                    _searchBox.Clear();
                    e.Handled = true;
                }
                else
                {
                    IsOpen = false;
                }
            }
            else if (e.Key == Key.Down)
            {
                MoveDown();
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                MoveUp();
                e.Handled = true;
            }

        }

        private void MoveUp()
        {
            List<ExtendedContextMenuItem> items = GetVisibleItem();

            if (items.Count <= 0) return;

            if (_item == null)
            {
                SelectLast(items);
            }
            else
            {
                SelectPrevious(items);
            }
        }

        private void MoveDown()
        {
            List<ExtendedContextMenuItem> items = GetVisibleItem();

            if (items.Count > 0)
            {
                if (_item == null)
                {
                    SelectFirst(items);
                }
                else
                {
                    SelectNext(items);
                }
            }
        }

        private List<ExtendedContextMenuItem> GetVisibleItem()
        {
            return Items.Cast<ExtendedContextMenuItem>().Where(x => x.Visibility == Visibility.Visible && x.IsSeparator == false).ToList();
        }

        private void SelectPrevious(List<ExtendedContextMenuItem> items)
        {
            int index = items.IndexOf(_item);

            index--;

            if (index >= 0)
            {
                ExtendedContextMenuItem item = items[index];

                Keyboard.Focus(item);

                _item = item;
            }
            else
            {
                //SelectLast(items);
                SelectSearchBox();
            }
        }

        private void SelectNext(List<ExtendedContextMenuItem> items)
        {
            int index = items.IndexOf(_item);

            index++;

            if (index <= items.Count - 1)
            {
                ExtendedContextMenuItem item = items[index];

                Keyboard.Focus(item);

                _item = item;
            }
            else
            {
                //SelectFirst(items);
                SelectSearchBox();
            }
        }

        private void SelectLast(List<ExtendedContextMenuItem> items)
        {
            ExtendedContextMenuItem item = items.Last();
            Keyboard.Focus(item);

            _item = item;
        }

        private void SelectFirst(List<ExtendedContextMenuItem> items)
        {
            ExtendedContextMenuItem item = items.First();
            Keyboard.Focus(item);

            _item = item;
        }

        public void SelectSearchBox()
        {
            _item = null;

            _searchBox.ForceFocus();
            _searchBox.CaretIndex = 0;
            _searchBox.SelectAll();
        }

    }
}