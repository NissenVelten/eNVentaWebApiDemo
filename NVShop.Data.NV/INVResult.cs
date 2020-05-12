using System.Collections.Generic;

namespace NVShop.Data.NV
{
    public interface INVResult
    {
        IDictionary<string, List<string>> Errors { get; }
    }
}
