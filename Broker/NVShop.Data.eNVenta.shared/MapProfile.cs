using NV.ERP.Base.Article;

namespace NVShop.Data.eNVenta
{
    using AutoMapper;

    using FrameworkSystems.FrameworkBase.Metadatatype;

    using NVShop.Data.FS;
    using NVShop.Data.NV.Model;

    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<FSIdent, NVIdent>(MemberList.None);

            #region NVArticle

            CreateMap<IcdArticle, NVArticle>().ForMember(dst => dst.RowId, opt => opt.MapFrom(src => src.ROWID))
                .ForMember(dst => dst.RowVersion, opt => opt.MapFrom(src => src.ROWVERSION))
                .ForMember(dst => dst.ArticleId, opt => opt.MapFrom(src => src.sArticleID))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.sArticleName))
                .ForMember(dst => dst.Ean, opt => opt.MapFrom(src => src.sEan))
                .ForMember(dst => dst.Gtin, opt => opt.MapFrom(src => src.sGTIN));

            #endregion
        }
    }
}