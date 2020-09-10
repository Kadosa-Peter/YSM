using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ysm.Controls
{
    public abstract class TreeItemBase : TreeViewItem
    {
        //! Dependency Properties

        #region Title

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof (string), typeof (TreeItemBase), new PropertyMetadata(default(string)));

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region ShowIcon

        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register(
            "ShowIcon", typeof (bool), typeof (TreeItemBase), new PropertyMetadata(true));

        public bool ShowIcon
        {
            get => (bool) GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        #endregion

        #region Icon

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof (ImageSource), typeof (TreeItemBase), new PropertyMetadata(default(ImageSource)));

        public ImageSource Icon
        {
            get => (ImageSource) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        #endregion

        #region IsSelected

        public new static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof (bool), typeof (TreeItemBase), new PropertyMetadata(default(bool), IsSelectedProperty_Changed));

        public new bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        private static void IsSelectedProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TreeItemBase item = sender as TreeItemBase;
            item?.RaiseSelectionChangedEvent();
        }

        #endregion

        #region CanDelete

        public static readonly DependencyProperty CanDeleteProperty = DependencyProperty.Register
            ("CanDelete", typeof(bool), typeof(TreeItemBase), new PropertyMetadata(true));

        public bool CanDelete
        {
            get => (bool)GetValue(CanDeleteProperty);
            set => SetValue(CanDeleteProperty, value);
        }
        #endregion

        #region CanRename

        public static readonly DependencyProperty CanRenameProperty = DependencyProperty.Register(
            "CanRename", typeof(bool), typeof(TreeItemBase), new PropertyMetadata(true));

        public bool CanRename
        {
            get => (bool)GetValue(CanRenameProperty);
            set => SetValue(CanRenameProperty, value);
        }

        #endregion

        #region CanMove

        public static readonly DependencyProperty CanMoveProperty = DependencyProperty.Register(
            "CanMove", typeof(bool), typeof(TreeItemBase), new PropertyMetadata(default(bool)));

        public bool CanMove
        {
            get => (bool) GetValue(CanMoveProperty);
            set => SetValue(CanMoveProperty, value);
        }

        #endregion

        #region IsRendered

        public static readonly DependencyProperty IsRenderedProperty = DependencyProperty.Register(
            "IsRendered", typeof(bool), typeof(TreeItemBase), new PropertyMetadata(default(bool)));

        public bool IsRendered
        {
            get => (bool)GetValue(IsRenderedProperty);
            set => SetValue(IsRenderedProperty, value);
        }

        #endregion

        #region IsDragOver

        public static readonly DependencyProperty IsDragOverProperty = DependencyProperty.Register(
            "IsDragOver", typeof(bool), typeof(TreeItemBase), new PropertyMetadata(default(bool)));

        public bool IsDragOver
        {
            get => (bool)GetValue(IsDragOverProperty);
            set => SetValue(IsDragOverProperty, value);
        }

        #endregion

        #region IsRenaming

        public static readonly DependencyProperty IsRenamingProperty = DependencyProperty.Register(
            "IsRenaming", typeof(bool), typeof(TreeItemBase), new PropertyMetadata(default(bool)));

        public bool IsRenaming
        {
            get => (bool) GetValue(IsRenamingProperty);
            set => SetValue(IsRenamingProperty, value);
        }

        #endregion

        #region Count
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(TreeItemBase), new PropertyMetadata(default(int)));

        public int Count
        {
            get => (int)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }
        #endregion

        #region ShowCount
        public static readonly DependencyProperty ShowCountProperty = DependencyProperty.Register
           ("ShowCount", typeof(bool), typeof(TreeItemBase), new PropertyMetadata(default(bool)));

        public bool ShowCount
        {
            get => (bool)GetValue(ShowCountProperty);
            set => SetValue(ShowCountProperty, value);
        }
        #endregion

        #region CanCollapse

        public static readonly DependencyProperty CanCollapseProperty = DependencyProperty.Register(
            "CanCollapse", typeof(bool), typeof(TreeItemBase), new PropertyMetadata(true));

        public bool CanCollapse
        {
            get => (bool) GetValue(CanCollapseProperty);
            set => SetValue(CanCollapseProperty, value);
        }

        #endregion

        //! Events

        #region Rendered

        public static readonly RoutedEvent RenderedEvent = EventManager.RegisterRoutedEvent(
            "Rendered", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeItemBase));

        public event RoutedEventHandler Rendered
        {
            add => AddHandler(RenderedEvent, value);
            remove => RemoveHandler(RenderedEvent, value);
        }

        private void RaiseRenderedEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(RenderedEvent);
            RaiseEvent(args);
        }

        #endregion

        #region SelectionChangedEvent

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent
            ("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeItemBase));

        public event RoutedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        public void RaiseSelectionChangedEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(SelectionChangedEvent);
            RaiseEvent(args);
        }

        #endregion

        #region AfterRename

        public static readonly RoutedEvent AfterRenameEvent = EventManager.RegisterRoutedEvent
          ("AfterRename", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(TreeItemBase));

        public event RoutedPropertyChangedEventHandler<string> AfterRename
        {
            add => AddHandler(AfterRenameEvent, value);
            remove => RemoveHandler(AfterRenameEvent, value);
        }

        protected void RaiseAfterRenameEvent(string oldName, string newName)
        {
            RoutedPropertyChangedEventArgs<string> args = new RoutedPropertyChangedEventArgs<string>(oldName, newName, AfterRenameEvent);
            RaiseEvent(args);
        }

        #endregion

        #region BeforeRename

        public static readonly RoutedEvent BeforeRenameEvent = EventManager.RegisterRoutedEvent(
            "BeforeRename", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeItemBase));

        public event RoutedEventHandler BeforeRename
        {
            add => AddHandler(BeforeRenameEvent, value);
            remove => RemoveHandler(BeforeRenameEvent, value);
        }

        protected void RaiseBeforeRenameEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(BeforeRenameEvent);
            RaiseEvent(args);
        }

        #endregion

        protected TreeItemBase()
        {
            Loaded += TreeItemBase_Loaded;
        }

        private void TreeItemBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsRendered)
            {
                IsRendered = true;

                RaiseRenderedEvent();
            }
        }
    }
}
