namespace Ysm.Data
{
    internal static class ChannelSql
    {
        internal static string Insert()
        {
            return @"INSERT INTO Channels(Id, SubscriptionId, Parent, Title, Date, Thumbnail) 
                        VALUES(@Id, @SubscriptionId, @Parent, @Title, @Date, @Thumbnail)";
        }

        internal static string Get()
        {
            return "SELECT * FROM Channels";
        }

        internal static string Get_By_Id(string id)
        {
            return $"SELECT * FROM Channels WHERE Id = '{id}'";
        }

        internal static string Get_By_Parent(string parent)
        {
            return $"SELECT * FROM Channels WHERE Parent = '{parent}'";
        }

        internal static string Move(string id, string parent)
        {
            return $"UPDATE Channels SET Parent='{parent}' WHERE Id='{id}'";
        }

        internal static string Search(string query)
        {
            return $"SELECT * FROM Channels WHERE Title LIKE '%{query}%'";
        }

        internal static string Delete(string id)
        {
            return $"DELETE FROM Channels WHERE Id='{id}'";
        }

        internal static string DeleteAll()
        {
            return "DELETE FROM Channels";
        }


        internal static string Update()
        {
            return @"UPDATE Channels SET Parent=@Parent WHERE Id=@Id";
        }
    }
}
