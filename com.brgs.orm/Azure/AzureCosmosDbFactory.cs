namespace com.brgs.orm.Azure
{
    public interface IDocumentDbFactory
    {

    }
    public class DocumentDbFactory : IDocumentDbFactory
    {
        private readonly ICloudStorageAccount _account;
        public DocumentDbFactory(ICloudStorageAccount account)
        {
            _account = account;
        }
    }
}