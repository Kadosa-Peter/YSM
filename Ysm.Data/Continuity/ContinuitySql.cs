namespace Ysm.Data
{
    internal class ContinuitySql
    {
        internal static string Save()
        {
            return @"INSERT OR REPLACE INTO Continuity (Id, Second, Date) values (@Id, @Second, @Date)";
        }

        internal static string Get(string id)
        {
            return $"SELECT Second FROM Continuity Where Id = '{id}'";
        }
    }
}
