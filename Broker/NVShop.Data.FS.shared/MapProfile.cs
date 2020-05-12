namespace NVShop.Data.FS
{
    using System;
    using AutoMapper;
    using FrameworkSystems.FrameworkBase.Metadatatype;

    public class MapProfile : Profile
    {

        public MapProfile()
        {
			CreateMap<FSint, int?>().ProjectUsing(x => x.ValueAsNullable);


            CreateMap<FSdecimal, long?>().ProjectUsing(x => x.HasValue ? Convert.ToInt64(x.Value) : (long?)null);
            CreateMap<long?, FSdecimal>().ProjectUsing(x => x.HasValue ? (FSdecimal)(decimal)x.Value : FSdecimal.Null);

            CreateMap<FSshort, short>().ProjectUsing(x => x.Value);
			CreateMap<FSshort, short?>().ProjectUsing(x => x.ValueAsNullable);
			CreateMap<FSshort, int>().ProjectUsing(x => Convert.ToInt32(x.Value));
			CreateMap<FSshort, int?>().ProjectUsing(x => x.IsNull ? (int?)null : Convert.ToInt32(x.Value));
			CreateMap<FSshort, bool>().ProjectUsing(x => x.HasValue && Convert.ToBoolean(x.Value));
			CreateMap<FSshort, bool?>().ProjectUsing(x => x.IsNull ? (bool?)null : Convert.ToBoolean(x.Value));

			CreateMap<long?, FSlong>().ProjectUsing(x => !x.HasValue ? FSlong.Null : (FSlong)x.Value);
			CreateMap<FSlong, long>().ProjectUsing(x => x.Value);
			CreateMap<FSlong, long?>().ProjectUsing(x => x.ValueAsNullable);
			CreateMap<FSlong, short>().ProjectUsing(x => Convert.ToInt16(x.Value));
			CreateMap<FSlong, short?>().ProjectUsing(x => x.IsNull ? (short?)null : Convert.ToInt16(x.Value));
			CreateMap<FSlong, int>().ProjectUsing(x => Convert.ToInt32(x.Value));
			CreateMap<FSlong, int?>().ProjectUsing(x => x.IsNull ? (int?)null : Convert.ToInt32(x.Value));
			CreateMap<FSlong, bool>().ProjectUsing(x => x.HasValue && Convert.ToBoolean(x.Value));
			CreateMap<FSlong, bool?>().ProjectUsing(x => x.IsNull ? (bool?)null : Convert.ToBoolean(x.Value));

			CreateMap<FSdecimal, decimal>().ProjectUsing(x => x.Value);
			CreateMap<FSdecimal, decimal?>().ProjectUsing(x => x.ValueAsNullable);
			CreateMap<FSdecimal, double>().ProjectUsing(x => Convert.ToDouble(x.Value));
			CreateMap<FSdecimal, double?>().ProjectUsing(x => x.IsNull ? (double?)null : Convert.ToDouble(x.Value));
			CreateMap<FSdecimal, bool>().ProjectUsing(x => x.HasValue && Convert.ToBoolean(x.Value));
			CreateMap<FSdecimal, bool?>().ProjectUsing(x => x.IsNull ? (bool?)null : Convert.ToBoolean(x.Value));

            CreateMap<FSdouble, double>().ProjectUsing(x => x.Value);
            CreateMap<FSdouble, double?>().ProjectUsing(x => x.ValueAsNullable);

            CreateMap<FSDateTime, DateTime?>().ProjectUsing(x => x.ValueAsNullable);
			CreateMap<FSDateTime, DateTime>().ProjectUsing(x => x.Value);

			CreateMap<FSstring, string>().ProjectUsing(x => x.HasValue ? x.Value : string.Empty);
			CreateMap<FSSystemGuid, string>().ProjectUsing(x => x.ToString());
			CreateMap<string, FSSystemGuid>().ProjectUsing(x => !string.IsNullOrEmpty(x) ? new FSSystemGuid(Guid.Parse(x)) : FSSystemGuid.Null);

			CreateMap<bool, FSshort>().ProjectUsing(x => Convert.ToInt16(x));
			CreateMap<bool?, FSshort>().ProjectUsing(x => !x.HasValue ? FSshort.Null : (FSshort)Convert.ToInt16(x.Value));
        }
    }
}