using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
// Add library by running command
// dotnet add package Microsoft.Azure.Cosmos.Table --version 1.0.6

namespace demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Table storage sample");

            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=storage1forblobs;AccountKey=Gdn1ylxdvgal+hGv7aUfToBARVFEeVHHfPoz/OJFhbtdzJMgzbG5L3guEZ5pLoY7MzWSOqT6kt2B+AStDi8xxg==;EndpointSuffix=core.windows.net";
            var tableName = "demo2";

            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(tableName);


            CustomerEntity customer = new CustomerEntity("Basu", "BlobTextFile")
            {
                
                URL = "https://storage1forblobs.blob.core.windows.net/trialcontainer1/TrialText.txt",
                Email = "basangouda.patil@meteoros.in",
                //PhoneNumber = "8861567555",
                Place = "Bangalore"
            };

            MergeUser(table, customer).Wait();
            QueryUser(table, "Basu", "BlobTextFile").Wait();

        }

        public static async Task MergeUser(CloudTable table, CustomerEntity customer) {
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(customer);

            // Execute the operation.
            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
            CustomerEntity insertedCustomer = result.Result as CustomerEntity;

            Console.WriteLine("Added user.");
        }

        public static async Task QueryUser(CloudTable table, string firstName, string lastName) {
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>(firstName, lastName);
            
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            CustomerEntity customer = result.Result as CustomerEntity;

            if (customer != null)
            {
                Console.WriteLine("Fetched \t{0}\t{1}\t{2}\t{3}", 
                    customer.PartitionKey, customer.RowKey, customer.URL, customer.Email, customer.Place);
            }
        }
    }

    public class CustomerEntity : TableEntity
    {

        public CustomerEntity() {}
        public CustomerEntity(string lastName, string firstName)
        {
            PartitionKey = lastName;
            RowKey = firstName;
        }
        public string URL { get; set; }
        public string Email { get; set; }
        //public string PhoneNumber { get; set; }
        public string Place { get; set; }
    }
}