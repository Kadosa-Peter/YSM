using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models.Iteration;
using Ysm.Views;

namespace Ysm.Windows
{
    public partial class IterationWindow
    {
        public ObservableCollection<Node> Nodes { get; set; }
        public ObservableCollection<object> SelectedNodes { get; set; }

        public IterationWindow()
        {
            InitializeComponent();

            Nodes = new ObservableCollection<Node>();
            SelectedNodes = new ObservableCollection<object>();
        }

        private void IterationWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                IEnumerable<IGrouping<DateTime, DateTime>> list = Repository.Default.Videos.Get()
                    .Where(x => x.Added > DateTime.Now.AddDays(-30))
                    .Select(x => x.Added)
                    .Distinct()
                    .OrderByDescending(x => x)
                    .GroupBy(x => x.Date);

                List<Node> nodes = new List<Node>();

                foreach (IGrouping<DateTime, DateTime> dateTimes in list)
                {
                    Node dateNode = new Node();
                    dateNode.Title = dateTimes.Key.ToString("d", CultureInfo.CurrentCulture);
                    dateNode.DateTime = dateTimes.Key;
                    dateNode.CanCollapse = true;
                    dateNode.NodeType = NodeType.Date;

                    nodes.Add(dateNode);

                    foreach (DateTime dateTime in dateTimes)
                    {
                        Node iterationNode = new Node();
                        iterationNode.Title = dateTime.ToString("g", CultureInfo.CurrentCulture);
                        iterationNode.DateTime = dateTime;
                        iterationNode.NodeType = NodeType.Iteration;
                        iterationNode.Parent = dateNode;

                        dateNode.Items.Add(iterationNode);
                    }
                }

                return nodes;
            }).ContinueWith(t => { t.Result.ForEach(x => Nodes.Add(x)); }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void IterationWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Header_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Footer_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void NodeTree_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Task.Run(() =>
            {
                List<Node> selectedNodes = SelectedNodes.Cast<Node>().ToList();

                List<DateTimeOffset> date = selectedNodes.Where(x => x.NodeType == NodeType.Date).Select(x => x.DateTime).ToList();
                List<DateTimeOffset> iteration = selectedNodes.Where(x => x.NodeType == NodeType.Iteration).Select(x => x.DateTime).ToList();

                if (date.Any() || iteration.Any())
                {
                    Repository.Default.Videos.MarkAllWatched();

                    if (date.Any())
                    {
                        Repository.Default.Videos.MarkUnwatchedDate(date);
                    }

                    if (iteration.Any())
                    {
                        Repository.Default.Videos.MarkUnwatched(iteration);
                    }

                    Repository.Default.Videos.ResetVideoStated();
                }
            }).ContinueWith(t =>
            {
                ViewRepository.Get<ChannelView>().Reset();
                ViewRepository.Get<VideoView>().Reset();
                ViewRepository.Get<FooterView>().UpdateCount();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        
    }
}
