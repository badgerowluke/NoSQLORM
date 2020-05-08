using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

[assembly:InternalsVisibleTo("com.brgs.orm.test")]
namespace com.brgs.orm.Azure
{
    public interface ICosmosDbBuilder
    {
        Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T,bool>> predicate, string collection=null);
        Task<string> PostAsync<T>(T record);
        Task DeleteAsync<T>(string id, string partiton = null);
        void DeleteBatchAsync<T>(IEnumerable<T> records, string procName, string partitionKey);
    }
    public class CosmosDbBuilder : AzureStorageFactory, ICosmosDbBuilder
    {

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
        public async Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T,bool>> predicate, string collection = null)
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

        public override async Task<string> PostAsync<T>(T value)
        {

            var result = await _client.UpsertDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), value);
            return result.StatusCode.ToString();
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


    }
}