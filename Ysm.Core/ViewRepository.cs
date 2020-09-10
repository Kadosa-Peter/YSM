using System.Collections.Generic;
using System.Windows.Controls;

namespace Ysm.Core
{
    public static class ViewRepository
    {
        private static readonly Dictionary<string, object> _views;

        static ViewRepository()
        {
            _views = new Dictionary<string, object>();
        }

        public static void Add(UserControl view, string name)
        {
            if (!_views.ContainsKey(name))
            {
                _views.Add(name, view);
            }
        }

        public static T Get<T>()
        {
            string name = typeof(T).Name;

            if (_views.ContainsKey(name))
            {
                return (T)_views[name];
            }

            return default(T);
        }
    }
}
