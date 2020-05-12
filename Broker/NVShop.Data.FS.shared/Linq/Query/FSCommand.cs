namespace NVShop.Data.FS.Linq.Query
{
    public class FSCommand
    {
        public FSCommand(string whereClause, string orderByClause, int? loadCount = null, int? offset = null, int? limit = null)
        {
            WhereClause = whereClause;
            OrderByClause = orderByClause;
            LoadCount = loadCount;
            Offset = offset;
            Limit = limit;
        }

        public string WhereClause { get; }
        public string OrderByClause { get; }

        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public int? LoadCount { get; set; }
        public bool IsCountQuery { get; set; }
        public bool IsLongCountQuery { get; set; }
    }
}