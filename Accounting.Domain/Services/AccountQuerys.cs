using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountingApi.Domain;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace AccountingApi.Services
{
    public class AccountQuerys : IAccountQuerys
    {
        public DocumentClient DocumentClient { get; }

        public AccountQuerys(DocumentClient documentClient)
        {
            DocumentClient = documentClient ?? throw new ArgumentNullException(nameof(documentClient));
        }

        public bool IsAccountExisting(string accountNumber)
        {
            string accountDocumentId = Account.CreateAggregateId(accountNumber);

            var accountCount = this.DocumentClient.CreateDocumentQuery<Account>(
                UriFactory.CreateDocumentCollectionUri(Constants.DatabaseName, Constants.ViewsCollectionName),
                new FeedOptions() { PartitionKey = new PartitionKey(accountNumber) })
                .Where(a => a.AccountNumber == accountNumber)
                .Count();
            return accountCount > 0;
        }

        public async Task<Account> GetAccountByNumberAsync(string accountNumber)
        {
            string accountDocumentId = Account.CreateAggregateId(accountNumber);

            var account = await this.DocumentClient.ReadDocumentAsync<Account>(
                UriFactory.CreateDocumentUri(Constants.DatabaseName, Constants.ViewsCollectionName, accountDocumentId), 
                new RequestOptions() { PartitionKey = new PartitionKey(accountDocumentId) });
            return account;
        }
    }
}
