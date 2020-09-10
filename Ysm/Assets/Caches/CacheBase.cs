using System.Collections.Generic;

namespace Ysm.Assets.Caches
{
    public abstract class CacheBase
    {
        private readonly List<string> _ids;

        public event CacheDelegate Added;
        public event CacheDelegate Removed;
        public event CacheDelegate AllRemoved;


        public int Count => _ids.Count;

        protected CacheBase()
        {
            _ids = new List<string>();
        }

        public void Add(string id)
        {
            _ids.Add(id);

            Added?.Invoke(id);
        }

        public void AddRange(IEnumerable<string> ids)
        {
            _ids.AddRange(ids);
        }

        public void Remove(string id)
        {
            _ids.Remove(id);

            Removed?.Invoke(id);
        }

        public void RemoveAll()
        {
            _ids.Clear();

            AllRemoved?.Invoke(null);
        }

        public bool Contains(string id)
        {
            return _ids.Contains(id);
        }
    }
}
