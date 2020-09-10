using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models;
using Ysm.Views;

namespace Ysm.Assets
{
    internal class Mark
    {
        public void MarkUnwached(ObservableCollection<object> selectedNodes, Node root)
        {
            List<Node> nodes = GetNodes(selectedNodes);

            Task.Run(() =>
            {
                List<string> channels = GetChannels(nodes);

                Repository.Default.Videos.MarkUnwatchedByChannelId(channels);

            }).ContinueWith(t =>
            {
                root.Count = Repository.Default.Videos.UnwatchedCount();

                UpdateNodeCount(nodes);

                if (Settings.Default.VideoDisplayMode == VideoDisplayMode.AllVideos)
                {
                    ViewRepository.Get<VideoView>().MarkUnwatched();
                }
                else
                {
                    ViewRepository.Get<VideoView>().ListVideos(Kernel.Default.SelectedChannels);

                }

                ViewRepository.Get<FooterView>().UpdateCount();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void MarkAllUnwached(ObservableCollection<object> selectedNodes, Node root)
        {
            Task.Run(() => { Repository.Default.Videos.MarkAllUnwatched(); }).ContinueWith(t =>
            {
                ViewRepository.Get<ChannelView>().Reset();
                ViewRepository.Get<VideoView>().RemoveAll();
                ViewRepository.Get<FooterView>().UpdateCount();

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        public void MarkWached(ObservableCollection<object> selectedNodes, Node root)
        {
            List<Node> nodes = GetNodes(selectedNodes);

            Task.Run(() =>
            {
                List<string> channels = GetChannels(nodes);

                Repository.Default.Videos.MarkWatched(channels);

            }).ContinueWith(t =>
            {
                root.Count = Repository.Default.Videos.UnwatchedCount();
                SetNodeCountToZero(nodes);

                if (Settings.Default.VideoDisplayMode == VideoDisplayMode.AllVideos)
                {
                    ViewRepository.Get<VideoView>().MarkWatched();
                }
                else
                {
                    ViewRepository.Get<VideoView>().RemoveAll();
                }

                ViewRepository.Get<FooterView>().UpdateCount();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void MarkAllWached(Node root)
        {
            Task.Run(() => { Repository.Default.Videos.MarkAllWatched(); }).ContinueWith(t =>
            {
                ViewRepository.Get<ChannelView>().Reset();
                ViewRepository.Get<VideoView>().RemoveAll();
                ViewRepository.Get<FooterView>().UpdateCount();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private List<string> GetChannels(List<Node> nodes)
        {
            List<string> channels = nodes.Where(x => x.NodeType == NodeType.Channel).Select(x => x.Id).ToList();
            List<string> categories = nodes.Where(x => x.NodeType == NodeType.Category).Select(x => x.Id).ToList();

            foreach (string category in categories)
            {
                foreach (string channel in Repository.Default.Schema.GetChannelChildren(category))
                {
                    if (channels.Contains(channel) == false)
                        channels.Add(channel);
                }
            }

            return channels;
        }

        private static List<Node> GetNodes(ObservableCollection<object> selectedNodes)
        {
            List<Node> nodes = new List<Node>();

            foreach (Node node in selectedNodes.Cast<Node>())
            {
                nodes.Add(node);

                if (node.NodeType == NodeType.Category)
                {
                    nodes.AddRange(node.GetChildren());
                }
            }

            return nodes;
        }

        private void SetNodeCountToZero(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                if (node.NodeType == NodeType.Category)
                {
                    node.Count = 0;


                    if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                    {
                        bool hasCategoryChildren = Repository.Default.Schema.HasCategoryChildren(node.Id);

                        if (node.IsExpanded == false)
                        {
                            if (hasCategoryChildren)
                            {
                                node.Items.Clear();
                                node.Items.Add(new Node());
                            }
                            else
                            {
                                node.Items.Clear();
                            }
                        }
                    }
                }
                else
                {
                    node.Count = 0;

                    if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                    {
                        node.Remove();
                    }
                }
            }
        }

        private void UpdateNodeCount(List<Node> nodes)
        {
            List<Node> parents = new List<Node>();

            foreach (Node node in nodes)
            {
                List<Node> ancestors = node.GetAncestors();
                ancestors.ForEach(x => parents.AddOrThrow(x));

                if (node.NodeType == NodeType.Category)
                {
                    node.Count = Repository.Default.Categories.GetState(node.Id);

                    if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                    {
                        if (node.IsExpanded)
                        {
                            ViewRepository.Get<ChannelView>().ListItems(node);
                        }
                        else
                        {
                            node.Items.Add(new Node());
                        }
                    }
                }
                else
                {
                    node.Count = Repository.Default.Channels.GetState(node.Id);
                }
            }

            foreach (Node node in parents)
            {
                node.Count = Repository.Default.Categories.GetState(node.Id);
            }
        }

    }
}
