using System.Collections.Generic;
using System.Text;

namespace Ysm.Data
{
    internal class HistorySql
    {
        internal static string Insert()
        {
            return @"INSERT INTO History(Id, Added) VALUES(@Id, @Added)";
        }

        internal static string Get()
        {
            return "SELECT Videos.*, History.Added FROM Videos INNER JOIN History ON History.Id = Videos.VideoId";
        }

        internal static string Remove(IEnumerable<string> ids)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM History WHERE Id IN(");

            foreach (string id in ids)
            {
                builder.Append("'");
                builder.Append(id);
                builder.Append("',");
            }

            builder.Remove(builder.Length - 1, 1);

            builder.Append(");");

            return builder.ToString();
        }

        internal static string Remove(string id)
        {
            return $"DELETE FROM History WHERE Id ='{id}'";
        }

        internal static string RemoveAll()
        {
            return "DELETE FROM History";
        }
    }
}
