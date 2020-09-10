using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using Ysm.Annotations;
using Ysm.Core;
using Ysm.Models;
using Ysm.Views;

namespace Ysm.Assets
{
    public class PlayEngine : INotifyPropertyChanged
    {
        public bool AutoPlay
        {
            get => _autoPlay;

            set
            {
                if (_autoPlay != value)
                {
                    _autoPlay = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _autoPlay;

        public bool Shuffle
        {
            get => _shuffle;

            set
            {
                if (_shuffle != value)
                {
                    _shuffle = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _shuffle;

        public bool Repeat
        {
            get => _repeat;

            set
            {
                if (_repeat != value)
                {
                    _repeat = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _repeat;

        private ObservableCollection<VideoItem> _collection;

        private List<string> _history;

        private List<string> _random;

        private string _currentItem;

        public PlayEngine(ObservableCollection<VideoItem> collection)
        {
            _collection = collection;

            _history = new List<string>();
            _random = new List<string>();
        }

        public void Reset()
        {
            // csak mappa váltásnál állítom vissza alapértelmezettre, keresésnél nem
            _currentItem = null;
            _random.Clear();
            _history.Clear();

            AutoPlay = false;
            Shuffle = false;
            Repeat = false;
        }

        public VideoItem RandomNext()
        {
            if (_collection.Empty()) return null;

            try
            {
                if (_collection.Count == _random.Count && Repeat)
                {
                    _random.Clear();
                }

                // Elindítok egy videót és beállítom a véletlenszerű lejátszást, akkor ugyanaz a szám visszajöhet következőnek,
                // mert az még nincs a random listában.
                if (!_random.Contains(_currentItem))
                {
                    _random.Add(_currentItem);
                }

                List<VideoItem> items = _collection.Where(x => _random.All(y => y != x.Id)).ToList();

                if (items.Empty()) return null;

                Random rnd = new Random();
                int index = rnd.Next(0, items.Count);

                VideoItem ni = items[index];

                _random.Add(ni.Id);
                _history.Add(ni.Id);
                _currentItem = ni.Id;

                return ni;

            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }

            return null;
        }

        public VideoItem Next()
        {
            if (_collection.Empty()) return null;

            try
            {
                VideoItem ci = _collection.FirstOrDefault(x => x.Id == _currentItem);

                if (ci != null)
                {
                    int index = _collection.IndexOf(ci);

                    index++;

                    if (index < _collection.Count)
                    {
                        VideoItem ni = _collection[index];

                        // Amikor egy Previous()-al előhívott videó lejátszása befejeződött akkor egy olyan videó következik, ami már szerepel a listában.
                        // Ez alól csak egy kivétel van, amikor véletlenszerűen játszok le videókat.
                        if (_history.Last() != ni.Id)
                        {
                            _currentItem = ni.Id;
                            _history.Add(ni.Id);
                        }

                        return ni;
                    }

                    if (Repeat)
                    {
                        VideoItem ni = _collection[0];
                        _currentItem = ni.Id;
                        _history.Add(ni.Id);
                        return ni;
                    }

                }
                else
                {
                    VideoItem ni = _collection[0];
                    _currentItem = ni.Id;
                    _history.Add(ni.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }

            return null;
        }

        public VideoItem Previous()
        {
            if (_history.Count < 2) return null;

            try
            {
                int index = _history.Count - 2;

                string item = _history[index];

                var pi = _collection.FirstOrDefault(x => x.Id == item);

                if (pi != null) // keresésnél lehet null, mert az a videó már nincs a listában
                    _currentItem = pi.Id;
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }

            return null;
        }

        // Ha manuálisan egérrel nyitok meg egy videót, akkor azt vissza kell rögzítenem ide.
        // Ezt a metódust a VideoStarted eseményből hívom meg.
        // Ezzel ki tudom védeni, hogy a Previos-nál be legyen rögzitve a videó.
        public void Set(string id)
        {
            if (_currentItem != id)
            {
                _currentItem = id;

                _history.Add(id);
            }
        }

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == "AutoPlay")
            {
                if (AutoPlay)
                    ViewRepository.Get<FooterView>().SetAutoplay(Visibility.Visible);
                else
                    ViewRepository.Get<FooterView>().SetAutoplay(Visibility.Collapsed);
            }

            if (propertyName == "Shuffle")
            {
                if (Shuffle)
                    ViewRepository.Get<FooterView>().SetShuffle(Visibility.Visible);
                else
                    ViewRepository.Get<FooterView>().SetShuffle(Visibility.Collapsed);
            }

            if (propertyName == "Repeat")
            {
                if (Repeat)
                    ViewRepository.Get<FooterView>().SetRepeatAll(Visibility.Visible);
                else
                    ViewRepository.Get<FooterView>().SetRepeatAll(Visibility.Collapsed);
            }
        }

        #endregion
    }
}
