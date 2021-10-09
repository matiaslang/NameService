using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using NameSorterProcessor.DynamoDB;
using NameSorterProcessor.Models;
using static NameSorterProcessor.DynamoDB.DynamoClient;

namespace NameSorterProcessor.Commands {
    public class HandleNameEventCommand {

        public static async Task<EventResult> ProcessNamePostingEvent(NameModel[] names) {
            createClient(false);
            await CreateTableIfNotExisting();
            DynamoDBContext context = new DynamoDBContext(Client);
            var nameBatch = context.CreateBatchWrite<NameModel>(new DynamoDBOperationConfig());
            nameBatch.AddPutItems(names);
            
            try {
                await nameBatch.ExecuteAsync();
            }
            catch (Exception e) {
                Console.WriteLine($"There was an error posting item to dynamo: {e.Message}");
                return new EventResult {OperationSucceeded = false};
            }

            return new EventResult {OperationSucceeded = true};
        }

        public static async Task<EventResult> ProcessNameListGet() {
            var result = await DynamoClient.GetList();
            var list = result.Select(r => new NameModel
                {name = r["name"].S.ToString(), amount = int.Parse(r["amount"].N.ToString())}).ToList();
            var sortedList = list.OrderByDescending(o => o.amount).ToList();
            return new EventResult{NameList = sortedList, OperationSucceeded = true};

        }
        
    }
}
