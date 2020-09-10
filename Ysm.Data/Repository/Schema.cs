using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ysm.Core;

namespace Ysm.Data
{
    public class Schema
    {
        private readonly List<SchemaObj> _videos;
        private readonly List<SchemaObj> _categories;
        private readonly List<SchemaObj> _channels;

        public Schema()
        {
            Task<IEnumerable<SchemaObj>> t1 = Task.Run(() => { return VideoQueries.Get().Select(x => new SchemaObj { Id = x.VideoId, Parent = x.ChannelId, State = x.State }); });

            Task<IEnumerable<SchemaObj>> t2 = Task.Run(() => { return CategoryQueries.Get().Select(x => new SchemaObj { Id = x.Id, Parent = x.Parent }); });

            Task<IEnumerable<SchemaObj>> t3 = Task.Run(() => { return ChannelQueries.Get().Select(x => new SchemaObj { Id = x.Id, Parent = x.Parent }); });

            Task.WaitAll(t1, t2, t3);

            _videos = new List<SchemaObj>();
            _videos.AddRange(t1.Result);

            _categories = new List<SchemaObj>();
            _categories.AddRange(t2.Result);

            _channels = new List<SchemaObj>();
            _channels.AddRange(t3.Result);

            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

            Parallel.ForEach(_channels, options, channel =>
            {
                channel.State = _videos.Count(x => x.Parent == channel.Id && x.State == 0);
            });

            Parallel.ForEach(_categories, options, category =>
            {
                category.State = _channels.Where(x => x.Parent == category.Id).Sum(x => x.State);
            });
        }

        public void Reset()
        {
            _videos.Clear();
            _videos.AddRange(VideoQueries.Get().Select(x => new SchemaObj { Id = x.VideoId, Parent = x.ChannelId, State = x.State }));

            foreach (SchemaObj channel in _channels)
            {
                channel.State = _videos.Count(x => x.Parent == channel.Id && x.State == 0);
            }

            foreach (SchemaObj category in _categories)
            {
                category.State = _channels.Where(x => x.Parent == category.Id).Sum(x => x.State);
            }
        }

        // ------------------------------------------------ //

        public void Insert(List<Category> categories)
        {
            foreach (Category category in categories)
            {
                SchemaObj obj = new SchemaObj
                {
                    Parent = category.Parent,
                    Id = category.Id
                };

                _categories.Add(obj);
            }
        }

        public void Insert(Category category)
        {
            SchemaObj obj = new SchemaObj
            {
                Parent = category.Parent,
                Id = category.Id
            };

            _categories.Add(obj);
        }

        public void Insert(List<Channel> channels)
        {
            _channels.AddRange(channels.Select(x => new SchemaObj { Id = x.Id, Parent = x.Parent, State = x.State }));
        }

        public void Insert(List<Video> videos)
        {
            _videos.AddRange(videos.Select(x => new SchemaObj { Id = x.VideoId, Parent = x.ChannelId }));

            ILookup<string, Video> lookup = videos.ToLookup(x => x.ChannelId);

            foreach (IGrouping<string, Video> grouping in lookup)
            {
                SchemaObj channel = _channels.FirstOrDefault(x => x.Id == grouping.Key);

                if (channel != null)
                {
                    int count = grouping.Count();

                    channel.State += count;

                    SchemaObj category = _categories.FirstOrDefault(x => x.Id == channel.Parent);

                    if (category != null)
                    {
                        category.State += count;
                    }
                }
            }
        }

        // ------------------------------------------------ //

        public int GetChannelCount()
        {
            return _channels.Count;
        }

        public int GetUnwatchedVideoCount()
        {
            return _videos.Count(x => x.State == 0);
        }

        public int GetUnwatchedVideoCount(string parent)
        {
            return _videos.Where(x => x.Parent == parent).Count(x => x.State == 0);
        }

        public bool HasUnwatchedVideoOrCategory(string id)
        {
            if (_categories.Any(x => x.Parent == id))
                return true;

            List<SchemaObj> objs = GetChildrenObjs(id);

            return objs.Any(x => x.State > 0);
        }

        private List<SchemaObj> GetChildrenObjs(string id)
        {
            List<SchemaObj> objs = new List<SchemaObj>();

            objs.AddRange(_channels.Where(x => x.Parent == id));

            foreach (SchemaObj obj in _categories)
            {
                if (obj.Parent == id)
                {
                    objs.AddRange(GetChildrenObjs(obj.Id));
                }
            }

            return objs;
        }

        public int GetChannelState(string id)
        {
            SchemaObj obj = _channels.FirstOrDefault(x => x.Id == id);
            if (obj != null) return obj.State;

            return 0;
        }

        public int GetCategoryState(string id)
        {
            int count = 0;

            foreach (SchemaObj obj in _channels)
            {
                if (obj.Parent == id)
                {
                    count += obj.State;
                }
            }

            foreach (SchemaObj obj in _categories)
            {
                if (obj.Parent == id)
                {
                    count += GetCategoryState(obj.Id);
                }
            }

            return count;
        }

        public List<string> GetActiveChannels()
        {
            List<string> channels = new List<string>();

            foreach (SchemaObj obj in _channels)
            {
                if (obj.State > 0)
                    channels.Add(obj.Id);
            }

            return channels;
        }

        // ------------------------------------------------ //

        public List<string> GetChildren(string parent)
        {
            List<string> children = new List<string>();

            foreach (SchemaObj obj in _channels)
            {
                if (obj.Parent == parent)
                {
                    children.Add(obj.Id);
                }
            }

            foreach (SchemaObj obj in _categories)
            {
                if (obj.Parent == parent)
                {
                    children.Add(obj.Id);

                    children.AddRange(GetChildren(obj.Id));
                }
            }

            return children;
        }

