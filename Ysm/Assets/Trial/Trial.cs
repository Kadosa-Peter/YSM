using System;

namespace Ysm.Assets.Trial
{
    public static class Trial
    {
        // Hours left
        public static double Hours { get; set; }

        public static bool Check()
        {
            // -- lépések
            // 1. szerepel e a settingsben?
            // 2 ha nem szerepel a settingsben, akkor szerepel e az adatbázisban
            // 3 ha igen akkor érvényes-e
            // 4 ha nem akkor elindítom

            // ha ez bárhol igaz akkor engedélyezem a trial-t (vagy folytatom a korábban megkzedetet, vagy elindítom)
            TrialState state = ReadSettings();

            if (state == TrialState.Expired)
                return false;
            else if (state == TrialState.Valid)
                return true;

            state = ReadDatabase();

            // az adatbázisban sem szerepel, tehát el lehet indítani a trialt
            if (state == TrialState.Unkown)
            {
                StartTiral();

                return true;
            }
            else if (state == TrialState.Expired)
            {
                return false;
            }
            else // rialState.Valid
            {
                return true;
            }
        }

        private static TrialState ReadDatabase()
        {
            string id = TrialHelpers.GetIdentifier();
            string version = TrialHelpers.GetFileVersion();

            TrialEntry entry = TrialHelpers.DBReadTrial(id, version);

            // az adatbázisban sem szerepel, tehát el lehet indítani a trialt
            if (entry == null)
            {
                return TrialState.Unkown;
            }
            else
            {
                // ha kapok vissza értéket az adatbázisból akkor biztos, hogy a jelenlegi ysm verzió és pc van bejegyezve
                // nem kell összehasonlítani az id-t és a version-t
                TrialHelpers.SettingsSaveTrial(entry.DateTime);

                TimeSpan ts = DateTime.Now - entry.DateTime;

                double hours = Math.Round(ts.TotalHours);

                Hours = 168 - hours;
                if (Hours < 0)
                    Hours = 0;

                if (hours > 168)
                {
                    return TrialState.Expired;
                }
                else
                {
                    return TrialState.Valid;
                }
            }
        }

        private static TrialState ReadSettings()
        {
            double hours = TrialHelpers.SettingsReadTrial();

            Hours = 168 - hours;
            if (Hours < 0)
                Hours = 0;
            if (Hours > 168) // 168 - (-1) = 169 ?
                Hours = 168;

            // ha -1 kapok vissza, akkor nincs vagy hibás a bejegyzés
            // double miatt < 0
            if (hours < 0)
            {
                return TrialState.Unkown;
            }

            // ha 168-nél nagyobb értéket kapok vissza, akkor lejárt a trial ideje
            if (hours > 168)
            {
                return TrialState.Expired;
            }

            return TrialState.Valid;
        }

        private static void StartTiral()
        {
            TrialEntry entry = new TrialEntry();
            entry.Id = TrialHelpers.GetIdentifier();
            entry.Version = TrialHelpers.GetFileVersion();
            entry.DateTime = DateTime.Now;
            entry.Country = TrialHelpers.GetCountry();

            Hours = 168;

            TrialHelpers.SettingsSaveTrial(entry.DateTime);

            if (entry.Id != null)
            {
                TrialHelpers.DBSaveTrial(entry);
            }
        }
    }
}
