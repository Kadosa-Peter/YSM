using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models;
using Ysm.Windows;

namespace Ysm.Assets.Behaviors
{
    public class VideoDragBehvior : Behavior<ListView>
    {
        private bool _forbidDrag;

        private bool _dragCanceled;

        private Point _startPoint;

        private ListView _listView;

        private Video _video;

        private VideoDragWindow _dragWindow;

        protected override void OnAttached()
        {
            base.OnAttached();

            _listView = AssociatedObject;

            _listView.PreviewMouseLeftButtonDown += ListView_PreviewMouseLeftButtonDown;
            _listView.MouseMove += ListView_MouseMove;
            _listView.GiveFeedback += ListView_GiveFeedback;
            _listView.QueryContinueDrag += ListView_QueryContinueDrag;
        }

        private void ListView_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed)
            {
                _dragCanceled = true;

                e.Action = DragAction.Cancel;

                e.Handled = true;
            }
        }

        private void ListView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
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

        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is ListView)
            {
                _dragCanceled = false;

                _startPoint = e.GetPosition(_listView);
            }
        }

        private void ListView_MouseMove(object sender, MouseEventArgs e)
        {
            if (CheckDragConditions(e))
            {
                Point endPoint = e.GetPosition(_listView);

                if (Math.Abs(endPoint.X - _startPoint.X) > (SystemParameters.MinimumHorizontalDragDistance + 4) |
                    Math.Abs(endPoint.Y - _startPoint.Y) > (SystemParameters.MinimumVerticalDragDistance + 4))
                {
                    if (e.OriginalSource is Thumb == false)
                    {
                        ListViewItem listViewItem = (e.OriginalSource as DependencyObject).GetParentOfType<ListViewItem>();

                        if (listViewItem?.Content is VideoItem videoItem)
                        {
                            _video = videoItem.Video;

                            StartDrag();
                        }
                    }
                }
            }
        }

        private bool CheckDragConditions(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return false;

            if (Kernel.Default.View != View.Playlists) return false;

            if (_dragCanceled) return false;

            if (_forbidDrag) return false;

            if (Kernel.Default.Search) return false;

            if (Kernel.Default.MenuIsOpen) return false;

            return true;
        }

        private void StartDrag()
        {
            _forbidDrag = true;
            Kernel.Default.IsDragDrop = true;

            DataObject data = new DataObject("YSM-Video", _video);

            CreateDragWindow();

            DragDrop.DoDragDrop(_listView, data, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link | DragDropEffects.None);

            DestroyDragWindow();

            _forbidDrag = false;
            Kernel.Default.IsDragDrop = false;
        }

        private void DestroyDragWindow()
        {
            if (_dragWindow != null)
            {
                _dragWindow.Close();
                _dragWindow = null;
            }
        }

        private void CreateDragWindow()
        {
            _dragWindow = new VideoDragWindow(_video);

            Point p = CursorPosition.Get();

            _dragWindow.Left = p.X + 10;
            _dragWindow.Top = p.Y + 10;

            _dragWindow.Show();
        }
    }
}
