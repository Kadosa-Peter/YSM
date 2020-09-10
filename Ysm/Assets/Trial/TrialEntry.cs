using System;

namespace Ysm.Assets.Trial
{
    public class TrialEntry
    {
        // CPU or Motherboard
        public string Id { get; set; }

        // YSM version 1.0.0.
        public string Version { get; set; }

        // Trial started
        public DateTime DateTime { get; set; }

        public string Country { get; set; }
    }
}