        public bool HasChildren(string id)
        {
            return _channels.Any(x => x.Parent == id) || _categories.Any(x => x.Parent == id);
        }

        public List<string> GetChannelChildren(string parent)
        {
            List<string> children = new List<string>();

            foreach (SchemaObj obj in _channels)
            {
                if (obj.Parent == parent)
                {
                    children.Add(obj.Id);
                }
            }

            foreach (SchemaObj obj in _categories)
            {
                if (obj.Parent == parent)
                {
                    children.AddRange(GetChannelChildren(obj.Id));
                }
            }

            return children;
        }

        public List<string> GetCategoryChildren(string parent)
        {
            List<string> children = new List<string>();

            foreach (SchemaObj obj in _categories)
            {
                if (obj.Parent == parent)
                {
                    children.Add(obj.Id);

                    children.AddRange(GetCategoryChildren(obj.Id));
                }
            }

            return children;
        }

        public bool HasCategoryChildren(string id)
        {
            return _categories.Any(x => x.Parent == id);
        }

        public List<string> GetAncestors(string id)
        {
            List<string> list = new List<string>();

            GetAncestors(list, id);

            return list;
        }

        private void GetAncestors(List<string> list, string id)
        {
            SchemaObj obj = _channels.FirstOrDefault(x => x.Id == id);

            if (obj != null)
            {
                list.Insert(0, obj.Id);
                if (obj.Id != Identifier.Empty)
                    GetAncestors(list, obj.Parent);
            }

            if (obj == null)
            {
                obj = _categories.FirstOrDefault(x => x.Id == id);

                if (obj != null)
                {
                    list.Insert(0, obj.Id);
                    if (obj.Id != Identifier.Empty)
                        GetAncestors(list, obj.Parent);
                }
            }
        }

        public bool IsDescendant(string parent, string id)
        {
            return GetChildren(parent).Contains(id);
        }

        // ------------------------------------------------ //

        public void MarkVideoWatched(string videoId, string channelId)
        {
            string categoryId = _channels.FirstOrDefault(x => x.Id == channelId)?.Parent;

            SchemaObj obj1 = _videos.FirstOrDefault(x => x.Id == videoId);
            if (obj1 != null) obj1.State = 1;

            SchemaObj obj2 = _channels.FirstOrDefault(x => x.Id == channelId);
            if (obj2 != null) obj2.State--;

            SchemaObj obj3 = _categories.FirstOrDefault(x => x.Id == categoryId);
            if (obj3 != null) obj3.State--;
        }

        public void MarkAllWatched()
        {
            foreach (SchemaObj obj in _videos)
            {
                obj.State = 1;
            }

            foreach (SchemaObj obj in _channels)
            {
                obj.State = 0;
            }

            foreach (SchemaObj category in _categories)
            {
                category.State = 0;
            }
        }

        public void MarkWatched(List<string> ids)
        {
            foreach (string id in ids)
            {
                SchemaObj channel = _channels.FirstOrDefault(x => x.Id == id);
                if (channel != null)
                {
                    foreach (SchemaObj obj in _videos)
                    {
                        if (obj.Parent == id)
                            obj.State = 1;
                    }

                    channel.State = 0;
                    continue;
                }

                SchemaObj category = _categories.FirstOrDefault(x => x.Id == id);
                if (category != null)
                {
                    category.State = 0;
                }
            }
        }

        // ------------------------------------------------ //

        public void MoveCategory(Dictionary<string, string> dictionary)
        {
            foreach (KeyValuePair<string, string> kvp in dictionary)
            {
                SchemaObj obj = _categories.FirstOrDefault(x => x.Id == kvp.Key);

                if (obj != null)
                {
                    obj.Parent = kvp.Value;
                }
            }
        }

        public void MoveChannel(Dictionary<string, string> dictionary)
        {
            // kvp => key = id, value = new parent id

            foreach (KeyValuePair<string, string> kvp in dictionary)
            {
                SchemaObj obj = _channels.FirstOrDefault(x => x.Id == kvp.Key);

                if (obj == null) continue;

                // decrease oldparent state (video count)
                SchemaObj oldParent = _categories.FirstOrDefault(x => x.Id == obj.Parent);

                if (oldParent != null)
                {
                    oldParent.State -= obj.State;
                }

                // assign new parent
                obj.Parent = kvp.Value;

                SchemaObj newParent = _categories.FirstOrDefault(x => x.Id == obj.Parent);

                if (newParent != null)
                {
                    newParent.State += obj.State;
                }
            }
        }

        // ------------------------------------------------ //

        public void DeleteVideosByParent(List<string> ids)
        {
            foreach (SchemaObj obj in _videos.ToList())
            {
                if (ids.Contains(obj.Parent))
                    _videos.Remove(obj);
            }
        }

        public void DeleteChannels(List<string> ids)
        {
            foreach (SchemaObj obj in _channels.ToList())
            {
                if (ids.Contains(obj.Id))
                    _channels.Remove(obj);
            }
        }

        public void DeleteCategories(List<string> ids)
        {
            foreach (SchemaObj obj in _categories.ToList())
            {
                if (ids.Contains(obj.Id))
                    _categories.Remove(obj);
            }
        }

        public void DeleteCategories()
        {
            _categories.Clear();

            foreach (SchemaObj obj in _channels)
            {
                obj.Parent = Identifier.Empty;
            }
        }
    }
}
