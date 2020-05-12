namespace NVShop.Data.NV.Model
{
    public class NVIdent : INVIdent
    {
        public string RowId { get; set; }
        public long RowVersion { get; set; }
    }
}