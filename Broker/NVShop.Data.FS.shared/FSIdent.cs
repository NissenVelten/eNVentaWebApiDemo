namespace NVShop.Data.FS
{
    using FrameworkSystems.FrameworkBase.Metadatatype;

    public class FSIdent
    {
        public FSIdent(FSSystemGuid rowId, FSlong rowVersion)
        {
            Guard.ArgumentNotNull(() => rowId);
            Guard.ArgumentNotNull(() => rowVersion);

            RowId = rowId;
            RowVersion = rowVersion;
        }

        public FSSystemGuid RowId { get; private set; }
        public FSlong RowVersion { get; private set; }
    }
}