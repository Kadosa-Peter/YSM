using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models;
using Ysm.Views;
using Ysm.Windows;

namespace Ysm.Assets.Behaviors
{
    public class ChannelDragBehavior : Behavior<ExtendedTreeView>
    {
        private ExtendedTreeView _treeView;

        private ScrollViewer _scrollViewer;

        private bool _forbidDrag;

        private bool _isCanceled;

        private Point _startPoint;

        private DragWindow _dragWindow;

        protected override void OnAttached()
        {
            base.OnAttached();

            _treeView = AssociatedObject;

            _treeView.PreviewMouseLeftButtonDown += TreeView_PreviewMouseLeftButtonDown;
            _treeView.MouseMove += TreeView_MouseMove;
            _treeView.DragOver += TreeView_DragOver;
            _treeView.QueryContinueDrag += TreeView_QueryContinueDrag;
            _treeView.Drop += TreeView_Drop;
            _treeView.GiveFeedback += TreeView_GiveFeedback;
        }

        private void TreeView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;

            if (_dragWindow != null)
            {
                Point p = CursorPosition.Get();

                _dragWindow.Left = p.X + 10;
                _dragWindow.Top = p.Y + 10;
            }

            e.Handled = true;
        }

        private void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is ExtendedTreeView)
            {
                _isCanceled = false;

                _startPoint = e.GetPosition(_treeView);
            }
        }

        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (CheckDragConditions(e))
            {
                Point endPoint = e.GetPosition(_treeView);

                if (Math.Abs(endPoint.X - _startPoint.X) > (SystemParameters.MinimumHorizontalDragDistance + 4) |
                    Math.Abs(endPoint.Y - _startPoint.Y) > (SystemParameters.MinimumVerticalDragDistance + 4))
                {
                    if (e.OriginalSource is Thumb == false)
                        StartDrag();
                }
            }
        }

        private bool CheckDragConditions(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return false;

            if (_isCanceled) return false;

            if (_forbidDrag) return false;

            if (Kernel.Default.Search) return false;

            if (Kernel.Default.MenuIsOpen) return false;

            if (Kernel.Default.IsRootOnlySelected) return false;

            if (_treeView.SelectedItems.Count == 0) return false;

            return true;
        }

        private void StartDrag()
        {
            _forbidDrag = true;
            Kernel.Default.IsDragDrop = true;

            List<Node> nodes = _treeView.SelectedItems.Cast<Node>().Where(x => x.Id != Identifier.Empty).ToList();

            DataObject data = new DataObject("YSM", nodes);

            CreateDragWindow(nodes);

            DragDrop.DoDragDrop(_treeView, data, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link | DragDropEffects.None);

            DestroyDragWindow();

            _forbidDrag = false;
            Kernel.Default.IsDragDrop = false;
        }

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            if (_scrollViewer == null)
            {
                _scrollViewer = _treeView.GetChildOfType<ScrollViewer>("_tv_scrollviewer_");
            }

            _scrollViewer.AutoScroll(e);
        }

        private void TreeView_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;

                _isCanceled = true;

                e.Handled = true;
            }
        }

        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            ChannelDropTarget dropTarget = new ChannelDropTarget(e);

            List<Node> nodes = (List<Node>)e.Data.GetData("YSM");

            if (nodes?.Count > 0 && dropTarget.Node != null)
            {
                Dictionary<string, string> channels = new Dictionary<string, string>();
                Dictionary<string, string> categories = new Dictionary<string, string>();

                List<string> parents = new List<string>();

                foreach (Node node in nodes)
                {
                    Node oldParent = node.Parent as Node;
                    Node newParent = dropTarget.Node;

                    if (oldParent == null 
                        || newParent == null 
                        || oldParent.Equals(newParent) // ugyanarra a parent node-ra dobom
                        || newParent.Equals(node) // saját magára dobom
                        || newParent.NodeType == NodeType.Channel
                        || Repository.Default.Schema.IsDescendant(node.Id, newParent.Id)) continue;

                    if(node.NodeType == NodeType.Channel)
                    {
                        parents.AddOrThrow(oldParent.Id);
                        parents.AddOrThrow(newParent.Id);
                    }

                    node.Move(oldParent, newParent);

                    if (node.NodeType == NodeType.Channel)
                    {
                        channels.Add(node.Id, newParent.Id);
                    }
                    else
                    {
                        categories.Add(node.Id, newParent.Id);
                    }
                }

                Repository.Default.Channels.Move(channels);
                Repository.Default.Categories.Move(categories);
                
                ViewRepository.Get<ChannelView>().UpdateParents(parents);

                dropTarget.Node.Sort();

                dropTarget.Source.IsDragOver = false;

                _treeView.ForceFocus();
            }
        }

        private void CreateDragWindow(List<Node> nodes)
        {
            _dragWindow = new DragWindow(nodes);

            Point p = CursorPosition.Get();

            _dragWindow.Left = p.X + 10;
            _dragWindow.Top = p.Y + 10;

            _dragWindow.Show();
        }

        private void DestroyDragWindow()
        {
            if (_dragWindow != null)
            {
                _dragWindow.Close();
                _dragWindow = null;
            }
        }

    }
}
