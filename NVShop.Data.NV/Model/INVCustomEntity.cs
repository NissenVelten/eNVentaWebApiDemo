using System.Collections.Generic;

namespace NVShop.Data.NV.Model
{
    public interface INVCustomEntity
    {
        IDictionary<string, object> CustomAttributes { get; }
    }
}
