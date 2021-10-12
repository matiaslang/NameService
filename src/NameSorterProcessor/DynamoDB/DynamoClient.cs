using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using NameSorterProcessor.Models;

namespace NameSorterProcessor.DynamoDB {
    public static class DynamoClient {
        private static string tableName = "namesTable";
        private static readonly string Ip = "localhost";
        private static readonly int Port = 8000;
        private static readonly string EndpointUrl = "http://" + Ip + ":" + Port;
        public static AmazonDynamoDBClient Client;

        private static bool IsPortInUse() {
            var isAvailable = true;
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();
            foreach (var endpoint in tcpConnInfoArray)
                if (endpoint.Port == Port) {
                    isAvailable = false;
                    break;
                }

            return isAvailable;
        }

        public static bool createClient(bool useDynamoDbLocal) {
            if (useDynamoDbLocal) {
                // First, check to see whether anyone is listening on the DynamoDB local port
                // (by default, this is port 8000, so if you are using a different port, modify this accordingly)
                var portUsed = IsPortInUse();
                if (portUsed) {
                    Console.WriteLine("The local version of DynamoDB is NOT running.");
                    return false;
                }

                // DynamoDB-Local is running, so create a client
                Console.WriteLine("  -- Setting up a DynamoDB-Local client (DynamoDB Local seems to be running)");
                var ddbConfig = new AmazonDynamoDBConfig();
                ddbConfig.ServiceURL = EndpointUrl;
                try {
                    Client = new AmazonDynamoDBClient(ddbConfig);
                }
                catch (Exception ex) {
                    Console.WriteLine("     FAILED to create a DynamoDBLocal client; " + ex.Message);
                    return false;
                }
            }
            else {
                Client = new AmazonDynamoDBClient();
            }

            return true;
        }

        public static async Task<TableDescription> GetTableDescription(string tableName) {
            TableDescription result = null;

            // If the table exists, get its description.
            try {
                var response = await Client.DescribeTableAsync(tableName);
                result = response.Table;
            }
            catch (Exception) { }

            return result;
        }
        
        public static PutItemRequest CreatePutItemRequest(NameModel name) {
            LambdaLogger.Log($"Starting to log user {name.name}");
            Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>();
            attributes["name"] = new AttributeValue {S = name.name};
            attributes["amount"] = new AttributeValue {N = name.amount.ToString()};
            var request = new PutItemRequest {
                TableName = tableName,
                Item = attributes,
            };
            return request;
        }
        
        
        public static async Task CreateTableIfNotExisting() {
            try {
                var result = await DynamoClient.Client.DescribeTableAsync(tableName);
            }
            catch (ResourceNotFoundException e) {
                await CreateTable();
            }
        }
        
        public static async Task<CreateTableResponse> CreateTable() {
            var request = new CreateTableRequest {
                TableName = tableName,
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement {KeyType = KeyType.HASH, AttributeName = "name"},
                },
                AttributeDefinitions = new List<AttributeDefinition>{new AttributeDefinition{AttributeName = "name", AttributeType = "S"}},
                ProvisionedThroughput = new ProvisionedThroughput {ReadCapacityUnits = 1, WriteCapacityUnits = 1},
            };
            return await DynamoClient.Client.CreateTableAsync(request);
        }

        public static async Task<List<Dictionary<string, AttributeValue>>> GetList() {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            
            var request = new ScanRequest {
                TableName = tableName
            };

            var response = await client.ScanAsync(request);
            var result = response.Items;
            return result;
        }
        
    }
}
