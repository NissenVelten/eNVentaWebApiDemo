namespace NVShop.Data.NV.Model
{
    public abstract class NVEntity : INVIdent
    {
        public string RowId { get; set; }
        public long RowVersion { get; set; }
    }
}