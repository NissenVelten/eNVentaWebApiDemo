using AutoMapper;
using FrameworkSystems.FrameworkBase.Metadatatype;
using System;
using System.Linq.Expressions;

namespace NVShop.Data.FS
{
	public partial class FSTypeConverter
	{
		public class FSstringToString : ITypeConverter<FSstring, string>
		{
			public string Convert(FSstring source, string destination, ResolutionContext context)
			{
				return source.HasValue ? source.Value : string.Empty;
			}

			public static Expression<Func<FSstring, string>> Expression => x => x.HasValue ? x.Value : string.Empty;
		}
		public class StringToFSstring : ITypeConverter<string, FSstring>
		{
			public FSstring Convert(string source, FSstring destination, ResolutionContext context)
			{
				return string.IsNullOrEmpty(source) ? FSstring.Empty : new FSstring(source);
			}

			public static Expression<Func<string, FSstring>> Expression => x => String.IsNullOrEmpty(x) ? FSstring.Empty : (FSstring)x;
		}
	}
}
