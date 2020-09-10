using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Ysm.Core;


namespace Ysm.Controls
{
    public class ExtendedTreeView : TreeView
    {
        #region Events

        private static readonly RoutedEvent SelectedItemsChangedEvent = EventManager.RegisterRoutedEvent
            ("SelectedItemsChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeView));

        public event RoutedEventHandler SelectedItemsChanged
        {
            add => AddHandler(SelectedItemsChangedEvent, value);
            remove => RemoveHandler(SelectedItemsChangedEvent, value);
        }

        private void RaiseSelectedItemsChangedEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(SelectedItemsChangedEvent);
            RaiseEvent(args);
        }
        #endregion

        #region SelectedItems

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register
        ("SelectedItems", typeof(ObservableCollection<object>), typeof(ExtendedTreeView),
            new PropertyMetadata(default(ObservableCollection<object>)));

        public ObservableCollection<object> SelectedItems
        {
            get => (ObservableCollection<object>)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        #endregion

        #region IsSingleSelectionProperty

        public static readonly DependencyProperty IsSingleSelectionProperty =
            DependencyProperty.Register("IsSingleSelection", typeof(bool), typeof(ExtendedTreeView),
                new PropertyMetadata(default(bool)));

        public bool IsSingleSelection
        {
            get => (bool)GetValue(IsSingleSelectionProperty);
            set => SetValue(IsSingleSelectionProperty, value);
        }

        #endregion

        #region IsDefaultScrollViewer

        public static readonly DependencyProperty IsDefaultScrollViewerProperty = DependencyProperty.Register(
            "IsDefaultScrollViewer", typeof(bool), typeof(ExtendedTreeView), new PropertyMetadata(default(bool)));

        public bool IsDefaultScrollViewer
        {
            get => (bool)GetValue(IsDefaultScrollViewerProperty);
            set => SetValue(IsDefaultScrollViewerProperty, value);
        }

        #endregion

        private ScrollViewer _scrollViewer;

        private ItemsControl _parent;

        private string _key;

        private bool _resetScroll;

        private double _horizontalScrollPosition;

        private double _verticalScrollPosition;

        private int _index = -1;

        private bool _upSelection;

        //+ sctro
        static ExtendedTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedTreeView), new FrameworkPropertyMetadata(typeof(ExtendedTreeView)));
        }

        //+ ctro
        public ExtendedTreeView()
        {
            SelectedItems = new ObservableCollection<object>();

            RequestBringIntoView += ExtendedTreeView_RequestBringIntoView;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _scrollViewer = (ScrollViewer)Template.FindName("_tv_scrollviewer_", this);
            _scrollViewer.ScrollChanged += ExtendedTreeView_ScrollChanged;
        }

        private void ExtendedTreeView_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            _resetScroll = true;

            _horizontalScrollPosition = _scrollViewer.HorizontalOffset;
            _verticalScrollPosition = _scrollViewer.VerticalOffset;
        }

        private void ExtendedTreeView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_resetScroll)
            {
                _scrollViewer.ScrollToVerticalOffset(_verticalScrollPosition);
                _scrollViewer.ScrollToHorizontalOffset(_horizontalScrollPosition);
            }

            _resetScroll = false;
        }


        #region Container

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ExtendedTreeItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ExtendedTreeItem;
        }

        #endregion

        #region Selection

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            OnMouseLeftButtonUp(e);

            if (e.OriginalSource is ToggleButton) return;

            if (_upSelection)
            {
                _upSelection = false;

                NodeBase node = GetNode(e.OriginalSource);

                if (node == null) return;

                if (ModifierKeyHelper.IsCtrlDown)
                {
                    CtrlSelect(node);
                }
                else if (ModifierKeyHelper.IsShiftDown)
                {
                    ShiftSelect(node);
                }
                else
                {
                    MouseUpSelect(node);
                }
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            NodeBase node = GetNode(e.OriginalSource);

            if (node != null)
                if (SelectedItems.Contains(node) == false)
                {
                    DeselectAll();

                    node.IsSelected = true;

                    SelectedItems.Add(node);

                    RaiseSelectedItemsChangedEvent();
                }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            OnMouseLeftButtonDown(e);

            // expander
            if ((e.OriginalSource as DependencyObject).GetParentOfType<ToggleButton>() != null) return;

            NodeBase node = GetNode(e.OriginalSource);

            if (node == null) return;

            // ez a drag and drop miatt kell
            // ha úgy indítom a drag and drop-ot, hogy ctrl már nyomva van,
            // akkor itt leugrana az elemről a kiválasztás
            if (node.IsSelected)
            {
                _upSelection = true;
                return;
            }

            // a betűs léptetéshez kell
            _parent = GetParent(e.OriginalSource);

            if (ModifierKeyHelper.IsCtrlDown && !IsSingleSelection)
            {
                CtrlSelect(node);
            }
            else if (ModifierKeyHelper.IsShiftDown && !IsSingleSelection)
            {
                ShiftSelect(node);
            }
            else
            {
                MouseDownSelect(node);
            }
        }

        private void MouseUpSelect(NodeBase newOne)
        {
            if (SelectedItems.Count == 1 && newOne.Equals(SelectedItems.First()))
            {
                // ugyanazt az elemet akarom kiválasztani
            }
            else
            {
                DeselectAll();

                newOne.IsSelected = true;

                SelectedItems.Add(newOne);

                RaiseSelectedItemsChangedEvent();
            }
        }

        private void MouseDownSelect(NodeBase newOne)
        {
            if (SelectedItems.Count == 1 && newOne.Equals(SelectedItems.First()))
            {
                // ugyanazt az elemet akarom kiválasztani
            }
            else if (SelectedItems.Count > 1)
            {
                if (newOne.IsSelected)
                {
                    // akkor változtatom meg a kijelölést amikor felengedem az egér gombot
                    // különben nem működne a drag and drop
                    _upSelection = true;
                }
                else
                {
                    DeselectAll();

                    newOne.IsSelected = true;

                    SelectedItems.Add(newOne);

                    RaiseSelectedItemsChangedEvent();
                }
            }
            else
            {
                DeselectAll();

                newOne.IsSelected = true;

                SelectedItems.Add(newOne);

                RaiseSelectedItemsChangedEvent();
            }
        }

        private void CtrlSelect(NodeBase newOne)
        {
            if (newOne.IsSelected)
            {
                // csak akkor szüntetem meg a kiválasztást ha 1-nél több elem van kiválasztva
                if (SelectedItems.Count > 1)
                {
                    newOne.IsSelected = false;

                    SelectedItems.Remove(newOne);

                    RaiseSelectedItemsChangedEvent();
                }
            }
            else
            {
                newOne.IsSelected = true;

                SelectedItems.Add(newOne);

                RaiseSelectedItemsChangedEvent();
            }
        }

        private void ShiftSelect(NodeBase newOne)
        {
            // ReSharper disable PossibleMultipleEnumeration
            if (SelectedItems.Count == 0)
            {
                newOne.IsSelected = true;
                SelectedItems.Add(newOne);
                RaiseSelectedItemsChangedEvent();
            }
            else
            {
                NodeBase lastOne = (NodeBase)SelectedItems.First();

                IEnumerable lastItems = GetItems(lastOne);

                IEnumerable newItems = GetItems(newOne);

                if (Equals(lastItems, newItems))
                {
                    int primaryIndex = GetIndex(lastItems, lastOne);
                    int secondaryIndex = GetIndex(newItems, newOne);

                    int strat = GetStartIndex(primaryIndex, secondaryIndex);
                    int finsih = GetFinishIndex(primaryIndex, secondaryIndex);

                    DeselectExcept(lastOne);

                    foreach (NodeBase obj in newItems)
                    {
                        if (obj.Equals(lastOne)) continue;

                        int index = GetIndex(newItems, obj);

                        if (index >= strat && index <= finsih)
                        {
                            obj.IsSelected = true;

                            SelectedItems.Add(obj);
                        }
                    }

                    RaiseSelectedItemsChangedEvent();
                }
            }
            // ReSharper restore PossibleMultipleEnumeration
        }

        private NodeBase GetNode(object obj)
        {
            ExtendedTreeItem item = (obj as DependencyObject).GetParentOfType<ExtendedTreeItem>();

            return item?.Header as NodeBase;
        }

        private ItemsControl GetParent(object obj)
        {
            ExtendedTreeItem item = (obj as DependencyObject).GetParentOfType<ExtendedTreeItem>();

            if (item != null)
            {
                DependencyObject target = VisualTreeHelper.GetParent(item);

                while (target != null)
                {
                    if (target is ItemsControl)
                        return target as ItemsControl;

                    target = VisualTreeHelper.GetParent(target);
                }
            }

            return null;
        }

        // BUG: Kurva fontos hogy a kiválasztás működjön lennie kell parent-nek, abban az esetben ha parent nem közvetlenűl a TreeView!!!
        private IEnumerable GetItems(NodeBase node)
        {
            if (node.Parent == null)
                return Items;

            return node.Parent.Items;
        }

        public static ItemsControl GetParent(ExtendedTreeItem item)
        {
            DependencyObject target = VisualTreeHelper.GetParent(item);

            while (target != null)
            {
                if (target is ItemsControl)
                    return target as ItemsControl;

                target = VisualTreeHelper.GetParent(target);
            }
            return null;
        }

        private int GetIndex(IEnumerable items, object item)
        {
            int index = 0;

            foreach (object obj in items)
            {
                if (Equals(obj, item))
                    break;

                index++;
            }

            return index;
        }

        private static int GetFinishIndex(int lastSelected, int newSelected)
        {
            if (lastSelected > newSelected)
                return lastSelected;

            return newSelected;
        }

        private static int GetStartIndex(int lastSelected, int newSelected)
        {
            if (lastSelected > newSelected)
                return newSelected;

            return lastSelected;
        }

        public void Select(NodeBase node)
        {
            DeselectAll();

            node.IsSelected = true;
            SelectedItems.Add(node);

            RaiseSelectedItemsChangedEvent();
        }

        private void DeselectAll()
        {
            foreach (NodeBase node in SelectedItems)
            {
                node.IsSelected = false;
            }

            SelectedItems.Clear();
        }

        private void DeselectExcept(NodeBase except)
        {
            List<object> items = SelectedItems.Where(x => x.Equals(except) == false).ToList();

            foreach (NodeBase item in items)
            {
                item.IsSelected = false;
                SelectedItems.Remove(item);
            }
        }

        private void Deselect(NodeBase node)
        {
            node.IsSelected = false;
            SelectedItems.Remove(node);
        }

        #endregion

        #region Move

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (ModifierKeyHelper.Any()
                || Keyboard.FocusedElement?.GetType() == typeof(TextBox))
                return;

            if (e.Key == Key.Down)
            {
                MoveDown();
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                MoveUp();
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                MoveRight();
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                MoveLeft();
                e.Handled = true;
            }
            else if (e.Key == Key.PageUp)
            {
                _scrollViewer.PageUp();
                e.Handled = true;
            }
            else if (e.Key == Key.PageDown)
            {
                _scrollViewer.PageDown();
                e.Handled = true;
            }
            else if (e.Key == Key.Home)
            {
                _scrollViewer.ScrollToHome();
                e.Handled = true;
            }
            else if (e.Key == Key.End)
            {
                _scrollViewer.ScrollToEnd();
                e.Handled = true;
            }
            else
            {
                string key = e.Key.ToString().ToLower();

                if (key.IsLetterOrNumber())
                {
                    Move(key);
                }
            }
        }

        private void MoveDown()
        {
            if (SelectedItems.Count > 0)
            {
                NodeBase currentNode = (NodeBase)SelectedItems.Last();

                if (currentNode != null)
                {
                    if (!IsLast(currentNode))
                    {
                        NodeBase newNode = GetNextNode(currentNode);

                        if (newNode != null)
                        {
                            //Select(newNode);

                            ScrollToNode(newNode, true);
                        }
                    }
                }
            }
        }

        private void MoveUp()
        {
            if (SelectedItems.Count > 0)
            {
                NodeBase currentNode = (NodeBase)SelectedItems.Last();

                if (currentNode != null)
                {
                    if (!IsFirst(currentNode))
                    {

                        NodeBase newNode = GetPreviousNode(currentNode);

                        if (newNode != null)
                        {
                            //Select(newNode);

                            //_scrollViewer.LineUp();
                            ScrollToNode(newNode, true);
                        }
                    }
                }
            }
        }

        private void MoveRight()
        {
            if (SelectedItems.Count > 0)
            {
                NodeBase currentNode = (NodeBase)SelectedItems.Last();

                if (currentNode != null)
                {
                    if (!currentNode.IsExpanded && currentNode.Items.Count > 0)
                    {
                        currentNode.IsExpanded = true;
                    }
                    else if (currentNode.IsExpanded && currentNode.Items.Count > 0)
                    {
                        NodeBase node = currentNode.Items[0];

                        Select(node);

                        ScrollToNode(node);
                    }
                    else
                    {
                        // do nothing
                    }
                }
            }
        }

        private void MoveLeft()
        {
            if (SelectedItems.Count > 0)
            {
                NodeBase currentNode = (NodeBase)SelectedItems.Last();

                if (currentNode?.Parent != null)
                {
                    NodeBase newNode = currentNode.Parent;

                    Select(newNode);

                    ScrollToNode(newNode);

                    newNode.IsExpanded = false;
                }
            }
        }

        private void Move(string key)
        {
            if (Keyboard.FocusedElement?.GetType() != typeof(TextBox) && _parent != null && key.Length == 1)
            {
                if (key != _key)
                {
                    _index = -1;
                    _key = key;
                }

                List<NodeBase> items = new List<NodeBase>();

                foreach (NodeBase nodeBase in _parent.Items)
                {
                    if (nodeBase != null && nodeBase.Title.NotNull() && nodeBase.Title.ToLower().StartsWith(key))
                    {
                        items.Add(nodeBase);
                    }
                }

                for (int i = 0; i < items.Count; i++)
                {
                    bool last = i == items.Count - 1;

                    NodeBase item = items[i];
                    int index = _parent.Items.IndexOf(item);

                    if (index > _index)
                    {
                        DeselectAll();

                        Select(item);

                        ScrollToNode(item);

                        _index = index;

                        if (last)
                        {
                            _index = -1;
                        }

                        break;
                    }
                }
            }
        }

        public void ScrollToNode(NodeBase node, bool select = false)
        {
            int index = 1;
            bool found = false;

            CalculateTop(ref index, ref found, node, Items);

            // TODO: 21 TreeItemHeight
            double top = index * 26;

            // BUG: 2x kell meghívni a ScrollToVerticalOffset-et!
            if (top < _scrollViewer.ActualHeight)
            {

                _scrollViewer.ScrollToVerticalOffset(0);
                _scrollViewer.UpdateLayout();
                _scrollViewer.ScrollToVerticalOffset(0);
            }
            else if (top > _scrollViewer.VerticalOffset + _scrollViewer.ActualHeight)
            {
                top = top - _scrollViewer.ActualHeight / 2;
                _scrollViewer.ScrollToVerticalOffset(top);
                _scrollViewer.UpdateLayout();
                _scrollViewer.ScrollToVerticalOffset(top);
            }
            else if (top < _scrollViewer.VerticalOffset + 26)
            {
                top = top - _scrollViewer.ActualHeight / 2;
                _scrollViewer.ScrollToVerticalOffset(top);
                _scrollViewer.UpdateLayout();
                _scrollViewer.ScrollToVerticalOffset(top);
            }
            if (select)
            {
                Select(node);
            }
        }


        private void CalculateTop(ref int index, ref bool found, NodeBase node1, IEnumerable items)
        {
            foreach (NodeBase node2 in items)
            {
                if (Equals(node1, node2))
                {
                    found = true;
                    break;
                }

                index++;

                if (node2.IsExpanded)
                    CalculateTop(ref index, ref found, node1, node2.Items);

                if (found)
                {
                    break;
                }
            }
        }

        private NodeBase GetNextNode(NodeBase currentNode)
        {
            int index = GetIndex(currentNode);

            index++;

            if (currentNode.Parent != null)
            {
                return currentNode.Parent.Items[index];
            }
            else
            {
                return Items[index] as NodeBase;
            }
        }

        private NodeBase GetPreviousNode(NodeBase currentNode)
        {
            int index = GetIndex(currentNode);

            index--;

            if (currentNode.Parent != null)
            {
                return currentNode.Parent.Items[index];
            }
            else
            {
                return Items[index] as NodeBase;
            }
        }

        private int GetIndex(NodeBase node)
        {
            if (node.Parent != null)
            {
                return node.Parent.Items.IndexOf(node);
            }
            else
            {
                return Items.IndexOf(node);
            }
        }

        private bool IsLast(NodeBase node)
        {
            if (node.Parent != null)
            {
                int index = node.Parent.Items.IndexOf(node);

                return index == node.Parent.Items.Count - 1;
            }
            else
            {
                int index = Items.IndexOf(node);

                return index == Items.Count - 1;
            }
        }

        private bool IsFirst(NodeBase node)
        {
            int index;

            if (node.Parent != null)
            {
                index = node.Parent.Items.IndexOf(node);
            }
            else
            {
                index = Items.IndexOf(node);
            }

            return index == 0;
        }

        #endregion
    }
}
