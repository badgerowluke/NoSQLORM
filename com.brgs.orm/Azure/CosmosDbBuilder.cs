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
    public partial class  AzureStorageFactory: ICosmosDbBuilder
    {
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

        public async Task<string> PostAsync<T>(T value)
        {

            var result = await _client.UpsertDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), value);
            return result.StatusCode.ToString();
        }

        public virtual async Task DeleteAsync<T>(string id, string partiton = null)
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

        ///<summary>
        ///In order to affect a batch operation against Cosmos, you'll need to create a stored procedure (js) 
        ///and host it in your CosmosDB instance.
        ///</summary>
        public virtual async void DeleteBatchAsync<T>(IEnumerable<T> records, string procName, string partitionKey)
        {
            await _client.ExecuteStoredProcedureAsync<T>(UriFactory.CreateStoredProcedureUri(DatabaseId,CollectionId, procName),
            new RequestOptions()
            {
                PartitionKey = new Microsoft.Azure.Documents.PartitionKey(partitionKey)
            }, records);            
            
        }  


    }
}