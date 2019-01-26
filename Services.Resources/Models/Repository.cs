using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Services.Resources.Models {

    public interface IRepository<T> where T : class {

        //Task<Document> CreateItemAsync(T item);

        Task<IEnumerable<T>> GetAllItemsAsync();

        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);

        Task<T> GetItemAsync(string id);

        //Task<Document> UpdateItemAsync(string id, T item);

        Task<Document> UpsertItemAsync(string id, T item);

        Task DeleteItemAsync(string id);
    }

    public abstract class Repository<T> : IRepository<T> where T : class {

        protected readonly DocumentClient Client;
        protected readonly string DatabaseId;
        protected readonly string CollectionId;

        public Repository(DocumentClient client, string databaseId, string collectionId) {

            this.Client = client;
            this.DatabaseId = databaseId;
            this.CollectionId = collectionId;
        }

        public async Task<IEnumerable<T>> GetAllItemsAsync() {

            IDocumentQuery<T> query = this.Client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults) {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate) {

            IDocumentQuery<T> query = this.Client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults) {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<T> GetItemAsync(string id) {

            try {
                Document document = await this.Client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e) {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound) {
                    return null;
                }
                else {
                    throw;
                }
            }
        }

        //public async Task<Document> CreateItemAsync(T item) {
        //    return await this.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
        //}

        //public async Task<Document> UpdateItemAsync(string id, T item) {
        //    return await this.Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
        //}

        public async Task<Document> UpsertItemAsync(string id, T item) {
            return await this.Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
        }

        public async Task DeleteItemAsync(string id) {
            await this.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
        }

        private async Task CreateDatabaseIfNotExistsAsync() {

            try {
                await this.Client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e) {

                if (e.StatusCode == System.Net.HttpStatusCode.NotFound) {
                    await this.Client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync() {

            try {
                await this.Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e) {

                if (e.StatusCode == System.Net.HttpStatusCode.NotFound) {

                    await this.Client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else {
                    throw;
                }
            }
        }
    }
}