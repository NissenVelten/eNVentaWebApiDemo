namespace NVShop.Data.NV.Model
{
    using System;
    using System.Collections.Generic;

    public partial class NVArticle : NVEntity, INVCustomEntity
    {
        public string ArticleId { get; set; }
        public string Name { get; set; }
        public string Ean { get; set; }
        public string Gtin { get; set; }

        public IDictionary<string, object> CustomAttributes => new Dictionary<string, object>();
    }
}