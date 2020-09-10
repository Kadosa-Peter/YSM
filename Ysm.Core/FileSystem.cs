using System;
using System.IO;
using System.Reflection;

namespace Ysm.Core
{
    public class FileSystem
    {
        private static string _startup;
        private static string _appData;
        private static string _settings;
        private static string _update;
        private static string _logs;
        private static string _user;
        private static string _currentUser;
        private static string _service;
        private static string _thumbnails;
        private static string _playlists;
        private static string _markers;
        private static string _downloads;
        private static string _database;
        private static string _connectionString;
        private static string _temp;
        private static string _backup;

        public static string Startup
        {
            get
            {
                if (_startup == null)
                {
                    _startup = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }

                return _startup;
            }
        }

        public static string AppData
        {
            get
            {
                if (_appData == null)
                {
                    _appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                    if (DebugMode.IsDebugMode)
                    {
                        _appData = Path.Combine(_appData, "Debug", "Ysm", "v2");
                    }
                    else
                    {
                        _appData = Path.Combine(_appData, "Ysm", "v2");
                    }

                    if (Directory.Exists(_appData) == false)
                    {
                        Directory.CreateDirectory(_appData);
                    }
                }

                return _appData;
            }
        }

        public static string Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = Path.Combine(AppData, "Settings.json");
                }

                return _settings;
            }
        }

        public static string Update
        {
            get
            {
                if (_update == null)
                {
                    _update = Path.Combine(AppData, "Update.txt");
                }

                return _update;
            }
        }

        public static string Logs
        {
            get
            {
                if (_logs == null)
                {
                    _logs = Path.Combine(AppData, "Logs");

                    if (!Directory.Exists(_logs))
                    {
                        Directory.CreateDirectory(_logs);
                    }
                }

                return _logs;
            }
        }

        public static string User
        {
            get
            {
                if (Directory.Exists(_user) == false)
                {
                    Directory.CreateDirectory(_user);
                }

                return _user;
            }
            set
            {
                _user = value;
                Reset();
            }
        }

        public static string CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    _currentUser = Path.Combine(AppData, "CurrentUser");
                }

                return _currentUser;
            }
        }

        public static string Service
        {
            get
            {
                if (_service == null)
                {
                    _service = Path.Combine(User, "Service");
                }

                return _service;
            }
        }

        public static string Thumbnails
        {
            get
            {
                if (_thumbnails == null)
                {
                    _thumbnails = Path.Combine(User, "Thumbnails");

                    if (!Directory.Exists(_thumbnails))
                    {
                        Directory.CreateDirectory(_thumbnails);
                    }
                }

                return _thumbnails;
            }
        }

        public static string Playlists
        {
            get
            {
                if (_playlists == null)
                {
                    _playlists = Path.Combine(User, "Playlists");

                    if (!Directory.Exists(_playlists))
                    {
                        Directory.CreateDirectory(_playlists);
                    }
                }

                return _playlists;
            }
        }

        public static string Markers
        {
            get
            {
                if (_markers == null)
                {
                    _markers = Path.Combine(User, "Markers");

                    if (!Directory.Exists(_markers))
                    {
                        Directory.CreateDirectory(_markers);
                    }
                }

                return _markers;
            }
        }

        public static string Downloads
        {
            get
            {
                if (_downloads == null)
                {
                    _downloads = Path.Combine(AppData, "Downloads");
                }

                return _downloads;
            }
        }

        public static string Database
        {
            get
            {
                if (_database == null)
                {
                    _database = Path.Combine(User, "Data.sqlite3");
                }

                return _database;
            }
        }

        public static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    string database = Database;

                    _connectionString = $"Data Source={database};Version=3;PRAGMA automatic_index = 0;";
                }

                return _connectionString;
            }
        }

        public static string Temp
        {
            get
            {
                if (_temp == null)
                {
                    _temp = Path.Combine(AppData, "Temp");

                    if (!Directory.Exists(_temp))
                    {
                        Directory.CreateDirectory(_temp);
                    }
                }

                return _temp;
            }
        }

        public static string Backup
        {
            get
            {
                if (_backup == null)
                {
                    _backup = Path.Combine(AppData, "Backup");

                    if (!Directory.Exists(_backup))
                    {
                        Directory.CreateDirectory(_backup);
                    }
                }

                return _backup;
            }
        }

        private static void Reset()
        {
            _service = null;
            _thumbnails = null;
            _playlists = null;
            _downloads = null;
            _database = null;
            _connectionString = null;
            _temp = null;
        }
    }
}
