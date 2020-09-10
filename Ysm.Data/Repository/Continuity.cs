namespace Ysm.Data
{
    public class Continuity
    {
        public void Save(string id, int end)
        {
            ContinuityQueries.Save(id, end);
        }

        public int Get(string id)
        {
            return ContinuityQueries.Get(id);
        }
    }
}
