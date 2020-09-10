using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Ysm.Core;

namespace Ysm.Assets
{
    public class SearchEngine<T>
    {
        public ReactiveProperty<string> SearchQuery { get; set; }

        public event EventHandler<SearchEventArgs<T>> Search;

        private readonly Func<string, IEnumerable<T>> _func;

        // a channelview-ban kell, hogy ne történjen meg a csatorna újrakivállasztása keresés után
        // ezzel megakadályozom a videoview scroll poziciója visszaugorjon a lista tetejére
        private bool _reset;

        public object Catch { get; set; }

        public bool IsSearch { get; set; }

        public SearchEngine(Func<string, IEnumerable<T>> func)
        {
            SearchQuery = new ReactiveProperty<string>(default(string));
            SearchQuery.PropertyChanged += Query_PropertyChanged;
            
            _func = func;

            IObservable<IEnumerable<T>> queryObsevable =
                SearchQuery.
                    Throttle(TimeSpan.FromMilliseconds(500)).
                    Where(x => x.NotNull()).
                    Select(query =>
                        GetSearchResult(query).
                            ToObservable().
                            Timeout(TimeSpan.FromMilliseconds(2000))).
                    Switch().
                    ObserveOnUIDispatcher().
                    Retry(3);

            queryObsevable.Subscribe(GenerateList);
        }

        private void Query_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SearchQuery.Value.IsNull())
            {
                IsSearch = false;

                _reset = true;

                Search?.Invoke(this, new SearchEventArgs<T> { Reset = true });
            }
        }

        private void GenerateList(IEnumerable<T> items)
        {
            IsSearch = true;

            Search?.Invoke(this, new SearchEventArgs<T> { Result = items });
        }

        private Task<IEnumerable<T>> GetSearchResult(string query)
        {
            return Task.FromResult(_func(query));
        }

        public bool IsReset()
        {
            if (_reset)
            {
                _reset = false;
                return true;
            }

            return false;
        }

        public void Clear()
        {
            Catch = null;

            if (SearchQuery.Value.NotNull())
            {
                SearchQuery.Value = string.Empty;
            }
        }
    }
}
