namespace Ysm.Data
{
    internal static class CategorySql
    {
        internal static string Insert()
        {
            return @"INSERT INTO Categories(Id, Parent, Title, Color) VALUES(@Id, @Parent, @Title, @Color)";
        }

        internal static string Get()
        {
            return "SELECT * FROM Categories";
        }

        internal static string Get(string parent)
        {
            return $"SELECT * FROM Categories Where Parent = '{parent}' ORDER BY Title COLLATE NOCASE ASC";
        }

        internal static string Get_By_Id(string id)
        {
            return $"SELECT * FROM Categories WHERE Id = '{id}'";
        }

        internal static string Update()
        {
            return @"UPDATE Categories SET Title=@Title, Parent=@Parent, Color=@Color WHERE Id=@Id";
        }

        internal static string Delete(string id)
        {
            return $"DELETE FROM Categories WHERE Id='{id}'";
        }

        internal static string Delete()
        {
            return "DELETE FROM Categories";
        }

        internal static string Move(string id, string parent)
        {
            return $"UPDATE Categories SET Parent='{parent}' WHERE Id='{id}'";
        }
    }
}
