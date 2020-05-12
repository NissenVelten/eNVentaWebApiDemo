namespace NVShop.Data.NV
{
    using System;
    using Model;

    public interface INVNestedQueryBuilder<TQueryBuilder>
    {
        TQueryBuilder Nested(Func<TQueryBuilder, TQueryBuilder> builder, NVQueryOperator op = NVQueryOperator.Or);
    }
}