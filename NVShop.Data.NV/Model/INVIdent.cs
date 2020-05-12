namespace NVShop.Data.NV.Model
{
    public interface INVIdent
    {
        string RowId { get; }
        long RowVersion { get; }
    }
}