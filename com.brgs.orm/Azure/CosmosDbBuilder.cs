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
        public async Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T,bool>> predicate)
        {
            var query = _client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), new FeedOptions
                {
                    EnableCrossPartitionQuery = true
                })
                .Where(predicate)
                .AsDocumentQuery();
            
            var results = new List<T>();
            while(query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }
            return results;
        }
        public async Task PostAsync<T>(T record)
        {
            await _client.UpsertDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),record);
        }
        ///<summary>
        ///In order to affect a batch operation against Cosmos, you'll need to create a stored procedure (js) 
        ///and host it in your CosmosDB instance.
        ///</summary>
        public async Task PostBatchAsync<T>(IEnumerable<T> records, string procName, string partitionKey)
        {
            await _client.ExecuteStoredProcedureAsync<T>(UriFactory.CreateStoredProcedureUri(DatabaseId,CollectionId, procName),
            new RequestOptions()
            {
                PartitionKey = new Microsoft.Azure.Documents.PartitionKey(partitionKey)
            }, records);
            
        }
        public async Task DeleteAsync<T>(string id, string partiton = null)
        {
            if(partiton == null)
            {

                await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), new RequestOptions
                {
                    PartitionKey = new PartitionKey(Undefined.Value)
                });          
            }


            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), new RequestOptions
            {
                PartitionKey = new PartitionKey(partiton)
            });          

        }
        public async void DeleteBatchAsync<T>(IEnumerable<T> records, string procName, string partitionKey)
        {
            await _client.ExecuteStoredProcedureAsync<T>(UriFactory.CreateStoredProcedureUri(DatabaseId,CollectionId, procName),
            new RequestOptions()
            {
                PartitionKey = new Microsoft.Azure.Documents.PartitionKey(partitionKey)
            }, records);            
            
        }

    }
}