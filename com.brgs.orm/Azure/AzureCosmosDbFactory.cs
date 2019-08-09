// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Linq.Expressions;
// using System.Threading.Tasks;
// using Microsoft.Azure.Documents;
// using Microsoft.Azure.Documents.Client;
// using Microsoft.Azure.Documents.Linq;

// namespace com.brgs.orm.Azure
// {
//     public interface ICosmosOptions
//     {
//         string CosmosUrl { get; set; }
//         string CosmosKey { get; set; }       
//         string PartitionKey { get; set; }
//         string CollectionName { get; set; } 
//     }
//     public interface IDocumentDbFactory
//     {
//         void Register(ICosmosOptions options);
//         T GetItem<T>(Func<T, bool> predicate);
//         Task<Document> UpsertDocumentAsync<T>(T item);
//         Task<Document> DeleteItemAsync(string id);
//     }
//     public class DocumentDbFactory : IDocumentDbFactory
//     {
//         private readonly ICloudStorageAccount _account;
//         public DocumentDbFactory(ICloudStorageAccount account)
//         {
//             _account = account;
//         }

//         private IDocumentClient _client;
//         private ICosmosOptions _options;


//         public void Register(ICosmosOptions options)
//         {
//             _options = options;
//             if(_client == null)
//             {
//                 _client = _account.CreateDocumentClient(options.CosmosUrl, options.CosmosKey);
//                 CreateDatabaseIfNotExistsAsync().Wait();
//                 CreateCollectionIfNotExistsAsync().Wait();
//             }
//         }
//         private async Task CreateDatabaseIfNotExistsAsync()
//         {
//             try
//             {
//                 await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_options.PartitionKey));
//             }
//             catch (DocumentClientException e)
//             {
//                 if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
//                 {
//                     await _client.CreateDatabaseAsync(new Database { Id = _options.PartitionKey });
//                 }
//                 else
//                 {
//                     throw;
//                 }
//             }
//         }
//         private async Task CreateCollectionIfNotExistsAsync()
//         {
//             try
//             {
//                 await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_options.PartitionKey, _options.CollectionName));
//             }
//             catch (DocumentClientException e)
//             {
//                 if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
//                 {
//                     await _client.CreateDocumentCollectionAsync(
//                         UriFactory.CreateDatabaseUri(_options.PartitionKey),
//                         new DocumentCollection { Id = _options.CollectionName },
//                         new RequestOptions { OfferThroughput = 1000 });
//                 }
//                 else
//                 {
//                     throw;
//                 }
//             }
//         }        
//         public T GetItem<T>(Func<T, bool> predicate) 
//         {
//             var results = _client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_options.PartitionKey, _options.CollectionName))
//                 .Where(predicate)
//                 .FirstOrDefault();   
//             return results;
//         }
//         public async Task<IEnumerable<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate)
//         {
//             IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
//                 UriFactory.CreateDocumentCollectionUri(_options.PartitionKey, _options.CollectionName))
//                 .Where(predicate)
//                 .AsDocumentQuery();
//             List<T> results = new List<T>();
//             while (query.HasMoreResults)
//             {
//                 results.AddRange(await query.ExecuteNextAsync<T>());
//             }
//             return results;
//         }   
//         public async Task<Document> UpsertDocumentAsync<T>(T item)
//         {
//             return await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_options.PartitionKey, _options.CollectionName), item);
//         }     
//         public async Task<Document> DeleteItemAsync(string id)
//         {
//             return await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_options.PartitionKey, _options.CollectionName, id));
//         }
//         public async Task<Document> ReplaceItemAsync(Document item, string eTag)
//         {
//             var ac = new AccessCondition { Condition = eTag, Type = AccessConditionType.IfMatch };            
//             var resp = await _client.ReplaceDocumentAsync(item, new RequestOptions { AccessCondition = ac });

//             return null;
//         }        
//     }
// }