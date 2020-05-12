namespace NVShop.Data.FS.Linq.Query
{
    using System.Collections.Generic;
    using System.Text;

    public class FSQueryPartsAggregator
    {
        public FSQueryPartsAggregator()
        {
            WhereParts = new List<string>();
            OrderByParts = new List<string>();
        }

        private List<string> WhereParts { get; }
        private List<string> OrderByParts { get; }

        public void AddWherePart(string formatString, params object[] args)
        {
            WhereParts.Add(string.Format(formatString, args));
        }

        public void AddOrderByPart(IEnumerable<string> orderings)
        {
            OrderByParts.Insert(0, orderings.Join(", "));
        }

		public void Merge(FSQueryPartsAggregator queryParts)
		{
			WhereParts.AddRange(queryParts.WhereParts);
			OrderByParts.AddRange(queryParts.OrderByParts);
		}

        public string BuildFSWhere()
        {
            var stringBuilder = new StringBuilder();

            if (WhereParts.Count > 0)
            {
                stringBuilder.AppendFormat("{0}", WhereParts.Join(" and "));
            }

            return stringBuilder.ToString();
        }

        public string BuildFSOrderBy()
        {
            var stringBuilder = new StringBuilder();

            if (OrderByParts.Count > 0)
            {
                stringBuilder.AppendFormat("{0}", OrderByParts.Join(", "));
            }

            return stringBuilder.ToString();
        }
    }
}