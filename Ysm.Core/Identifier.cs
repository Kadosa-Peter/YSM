using System;

namespace Ysm.Core
{
    public static class Identifier
    {
        private static string _empty;

        public static string Empty => _empty ?? (_empty = Guid.Empty.ToString());

        public static string New => Guid.NewGuid().ToString();

        public static string Sort
        {
            get
            {
                string id = Guid.NewGuid().ToString();

                id = id.Replace("-", "");

                id = id.Remove(8);

                return id;
            }

        }
    }
}
