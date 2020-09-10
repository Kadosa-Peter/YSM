using System.Windows;
using System.Windows.Controls.Primitives;

namespace Ysm.Controls
{
    public class ExtendedTreeItem : TreeItemBase
    {
        //! Controls
        private const string PART_RenameBox = "PART_RenameBox";
        private const string PART_Expander = "PART_Expander";
        private RenameBox _renameBox;
        private ToggleButton _expander;

        //! CTRO
        static ExtendedTreeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedTreeItem), new FrameworkPropertyMetadata(typeof(ExtendedTreeItem)));
        }

        //! OnApplyTemplate
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _renameBox = GetTemplateChild(PART_RenameBox) as RenameBox;

            if (_renameBox != null)
            {
                _renameBox.BeforeRename -= RenameBox_BeforeRename;
                _renameBox.BeforeRename += RenameBox_BeforeRename;

                _renameBox.AfterRename -= RenameBox_AfterRename;
                _renameBox.AfterRename += RenameBox_AfterRename;
            }

            _expander = GetTemplateChild(PART_Expander) as ToggleButton;

            if (_expander != null)
            {
                _expander.DragEnter -= Expander_DragEnter;
                _expander.DragEnter += Expander_DragEnter;
            }
        }

        private void RenameBox_BeforeRename(object sender, RoutedEventArgs e)
        {
            RaiseBeforeRenameEvent();
        }

        private void RenameBox_AfterRename(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            RaiseAfterRenameEvent(e.OldValue, e.NewValue);
        }

        private void Expander_DragEnter(object sender, DragEventArgs e)
        {
            if (!IsExpanded)
            {
                IsExpanded = true;
            }
        }

        protected override void OnCollapsed(RoutedEventArgs e)
        {
            if (CanCollapse == false)
            {
                e.Handled = true;
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            IsDragOver = true;

            e.Handled = true;
        }

        

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);

            IsDragOver = false;

            e.Handled = true;
        }

        public void StartRename()
        {
            if (CanRename && IsRendered)
            {
                _renameBox.StartRename();
            }
        }

        public void EndRename()
        {
            if (IsRendered)
            {
                _renameBox.StartRename();
            }
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
    }
}
