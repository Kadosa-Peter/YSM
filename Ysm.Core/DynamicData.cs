using System.Collections.Generic;

namespace Ysm.Core
{
    public sealed class DynamicData
    {
        private readonly Dictionary<string, dynamic> _data;

        public DynamicData()
        {
            _data = new Dictionary<string, dynamic>();
        }

        public void Add(string key, dynamic data)
        {
            _data.Add(key, data);
        }

        public dynamic Get(string key)
        {
            if (_data.TryGetValue(key, out var data))
            {
                return data;
            }

            return null;
        }

        public dynamic Take(string key)
        {
            if (_data.TryGetValue(key, out var data))
            {
                _data.Remove(key);

                return data;
            }

            return null;
        }

        public dynamic this[string index]
        {
            get
            {
                _data.TryGetValue(index, out var result);
                return result;
            }
            set => _data[index] = value;
        }

        public bool IsEmpty()
        {
            return _data.Count == 0;
        }

        public int Count()
        {
            return _data.Count;
        }

        public bool Contains(string key)
        {
            return _data.ContainsKey(key);
        }
    }
}
