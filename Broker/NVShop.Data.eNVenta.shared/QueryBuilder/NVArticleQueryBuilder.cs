using AutoMapper;
using NV.ERP.Base.Article;

namespace NVShop.Data.eNVenta.QueryBuilder
{
    using NVShop.Data.eNVenta.NVRepository;
    using NVShop.Data.FS;
    using NVShop.Data.NV.Model;
    using NVShop.Data.NV.QueryBuilder;

    using System;
    using System.Collections.Generic;

    public partial class NVArticleQueryBuilder : NVQueryBuilderBase<NVArticle, IcdArticle>, INVArticleQueryBuilder
    {
        private string _articleId;
        private string _ean;
        private string _gtin;
        private string _name;

        public NVArticleQueryBuilder(INVSelectRepository<NVArticle> rep, IMapper mapper, FSUtil fsUtil)
			: base(rep, mapper, fsUtil)
		{
		}

        public INVArticleQueryBuilder ByArticleId(string articleId)
        {
            _articleId = articleId;

            return this;
        }

        public INVArticleQueryBuilder ByEan(string ean)
        {
            _ean = ean;

            return this;
        }

        public INVArticleQueryBuilder ByGtin(string gtin)
        {
            _gtin = gtin;

            return this;
        }

        public INVArticleQueryBuilder LikeName(string name)
        {
            _name = name;
            return this;
        }

        public override FSQueryList<IcdArticle> BuildConditions(FSQueryList<IcdArticle> conditions)
        {
            if (!string.IsNullOrEmpty(_articleId))
            {
                conditions.Eq(p => p.sArticleID, _articleId);
            }

            if (!string.IsNullOrEmpty(_ean))
            {
                conditions.Eq(p => p.sEan, _ean);
            }

            if (!string.IsNullOrEmpty(_gtin))
            {
                conditions.Eq(p => p.sGTIN, _gtin);
            }

            if (!string.IsNullOrEmpty(_name))
            {
                conditions.Like(p => p.sArticleName, _name, true);
            }

            return conditions;
        }

        public INVArticleQueryBuilder Nested(
            Func<INVArticleQueryBuilder, INVArticleQueryBuilder> builder,
            NVQueryOperator op)
        {
            Where(builder(new NVArticleQueryBuilder(Repository, Mapper, FSUtil))
                .BuildWhere(op));

            return this;
        }

        protected override IEnumerable<string> DefaultOrder() => new[] {
            OrderByExpr(x => x.sArticleID)
        };
    }
}