using System;
using System.Collections.Generic;
using  System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

[assembly:InternalsVisibleTo("com.brgs.orm.test")]
namespace com.brgs.orm.Azure
{
    public class CosmosDbBuilder 
    {
        private ICloudStorageAccount _account { get; set; }
        private IDocumentClient _client { get; set; }
        public string DatabaseId { get; set; }
        public string CollectionId { get; set; }
        public CosmosDbBuilder(ICloudStorageAccount account )
        {
            _account = account;
            _client = _account.CreateDocumentClient();
        }
        public CosmosDbBuilder(ICloudStorageAccount account, string database, string collection)
        {
            _account = account;
            _client = _account.CreateDocumentClient();
            DatabaseId = database;
            CollectionId = collection;
            
        }
        public T Get<T>()
        {
            return default(T);
        }
        public async Task<IEnumerable<T>> Get<T>(Expression<Func<T,bool>> predicate)
        {
            var query = _client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                .Where(predicate)
                .AsDocumentQuery();
            var results = new List<T>();
            while(query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }
            return results;
        }
        public async void PostAsync<T>(T record)
        {
            
        }
        public async void PostBatchAsync<T>(IEnumerable<T> records)
        {

        }
        public void DeleteAsync<T>(T record)
        {

        }

    }
}